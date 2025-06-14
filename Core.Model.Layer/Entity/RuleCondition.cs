using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Model.Layer.Entity
{
    public class RuleCondition
    {
        public int Id { get; set; }

        public int DiagnosticRuleId { get; set; }

        public int QuestionId { get; set; }

        public string ExpectedAnswer { get; set; }

        public DiagnosticRule DiagnosticRule { get; set; }
    }
}
