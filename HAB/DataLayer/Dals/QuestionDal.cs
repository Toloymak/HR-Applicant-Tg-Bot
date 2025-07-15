using System.ComponentModel.DataAnnotations;
using Shared.Models;

namespace DataLayer.Dals;

public class QuestionDal
{
    public Guid Id { get; set; }

    [MaxLength(4000)]
    public required string Text { get; set; }
    
    public Guid VacancyId { get; set; }
    public VacancyDal? Vacancy { get; set; }
    
    public required int OrderNumber { get; set; }
    
    public required IAnswerTypeDal Answer { get; set; }

    public ICollection<ConditionDal>? Conditions { get; set; }
}

public interface IAnswerTypeDal
{
    string Type { get; }
}

public record TextAnswerTypeDal : IAnswerTypeDal
{
    public const string TypeName = "text";
    public string Type => TypeName;
}

public record YesNoAnswerTypeDal : IAnswerTypeDal
{
    public const string TypeName = "yesno";
    public string Type => TypeName;
}

public record YesNoWithRequiredCorrectAnswerTypeDal : IAnswerTypeDal
{
    public const string TypeName = "yesno_required";
    public string Type => TypeName;
    
    public required bool Expected { get; set; }
    public required string UnexpectedAnswerRejectText { get; set; } = string.Empty;
}
