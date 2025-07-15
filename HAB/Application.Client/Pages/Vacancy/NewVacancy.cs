namespace Application.Client.Pages.Vacancy;

public class NewVacancy
{
    public string Title { get; set; } = String.Empty;
    public string Description { get; set; } = String.Empty;
    
    public string DefaultRejectText { get; set; } = String.Empty;
    public string DefaultAcceptedToReviewText { get; set; } = String.Empty;
    
    public List<QuestionEdit> Questions { get; set; } = [];
    
    public void AddNewQuestion() => Questions.Add(QuestionEdit.NewQuestion());
    
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
