using CandateTgBot.Shared.Services;
using CandidateTgBot.Services;
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

    public VacancyListCommunicationService(
        ITelegramBotClient botClient,
        IProvideAvailablePositions availablePositions,
        CancelApplicationButtonService cancelButtonService,
        HrBotContext context)
    {
        _botClient = botClient;
        _availablePositions = availablePositions;
        _cancelButtonService = cancelButtonService;
        _context = context;
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

        // Check if user has relevant applications to show status button
        var hasRelevantApplications = false;
        if (botUserId.HasValue)
        {
            hasRelevantApplications = await _context.UserApplications
                .AnyAsync(a => a.BotUserId == botUserId.Value && 
                              (a.State == ApplicationStatus.CompletedByUser ||
                               a.State == ApplicationStatus.ApprovedByHr ||
                               a.State == ApplicationStatus.RejectedByHr ||
                               a.State == ApplicationStatus.CompeatedByUserAndStartedNew), 
                              cancellationToken);
        }

        // Add action buttons row
        var actionButtons = new List<InlineKeyboardButton>();
        actionButtons.Add(InlineKeyboardButton.WithCallbackData(
            text: "Update list",
            callbackData: new UpdateVacancyListCallback()
                .ToTgString().ToString()
        ));

        if (hasRelevantApplications)
        {
            actionButtons.Add(InlineKeyboardButton.WithCallbackData(
                text: "📋 Status",
                callbackData: new ShowStatusCallback()
                    .ToTgString().ToString()
            ));
        }

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
}

