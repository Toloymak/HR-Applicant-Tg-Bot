namespace Shared.Models;

public record Vacancy
{
    public Guid Id { get; set; }
    
    public required string Name { get; set; }
    public required string Description { get; set; }
    
    public required Guid HrId { get; set; }
    
    public Question? RootQuestion { get; set; }
    
    public required string NoText { get; set; }
    public required string YesText { get; set; }
}

public record BotUser
{
    public required Guid Id { get; set; }
    public required string TelegramId { get; set; }
    public required string Name { get; set; }
}

public record NewBotUser
{
    public required Guid Id { get; set; }
    
    public required string TgName { get; set; }
    public required string TgId { get; set; }
}

public record Question
{
    public required Guid Id { get; set; }
    public required string Text { get; set; }
    public required AnswerType Answer { get; set; }
    public required HashSet<Condition> Conditions { get; set; }
    public required Guid NextQuestionId { get; set; }
}

public record Condition
{
    public required Guid Id { get; set; }
    public required Guid QuestionId { get; set; }
    
    // DSL?
    public required string Answer { get; set; }
    
    // If it's true, the question is critical. if condition is false, we will stop application filling
    public required bool IsCritical { get; set; }
    public required string CriticalText { get; set; }
}

public enum AnswerType
{
    Text,
    YesNo,
    OneOf,
}

public record UserApplication
{
    public required Guid Id { get; set; }
    public required Guid VacationId { get; set; }
    public required HashSet<Answer> Answers { get; set; }
    public required Guid? LastQuestionId { get; set; }
    public required ApplicationStatus State { get; set; }
    public required string UserAnswer { get; set; }
}

public enum ApplicationStatus
{
    Created,
    InProgress,
    Completed,
    Rejected,
    Approved,
}

public record Answer
{
    public required Guid QuestionId { get; set; }
    public required string AnswerText { get; set; }
}