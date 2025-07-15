using System.ComponentModel.DataAnnotations;

namespace DataLayer.Dals;

public class ConditionDal
{
    public Guid Id { get; set; }

    public Guid QuestionId { get; set; }
    public QuestionDal? Question { get; set; }

    [MaxLength(4000)]
    public required IAnswerCondition Answer { get; set; } 

    public bool IsCritical { get; set; }

    public bool? CriticalText { get; set; }
}

public interface IAnswerCondition
{
    
}

public record YesNoAnswerCondition : IAnswerCondition
{
    public required bool Answer { get; set; }
}