using CandateTgBot.Shared.Services;
using CandidateTgBot.Extensions;
using CandidateTgBot.Services;
using CandidateTgBot.Services.DataProviders;
using CandidateTgBot.Types.Callbacks;
using DataLayer.Contexts;
using Microsoft.EntityFrameworkCore;
using Shared.Models;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;

namespace CandidateTgBot.Services.CommunicationServices;

public class VacancyListCommunicationService
{
    private readonly ITelegramBotClient _botClient;
    private readonly IProvideAvailablePositions _availablePositions;
    private readonly CancelApplicationButtonService _cancelButtonService;
    private readonly HrBotContext _context;
    private readonly ApplicationStatusProvider _statusProvider;

    public VacancyListCommunicationService(
        ITelegramBotClient botClient,
        IProvideAvailablePositions availablePositions,
        CancelApplicationButtonService cancelButtonService,
        HrBotContext context,
        ApplicationStatusProvider statusProvider)
    {
        _botClient = botClient;
        _availablePositions = availablePositions;
        _cancelButtonService = cancelButtonService;
        _context = context;
        _statusProvider = statusProvider;
    }

    public async Task SendVacancyListAsync(
        long chatId,
        CancellationToken cancellationToken)
    {
        await SendVacancyListAsync(chatId, null, cancellationToken);
    }

    public async Task SendVacancyListAsync(
        long chatId,
        Guid? botUserId,
        CancellationToken cancellationToken)
    {
        var positions = await _availablePositions
            .GetAvailablePositions(cancellationToken);

        if (positions.Count == 0)
        {
            var noCancelKeyboard = botUserId.HasValue 
                ? await _cancelButtonService.CreateCancelButtonKeyboardAsync(botUserId.Value, cancellationToken)
                : null;

            await _botClient.SendMessage(
                chatId: chatId,
                text: "Unfortunately, there are no available positions at the moment available for fast-applying.\n" +
                      "You can visit our chanel to check if we have some other positions.",
                replyMarkup: noCancelKeyboard,
                cancellationToken: cancellationToken
            );
            return;
        }

        var keyboard = positions.Select(p =>
                InlineKeyboardButton.WithCallbackData(
                    text: p.Name,
                    callbackData: new VacancyInfoCallback
                        {
                            VacancyId = p.VacancyId
                        }
                        .ToTgString().ToString()
                ))
            .Chunk(2)
            .ToList();

        // Add action buttons row
        var actionButtons = TgButtonProvider.Vacancy.UpdateStatus
            .ToList();

        await AddStatusBtnIfRequired(
            actionButtons, chatId,
            botUserId, cancellationToken);

        keyboard.Add(actionButtons.ToArray());

        // Add cancel button if user has active applications
        var finalKeyboard = botUserId.HasValue
            ? await _cancelButtonService.AddCancelButtonIfNeededAsync(botUserId.Value, keyboard.ToArray(), cancellationToken)
            : new InlineKeyboardMarkup(keyboard.ToArray());

        await _botClient.SendMessage(
            chatId: chatId,
            text: "Please select the position you'd like to apply for:",
            replyMarkup: finalKeyboard,
            cancellationToken: cancellationToken
        );
    }

    private async Task AddStatusBtnIfRequired(
        IList<InlineKeyboardButton> actionButtons,
        long chatId,
        Guid? botUserId,
        CancellationToken cancellationToken)
    {
        if (botUserId.HasValue
            && await _statusProvider
                .HasApplicationForStatusCheck(
                    chatId, botUserId.Value, cancellationToken))
        {
            actionButtons.Add(TgButtonProvider.Applications.Status);
        }
        
    }
}

