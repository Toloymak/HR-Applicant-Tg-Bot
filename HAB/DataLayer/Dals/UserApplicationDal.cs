using Shared.Models;

namespace DataLayer.Dals;

public class UserApplicationDal
{
    public Guid Id { get; set; }

    /// <summary>
    /// Foreign key to the BotUser who submitted this application
    /// </summary>
    public Guid BotUserId { get; set; }
    public BotUserDal BotUser { get; set; } = null!;

    public Guid VacancyId { get; set; }
    public VacancyDal? Vacancy { get; set; }

    public Guid? LastQuestionId { get; set; }
    public QuestionDal? LastQuestion { get; set; }

    public ApplicationStatus State { get; set; }
    
    public long ChatId { get; set; }

    /// <summary>
    /// When the user started filling out this application
    /// </summary>
    public required DateTime StartDate { get; set; }

    /// <summary>
    /// Last time the user interacted with this application
    /// </summary>
    public required DateTime LastActivity { get; set; }

    public ICollection<ApplicationAnswerDal>? Answers { get; set; }
}