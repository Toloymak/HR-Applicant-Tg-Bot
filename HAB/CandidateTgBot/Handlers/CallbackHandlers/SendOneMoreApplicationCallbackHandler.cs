using CandidateTgBot.Services;
using CandidateTgBot.Services.CommunicationServices;
using CandidateTgBot.Services.DataProviders;
using CandidateTgBot.Types.Callbacks;
using DataLayer.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shared.Models;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace CandidateTgBot.Handlers.CallbackHandlers;

public class SendOneMoreApplicationCallbackHandler : ICallbackHandler<SendOneMoreApplicationCallback>
{
    private readonly ITelegramBotClient _tg;
    private readonly WelcomeCommunicationService _welcomeService;
    private readonly ILogger<SendOneMoreApplicationCallbackHandler> _logger;
    private readonly ISendUnableToIdentifyMessage _sendUnableToIdentifyMessage;
    private readonly IProvideUserFromCallback _userProvider;

    public SendOneMoreApplicationCallbackHandler(
        ITelegramBotClient tg,
        BotUserService botUserService,
        ApplicationService applicationService,
        HrBotContext context,
        WelcomeCommunicationService welcomeService,
        ILogger<SendOneMoreApplicationCallbackHandler> logger,
        ISendUnableToIdentifyMessage sendUnableToIdentifyMessage,
        IProvideUserFromCallback userProvider)
    {
        _tg = tg;
        _welcomeService = welcomeService;
        _logger = logger;
        _sendUnableToIdentifyMessage = sendUnableToIdentifyMessage;
        _userProvider = userProvider;
    }

    public async Task Handle(
        long chatId,
        SendOneMoreApplicationCallback command,
        CallbackQuery query,
        CancellationToken ct)
    {
        var botUserId = await _userProvider.GetBotUserId(query, ct);
        if (botUserId is null)
        {
            await _sendUnableToIdentifyMessage.Send(chatId, ct);
            return;
        }

        // Show available vacancies
        await _welcomeService.SendWelcomeMessageAsync(chatId, botUserId, ct);

        await _tg.AnswerCallbackQuery(
            callbackQueryId: query.Id,
            text: "Showing available positions!",
            showAlert: false,
            cancellationToken: ct
        );

        _logger.LogInformation(
            "User {BotUserId} marked application {ApplicationId} as completed and started new",
            botUserId, command.CompletedApplicationId);
    }
}