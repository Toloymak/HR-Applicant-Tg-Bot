using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace DataLayer.Dals;

public class ConditionDal
{
    public Guid Id { get; set; }

    public Guid QuestionId { get; set; }
    public QuestionDal? Question { get; set; }

    [MaxLength(4000)]
    public required AnswerCondition Answer { get; set; } 

    public bool IsCritical { get; set; }

    public bool? CriticalText { get; set; }
}

public abstract record AnswerCondition
{
    
}

public record YesNoAnswerCondition : AnswerCondition
{
    public required bool Answer { get; set; }
}