# Как DTOs и ViewModels отражают игровую механику

## Основная игровая механика (из описания проекта)

### Главные компоненты игры:
1. **Параметры подозреваемого**: доверие (0-100) и давление (0-100)
2. **Выбор фраз**: детектив выбирает заготовленные фразы об уликах
3. **Блокировка улик**: после использования одной фразы из группы улики остальные недоступны
4. **Ответы подозреваемого**: зависят от параметров и выбранной фразы
5. **Условия завершения**: доверие=100 & давление=100 (признание) или другие финалы
6. **Достижения**: выдаются за удачные решения

---

## 📊 Отражение в InterrogationSessionViewModel

```csharp
public class InterrogationSessionViewModel
{
    public int SessionId { get; set; }
    public string SuspectName { get; set; }
    public string CaseName { get; set; }
    
    // ⭐ ГЛАВНОЕ: два скрытых параметра подозреваемого
    public int CurrentTrust { get; set; }      // Доверие (0-100)
    public int CurrentAggression { get; set; } // Давление (0-100)
    
    public string Status { get; set; } // InProgress, Confession, Refused
    
    // ⭐ Доступные фразы (зависят от использованных улик)
    public List<AvailablePhraseViewModel> AvailablePhrases { get; set; }
    
    public string? LastReply { get; set; } // Последний ответ подозреваемого
}
```

**Что происходит:**
- `CurrentTrust` и `CurrentAggression` - **видимая для игрока информация о состоянии подозреваемого**
- `AvailablePhrases` - **переменный список** (зависит от использованных улик)
- `Status` - **отслеживает окончание допроса**

---

## 🎯 Выбор фраз и блокировка улик

### AvailablePhraseViewModel - фраза для выбора

```csharp
public class AvailablePhraseViewModel
{
    public int Id { get; set; }
    public string Text { get; set; }           // "Я нашел ваши отпечатки на рукоятке"
    public string EvidenceTitle { get; set; }  // "Отпечатки на ноже"
    public int EvidenceId { get; set; }        // Связь с уликой
}
```

**Механика:**
1. Фраза связана с **конкретной уликой** (EvidenceId)
2. После выбора одной фразы из улики → остальные фразы этой улики удаляются из `AvailablePhrases`
3. В БД: `SessionUsedEvidence` отмечает использованные улики
4. Следующий запрос `GetAvailablePhrasesAsync()` уже не вернёт фразы удалённых улик

**Пример:**
```
Улика: "Отпечатки на ноже"
├─ Фраза 1: "Я нашел ваши отпечатки на рукоятке" ← Выбрана
├─ Фраза 2: "Ваши отпечатки есть на оружии убийства" ✗ ЗАБЛОКИРОВАНА
└─ Фраза 3: "Это доказывает вашу причастность" ✗ ЗАБЛОКИРОВАНА
```

---

## 🗣️ Ответы подозреваемого и изменение параметров

### SuspectReplyViewModel - ответ на фразу

```csharp
public class SuspectReplyViewModel
{
    public string ReplyText { get; set; }
    // ⭐ Текущие значения ПОСЛЕ выбора фразы
    public int NewTrust { get; set; }          // Новое доверие
    public int NewAggression { get; set; }     // Новое давление
    
    // ⭐ Как изменились параметры
    public int TrustChange { get; set; }       // +15 или -10
    public int AggressionChange { get; set; }  // -5 или +20
    
    public string EvidenceTitle { get; set; }  // Какая улика была использована
    public string? Message { get; set; }       // "Подозреваемый начинает нервничать"
}
```

**Как это работает в Service:**

```csharp
// В InterrogationService.ProcessPhraseSelectionAsync()
var reply = await _context.SuspectReplies
    .FirstOrDefaultAsync(sr => sr.SuspectId == session.SuspectId 
                            && sr.PhraseId == phraseId);

// ⭐ Обновляем параметры
session.CurrentTrust = Math.Clamp(
    session.CurrentTrust + reply.TrustChange, 0, 100);

session.CurrentAggression = Math.Clamp(
    session.CurrentAggression + reply.AggressionChange, 0, 100);
```

**Пример игровой сцены:**
```
Детектив выбирает: "Я нашел ваши отпечатки на рукоятке"

Система вычисляет:
  - Текущие параметры: Доверие=40, Давление=30
  - Ответ подозреваемого: TrustChange=+20, AggressionChange=+10
  - Новые параметры: Доверие=60, Давление=40

Подозреваемый говорит: "Это не может быть! Я... я, конечно, трогал нож, 
но это было давно, еще до того как..."

UI обновляет шкалы:
  Trust: ▓▓▓▓▓▓░░░░ (60%)
  Aggression: ▓▓▓▓░░░░░░ (40%)
```

---

## ⏹️ Условия завершения допроса

### InterrogationResultViewModel - финальный результат

```csharp
public class InterrogationResultViewModel
{
    public int SessionId { get; set; }
    public string SuspectName { get; set; }
    
    // ⭐ Три возможных финала
    public string FinalStatus { get; set; }
    // "Confession" - Доверие=100 && Давление=100
    // "Refused"    - Доверие=0   && Давление=100
    // "Neutral"    - Другие комбинации
    
    public int FinalTrust { get; set; }
    public int FinalAggression { get; set; }
    
    public string ConclusionText { get; set; }
    // "Джеффри сломался под давлением улик и признался в планировании убийства..."
    
    public List<string> AwardedAchievements { get; set; }
    // ["Truth Seeker", "Pressure Expert"]
}
```

**Логика завершения в Service:**

```csharp
private void CheckInterrogationEnd(InterrogationSession session)
{
    // Признание: максимум доверия И максимум давления
    if (session.CurrentTrust == 100 && session.CurrentAggression == 100)
    {
        session.Status = "Confession";
        // Подозреваемый подписывает признание
    }
    // Отказ: минимум доверия И максимум давления
    else if (session.CurrentTrust == 0 && session.CurrentAggression == 100)
    {
        session.Status = "Refused";
        // Подозреваемый отказывается разговаривать
    }
}
```

---

## 🏆 Система достижений

### Как работают:

1. **AchievementViewModel** - информация о достижении
   ```csharp
   public class AchievementViewModel
   {
       public string Title { get; set; }        // "Truth Seeker"
       public string Description { get; set; }  // "Reach 100 trust with a suspect"
       public bool IsUnlocked { get; set; }     // Получено ли пользователем
   }
   ```

2. **Выдача достижений** происходит в `InterrogationService`:
   ```csharp
   // Достижение: максимум доверия
   if (session.CurrentTrust == 100)
   {
       await _achievementService.AwardAchievementAsync(
           session.UserId, 
           achievementId: "Truth Seeker"
       );
   }
   
   // Достижение: максимум давления
   if (session.CurrentAggression == 100)
   {
       await _achievementService.AwardAchievementAsync(
           session.UserId,
           achievementId: "Pressure Expert"
       );
   }
   ```

3. **Просмотр в профиле** через `UserAchievementsViewModel`

---

## 🔄 Полный цикл допроса

Как всё взаимодействует вместе:

```
1️⃣ СТАРТ ДОПРОСА
   └─ StartInterrogationDto (CaseId, SuspectId)
   └─ InterrogationService.StartInterrogationSessionAsync()
      └─ Создаёт session с начальными параметрами
      └─ CurrentTrust = Suspect.InitialTrust (50)
      └─ CurrentAggression = Suspect.InitialAggression (50)
   └─ Возвращает InterrogationSessionViewModel

2️⃣ ИГРОК ВИДИТ СОСТОЯНИЕ
   └─ GET /api/interrogation/state/{sessionId}
   └─ Получает InterrogationSessionViewModel с:
      - CurrentTrust, CurrentAggression (шкалы)
      - AvailablePhrases (список доступных фраз)

3️⃣ ИГРОК ВЫБИРАЕТ ФРАЗУ
   └─ POST /api/interrogation/phrase/{sessionId}
   └─ ProcessPhraseDto { PhraseId = 5 }
   └─ InterrogationService.ProcessPhraseSelectionAsync()
      ├─ Проверяет, не использована ли уже эта улика
      ├─ Помечает улику как использованную (SessionUsedEvidence)
      ├─ Получает SuspectReply (ответ на эту фразу)
      ├─ Обновляет параметры:
      │  └─ CurrentTrust += reply.TrustChange
      │  └─ CurrentAggression += reply.AggressionChange
      ├─ Проверяет условие завершения CheckInterrogationEnd()
      └─ Сохраняет session
   └─ Возвращает SuspectReplyViewModel

4️⃣ ИГРОК ВИДИТ ОТВЕТ
   └─ UI обновляет:
      - NewTrust, NewAggression (новые шкалы)
      - TrustChange, AggressionChange (индикаторы изменения)
      - ReplyText (что сказал подозреваемый)
      - EvidenceTitle (какая улика была использована)

5️⃣ ШАГИ 2-4 ПОВТОРЯЮТСЯ
   └─ Каждый раз AvailablePhrases сокращается
   └─ Используется GetAvailablePhrasesAsync()
   └─ Она не возвращает фразы использованных улик

6️⃣ ФИНАЛ ДОПРОСА
   └─ Условие: session.Status != "InProgress"
   └─ POST /api/interrogation/end/{sessionId}
   └─ InterrogationService.EndInterrogationSessionAsync()
      ├─ Выдаёт достижения
      ├─ Рассчитывает итог
      └─ Создаёт InterrogationResultViewModel
   └─ UI показывает InterrogationResultViewModel:
      - FinalStatus (Confession/Refused/Neutral)
      - ConclusionText (что произошло)
      - AwardedAchievements (какие достижения получены)
```

---

## 📍 Сюжет в коде

### Сценарий: Джеффри Робинсон (муж)

**В Models.Entities:**
```csharp
var suspect = new Suspect
{
    Name = "Джеффри Робинсон",
    Description = "Муж судьи, скрывает измену...",
    InitialTrust = 30,      // Он нервный
    InitialAggression = 20, // Но не агрессивный
    IsGuilty = false        // Но виновен ли?
};
```

**Улики против него:**
- `Evidence: "Визитка адвоката по разводам"`
  - `Phrase: "Ваша жена намеревалась подать на развод"`
    - `SuspectReply.TrustChange = -15` (раздражается)
    - `SuspectReply.AggressionChange = +30` (защищается)
    
- `Evidence: "Нахождение у Линетт Тайрен"`
  - `Phrase: "Мы знаем о вашей связи"`
    - `SuspectReply.TrustChange = +50` (сломается)
    - `SuspectReply.AggressionChange = +20`

**Дело раскрывается** через комбинацию правильно выбранных улик!

---

## ✅ Итого: Как всё связано

| Компонент | Отражаемая механика |
|-----------|-------------------|
| `CurrentTrust`, `CurrentAggression` | Два скрытых параметра подозреваемого |
| `AvailablePhrases` | Доступные фразы (динамически обновляется) |
| `EvidenceId` в Phrase | Связь фразы с уликой (для блокировки) |
| `SessionUsedEvidence` | Отслеживание использованных улик |
| `SuspectReply.TrustChange` | Влияние фразы на доверие |
| `SuspectReply.AggressionChange` | Влияние фразы на давление |
| `InterrogationResultViewModel` | Финальный результат и достижения |
| `CheckInterrogationEnd()` | Условия завершения допроса |

Всё в коде точно соответствует описанной в документе механике! 🎮✨
