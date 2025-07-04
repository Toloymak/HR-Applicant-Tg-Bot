namespace Application.Client.Pages.Vacancy;

public record VacancyEdit
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string DefaultRejectText { get; set; } = string.Empty;
    public string DefaultAcceptedToReviewText { get; set; } = string.Empty;

    public List<QuestionEdit> Questions { get; set; } = [QuestionEdit.NewQuestion()];

    public void AddNewQuestion() => Questions.Add(QuestionEdit.NewQuestion());
} 