using CandidateTgBot.Services;
using CandidateTgBot.Services.CommunicationServices;
using CandidateTgBot.Services.DataProviders;
using CandidateTgBot.Types.Callbacks;
using DataLayer.Contexts;
using DataLayer.Dals;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shared.Models;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace CandidateTgBot.Handlers.CallbackHandlers;

public class ConfirmResetCallbackHandler : ICallbackHandler<ConfirmResetCallback>
{
    private readonly ITelegramBotClient _tg;
    private readonly ApplicationService _applicationService;
    private readonly BotMessageService _botMessageService;
    private readonly ILogger<ConfirmResetCallbackHandler> _logger;
    private readonly ISendUnableToIdentifyMessage _sendUnableToIdentifyMessage;
    private readonly IProvideUserFromCallback _userProvider;

    public ConfirmResetCallbackHandler(
        ITelegramBotClient tg,
        HrBotContext context,
        ApplicationService applicationService,
        BotUserService botUserService,
        BotMessageService botMessageService,
        ILogger<ConfirmResetCallbackHandler> logger,
        ISendUnableToIdentifyMessage sendUnableToIdentifyMessage,
        IProvideUserFromCallback userProvider)
    {
        _tg = tg;
        _applicationService = applicationService;
        _botMessageService = botMessageService;
        _logger = logger;
        _sendUnableToIdentifyMessage = sendUnableToIdentifyMessage;
        _userProvider = userProvider;
    }

    public async Task Handle(
        long chatId,
        ConfirmResetCallback command,
        CallbackQuery query,
        CancellationToken ct)
    {
        var botUser = await _userProvider.GetBotUser(query, ct);
        if (botUser is null)
        {
            await _sendUnableToIdentifyMessage.Send(chatId, ct);
            return;
        }

        // Get all active applications for the user
        var activeApplications = await _applicationService
            .GetUserActiveApplicationsAsync(botUser.Id, ct);

        // Cancel all active applications
        foreach (var application in activeApplications)
            await _applicationService.UpdateApplicationStatus(
                application.Id, ApplicationStatus.Canceled, ct);

        await SendResetCompletionMessage(chatId, ct, activeApplications, botUser);

        // Send fresh start message
        await _botMessageService.SendStartMessage(chatId, botUser.Id, ct);
    }

    private async Task SendResetCompletionMessage(
        long chatId,
        CancellationToken ct,
        List<UserApplicationDal> activeApplications,
        BotUserDal botUser)
    {
        if (activeApplications.Count > 0)
        {
            await _tg.SendMessage(
                chatId: chatId,
                text: $"🔄 **Reset Complete**\n\n" +
                      $"Canceled {activeApplications.Count} active application{(activeApplications.Count > 1 ? "s" : "")}.\n\n" +
                      $"Starting fresh...",
                parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                cancellationToken: ct
            );

            _logger.LogInformation(
                "User {TgId} ({Username}) reset and canceled {ApplicationCount} applications",
                botUser.TgId, botUser.TgName, activeApplications.Count);
        }
        else
        {
            await _tg.SendMessage(
                chatId: chatId,
                text: "🔄 **Reset Complete**\n\nStarting fresh...",
                parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                cancellationToken: ct
            );
        }
    }
}