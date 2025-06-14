using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Model.Layer.Entity
{
    public class Answer
    {
        public int Id { get; set; }
        public int? QuestionId { get; set; }
        public bool Response { get; set; }
        public string SessionId { get; set; }
        public Question Question { get; set; }
        public string Text { get; set; }
    }
}
    