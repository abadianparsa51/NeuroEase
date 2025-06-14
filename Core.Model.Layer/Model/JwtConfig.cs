using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Model.Layer.Model
{
    public class JwtConfig
    {
        public string Secret { get; set; }
        public int ExpiryInDays { get; set; }
    }
}
