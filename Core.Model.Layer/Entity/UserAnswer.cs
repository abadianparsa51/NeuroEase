using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Model.Layer.Entity
{
    public class UserAnswer
    {
        public int Id { get; set; }
        public int QuestionId { get; set; }
        public string Answer { get; set; }
        public string UserId { get; set; }
        public string SessionId { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
