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

public class ConfirmRevokeApplicationCallbackHandler
    : ICallbackHandler<ConfirmRevokeApplicationCallback>
{
    private readonly ITelegramBotClient _tg;
    private readonly ApplicationService _applicationService;
    private readonly HrBotContext _context;
    private readonly ILogger<ConfirmRevokeApplicationCallbackHandler> _logger;
    private readonly ISendUnableToIdentifyMessage _sendUnableToIdentifyMessage;
    private readonly IProvideUserFromCallback _userProvider;

    public ConfirmRevokeApplicationCallbackHandler(
        ITelegramBotClient tg,
        BotUserService botUserService,
        ApplicationService applicationService,
        HrBotContext context,
        ILogger<ConfirmRevokeApplicationCallbackHandler> logger,
        ISendUnableToIdentifyMessage sendUnableToIdentifyMessage,
        IProvideUserFromCallback userProvider)
    {
        _tg = tg;
        _applicationService = applicationService;
        _context = context;
        _logger = logger;
        _sendUnableToIdentifyMessage = sendUnableToIdentifyMessage;
        _userProvider = userProvider;
    }

    public async Task Handle(
        long chatId,
        ConfirmRevokeApplicationCallback command,
        CallbackQuery query,
        CancellationToken ct)
    {
        var botUserId = await _userProvider.GetBotUserId(query, ct);
        if (botUserId is null)
        {
            await _sendUnableToIdentifyMessage.Send(chatId, ct);
            return;
        }

        var application = await _context.UserApplications
            .Include(a => a.Vacancy)
            .FirstOrDefaultAsync(a =>
                    a.Id == command.ApplicationId
                    && a.BotUserId == botUserId.Value
                    && (a.State == ApplicationStatus.ReviewByHr),
                ct);

        if (application == null)
        {
            await _tg.SendMessage(
                chatId: chatId,
                text: "❌ **Application Not Found**\n\n" +
                      "The application you're trying to revoke could not be found or is not eligible for revocation.\n\n" +
                      "💡 *Only applications with 'Submitted for Review' or 'Review by HR' status can be revoked.*",
                parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                cancellationToken: ct
            );
            return;
        }

        await _applicationService.UpdateApplicationStatus(
            application.Id,
            ApplicationStatus.RevokedByUser,
            ct);

        await SendApplicationRevocationSuccessMessage(chatId, query, ct, application);

        _logger.LogInformation(
            "User {BotUserId} revoked application {ApplicationId} for vacancy '{VacancyTitle}' in chat {ChatId}",
            botUserId, application.Id, application.Vacancy?.Title, chatId);
    }

    private async Task SendApplicationRevocationSuccessMessage(
        long chatId,
        CallbackQuery query,
        CancellationToken ct,
        UserApplicationDal application)
    {
        await _tg.SendMessage(
            chatId: chatId,
            text: $"🗑️ **Application Revoked Successfully**\n\n" +
                  $"Your application for **{application.Vacancy?.Title}** has been revoked.\n\n" +
                  $"✅ The application has been removed from your active applications.",
            parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
            cancellationToken: ct
        );

        await _tg.AnswerCallbackQuery(
            callbackQueryId: query.Id,
            text: "Application revoked successfully!",
            showAlert: false,
            cancellationToken: ct
        );
    }
}