using Core.Model.Layer.Entity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text.Json;

namespace Core.Layer.Data
{
    public class MentalHealthDbContext : IdentityDbContext<ApplicationUser>
    {
        public MentalHealthDbContext(DbContextOptions<MentalHealthDbContext> options)
            : base(options) { }

        public DbSet<Question> Questions { get; set; }
        public DbSet<Answer> Answers { get; set; }
        public DbSet<Diagnosis> Diagnoses { get; set; }
        public DbSet<DiagnosticRule> DiagnosticRules { get; set; }
        public DbSet<RuleCondition> RuleConditions { get; set; }
        public DbSet<UserAnswer> UserAnswers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Answer>()
                .HasOne(a => a.Question)
                .WithMany()
                .HasForeignKey(a => a.QuestionId)
                .IsRequired(false);
            // Seed data for Answers
            SeedAnswers(modelBuilder);

            // Seed data for Questions
            SeedQuestions(modelBuilder);

            // Seed data for DiagnosticRules
            SeedDiagnosticRules(modelBuilder);

            // Seed data for RuleConditions
            SeedRuleConditions(modelBuilder);
        }

        private void SeedAnswers(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Answer>().HasData(
          new Answer { Id = 1, Text = "بله", SessionId = "SeedSession1", QuestionId = 1, Response = true },
                new Answer { Id = 2, Text = "خیر", SessionId = "SeedSession1", QuestionId = 1, Response = false }
            );
        }

        private void SeedQuestions(ModelBuilder modelBuilder)
        {
            string jsonData = @"{
                ""A"": {
                    ""section"": ""Episode of Major Depression"",
                    ""questions"": [
                        ""در دو هفته‌ی گذشته، آیا بیشتر از معمول احساس غمگینی یا بی‌انگیزگی داشته‌اید؟"",
                        ""آیا در این مدت از کارهایی که قبلاً برایتان لذت‌بخش بود کمتر لذت برده‌اید؟"",
                        ""آیا در اشتها یا وزن‌تان تغییری حس کرده‌اید؟"",
                        ""آیا با مشکلاتی در خواب (مثل کم‌خوابی یا خواب زیاد) مواجه شده‌اید؟"",
                        ""آیا خسته یا بی‌انرژی بوده‌اید؟"",
                        ""آیا احساس بی‌ارزشی یا گناه داشته‌اید؟"",
                        ""آیا تمرکز یا تصمیم‌گیری برایتان سخت شده؟"",
                        ""آیا به مرگ یا نبودن فکر کرده‌اید؟""
                    ]
                },
                ""B"": {
                    ""section"": ""Episode of Hypomania"",
                    ""questions"": [
                        ""آیا دوره‌ای بوده که احساس نشاط یا اعتماد به‌نفس بیش از حد داشته‌اید؟"",
                        ""آیا در آن زمان نیازتان به خواب کمتر از معمول شده بود؟"",
                        ""آیا خیلی بیشتر یا سریع‌تر از حد معمول صحبت می‌کردید؟"",
                        ""آیا ذهنتان مداوماً در حال پرش از یک فکر به فکر دیگر بود؟"",
                        ""آیا تمرکزتان به آسانی از بین می‌رفت؟"",
                        ""آیا احساس می‌کردید بیش‌فعال، اجتماعی یا اهل ریسک شده‌اید؟""
                    ]
                },
                ""C"": {
                    ""section"": ""Episode of Mania"",
                    ""questions"": [
                        ""آیا تا به حال دچار خلق‌وخوی بسیار بالا یا بی‌ثبات شده‌اید که کنترل آن برایتان دشوار باشد؟"",
                        ""آیا در آن زمان رفتارهایی داشته‌اید که برای کار یا روابطتان مشکل‌ساز شده باشد؟""
                    ]
                },
                ""D"": {
                    ""section"": ""Panic Disorder"",
                    ""questions"": [
                        ""آیا تجربه‌ی ناگهانی اضطراب شدید با علائمی مانند تپش قلب یا تنگی نفس داشته‌اید؟"",
                        ""آیا از اینکه چنین حملاتی دوباره رخ دهند، نگران بوده‌اید؟""
                    ]
                },
                ""E"": {
                    ""section"": ""Agoraphobia"",
                    ""questions"": [
                        ""آیا جاهایی وجود دارند که از رفتن به آن‌ها اجتناب می‌کنید چون فکر می‌کنید در صورت بروز مشکل نتوانید کمک بگیرید یا خارج شوید؟""
                    ]
                },
                ""F"": {
                    ""section"": ""Social Phobia"",
                    ""questions"": [
                        ""آیا از اینکه دیگران درباره‌تان قضاوت کنند (مثلاً هنگام صحبت کردن یا خوردن در جمع) احساس اضطراب می‌کنید؟"",
                        ""آیا به‌خاطر این نگرانی، از چنین موقعیت‌هایی دوری کرده‌اید؟""
                    ]
                },
                ""G"": {
                    ""section"": ""Obsessive-Compulsive Disorder (OCD)"",
                    ""questions"": [
                        ""آیا افکار مزاحم و غیرارادی دارید که بارها در ذهنتان تکرار می‌شوند؟"",
                        ""آیا برای کاهش این افکار، مجبور به انجام کارهایی مثل شست‌وشو یا چک کردن درب‌ها می‌شوید؟""
                    ]
                },
                ""H"": {
                    ""section"": ""Post-Traumatic Stress Disorder (PTSD)"",
                    ""questions"": [
                        ""آیا رویداد بسیار دردناک یا ترسناکی را تجربه کرده‌اید؟"",
                        ""آیا خاطرات آن رویداد بی‌اختیار به ذهن‌تان بازمی‌گردد؟"",
                        ""آیا سعی کرده‌اید از یادآورهای آن رویداد دوری کنید؟"",
                        ""آیا اغلب در حالت اضطراب یا آماده‌باش دائمی هستید؟""
                    ]
                }
            }";

            var jsonDoc = JsonDocument.Parse(jsonData);
            var questions = new List<Question>();
            int questionId = 1;
            DateTime staticDate = new DateTime(2025, 1, 1);

            foreach (var section in jsonDoc.RootElement.EnumerateObject())
            {
                var sectionName = section.Value.GetProperty("section").GetString();
                foreach (var q in section.Value.GetProperty("questions").EnumerateArray())
                {
                    questions.Add(new Question
                    {
                        Id = questionId,
                        Text = q.GetString(),
                        Section = sectionName,
                        CreatedAt = staticDate,
                        Order = questionId // اضافه کردن Order
                    });
                    questionId++;
                }
            }

            modelBuilder.Entity<Question>().HasData(questions);
        }

        private void SeedDiagnosticRules(ModelBuilder modelBuilder)
        {
            DateTime staticDate = new DateTime(2025, 1, 1); // Static date to avoid dynamic value warning
            modelBuilder.Entity<DiagnosticRule>().HasData(
                new DiagnosticRule { Id = 1, Code = "DEP001", Title = "اختلال افسردگی عمده", Description = "بر اساس پاسخ به سوالات مربوط به معیارهای افسردگی (حداقل 5 علامت شامل غمگینی یا بی‌علاقگی).", MinimumMatchesRequired = 5, CreatedAt = staticDate },
                new DiagnosticRule { Id = 2, Code = "HYP001", Title = "هیپومانیا", Description = "بر اساس پاسخ به سوالات مربوط به معیارهای هیپومانیا (حداقل 3 علامت).", MinimumMatchesRequired = 3, CreatedAt = staticDate },
                new DiagnosticRule { Id = 3, Code = "MAN001", Title = "مانیا", Description = "بر اساس پاسخ به سوالات مربوط به معیارهای مانیا (خلق بالا و رفتار مشکل‌ساز).", MinimumMatchesRequired = 2, CreatedAt = staticDate },
                new DiagnosticRule { Id = 4, Code = "PAN001", Title = "اختلال پانیک", Description = "بر اساس پاسخ به سوالات مربوط به حملات پانیک و نگرانی از تکرار آن‌ها.", MinimumMatchesRequired = 2, CreatedAt = staticDate }
            );
        }

        private void SeedRuleConditions(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<RuleCondition>().HasData(
                new RuleCondition { Id = 1, DiagnosticRuleId = 1, QuestionId = 1, ExpectedAnswer = "بله" },
                new RuleCondition { Id = 2, DiagnosticRuleId = 1, QuestionId = 2, ExpectedAnswer = "بله" },
                new RuleCondition { Id = 3, DiagnosticRuleId = 1, QuestionId = 3, ExpectedAnswer = "بله" },
                new RuleCondition { Id = 4, DiagnosticRuleId = 1, QuestionId = 4, ExpectedAnswer = "بله" },
                new RuleCondition { Id = 5, DiagnosticRuleId = 1, QuestionId = 5, ExpectedAnswer = "بله" },
                new RuleCondition { Id = 6, DiagnosticRuleId = 2, QuestionId = 9, ExpectedAnswer = "بله" },
                new RuleCondition { Id = 7, DiagnosticRuleId = 2, QuestionId = 10, ExpectedAnswer = "بله" },
                new RuleCondition { Id = 8, DiagnosticRuleId = 2, QuestionId = 11, ExpectedAnswer = "بله" },
                new RuleCondition { Id = 9, DiagnosticRuleId = 3, QuestionId = 15, ExpectedAnswer = "بله" },
                new RuleCondition { Id = 10, DiagnosticRuleId = 3, QuestionId = 16, ExpectedAnswer = "بله" },
                new RuleCondition { Id = 11, DiagnosticRuleId = 4, QuestionId = 17, ExpectedAnswer = "بله" },
                new RuleCondition { Id = 12, DiagnosticRuleId = 4, QuestionId = 18, ExpectedAnswer = "بله" }
            );
        }
    }
    public class SectionData
    {
        [JsonProperty("section")]
        public string Section { get; set; }

        [JsonProperty("questions")]
        public string[] Questions { get; set; }
    }
}
