using Application.Layer.Commands;
using Application.Layer.Querrys;
using Core.Layer.Data;
using Core.Model.Layer.Entity;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace NeuroEase.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class DiagnosisController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly MentalHealthDbContext _dbContext;

        public DiagnosisController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("evaluate")]
        public async Task<IActionResult> Evaluate([FromBody] UserAnswer answer)
        {
            if (answer == null)
                return BadRequest("پاسخ نامعتبر است.");

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("شناسه کاربر در توکن یافت نشد.");

            answer.UserId = userId;

            // گرفتن یا ایجاد SessionId
            var sessionId = HttpContext.Session.GetString("DiagnosisSessionId") ?? Guid.NewGuid().ToString();
            answer.SessionId = sessionId;
            HttpContext.Session.SetString("DiagnosisSessionId", sessionId);

            try
            {
                var diagnoses = await _mediator.Send(new EvaluateRulesCommand
                {
                    Answers = new List<UserAnswer> { answer }
                });

                var result = diagnoses.Select(d => new DiagnosisResult
                {
                    DiagnosisType = d,
                    DetailedResult = $"تشخیص: {d}. برای اطلاعات بیشتر با پزشک مشورت کنید."
                }).ToList();

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"خطا در ارزیابی: {ex.Message}");
            }
        }
        [HttpPost]
        public async Task<IActionResult> CreateDiagnosis([FromBody] DiagnosisCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var diagnosis = new Diagnosis
            {
                SessionId = dto.SessionId,
                Result ="",
                DiagnosticRuleId = dto.DiagnosticRuleId,
                Code = dto.Code,
                Title = dto.Title,
                UserId = dto.UserId,
                CreatedAt = DateTime.UtcNow
            };

            _dbContext.Diagnoses.Add(diagnosis);
            await _dbContext.SaveChangesAsync();

            return Ok(diagnosis);
        }

        [HttpGet("next")]
        public async Task<IActionResult> GetNextQuestion()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("شناسه کاربر در توکن یافت نشد.");

            try
            {
                var nextQuestion = await _mediator.Send(new GetNextQuestionQuery(userId));
                if (nextQuestion == null)
                    return Ok(new { Done = true, Message = "همه سوالات پاسخ داده شده‌اند." });

                return Ok(nextQuestion);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"خطا در دریافت سوال بعدی: {ex.Message}");
            }
        }
    }
}