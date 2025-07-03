using System.ComponentModel.DataAnnotations;

namespace DataLayer.Dals;

public class VacancyDal
{
    public Guid Id { get; set; }

    public int? HrId { get; set; }
    public HrUserDal? Hr { get; set; }

    [MaxLength(1000)]
    public required string Title { get; set; }
    
    [MaxLength(4000)]
    public required string Description { get; set; }

    public bool IsArchived { get; set; }
    
    [MaxLength(4000)]
    public required string DefaultRejectText { get; set; }

    [MaxLength(4000)]
    public required string FinishedApplicationText { get; set; }

    // Navigation to root question
    public Guid? RootQuestionId { get; set; }
    public QuestionDal? RootQuestion { get; set; }
    
    public required DateTime CreatedAt { get; set; }

    public HashSet<UserApplicationDal>? Applications { get; set; }
}