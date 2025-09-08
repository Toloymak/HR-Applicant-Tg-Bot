using CandidateTgBot.Services;
using DataLayer.Contexts;
using DataLayer.Dals;
using Microsoft.EntityFrameworkCore;
using Shared.Models;

namespace CandidateTgBot.Tests.Services;

public class ApplicationCompleterTests
{
    private HrBotContext CreateInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<HrBotContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        var context = new HrBotContext(options);
        
        // Seed test data
        SeedTestData(context);
        
        return context;
    }

    private void SeedTestData(HrBotContext context)
    {
        var vacancyId = Guid.NewGuid();
        var botUserId = Guid.NewGuid();
        var applicationId = Guid.NewGuid();
        
        // Create vacancy with 3 questions
        var vacancy = new VacancyDal
        {
            Id = vacancyId,
            Title = "Test Vacancy",
            Description = "Test Description",
            DefaultRejectText = "Rejected",
            FinishedApplicationText = "Finished",
            CreatedAt = DateTime.UtcNow,
            IsActive = true,
            IsArchived = false
        };
        
        var questions = new[]
        {
            new QuestionDal
            {
                Id = Guid.NewGuid(),
                VacancyId = vacancyId,
                Text = "Question 1",
                OrderNumber = 1,
                Answer = new TextAnswerTypeDal()
            },
            new QuestionDal
            {
                Id = Guid.NewGuid(),
                VacancyId = vacancyId,
                Text = "Question 2",
                OrderNumber = 2,
                Answer = new TextAnswerTypeDal()
            },
            new QuestionDal
            {
                Id = Guid.NewGuid(),
                VacancyId = vacancyId,
                Text = "Question 3",
                OrderNumber = 3,
                Answer = new TextAnswerTypeDal()
            }
        };
        
        var botUser = new BotUserDal
        {
            Id = botUserId,
            TgId = 123456,
            TgName = "TestUser",
            LastActivity = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow
        };
        
        context.Vacancies.Add(vacancy);
        context.Questions.AddRange(questions);
        context.BotUsers.Add(botUser);
        context.SaveChanges();
    }

    [Fact]
    public async Task TryCompleteApplication_WithAllQuestionsAnswered_ShouldCompleteApplication()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var completer = new ApplicationCompleter(context);
        
        var vacancy = context.Vacancies.Include(v => v.Questions).First();
        var botUser = context.BotUsers.First();
        var questions = vacancy.Questions!.ToList();
        
        var application = new UserApplicationDal
        {
            Id = Guid.NewGuid(),
            BotUserId = botUser.Id,
            VacancyId = vacancy.Id,
            State = ApplicationStatus.InProgress,
            ChatId = 123456,
            StartDate = DateTime.UtcNow,
            LastActivity = DateTime.UtcNow
        };
        
        // Add answers for all questions
        var answers = questions.Select(q => new ApplicationAnswerDal
        {
            Id = Guid.NewGuid(),
            UserApplicationId = application.Id,
            QuestionId = q.Id,
            AnswerValue = new DataLayer.Dals.TextAnswerValue { Value = "Test answer" }
        }).ToList();
        
        context.UserApplications.Add(application);
        context.Answers.AddRange(answers);
        await context.SaveChangesAsync();
        
        // Act
        await completer.TryCompleteApplication(botUser.Id, CancellationToken.None);
        
        // Assert
        var updatedApplication = await context.UserApplications.FirstAsync(a => a.Id == application.Id);
        Assert.Equal(ApplicationStatus.ReviewByHr, updatedApplication.State);
    }
    
    [Fact]
    public async Task TryCompleteApplication_WithPartialAnswers_ShouldNotCompleteApplication()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var completer = new ApplicationCompleter(context);
        
        var vacancy = context.Vacancies.Include(v => v.Questions).First();
        var botUser = context.BotUsers.First();
        var questions = vacancy.Questions!.ToList();
        
        var application = new UserApplicationDal
        {
            Id = Guid.NewGuid(),
            BotUserId = botUser.Id,
            VacancyId = vacancy.Id,
            State = ApplicationStatus.InProgress,
            ChatId = 123456,
            StartDate = DateTime.UtcNow,
            LastActivity = DateTime.UtcNow
        };
        
        // Add answers for only 2 out of 3 questions
        var answers = questions.Take(2).Select(q => new ApplicationAnswerDal
        {
            Id = Guid.NewGuid(),
            UserApplicationId = application.Id,
            QuestionId = q.Id,
            AnswerValue = new DataLayer.Dals.TextAnswerValue { Value = "Test answer" }
        }).ToList();
        
        context.UserApplications.Add(application);
        context.Answers.AddRange(answers);
        await context.SaveChangesAsync();
        
        // Act
        await completer.TryCompleteApplication(botUser.Id, CancellationToken.None);
        
        // Assert
        var updatedApplication = await context.UserApplications.FirstAsync(a => a.Id == application.Id);
        Assert.Equal(ApplicationStatus.InProgress, updatedApplication.State);
    }
    
    [Fact]
    public async Task TryCompleteApplication_WithNoAnswers_ShouldNotCompleteApplication()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var completer = new ApplicationCompleter(context);
        
        var vacancy = context.Vacancies.First();
        var botUser = context.BotUsers.First();
        
        var application = new UserApplicationDal
        {
            Id = Guid.NewGuid(),
            BotUserId = botUser.Id,
            VacancyId = vacancy.Id,
            State = ApplicationStatus.InProgress,
            ChatId = 123456,
            StartDate = DateTime.UtcNow,
            LastActivity = DateTime.UtcNow
        };
        
        context.UserApplications.Add(application);
        await context.SaveChangesAsync();
        
        // Act
        await completer.TryCompleteApplication(botUser.Id, CancellationToken.None);
        
        // Assert
        var updatedApplication = await context.UserApplications.FirstAsync(a => a.Id == application.Id);
        Assert.Equal(ApplicationStatus.InProgress, updatedApplication.State);
    }
    
    [Fact]
    public async Task TryCompleteApplication_WithCompletedApplication_ShouldNotChangeState()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var completer = new ApplicationCompleter(context);
        
        var vacancy = context.Vacancies.First();
        var botUser = context.BotUsers.First();
        
        var application = new UserApplicationDal
        {
            Id = Guid.NewGuid(),
            BotUserId = botUser.Id,
            VacancyId = vacancy.Id,
            State = ApplicationStatus.ReviewByHr, // Already completed
            ChatId = 123456,
            StartDate = DateTime.UtcNow,
            LastActivity = DateTime.UtcNow
        };
        
        context.UserApplications.Add(application);
        await context.SaveChangesAsync();
        
        // Act
        await completer.TryCompleteApplication(botUser.Id, CancellationToken.None);
        
        // Assert
        var updatedApplication = await context.UserApplications.FirstAsync(a => a.Id == application.Id);
        Assert.Equal(ApplicationStatus.ReviewByHr, updatedApplication.State);
    }
}
