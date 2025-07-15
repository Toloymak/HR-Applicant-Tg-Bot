using System.Text.Json.Serialization;

namespace Shared.Models.Vacancies;

public record VacancyQuestionDto
{
    public required Guid? Id { get; set; }
    public required string Text { get; set; }
    public required int OrderNumber { get; set; }
    public required IQuestionAnswerDto Answer { get; set; }
}


[JsonPolymorphic(TypeDiscriminatorPropertyName = "$type")]
[JsonDerivedType(typeof(TextQuestionAnswerDto), "text")]
[JsonDerivedType(typeof(YesNoQuestionAnswerDto), "yesno")]
[JsonDerivedType(typeof(YesNoWithRequiredCorrectQuestionAnswerDto), "yesno_required")]
public interface IQuestionAnswerDto
{
        
}
    
public record TextQuestionAnswerDto : IQuestionAnswerDto
{
}
    
public record YesNoQuestionAnswerDto : IQuestionAnswerDto
{
}
    
public record YesNoWithRequiredCorrectQuestionAnswerDto : IQuestionAnswerDto
{
    public required bool Expected { get; set; }
    public required string UnexpectedAnswerRejectText { get; set; } = string.Empty;
}