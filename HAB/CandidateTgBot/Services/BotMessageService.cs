using CandidateTgBot.Services.CommunicationServices;
using CandidateTgBot.Types.Callbacks;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;

namespace CandidateTgBot.Services;

/// <summary>
/// Service to handle bot messages with user context
/// </summary>
public class BotMessageService
{
    private readonly ITelegramBotClient _tgClient;
    private readonly WelcomeCommunicationService _welcomeService;
    private readonly CancelApplicationButtonService _cancelButtonService;
    private readonly ApplicationService _applicationService;
    private readonly CurrentQuestionService _questionService;
    private readonly ILogger<BotMessageService> _logger;

    public BotMessageService(
        ITelegramBotClient tgClient,
        WelcomeCommunicationService welcomeService,
        CancelApplicationButtonService cancelButtonService,
        ApplicationService applicationService,
        CurrentQuestionService questionService,
        ILogger<BotMessageService> logger,
        ISendUnableToIdentifyMessage sendUnableToIdentifyMessage)
    {
        _tgClient = tgClient;
        _welcomeService = welcomeService;
        _cancelButtonService = cancelButtonService;
        _applicationService = applicationService;
        _questionService = questionService;
        _logger = logger;
    }

    /// <summary>
    /// Sends a start message with user context
    /// </summary>
    public async Task SendStartMessage(
        long chatId, 
        Guid? botUserId, 
        CancellationToken cancellationToken)
    {
        if (botUserId.HasValue)
        {
            // Check if user has active applications
            var activeApplication = await _applicationService.GetUserMostRecentActiveApplicationAsync(
                botUserId.Value, cancellationToken);

            if (activeApplication?.Vacancy != null)
            {
                var cancelKeyboard = await _cancelButtonService.CreateCancelButtonKeyboardAsync(
                    botUserId.Value, cancellationToken);

                await _tgClient.SendMessage(
                    chatId: chatId,
                    text: $"📋 **Active Application Found**\n\n" +
                          $"You are already working with vacancy **{activeApplication.Vacancy.Title}**.\n\n" +
                          $"💡 *Continue filling the application or cancel it to start a new one.*",
                    parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                    replyMarkup: cancelKeyboard,
                    cancellationToken: cancellationToken
                );
                return;
            }
        }

        await _tgClient.SendMessage(
            chatId: chatId,
            text: "🚀 Welcome! Let's get started with your job application.",
            cancellationToken: cancellationToken
        );
        
        await _welcomeService.SendWelcomeMessageAsync(chatId, botUserId, cancellationToken);
    }

    /// <summary>
    /// Sends a help message with user context (no cancel button)
    /// </summary>
    public async Task SendHelpMessage(
        long chatId, 
        Guid? botUserId, 
        CancellationToken cancellationToken)
    {
        var helpText = "🤖 **Available Commands:**\n\n" +
                      "/start - Start using the bot or browse positions\n" +
                      "/continue - Continue your current application\n" +
                      "/status - Show your application status and answers\n" +
                      "/help - Show this help message\n" +
                      "/reset - Reset and start over\n\n" +
                      "💡 *Use /start to browse positions, /continue to resume, or /status to check your progress.*";

        await _tgClient.SendMessage(
            chatId: chatId,
            text: helpText,
            parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
            cancellationToken: cancellationToken
        );
    }

    /// <summary>
    /// Sends a reset confirmation message with user context
    /// </summary>
    public async Task SendResetMessage(
        long chatId, 
        Guid? botUserId, 
        CancellationToken cancellationToken)
    {
        if (botUserId.HasValue)
        {
            // Check if user has active applications
            var hasActiveApplications = await _applicationService.HasUserActiveApplicationsAsync(
                botUserId.Value, cancellationToken);

            if (hasActiveApplications)
            {
                // Show confirmation dialog for reset
                var keyboard = new InlineKeyboardMarkup([
                    [
                        InlineKeyboardButton.WithCallbackData(
                            text: "✅ Yes, Reset Everything",
                            callbackData: new ConfirmResetCallback().ToTgString().ToString()
                        )
                    ],
                    [
                        TgButtonProvider.Applications.Status,
                    ]
                ]);

                await _tgClient.SendMessage(
                    chatId: chatId,
                    text: "⚠️ **Confirm Reset**\n\n" +
                          "This will cancel all your active applications and start fresh.\n\n" +
                          "Are you sure you want to reset everything?",
                    parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                    replyMarkup: keyboard,
                    cancellationToken: cancellationToken
                );
                return;
            }
        }

        // No active applications, proceed with normal reset
        await _tgClient.SendMessage(
            chatId: chatId,
            text: "🔄 Resetting... Let's start fresh!",
            cancellationToken: cancellationToken
        );
        
        await _welcomeService.SendWelcomeMessageAsync(chatId, botUserId, cancellationToken);
    }

    /// <summary>
    /// Sends a continue message - only works if user has active applications
    /// </summary>
    public async Task SendContinueMessage(
        long chatId, 
        Guid? botUserId, 
        CancellationToken cancellationToken)
    {
        if (botUserId.HasValue)
        {
            // Check if user has active applications
            var activeApplication = await _applicationService.GetUserMostRecentActiveApplicationAsync(
                botUserId.Value, cancellationToken);

            if (activeApplication?.Vacancy != null)
            {
                await _tgClient.SendMessage(
                    chatId: chatId,
                    text: $"📋 **Continuing Your Application**\n\n" +
                          $"You're working on: **{activeApplication.Vacancy.Title}**\n\n" +
                          $"🔄 *Let's continue where you left off...*",
                    parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                    cancellationToken: cancellationToken
                );

                _logger.LogInformation(
                    "User continued application {ApplicationId} for vacancy '{VacancyTitle}' in chat {ChatId}",
                    activeApplication.Id, activeApplication.Vacancy.Title, chatId);

                // Show the current question
                var questionSent = await _questionService.SendCurrentQuestionAsync(
                    chatId, activeApplication.Id, botUserId.Value, cancellationToken);

                if (!questionSent)
                {
                    await _tgClient.SendMessage(
                        chatId: chatId,
                        text: "❌ Unable to load your current question. Please try again later.",
                        cancellationToken: cancellationToken
                    );
                }

                return;
            }
        }

        // No active applications found
        await _tgClient.SendMessage(
            chatId: chatId,
            text: "📭 **No Active Applications**\n\n" +
                  "You don't have any active applications to continue.\n\n" +
                  "💡 *Use /start to browse available positions and begin a new application.*",
            parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
            cancellationToken: cancellationToken
        );

        _logger.LogInformation(
            "User tried to continue but has no active applications in chat {ChatId}",
            chatId);
    }
}
