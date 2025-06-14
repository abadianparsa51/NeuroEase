using Application.Layer.Interface;
using Core.Layer.Data;
using Core.Model.Layer.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Layer.Services
{
    public class RuleService : IRuleService
    {
        private readonly MentalHealthDbContext _context;
        private readonly ILogger<RuleService> _logger;

        public RuleService(MentalHealthDbContext context, ILogger<RuleService> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<List<DiagnosticRule>> EvaluateAsync(List<UserAnswer> answers)
        {
            _logger.LogInformation("Starting rule evaluation in RuleService...");

            if (answers == null || !answers.Any())
            {
                _logger.LogWarning("No user answers provided for evaluation.");
                return new List<DiagnosticRule>();
            }

            var rules = await _context.DiagnosticRules
                .Include(r => r.Conditions)
                .ToListAsync();

            _logger.LogInformation($"Retrieved {rules.Count} diagnostic rules.");

            var triggeredRules = new List<DiagnosticRule>();

            foreach (var rule in rules)
            {
                int matches = 0;
                _logger.LogInformation($"Processing rule: {rule.Title} (ID: {rule.Id}, MinMatches: {rule.MinimumMatchesRequired})");

                foreach (var condition in rule.Conditions)
                {
                    var answer = answers.FirstOrDefault(a => a.QuestionId == condition.QuestionId);
                    if (answer != null)
                    {
                        _logger.LogDebug($"Condition for QuestionId: {condition.QuestionId}, ExpectedAnswer: '{condition.ExpectedAnswer}', UserAnswer: '{answer.Answer}'");

                        if (string.Equals(answer.Answer?.Trim(), condition.ExpectedAnswer?.Trim(), StringComparison.OrdinalIgnoreCase))
                        {
                            matches++;
                            _logger.LogDebug($"Match found for QuestionId: {condition.QuestionId}");
                        }
                        else
                        {
                            _logger.LogDebug($"No match for QuestionId: {condition.QuestionId}. UserAnswer: '{answer.Answer}' != ExpectedAnswer: '{condition.ExpectedAnswer}'");
                        }
                    }
                    else
                    {
                        _logger.LogDebug($"No user answer found for QuestionId: {condition.QuestionId}");
                    }
                }

                _logger.LogInformation($"Rule {rule.Title} has {matches}/{rule.MinimumMatchesRequired} matches.");

                if (matches >= rule.MinimumMatchesRequired)
                {
                    triggeredRules.Add(rule);
                    _logger.LogInformation($"Rule {rule.Title} triggered.");
                }
            }

            _logger.LogInformation($"Triggered rules: {(triggeredRules.Any() ? string.Join(", ", triggeredRules.Select(r => r.Title)) : "None")}");

            return triggeredRules;
        }

        public async Task SaveDiagnosesAsync(string sessionId, string userId, List<DiagnosticRule> triggeredRules)
        {
            if (triggeredRules == null || !triggeredRules.Any())
            {
                _logger.LogInformation($"No diagnoses to save for SessionId: {sessionId}");
                return;
            }

            foreach (var rule in triggeredRules)
            {
                var diagnosis = new Diagnosis
                {
                    SessionId = sessionId,
                    UserId = userId,
                    DiagnosticRuleId = rule.Id,
                    Code = rule.Code,
                    Title = rule.Title,
                    CreatedAt = DateTime.UtcNow,
                      Result = "some default or calculated value"
                };
                _context.Diagnoses.Add(diagnosis);
                _logger.LogDebug($"Added diagnosis: {rule.Title} for SessionId: {sessionId}");
            }

            await _context.SaveChangesAsync();
            _logger.LogInformation($"Saved {triggeredRules.Count} diagnoses for SessionId: {sessionId}");
        }
    }
}