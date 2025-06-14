using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Model.Layer.Entity
{
    public class Question
    {
        public int Id { get; set; }
        [Required]
        public string Section { get; set; }
        [Required]
        public string Text { get; set; }
        public int Order { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
