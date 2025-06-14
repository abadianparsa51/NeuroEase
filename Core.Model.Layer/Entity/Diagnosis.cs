using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Model.Layer.Entity
{
    public class Diagnosis
    {
        public int Id { get; set; }
        public string SessionId { get; set; }
        public string Result { get; set; }
        public int DiagnosticRuleId { get; set; }
        public string Code { get; set; }
        public string Title { get; set; }
        public DateTime CreatedAt { get; set; }
        public string UserId { get; set; }
    }
}
