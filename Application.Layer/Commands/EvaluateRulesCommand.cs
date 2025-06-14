using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Model.Layer.Entity;
using MediatR;

namespace Application.Layer.Commands
{
    public class EvaluateRulesCommand : IRequest<List<string>>
    {
        public List<UserAnswer> Answers { get; set; }
    }
    public class DiagnosisResult
    {
        public string DiagnosisType { get; set; }
        public string DetailedResult { get; set; }
    }
}
