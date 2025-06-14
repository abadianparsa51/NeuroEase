using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Layer.Commands;
using Application.Layer.Interface;
using Core.Layer.Data;
using Core.Model.Layer.Entity;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Core.Layer.Handlers
{
    public class EvaluateRulesCommandHandler : IRequestHandler<EvaluateRulesCommand, List<string>>
    {
        private readonly IRuleService _ruleService;
        private readonly MentalHealthDbContext _context;

        public EvaluateRulesCommandHandler(IRuleService ruleService, MentalHealthDbContext context)
        {
            _ruleService = ruleService ?? throw new ArgumentNullException(nameof(ruleService));
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<List<string>> Handle(EvaluateRulesCommand request, CancellationToken cancellationToken)
        {
            try
            {
                Console.WriteLine("شروع ارزیابی قواعد...");

                // اعتبارسنجی
                if (request?.Answers == null || !request.Answers.Any())
                {
                    Console.WriteLine("خطا: لیست پاسخ‌ها خالی است.");
                    throw new ArgumentException("لیست پاسخ‌ها نمی‌تواند خالی باشد.");
                }
                if (request.Answers.Any(a => string.IsNullOrEmpty(a.UserId)))
                {
                    Console.WriteLine("خطا: شناسه کاربر نامعتبر است.");
                    throw new ArgumentException("شناسه کاربر الزامی است.");
                }

                var sessionId = request.Answers.First().SessionId ?? Guid.NewGuid().ToString();
                Console.WriteLine($"SessionId: {sessionId}");

                // ذخیره پاسخ‌ها
                foreach (var answer in request.Answers)
                {
                    if (!await _context.Questions.AnyAsync(q => q.Id == answer.QuestionId, cancellationToken))
                    {
                        Console.WriteLine($"خطا: سوال با شناسه {answer.QuestionId} وجود ندارد.");
                        throw new ArgumentException($"سوال با شناسه {answer.QuestionId} وجود ندارد.");
                    }
                    answer.SessionId = sessionId;
                    answer.CreatedAt = DateTime.UtcNow;
                    _context.UserAnswers.Add(answer);
                    Console.WriteLine($"پاسخ برای سوال {answer.QuestionId} اضافه شد: {answer.Answer}");
                }

                // ذخیره پاسخ‌ها تو دیتابیس
                await _context.SaveChangesAsync(cancellationToken);
                Console.WriteLine("پاسخ‌ها ذخیره شدند.");

                // گرفتن تمام پاسخ‌های کاربر در این نشست
                var allAnswers = await _context.UserAnswers
                    .Where(ua => ua.SessionId == sessionId && ua.UserId == request.Answers.First().UserId)
                    .ToListAsync(cancellationToken);
                Console.WriteLine($"تعداد پاسخ‌های بازیابی‌شده: {allAnswers.Count}");

                // ارزیابی قواعد
                var rules = await _ruleService.EvaluateAsync(allAnswers); // لیست DiagnosticRule
                var triggeredRuleTitles = rules.Select(r => r.Title).ToList();

                Console.WriteLine($"قواعد فعال‌شده: {string.Join(", ", triggeredRuleTitles)}");

                // ذخیره تشخیص‌ها با استفاده از متد SaveDiagnosesAsync در سرویس
                if (triggeredRuleTitles.Any())
                {
                    await _ruleService.SaveDiagnosesAsync(sessionId, request.Answers.First().UserId, rules);
                    Console.WriteLine($"تشخیص‌ها ذخیره شدند: {string.Join(", ", triggeredRuleTitles)}");
                }
                else
                {
                    Console.WriteLine("هیچ قاعده‌ای فعال نشد. تشخیص ذخیره نمی‌شود.");
                }

                // ذخیره نهایی
                await _context.SaveChangesAsync(cancellationToken);
                Console.WriteLine("تغییرات نهایی ذخیره شدند.");

                return triggeredRuleTitles;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"خطا در ارزیابی: {ex.Message}\n{ex.StackTrace}");
                throw;
            }
        }

    }
}
