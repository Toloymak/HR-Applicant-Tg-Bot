using Shared.Models.Vacancies;

namespace Application.Client.Pages.Vacancy;

public static class QuestionsExtensions
{
    internal static IQuestionAnswerDto MapToDto(
        this IAnswerProperties answer)
    {
        if (answer is YesNoAnswer yesNoAnswer)
        {
            return yesNoAnswer.RequiredAnswer switch
            {
                null => new YesNoQuestionAnswerDto(),
                { } requiredAnswer => new YesNoWithRequiredCorrectQuestionAnswerDto
                {
                    Expected = requiredAnswer.ExpectedAnswer,
                    UnexpectedAnswerRejectText = requiredAnswer.UnexpectedAnswerRejectText
                }
            };
        }
            
        return new TextQuestionAnswerDto();
    }
}