using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Model.Layer.Entity;
using MediatR;

namespace Application.Layer.Querrys
{
    public class GetNextQuestionQuery : IRequest<Question>
    {
        public string UserId { get; }

        public GetNextQuestionQuery(string userId)
        {
            UserId = userId;
        }
    }
}
