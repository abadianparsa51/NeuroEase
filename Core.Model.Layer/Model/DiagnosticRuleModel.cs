using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Model.Layer.Model
{
    public class DiagnosticRuleModel
    {
        public string Code { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public List<RuleConditionModel> Conditions { get; set; }
        public int MinimumMatchesRequired { get; set; }
    }
}
