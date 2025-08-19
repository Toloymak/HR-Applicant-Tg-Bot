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
    private readonly ILogger<BotMessageService> _logger;

    public BotMessageService(
        ITelegramBotClient tgClient,
        WelcomeCommunicationService welcomeService,
        CancelApplicationButtonService cancelButtonService,
        ApplicationService applicationService,
        ILogger<BotMessageService> logger)
    {
        _tgClient = tgClient;
        _welcomeService = welcomeService;
        _cancelButtonService = cancelButtonService;
        _applicationService = applicationService;
        _logger = logger;
    }

    /// <summary>
    /// Sends a start message with user context
    /// </summary>
    public async Task SendStartMessageAsync(
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
    public async Task SendHelpMessageAsync(
        long chatId, 
        Guid? botUserId, 
        CancellationToken cancellationToken)
    {
        var helpText = "🤖 **Available Commands:**\n\n" +
                      "/start - Start using the bot\n" +
                      "/help - Show this help message\n" +
                      "/reset - Reset and start over\n\n" +
                      "💡 *Use /start to navigate through available positions.*";

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
    public async Task SendResetMessageAsync(
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
                var keyboard = new InlineKeyboardMarkup(new[]
                {
                    new[]
                    {
                        InlineKeyboardButton.WithCallbackData(
                            text: "✅ Yes, Reset Everything",
                            callbackData: new ConfirmResetCallback().ToTgString().ToString()
                        )
                    },
                    new[]
                    {
                        InlineKeyboardButton.WithCallbackData(
                            text: "❌ No, Keep Current Progress",
                            callbackData: new KeepProgressCallback().ToTgString().ToString()
                        )
                    }
                });

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
}
