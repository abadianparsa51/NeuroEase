using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Model.Layer.Entity;

namespace Application.Layer.Interface
{
    public interface IRuleService
    {
        Task<List<DiagnosticRule>> EvaluateAsync(List<UserAnswer> answers);
        Task SaveDiagnosesAsync(string sessionId, string userId, List<DiagnosticRule> rules);
    }
}
