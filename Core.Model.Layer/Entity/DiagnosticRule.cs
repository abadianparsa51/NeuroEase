using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Model.Layer.Entity
{
    public class DiagnosticRule
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int MinimumMatchesRequired { get; set; }
        public ICollection<RuleCondition> Conditions { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
