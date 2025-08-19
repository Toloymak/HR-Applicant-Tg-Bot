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
    private readonly ILogger<ApplyForVacancyCallbackHandler> _logger;

    public ApplyForVacancyCallbackHandler(
        ITelegramBotClient tg,
        BotUserService botUserService,
        ApplicationService applicationService,
        HrBotContext context,
        ILogger<ApplyForVacancyCallbackHandler> logger)
    {
        _tg = tg;
        _botUserService = botUserService;
        _applicationService = applicationService;
        _context = context;
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
                      $"📝 Your application has been created and saved. " +
                      $"Next, you'll be asked a series of questions to complete your application.\n\n" +
                      $"💡 *Tip: You can return to complete your application later if needed.*",
                parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                cancellationToken: ct
            );

            _logger.LogInformation(
                "User {TgId} ({Username}) started application {ApplicationId} for vacancy '{VacancyTitle}' ({VacancyId})",
                botUser.TgId, botUser.TgName, application.Id, vacancy.Title, vacancy.Id);

            // TODO: Start the question flow
            // This will be implemented in the next step to guide users through questions
            await _tg.SendMessage(
                chatId: chatId,
                text: "🔄 Question flow will be implemented next. Stay tuned!",
                cancellationToken: ct
            );
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

