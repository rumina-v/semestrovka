# DTOs и ViewModels структура

## Принцип разделения

**DTOs (Data Transfer Objects)** - для входящих данных (POST/PUT запросы)
**ViewModels** - для исходящих данных (ответы сервера)

---

## Auth (Аутентификация)

### DTOs
- **LoginDto** - логин пользователя
  - `Username`: строка
  - `Password`: строка

- **RegisterDto** - регистрация пользователя
  - `Username`: строка
  - `Email`: строка
  - `Password`: строка
  - `ConfirmPassword`: строка

### ViewModels
- **AuthResponseViewModel** - ответ на логин/регистрацию
  - `Success`: bool
  - `Token`: JWT токен
  - `Message`: сообщение
  - `User`: UserInfoViewModel (id, username, email)

---

## Case (Дела)

### DTOs
- **CreateCaseDto** - создание нового дела
  - `Title`: название
  - `NewspaperText`: газетная сводка
  - `ShortDescription`: краткое описание
  - `FullDescription`: полное описание

### ViewModels
- **CaseListItemViewModel** - элемент списка дел
  - `Id`: ID
  - `Title`: название
  - `NewspaperText`: сводка
  - `SuspectCount`: количество подозреваемых
  - `EvidenceCount`: количество улик

- **CaseDetailViewModel** - полная информация о деле
  - `Id`, `Title`, `NewspaperText`, `ShortDescription`, `FullDescription`
  - `Suspects`: список подозреваемых
  - `Evidence`: список улик

- **SuspectListItemViewModel** - информация о подозреваемом
  - `Id`, `Name`, `Description`
  - `InitialTrust`: начальное доверие
  - `InitialAggression`: начальная агрессия
  - `IsGuilty`: виновен ли

- **EvidenceListItemViewModel** - информация об улике
  - `Id`, `Title`, `ShortText`
  - `PhrasesCount`: количество фраз

---

## Interrogation (Допрос) ⭐ ГЛАВНОЕ

### DTOs
- **StartInterrogationDto** - запуск допроса
  - `CaseId`: ID дела
  - `SuspectId`: ID подозреваемого

- **ProcessPhraseDto** - выбор фразы
  - `PhraseId`: ID выбранной фразы

### ViewModels
- **InterrogationSessionViewModel** - состояние сессии
  - `SessionId`: ID сессии
  - `SuspectName`: имя подозреваемого
  - `CaseName`: название дела
  - `CurrentTrust`: текущее доверие (0-100)
  - `CurrentAggression`: текущая агрессия (0-100)
  - `Status`: статус (InProgress, Confession, Refused)
  - `AvailablePhrases`: список доступных фраз
  - `LastReply`: последний ответ подозреваемого

- **AvailablePhraseViewModel** - фраза для выбора
  - `Id`: ID фразы
  - `Text`: текст фразы
  - `EvidenceTitle`: название улики
  - `EvidenceId`: ID улики

- **SuspectReplyViewModel** - ответ подозреваемого
  - `ReplyText`: текст ответа
  - `NewTrust`: новое значение доверия
  - `NewAggression`: новое значение агрессии
  - `TrustChange`: изменение доверия
  - `AggressionChange`: изменение агрессии
  - `EvidenceTitle`: использованная улика
  - `Message`: доп. сообщение

- **InterrogationResultViewModel** - результат допроса
  - `SessionId`: ID сессии
  - `SuspectName`: имя подозреваемого
  - `FinalStatus`: финальный статус (Confession, Refused, Neutral)
  - `FinalTrust`: финальное доверие
  - `FinalAggression`: финальная агрессия
  - `ConclusionText`: текст развязки
  - `AwardedAchievements`: полученные достижения

---

## Achievement (Достижения)

### ViewModels
- **AchievementViewModel** - информация о достижении
  - `Id`: ID
  - `Title`: название
  - `Description`: описание
  - `IsUnlocked`: получено ли

- **UserAchievementsViewModel** - все достижения пользователя
  - `UserId`: ID пользователя
  - `Username`: имя пользователя
  - `TotalAchievements`: всего достижений
  - `UnlockedCount`: получено
  - `Achievements`: список достижений

---

## Admin (Админ-панель)

### ViewModels
- **UserListViewModel** - информация о пользователе
  - `Id`: ID
  - `Username`: имя
  - `Email`: почта
  - `InterrogationCount`: количество допросов
  - `AchievementCount`: количество достижений

- **GameStatisticsViewModel** - общая статистика
  - `TotalUsers`: всего пользователей
  - `TotalCases`: всего дел
  - `TotalInterrogations`: всего допросов
  - `TotalAchievementsAwarded`: выданных достижений
  - `AverageSessionLength`: средняя длина сессии
  - `AveragePlayerSuccess`: средняя успешность игроков

---

## Использование

### В Контроллерах
```csharp
[HttpPost("login")]
public async Task<IActionResult> Login([FromBody] LoginDto request)
{
    // ... обработка ...
    return Ok(new AuthResponseViewModel { Success = true, Token = token });
}
```

### Преобразование Entities -> ViewModels
```csharp
var result = new InterrogationSessionViewModel
{
    SessionId = session.Id,
    SuspectName = session.Suspect.Name,
    CurrentTrust = session.CurrentTrust,
    CurrentAggression = session.CurrentAggression
};
```

### Преобразование DTOs -> Entities
```csharp
var interrogation = new InterrogationSession
{
    CaseId = dto.CaseId,
    SuspectId = dto.SuspectId
};
```
