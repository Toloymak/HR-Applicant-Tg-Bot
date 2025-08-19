using CandidateTgBot.Services;
using CandidateTgBot.Types.Callbacks;
using DataLayer.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shared.Models;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace CandidateTgBot.Handlers.CallbackHandlers;

public class ApplyForVacancyCallbackHandler : ICallbackHandler<ApplyForVacancyCallback>
{
    private readonly ITelegramBotClient _tg;
    private readonly BotUserService _botUserService;
    private readonly ApplicationService _applicationService;
    private readonly HrBotContext _context;
    private readonly CurrentQuestionService _questionService;
    private readonly ILogger<ApplyForVacancyCallbackHandler> _logger;

    public ApplyForVacancyCallbackHandler(
        ITelegramBotClient tg,
        BotUserService botUserService,
        ApplicationService applicationService,
        HrBotContext context,
        CurrentQuestionService questionService,
        ILogger<ApplyForVacancyCallbackHandler> logger)
    {
        _tg = tg;
        _botUserService = botUserService;
        _applicationService = applicationService;
        _context = context;
        _questionService = questionService;
        _logger = logger;
    }

    public async Task Handle(
        long chatId,
        ApplyForVacancyCallback command,
        CallbackQuery query,
        CancellationToken ct)
    {
        try
        {
            // Ensure we have the user information
            if (query.From == null)
            {
                await _tg.SendMessage(
                    chatId: chatId,
                    text: "❌ Unable to identify user. Please try again.",
                    cancellationToken: ct
                );
                return;
            }

            // Get or create the bot user
            var botUser = await _botUserService.CreateOrUpdateUserAsync(query.From, ct);

            // Get vacancy information
            var vacancy = await _context.Vacancies
                .FirstOrDefaultAsync(v => v.Id == command.VacancyId && v.IsActive, ct);

            if (vacancy == null)
            {
                await _tg.SendMessage(
                    chatId: chatId,
                    text: "❌ Sorry, this vacancy is no longer available.",
                    cancellationToken: ct
                );
                return;
            }

            // Check if user already applied
            var hasApplied = await _applicationService.HasUserAppliedToVacancyAsync(
                botUser.Id, command.VacancyId, ct);

            if (hasApplied)
            {
                await _tg.SendMessage(
                    chatId: chatId,
                    text: $"📋 You have already applied to the **{vacancy.Title}** position. " +
                          "Your application is being reviewed by our HR team.",
                    parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                    cancellationToken: ct
                );
                return;
            }

            // Create the application
            var application = await _applicationService.CreateApplicationAsync(
                botUser.Id, command.VacancyId, ct);

            // Update application status to InProgress since user is starting
            await _applicationService.UpdateApplicationStatusAsync(
                application.Id, ApplicationStatus.InProgress, ct);

            // Send confirmation message
            await _tg.SendMessage(
                chatId: chatId,
                text: $"🎯 **Application Started!**\n\n" +
                      $"You are now applying for: **{vacancy.Title}**\n\n" +
                      $"📝 Let's begin with the first question...",
                parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                cancellationToken: ct
            );

            _logger.LogInformation(
                "User {TgId} ({Username}) started application {ApplicationId} for vacancy '{VacancyTitle}' ({VacancyId})",
                botUser.TgId, botUser.TgName, application.Id, vacancy.Title, vacancy.Id);

            // Start the question flow
            var questionSent = await _questionService.SendCurrentQuestionAsync(
                chatId, application.Id, botUser.Id, ct);

            if (!questionSent)
            {
                await _tg.SendMessage(
                    chatId: chatId,
                    text: "❌ Unable to load questions for this vacancy. Please contact support.",
                    cancellationToken: ct
                );
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, 
                "Error handling apply for vacancy callback for user {ChatId}, vacancy {VacancyId}",
                chatId, command.VacancyId);

            await _tg.SendMessage(
                chatId: chatId,
                text: "❌ An error occurred while processing your application. Please try again later.",
                cancellationToken: ct
            );
        }
    }
}

