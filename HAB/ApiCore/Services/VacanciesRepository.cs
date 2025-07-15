using System.Data;
using DataLayer.Contexts;
using DataLayer.Dals;
using Microsoft.EntityFrameworkCore;
using Shared.Models;
using LanguageExt;
using Shared.Models.Vacancies;

namespace Domain.Services;

public class VacanciesRepository
{
    private readonly HrBotContext _context;

    public VacanciesRepository(HrBotContext context)
    {
        _context = context;
    }

    public async Task<PaginationResult<VacancyListItem>> GetList(
        Pagination pagination,
        VacancyListFilter filter,
        CancellationToken ct)
    {
        var pageNumber = pagination.PageNumber < 1 ? 1 : pagination.PageNumber;
        var pageSize = pagination.PageSize < 1 ? 10 : pagination.PageSize;
        var skip = (pageNumber - 1) * pageSize;

        var query = _context.Vacancies
            .AsNoTracking()
            .Where(x => filter.IncludeArchived || !x.IsArchived)
            .Select(x => new VacancyListItem
            {
                Id = x.Id,
                Title = x.Title,
                HrId = x.HrId,
                HrName = x.Hr != null ? x.Hr.Alias : null,
                ApplicationsCount = x.Applications!.Count,
                CreatedAt = x.CreatedAt
            });

        var totalCount = await query.CountAsync(ct);
        var data = await query
            .Skip(skip)
            .Take(pageSize)
            .ToListAsync(ct);

        return new PaginationResult<VacancyListItem>
        {
            Data = data,
            TotalCount = totalCount,
            PageNumber = pagination.PageNumber
        };
    }

    public async Task<Either<Exception, Guid>> Create(
        CreateVacancyRequest request, CancellationToken ct)
    {
        try
        {
            var vacancy = new VacancyDal
            {
                Id = Guid.CreateVersion7(),
                Title = request.Name,
                Description = request.Description,
                DefaultRejectText = request.DefaultRejectText,
                FinishedApplicationText = request.DefaultAcceptedToReviewText,
                CreatedAt = DateTime.UtcNow,
                Questions = request.Questions
                    .Select(question => new QuestionDal
                    {
                        Id = question.Id ?? Guid.CreateVersion7(),
                        Text = question.Text,
                        OrderNumber = question.OrderNumber,
                        Answer = MapToDal(question.Answer),
                    })
                    .ToHashSet()
            };
            _context.Vacancies.Add(vacancy);
            await _context.SaveChangesAsync(ct);
            return vacancy.Id;
        }
        catch (Exception ex)
        {
            return ex;
        }
    }

    private IAnswerTypeDal MapToDal(IQuestionAnswerDto questionAnswer)
        => questionAnswer switch
        {
            TextQuestionAnswerDto => new TextAnswerTypeDal(),
            YesNoQuestionAnswerDto => new YesNoAnswerTypeDal(),
            YesNoWithRequiredCorrectQuestionAnswerDto yesNoWithRequired => new YesNoWithRequiredCorrectAnswerTypeDal()
            {
                Expected = yesNoWithRequired.Expected,
                UnexpectedAnswerRejectText = yesNoWithRequired.UnexpectedAnswerRejectText
            },
            _ => throw new ArgumentException("Unknown question answer type", nameof(questionAnswer))
        };

    public async Task<Either<Exception, Guid>> Edit(EditVacancyRequest request, CancellationToken ct)
    {
        try
        {
            using var trans = await _context.Database.BeginTransactionAsync(IsolationLevel.Serializable, ct);
            var vacancy = await _context.Vacancies
                .Include(v => v.Questions)
                .FirstOrDefaultAsync(v => v.Id == request.Id, ct);

            if (vacancy == null)
                return new Exception("Vacancy not found");

            vacancy.Title = request.Name;
            vacancy.Description = request.Description;
            vacancy.DefaultRejectText = request.DefaultRejectText;
            vacancy.FinishedApplicationText = request.DefaultAcceptedToReviewText;

            // _context.Add(new QuestionDal()
            // {
            //     Id = Guid.CreateVersion7(),
            //     Text = "New question",
            //     OrderNumber = 1,
            //     VacancyId = vacancy.Id,
            //     // Answer = new TextAnswerTypeDal()
            // });

            // --- Questions update logic ---
            var incomingQuestions = request.Questions.ToList();
            var existingQuestions = vacancy.Questions!.ToList();

            // Remove questions not present in the request
            var incomingIds = incomingQuestions.Where(q => q.Id.HasValue)
                .Select(q => q.Id)
                .Cast<Guid>()
                .ToHashSet();
            var toRemove = existingQuestions
                .Where(q => !incomingIds.Contains(q.Id))
                .ToList();
            foreach (var q in toRemove)
            {
                _context.Remove(q);
            }

            // Update existing and add new
            foreach (var incoming in incomingQuestions)
            {
                if (incoming.Id.HasValue)
                {
                    var existing = existingQuestions
                        .FirstOrDefault(q => q.Id == incoming.Id.Value);
                    if (existing != null)
                    {
                        // Update properties
                        existing.Text = incoming.Text;
                        existing.OrderNumber = incoming.OrderNumber;
                        existing.Answer = MapToDal(incoming.Answer);
                        continue;
                    }
                }

                // Add new question
                _context.Add(new QuestionDal
                {
                    Id = incoming.Id ?? Guid.CreateVersion7(),
                    Text = incoming.Text,
                    VacancyId = vacancy.Id,
                    OrderNumber = incoming.OrderNumber,
                    Answer = MapToDal(incoming.Answer),
                });
            }

            await _context.SaveChangesAsync(ct);

            await trans.CommitAsync(ct);
            return vacancy.Id;
        }
        catch (Exception ex)
        {
            return ex;
        }
    }

    public async Task<Either<Exception, VacancyDetailsDto>> GetDetailsById(Guid id, CancellationToken ct)
    {
        try
        {
            var vacancy = await _context.Vacancies
                .Where(x => x.Id == id)
                .Select(x => new VacancyDetailsDto
                {
                    Id = x.Id,
                    Title = x.Title,
                    Description = x.Description,
                    HrName = x.Hr != null ? x.Hr.Alias : string.Empty,
                    ApplicationsCount = x.Applications != null ? x.Applications.Count : 0,
                    CreatedAt = x.CreatedAt,
                    DefaultRejectText = x.DefaultRejectText,
                    DefaultAcceptedToReviewText = x.FinishedApplicationText,
                    Questions = x.Questions.Select(q => new VacancyQuestionDto
                    {
                        Id = q.Id,
                        Text = q.Text,
                        OrderNumber = q.OrderNumber,
                        Answer = ToDto(q)
                    }).ToArray()
                })
                .FirstOrDefaultAsync(ct);
            if (vacancy == null)
                return new Exception("Vacancy not found");
            return vacancy;
        }
        catch (Exception ex)
        {
            return ex;
        }
    }

    private static IQuestionAnswerDto ToDto(QuestionDal q)
        => q.Answer switch
        {
            TextAnswerTypeDal => new TextQuestionAnswerDto(),
            YesNoAnswerTypeDal => new YesNoQuestionAnswerDto(),
            YesNoWithRequiredCorrectAnswerTypeDal yesNoWithRequired => 
                new YesNoWithRequiredCorrectQuestionAnswerDto
                {
                    Expected = yesNoWithRequired.Expected,
                    UnexpectedAnswerRejectText = yesNoWithRequired.UnexpectedAnswerRejectText
                },
            _ => new TextQuestionAnswerDto(),
        };
}