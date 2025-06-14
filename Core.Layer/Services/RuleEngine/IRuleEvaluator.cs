using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Model.Layer.Entity;

namespace Core.Layer.Services.RuleEngine
{
    public interface IRuleEvaluator
    {
        List<string> Evaluate(List<UserAnswer> answers);

    }
}
