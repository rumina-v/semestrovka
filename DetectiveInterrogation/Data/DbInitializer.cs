using DetectiveInterrogation.Models.Entities;
using DetectiveInterrogation.Helpers;

namespace DetectiveInterrogation.Data;

public static class DbInitializer
{
    public static void Initialize(AppDbContext context)
    {
        Initialize(context, new PasswordHasher());
    }

    public static void Initialize(AppDbContext context, PasswordHasher passwordHasher)
    {
        context.Database.EnsureCreated();

        if (!context.Users.Any(u => u.Username == "admin"))
        {
            context.Users.Add(new User
            {
                Username = "admin",
                Email = "admin@detectives.local",
                PasswordHash = passwordHasher.HashPassword("Admin123!"),
                Role = User.AdminRole
            });
        }

        if (!context.Users.Any(u => u.Username == "player"))
        {
            context.Users.Add(new User
            {
                Username = "player",
                Email = "player@detectives.local",
                PasswordHash = passwordHasher.HashPassword("Player123!"),
                Role = User.PlayerRole
            });
        }

        UpsertAchievement("First Interrogation", "Первый допрос", "Завершить первый допрос подозреваемого.");
        UpsertAchievement("Truth Seeker", "Искатель правды", "Добиться важного признания через логичную последовательность вопросов.");
        UpsertAchievement("Pressure Expert", "Мастер давления", "Довести давление на подозреваемого до критической точки.");
        UpsertAchievement("Master Detective", "Настоящий детектив", "Завершить расследование дела об убийстве судьи.");
        UpsertAchievement("Perfect Investigation", "Идеальное расследование", "Использовать ключевые улики без лишних ошибок.");

        var caseEntity = context.Cases.OrderBy(c => c.Id).FirstOrDefault();
        if (caseEntity == null)
        {
            caseEntity = new Case();
            context.Cases.Add(caseEntity);
        }

        caseEntity.Title = "Убийство судьи";
        caseEntity.NewspaperText = "В собственной квартире найдена мертвой судья Мари Робинсон. Полиция не раскрывает детали, но в деле уже фигурируют муж погибшей и соседка, связанная с давним судебным решением.";
        caseEntity.ShortDescription = "Расследуйте смерть судьи Мари Робинсон и выясните, чья ложь скрывает настоящую причину трагедии.";
        caseEntity.FullDescription = "Мари Робинсон была найдена мертвой в своей квартире. В деле переплелись семейный конфликт, угроза развода, старый несправедливый приговор и месть человека, потерявшего близкого из-за судебной ошибки.";
        context.SaveChanges();

        var suspects = context.Suspects
            .Where(s => s.CaseId == caseEntity.Id)
            .OrderBy(s => s.Id)
            .ToList();

        var jeffrey = suspects.ElementAtOrDefault(0) ?? new Suspect { CaseId = caseEntity.Id };
        if (jeffrey.Id == 0)
            context.Suspects.Add(jeffrey);

        jeffrey.Name = "Джеффри Робинсон";
        jeffrey.Description = "Муж Мари Робинсон. Скрывает роман с Линетт Тайрен и боится, что развод лишит его имущества по брачному договору.";
        jeffrey.InitialTrust = 35;
        jeffrey.InitialAggression = 25;
        jeffrey.IsGuilty = false;

        var lilith = suspects.ElementAtOrDefault(1) ?? new Suspect { CaseId = caseEntity.Id };
        if (lilith.Id == 0)
            context.Suspects.Add(lilith);

        lilith.Name = "Лилит Маккэри";
        lilith.Description = "Соседка Мари. Ее муж погиб в тюрьме после старого приговора, который Лилит считает несправедливым.";
        lilith.InitialTrust = 25;
        lilith.InitialAggression = 35;
        lilith.IsGuilty = true;
        context.SaveChanges();

        var evidence = context.Evidence
            .Where(e => e.CaseId == caseEntity.Id)
            .OrderBy(e => e.Id)
            .ToList();

        var tornPage = UpsertEvidence(evidence.ElementAtOrDefault(0), "Разорванная страница", "На полу нашли клочки страницы из ежедневника Мари.", "На странице была запись о встрече с адвокатом по бракоразводным делам.");
        var threats = UpsertEvidence(evidence.ElementAtOrDefault(1), "Письма с угрозами", "В кабинете лежали письма, адресованные Мари.", "Автор писем требовал признать ошибку в старом деле Майка Маккэри.");
        var lawyerCard = UpsertEvidence(evidence.ElementAtOrDefault(2), "Визитка адвоката", "На столе лежала визитка адвоката по разводам.", "Визитка объясняет утренний конфликт между Мари и Джеффри.");
        var oldCase = UpsertEvidence(evidence.ElementAtOrDefault(3), "Материалы дела Маккэри", "В папке были копии старого дела против Майка Маккэри.", "Старое дело строилось на подложной улике, из-за которой Майк оказался в тюрьме.");
        context.SaveChanges();

        var tornPhrase = UpsertPhrase(tornPage, "Почему вы уничтожили страницу с записью о встрече Мари с адвокатом?");
        var threatsPhrase = UpsertPhrase(threats, "Эти угрозы написаны человеком, который винил Мари за смерть Майка Маккэри.");
        var lawyerPhrase = UpsertPhrase(lawyerCard, "Визитка адвоката доказывает, что Мари готовилась к разводу.");
        var oldCasePhrase = UpsertPhrase(oldCase, "Старое дело Маккэри снова всплыло как раз перед смертью Мари.");
        context.SaveChanges();

        UpsertReply(jeffrey, tornPhrase, "Я был зол. Она собиралась разрушить нашу жизнь, но это не значит, что я ее убил.", -5, 25);
        UpsertReply(jeffrey, threatsPhrase, "Я видел эти письма. Они пугали Мари куда сильнее, чем она показывала.", 15, 5);
        UpsertReply(jeffrey, lawyerPhrase, "Да, я знал про адвоката. И да, я боялся развода. Но вечером меня не было дома.", -10, 20);
        UpsertReply(jeffrey, oldCasePhrase, "Мари не любила говорить о старых делах. Особенно о тех, где могла ошибиться.", 10, 10);

        UpsertReply(lilith, tornPhrase, "Семейные ссоры Робинсон меня не касаются. У меня была другая причина прийти к Мари.", 5, 10);
        UpsertReply(lilith, threatsPhrase, "Да, это мои письма. Я хотела, чтобы она признала, что уничтожила жизнь Майка.", -10, 30);
        UpsertReply(lilith, lawyerPhrase, "Она боялась потерять репутацию больше, чем людей. Даже собственный брак был для нее делом.", 5, 15);
        UpsertReply(lilith, oldCasePhrase, "Майк умер из-за этого дела. Мари могла остановить ошибку, но выбрала карьеру.", 35, 45);

        context.SaveChanges();

        void UpsertAchievement(string oldTitle, string title, string description)
        {
            var achievement = context.Achievements.FirstOrDefault(a => a.Title == title)
                ?? context.Achievements.FirstOrDefault(a => a.Title == oldTitle);

            if (achievement == null)
            {
                context.Achievements.Add(new Achievement { Title = title, Description = description });
                return;
            }

            achievement.Title = title;
            achievement.Description = description;
        }

        Evidence UpsertEvidence(Evidence? item, string title, string shortText, string fullText)
        {
            item ??= new Evidence { CaseId = caseEntity.Id };
            if (item.Id == 0)
                context.Evidence.Add(item);

            item.CaseId = caseEntity.Id;
            item.Title = title;
            item.ShortText = shortText;
            item.FullText = fullText;
            return item;
        }

        EvidencePhrase UpsertPhrase(Evidence evidenceItem, string text)
        {
            var phrase = context.EvidencePhrases
                .Where(p => p.EvidenceId == evidenceItem.Id)
                .OrderBy(p => p.Id)
                .FirstOrDefault();

            phrase ??= new EvidencePhrase { EvidenceId = evidenceItem.Id };
            if (phrase.Id == 0)
                context.EvidencePhrases.Add(phrase);

            phrase.Text = text;
            return phrase;
        }

        void UpsertReply(Suspect suspect, EvidencePhrase phrase, string text, int trustChange, int aggressionChange)
        {
            var reply = context.SuspectReplies
                .FirstOrDefault(r => r.SuspectId == suspect.Id && r.PhraseId == phrase.Id);

            reply ??= new SuspectReply { SuspectId = suspect.Id, PhraseId = phrase.Id };
            if (reply.Id == 0)
                context.SuspectReplies.Add(reply);

            reply.ReplyText = text;
            reply.TrustChange = trustChange;
            reply.AggressionChange = aggressionChange;
        }
    }

}
