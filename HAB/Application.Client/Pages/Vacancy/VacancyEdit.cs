namespace Application.Client.Pages.Vacancy;

public record VacancyEdit
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string DefaultRejectText { get; set; } = string.Empty;
    public string DefaultAcceptedToReviewText { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;

    public List<QuestionEdit> Questions { get; set; } = [QuestionEdit.NewQuestion()];

    public void AddNewQuestion() => Questions.Add(QuestionEdit.NewQuestion(Questions));

    public void ReorderQuestions()
    {
        var newList = new List<QuestionEdit>();
        var order = 1;

        foreach (var question in Questions.OrderBy(x => x.OrderNumber))
        {
            question.OrderNumber = order++;
            newList.Add(question);
        }
        
        Questions = newList;
    }

    public void RemoveQuestion(QuestionEdit question)
    {
        Questions.Remove(question);
        ReorderQuestions();
    }
} 