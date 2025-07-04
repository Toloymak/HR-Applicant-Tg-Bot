using Shared.Models.Vacancies;

namespace Application.Client.Pages.Vacancy;

public static class QuestionsExtensions
{
    internal static IQuestionAnswer MapToDto(this IAnswerProperties answer)
    {
        if (answer is YesNoAnswer yesNoAnswer)
        {
            return yesNoAnswer.RequiredAnswer switch
            {
                null => new YesNoQuestionAnswer(),
                { } requiredAnswer => new YesNoWithRequiredCorrectQuestionAnswer
                {
                    Expected = requiredAnswer.ExpectedAnswer,
                    UnexpectedAnswerRejectText = requiredAnswer.UnexpectedAnswerRejectText
                }
            };
        }
            
        return new TextQuestionAnswer();
    }
}