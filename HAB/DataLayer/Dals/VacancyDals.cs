using System.ComponentModel.DataAnnotations;

namespace DataLayer.Dals;

public class VacancyDal
{
    public Guid Id { get; set; }

    public Guid HrId { get; set; }

    [MaxLength(4000)]
    public required string DefaultRejectText { get; set; }

    [MaxLength(4000)]
    public required string FinishedApplicationText { get; set; }

    // Navigation to root question
    public Guid? RootQuestionId { get; set; }
    public QuestionDal? RootQuestion { get; set; }

    public ICollection<UserApplicationDal>? Applications { get; set; }
}