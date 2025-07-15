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
    
    public bool IsActive { get; set; } = true;
    
    [MaxLength(4000)]
    public required string DefaultRejectText { get; set; }

    [MaxLength(4000)]
    public required string FinishedApplicationText { get; set; }

    public ICollection<QuestionDal>? Questions { get; set; }
    
    public required DateTime CreatedAt { get; set; }

    public ICollection<UserApplicationDal>? Applications { get; set; }
}