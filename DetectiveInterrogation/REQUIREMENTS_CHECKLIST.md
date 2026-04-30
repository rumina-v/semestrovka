# Соответствие проекта требованиям (Checklist)

## 📋 Общие требования

| Требование | Статус | Примечание |
|-----------|--------|-----------|
| Backend: .NET >= 6 | ✅ | Используется .NET 8.0 |
| ASP.NET Core | ✅ | Полностью используется |
| Функционал согласован в письменном виде | ✅ | [story.md](story.md) и [detective_project_description.md](detective_project_description.md) |
| Актуальность и новизна | ✅ | Интерактивный детектив с игровой механикой |

**Баллов: 0/0 (вспомогательные требования)**

---

## 🏛️ Требование к архитектуре (5 баллов)

| Требование | Статус | Примечание |
|-----------|--------|-----------|
| Выбран архитектурный паттерн | ✅ | **MVC + Service Layer** |
| Реализован минимальный функционал | ✅ | Вся бизнес-логика в Services |
| Объяснение выбора паттерна | ✅ | [ARCHITECTURE_ANALYSIS.md](ARCHITECTURE_ANALYSIS.md) |
| Обоснование выбора именно для этого проекта | ⚠️ НУЖНО УЛУЧШИТЬ | Описано но нужны более глубокие аргументы |

### Почему MVC + Service Layer для этого проекта? 

**Аргументы:**
1. **Сложная бизнес-логика допроса** - не подходит простой CRUD
   - Требуется пересчёт параметров (доверие/давление) по сложным правилам
   - Service Layer идеально инкапсулирует эту логику
   
2. **Множественные зависимости между сущностями**
   - `InterrogationSession` → `Suspect` → `SuspectReply` → `EvidencePhrase` → `Evidence`
   - Service Layer управляет этими зависимостями
   
3. **Не подходят альтернативы:**
   - ❌ Simple CRUD паттерн - недостаточно для игровой механики
   - ❌ CQRS - избыточный для этого масштаба (нет особых требований к разделению Commands/Queries)
   - ❌ Clean Architecture - слишком громоздка для курсового проекта
   - ✅ MVC + Service Layer - баланс простоты и функциональности

**Баллов: 5/5** ✅

---

## 🎨 Требование к фронтенд части (5 баллов)

| Требование | Статус | Примечание |
|-----------|--------|-----------|
| Выбран способ реализации | ⏳ | **Нужно выбрать и реализовать** |
| Асинхронное взаимодействие с бекендом | ⏳ | Контроллеры готовы для API |

### Варианты реализации:

**Вариант 1: Razor + HTML/CSS/JS (РЕКОМЕНДУЕТСЯ для быстрой разработки)**
- ✅ Views готовы к реализации
- ✅ Razor шаблонизатор идеален для MVC
- ✅ Асинхронность через Fetch API / jQuery
- ⏳ **СТАТУС: Views папка создана, нужно заполнить шаблоны**

**Вариант 2: Vue.js / React SPA**
- ✅ Асинхронное взаимодействие через API
- ✅ Современный подход
- ❌ Требует отдельную разработку фронтенда
- ❌ Требует Node.js и сборщик (Webpack, Vite)

**Вариант 3: Postman коллекция для тестирования**
- ✅ Все API endpoints готовы
- ⏳ **СТАТУС: Нужна коллекция Postman с полным сценарием**

**РЕКОМЕНДАЦИЯ: Реализовать Razor Views + Postman коллекция**

**Баллов: 0/5** (нужно реализовать)

---

## ⚙️ Требование к backend (базовый минимум 15+ баллов)

### 1. Версия .NET (обязательно)
| Требование | Статус |
|-----------|--------|
| .NET >= 6 | ✅ |
| Используется .NET 8.0 | ✅ |

**Баллов: N/A** ✅

### 2. Swagger (2 балла)
| Требование | Статус | Примечание |
|-----------|--------|-----------|
| Swagger добавлен в Program.cs | ✅ | Частично |
| Swagger UI доступен | ⏳ | Нужно проверить при запуске |
| Документированы все endpoints | ⏳ | Нужны XML-комментарии |

**Решение:**
```csharp
// В Program.cs уже есть:
builder.Services.AddSwaggerGen();
app.UseSwagger();
app.UseSwaggerUI();

// НУЖНО ДОБАВИТЬ: XML комментарии к контроллерам
/// <summary>
/// Запускает новый допрос
/// </summary>
/// <param name="request">CaseId и SuspectId</param>
/// <returns>Информация о сессии допроса</returns>
[HttpPost("start")]
public async Task<IActionResult> StartInterrogation(...)
```

**Баллов: 1-2/2** (частично готово)

### 3. Модель БД (3 балла)
| Требование | Статус | Примечание |
|-----------|--------|-----------|
| Минимум 5 основных сущностей | ✅ | Есть 10 сущностей |
| Правильные связи (1-M, M-M) | ✅ | Все Configurations созданы |
| EF Core конфигурации | ✅ | Fluent API полностью реализована |

**Сущности:**
1. `User` - пользователь
2. `Case` - дело
3. `Suspect` - подозреваемый
4. `Evidence` - улика
5. `Achievement` - достижение
6. (+ `EvidencePhrase`, `SuspectReply`, `InterrogationSession`, `SessionUsedEvidence`, `UserAchievement`)

**Баллов: 3/3** ✅

### 4. Аутентификация и авторизация (2 балла)
| Требование | Статус | Примечание |
|-----------|--------|-----------|
| JWT или Cookie | ✅ | JWT реализован |
| JwtTokenHelper | ✅ | Создан и регистрирован |
| ClaimsHelper | ✅ | Извлечение данных из JWT |
| Роли (Roles) | ⏳ | **НУЖНО ДОБАВИТЬ** |
| Permissions | ⏳ | **НУЖНО ДОБАВИТЬ** |
| Principal | ✅ | Используется в контроллерах (User) |
| [Authorize] атрибут | ✅ | На критичных контроллерах |
| [Authorize(Roles = "Admin")] | ⚠️ | Роли не реализованы |

**Что нужно добавить:**

```csharp
// 1. В User entity добавить роль
public class User
{
    public string Role { get; set; } = "Player"; // "Player", "Admin", "Moderator"
}

// 2. В JWT token добавить Claim для роли
var claims = new[]
{
    new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
    new Claim(ClaimTypes.Name, username),
    new Claim(ClaimTypes.Role, user.Role) // ← ДОБАВИТЬ
};

// 3. Использовать в контроллерах
[Authorize(Roles = "Admin")]
public IActionResult DeleteUser(int userId) { ... }
```

**Баллов: 1-1.5/2** (JWT работает, роли нужны)

### 5. Логирование (2 балла)
| Требование | Статус | Примечание |
|-----------|--------|-----------|
| Serilog добавлен | ✅ | Зависимость есть |
| Логирование в файл | ⏳ | **НУЖНО НАСТРОИТЬ** |
| Глобальный обработчик ошибок | ✅ | ExceptionHandlingMiddleware |
| Логирование запросов/ответов | ✅ | RequestLoggingMiddleware |

**Что нужно добавить в Program.cs:**

```csharp
// Настройка логирования в файл
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.Console()
    .WriteTo.File(
        path: "logs/app-.txt",
        rollingInterval: RollingInterval.Day,
        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
    .CreateLogger();
```

**Баллов: 1.5-2/2** (реализовано, нужна доработка)

### 6. Использование технологий (3 балла)

| Требование | Статус | Примечание |
|-----------|--------|-----------|
| Hangfire / SMTP / RabbitMQ / MinIO | ⏳ | **EmailService готов, но не полностью настроен** |

**Выбор: SMTP (EmailService)**
- ✅ Класс создан
- ✅ Интерфейс создан
- ✅ Зависимости зарегистрированы
- ⏳ Нужна настройка в appsettings.json
- ⏳ Нужны точки отправки писем (при регистрации, восстановлении пароля)

**Что нужно добавить:**

```csharp
// В AuthService при успешной регистрации:
await _emailService.SendWelcomeEmailAsync(user.Email, user.Username);

// Точки отправки:
// 1. Регистрация: SendWelcomeEmailAsync()
// 2. Восстановление пароля: SendPasswordResetEmailAsync()
// 3. Новое достижение: SendAchievementEmailAsync() (нужно создать)
```

**Баллов: 1.5-2/3** (реализовано частично)

### 7. Внешний API (1 балл)

| Требование | Статус | Примечание |
|-----------|--------|-----------|
| ExternalApiService создан | ✅ | Класс готов |
| Интерфейс создан | ✅ | IExternalApiService |
| **Подключен реальный API** | ❌ | **НУЖНО ВЫБРАТЬ И ПОДКЛЮЧИТЬ** |

**Варианты подключения:**
1. **Dadata API** - для геолокации (как в примере требований)
2. **Weather API** - для погоды
3. **News API** - для новостей
4. **Сustom API** - собственный сервис

**Рекомендация:** Подключить Weather API для вариации сценария допроса (например, "погода влияет на настроение подозреваемого")

**Баллов: 0-0.5/1** (нужно подключить реальный API)

### 8. Безопасность (2 балла)

| Атака | Защита | Статус |
|------|--------|--------|
| XSS (Cross-Site Scripting) | HTML Encoding в Razor, Content Security Policy | ⏳ НУЖНА |
| CSRF (Cross-Site Request Forgery) | AntiForgeryToken | ⏳ НУЖНА |
| Broken Access Control | [Authorize] атрибуты, роли | ✅ ГОТОВО |
| Broken Authentication | JWT с сроком действия, хеширование пароля (BCrypt) | ✅ ГОТОВО |
| Injection (SQL Injection) | EF Core параметризованные запросы | ✅ ГОТОВО |
| Валидация | Модели с Data Annotations | ⏳ НУЖНА |

**Что нужно добавить:**

```csharp
// 1. Валидация моделей
public class RegisterDto
{
    [Required(ErrorMessage = "Username required")]
    [StringLength(50, MinimumLength = 3)]
    public string Username { get; set; }
    
    [Required(ErrorMessage = "Email required")]
    [EmailAddress]
    public string Email { get; set; }
    
    [Required]
    [StringLength(100, MinimumLength = 6)]
    public string Password { get; set; }
}

// 2. CSRF защита (для форм)
// В Razor views: <input type="hidden" asp-for="@Html.AntiForgeryToken()" />

// 3. Content Security Policy (в Program.cs)
app.UseMiddleware<SecurityHeadersMiddleware>();
```

**Баллов: 1-1.5/2** (частично готово)

---

## 📊 Итоговый подсчёт баллов

| Раздел | Получено | Максимум | % |
|--------|----------|---------|---|
| Архитектура | 5 | 5 | 100% |
| Frontend | 0 | 5 | 0% |
| Swagger | 1.5 | 2 | 75% |
| БД | 3 | 3 | 100% |
| Auth/JWT | 1.5 | 2 | 75% |
| Логирование | 1.5 | 2 | 75% |
| Технологии (SMTP) | 1.5 | 3 | 50% |
| Внешний API | 0.5 | 1 | 50% |
| Безопасность | 1.5 | 2 | 75% |
| **ИТОГО** | **16.5** | **25** | **66%** |

### Штраф за невыполнение:
- На каждый невыполненный пункт -10% от максимума
- Frontend не реализован: -2.5 балла
- Внешний API не подключен: -0.1 балла
- Роли не реализованы: -0.2 балла

**ИТОГОВАЯ ОЦЕНКА БЕЗ ДОДЕЛОК: ~14 баллов из 25**

---

## ✅ План доделок для 25 баллов

### Приоритет 1 (КРИТИЧНО):
- [ ] **Frontend: Razor Views** (5 баллов)
  - [ ] Index.cshtml - главная страница
  - [ ] Case/Index.cshtml - список дел
  - [ ] Interrogation/Index.cshtml - допрос
  - [ ] Auth/Login.cshtml - логин
  - [ ] Auth/Register.cshtml - регистрация
  - [ ] Async операции через Fetch API

- [ ] **Роли и Permissions** (0.5 балла)
  - [ ] Добавить Role в User entity
  - [ ] Обновить JWT token с ролью
  - [ ] Использовать [Authorize(Roles = "Admin")] на контроллерах

- [ ] **Внешний API** (0.5 балла)
  - [ ] Выбрать API (Weather, Dadata, News)
  - [ ] Интегрировать в InterrogationService
  - [ ] Использовать в бизнес-логике

### Приоритет 2 (ВАЖНО):
- [ ] **Валидация моделей** (0.5 балла)
  - [ ] Data Annotations на DTOs
  - [ ] [Required], [StringLength], [EmailAddress]
  - [ ] Кастомные валидаторы если нужны

- [ ] **SMTP Email полностью** (0.5 балла)
  - [ ] Настроить appsettings.json
  - [ ] Добавить отправку при регистрации
  - [ ] Добавить отправку при достижениях

- [ ] **Логирование в файл** (0.5 балла)
  - [ ] Настроить Serilog в Program.cs
  - [ ] Включить RollingFile sink
  - [ ] Проверить logs/ папку

- [ ] **Безопасность** (0.5 балла)
  - [ ] CSRF токены в Forms
  - [ ] Content Security Policy headers
  - [ ] HTML Encoding в Razor views

### Приоритет 3 (БОНУС):
- [ ] **Postman коллекция** (для демонстрации)
  - [ ] Весь сценарий допроса
  - [ ] Аутентификация
  - [ ] CRUD операции

- [ ] **Swagger XML комментарии** (0.5 балла)
  - [ ] `<summary>` на всех endpoints
  - [ ] `<param>` для параметров
  - [ ] `<returns>` для возврата

---

## 🎯 Рекомендуемый порядок работ

```
1. Frontend (Razor Views) - самое трудозатратное
   ├─ Общий Layout.cshtml
   ├─ Auth Views
   ├─ Case Views
   ├─ Interrogation Views
   └─ Achievement Views

2. Доделать Backend
   ├─ Роли в User entity
   ├─ Обновить AuthService (выдавать role в JWT)
   ├─ Добавить [Authorize(Roles)]
   └─ Подключить внешний API

3. Безопасность
   ├─ Валидация моделей
   ├─ CSRF токены
   ├─ Security headers
   └─ Логирование в файл

4. Документирование
   ├─ XML комментарии для Swagger
   ├─ Postman коллекция
   └─ README с инструкциями
```

---

## 📝 Выводы

**Текущее состояние:** 66% (16.5/25 баллов)

**Сильные стороны:**
- ✅ Архитектура идеальна
- ✅ Модель БД полная и правильная
- ✅ JWT аутентификация работает
- ✅ Бизнес-логика (Service Layer) реализована
- ✅ Контроллеры API готовы

**Критические пробелы:**
- ❌ Frontend не реализован (-5 баллов)
- ❌ Роли не добавлены (-0.5 балла)
- ❌ Внешний API не подключен (-0.5 балла)
- ❌ Безопасность не полная (-1 балл)

**估计 времени на доделки:**
- Frontend (Razor Views): 6-8 часов
- Backend доделки: 2-3 часа
- Безопасность: 2 часа
- Документация: 1-2 часа

**ИТОГО: ~12-16 часов работы до полной готовности**

Начните с Frontend - это займёт больше всего времени!
