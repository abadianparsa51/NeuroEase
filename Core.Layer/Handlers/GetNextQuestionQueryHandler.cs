using MediatR;
using System.Threading;
using System.Threading.Tasks;
using Core.Model.Layer.Entity;
using Core.Layer.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Application.Layer.Querrys;

public class GetNextQuestionQueryHandler : IRequestHandler<GetNextQuestionQuery, Question>
{
    private readonly MentalHealthDbContext _context;

    public GetNextQuestionQueryHandler(MentalHealthDbContext context)
    {
        _context = context;
    }

    public async Task<Question> Handle(GetNextQuestionQuery request, CancellationToken cancellationToken)
    {
        var answeredQuestionIds = await _context.UserAnswers
       .Where(ua => ua.UserId == request.UserId)
       .Select(ua => ua.QuestionId)
       .ToListAsync(cancellationToken);

        var nextQuestion = await _context.Questions
            .Where(q => !answeredQuestionIds.Contains(q.Id))
            .OrderBy(q => q.Order)
            .FirstOrDefaultAsync(cancellationToken);

        return nextQuestion;
    }
}
