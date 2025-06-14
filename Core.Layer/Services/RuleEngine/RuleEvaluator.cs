using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Model.Layer.Entity;
using Core.Model.Layer.Model;

namespace Core.Layer.Services.RuleEngine
{
    public class RuleEvaluator : IRuleEvaluator
    {
        private readonly List<DiagnosticRuleModel> _rules;

        public RuleEvaluator(List<DiagnosticRuleModel> rules)
        {
            _rules = rules;
        }

        public List<string> Evaluate(List<UserAnswer> answers)
        {
            var triggeredRules = new List<string>();

            foreach (var rule in _rules)
            {
                int matchCount = 0;

                foreach (var condition in rule.Conditions)
                {
                    var answer = answers.FirstOrDefault(a => a.QuestionId == condition.QuestionId);
                    if (answer != null && answer.Answer.Equals(condition.ExpectedAnswer, StringComparison.OrdinalIgnoreCase))
                    {
                        matchCount++;
                    }
                }

                if (matchCount >= rule.MinimumMatchesRequired)
                {
                    triggeredRules.Add(rule.Title); // می‌تونه code هم باشه
                }
            }

            return triggeredRules;
        }
    }

}
