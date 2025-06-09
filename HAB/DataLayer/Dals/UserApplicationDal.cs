using Shared.Models;

namespace DataLayer.Dals;

public class UserApplicationDal
{
    public Guid Id { get; set; }

    public Guid VacancyId { get; set; }
    public VacancyDal? Vacancy { get; set; }

    public Guid? LastQuestionId { get; set; }
    public Question? LastQuestion { get; set; }

    public ApplicationStatus State { get; set; }

    public ICollection<AnswerDal>? Answers { get; set; }
}