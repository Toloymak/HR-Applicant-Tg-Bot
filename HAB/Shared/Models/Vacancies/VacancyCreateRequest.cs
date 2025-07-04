namespace Shared.Models.Vacancies;

public record VacancyQuestion
{
    public required Guid? Id { get; set; }
    public required string Text { get; set; }
    public required IQuestionAnswer Answer { get; set; }
}

public interface IQuestionAnswer
{
        
}
    
public record TextQuestionAnswer : IQuestionAnswer
{
}
    
public record YesNoQuestionAnswer : IQuestionAnswer
{
}
    
public record YesNoWithRequiredCorrectQuestionAnswer : IQuestionAnswer
{
    public required bool Expected { get; set; }
    public required string UnexpectedAnswerRejectText { get; set; } = string.Empty;
}