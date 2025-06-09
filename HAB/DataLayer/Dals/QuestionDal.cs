using System.ComponentModel.DataAnnotations;
using Shared.Models;

namespace DataLayer.Dals;

public class QuestionDal
{
    public Guid Id { get; set; }

    [MaxLength(4000)]
    public required string Text { get; set; } = string.Empty;
    
    public required AnswerType Answer { get; set; }

    public Guid? NextQuestionId { get; set; }
    public QuestionDal? NextQuestion { get; set; }

    public ICollection<ConditionDal>? Conditions { get; set; }
}