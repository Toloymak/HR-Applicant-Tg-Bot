using System.ComponentModel.DataAnnotations;

namespace DataLayer.Dals;

public class AnswerDal
{
    public Guid Id { get; set; }

    public Guid UserApplicationId { get; set; }
    public UserApplicationDal? UserApplication { get; set; }

    public Guid QuestionId { get; set; }
    public QuestionDal? Question { get; set; }

    [MaxLength(200)]
    public string? AnswerText { get; set; }
}