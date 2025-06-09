using DataLayer.Contexts;
using DataLayer.Dals;
using Microsoft.EntityFrameworkCore;
using Shared.Models;

namespace Domain.Services;

public class VacationCreator
{
    private readonly HrBotContext _context;

    public VacationCreator(HrBotContext context)
    {
        _context = context;
    }

    public async Task CreateVacation(CancellationToken ct)
    {
        var randomUser = await _context.BotUsers.FirstAsync(ct);
        
        var vacation = new VacancyDal
        {
            HrId = randomUser.Id,
            RootQuestion = null,
            DefaultRejectText = "Thank you for your application! Unfortunately, we are looking for a candidate with a different skill set.",
            FinishedApplicationText = "Our HR will contact you soon!",
        };
        
        var driverLicenseQuestionId = new Guid("00000000-0000-0000-0000-000000000001");

        vacation.RootQuestion = new QuestionDal()
        {
            Text = "Do you want to apply for a job?",
            Answer = AnswerType.YesNo,
            NextQuestion = new QuestionDal()
            {
                Text = "What is your name?",
                Answer = AnswerType.Text,
                NextQuestion = new QuestionDal()
                {
                    Id = driverLicenseQuestionId,
                    Text = "Do you have a drive license?",
                    Answer = AnswerType.YesNo,
                    NextQuestion = new QuestionDal()
                    {
                        Text = "How many years of experience do you have?",
                        Answer = AnswerType.Text,
                        Conditions =
                        [
                            new ConditionDal
                            {
                                QuestionId = driverLicenseQuestionId,
                                Answer = new YesNoAnswerCondition()
                                {
                                    Answer = true
                                }
                            }
                        ],
                        NextQuestion = new QuestionDal()
                        {
                            Text = "What is your experience?",
                            Answer = AnswerType.Text,
                            NextQuestion = null
                        }
                    }
                }
            }
        };
    }
}