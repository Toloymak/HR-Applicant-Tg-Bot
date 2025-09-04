using CandidateTgBot.Extensions;
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
using Telegram.Bot.Types.ReplyMarkups;

namespace CandidateTgBot.Handlers.CallbackHandlers;

public class CancelApplicationCallbackHandler : ICallbackHandler<CancelApplicationCallback>
{
    private readonly ITelegramBotClient _tg;
    private readonly HrBotContext _context;
    private readonly ILogger<CancelApplicationCallbackHandler> _logger;
    private readonly ISendUnableToIdentifyMessage _sendUnableToIdentifyMessage;
    private readonly IProvideUserFromCallback _userProvider;

    public CancelApplicationCallbackHandler(
        ITelegramBotClient tg,
        HrBotContext context,
        ApplicationService applicationService,
        BotUserService botUserService,
        ILogger<CancelApplicationCallbackHandler> logger,
        ISendUnableToIdentifyMessage sendUnableToIdentifyMessage,
        IProvideUserFromCallback userProvider)
    {
        _tg = tg;
        _context = context;
        _logger = logger;
        _sendUnableToIdentifyMessage = sendUnableToIdentifyMessage;
        _userProvider = userProvider;
    }

    public async Task Handle(
        long chatId,
        CancelApplicationCallback command,
        CallbackQuery query,
        CancellationToken ct)
    {
        var botUser = await _userProvider.GetBotUser(query, ct);
        if (botUser is null)
        {
            await _sendUnableToIdentifyMessage.Send(chatId, ct);
            return;
        }

        // Get the application with vacancy information
        var application = await _context.UserApplications
            .Include(a => a.Vacancy)
            .Where(x => x.State == ApplicationStatus.InProgress)
            .FirstOrDefaultAsync(a => a.Id == command.ApplicationId
                                      && a.BotUserId == botUser.Id, ct);

        if (application == null)
        {
            await _tg.SendMessage(
                chatId: chatId,
                text: "❌ Active application not found.",
                cancellationToken: ct
            );
            return;
        }

        // Show confirmation dialog
        var keyboard = new InlineKeyboardMarkup([
            TgButtonProvider.Applications.ConfirmCancel(command.ApplicationId).ToArray(),
            TgButtonProvider.Applications.Status.ToArray()
        ]);

        await ShowConfirmCancellationMsg(chatId, ct, application, keyboard);

        _logger.LogInformation(
            "User {TgId} ({Username}) requested to cancel application {ApplicationId} for vacancy '{VacancyTitle}'",
            botUser.TgId, botUser.TgName, application.Id, application.Vacancy?.Title);
    }

    private async Task ShowConfirmCancellationMsg(
        long chatId,
        CancellationToken ct,
        UserApplicationDal application,
        InlineKeyboardMarkup keyboard)
    {
        await _tg.SendMessage(
            chatId: chatId,
            text: $"🤔 **Confirm Application Cancellation**\n\n" +
                  $"Are you sure you want to cancel your application for **{application.Vacancy?.Title}**?\n\n" +
                  $"⚠️ *This action cannot be undone. You will need to start a new application if you change your mind.*",
            parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
            replyMarkup: keyboard,
            cancellationToken: ct
        );
    }
}