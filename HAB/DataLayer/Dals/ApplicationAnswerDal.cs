using System.ComponentModel.DataAnnotations;

namespace DataLayer.Dals;

public class ApplicationAnswerDal
{
    public Guid Id { get; set; }

    public Guid UserApplicationId { get; set; }
    public UserApplicationDal? UserApplication { get; set; }

    public Guid QuestionId { get; set; }
    public QuestionDal? Question { get; set; }

    public required IQuestionAnswerValue AnswerValue { get; set; }
}

public interface IQuestionAnswerValue
{
    string Type { get; }
}

public record BooleanAnswerValue : IQuestionAnswerValue
{
    public const string TypeName = "boolean";
    public string Type => TypeName;
    
    public required bool Value { get; set; }
}

public record TextAnswerValue : IQuestionAnswerValue
{
    public const string TypeName = "text";
    public string Type => TypeName;
    
    [MaxLength(2000)]
    public required string Value { get; set; }
}
