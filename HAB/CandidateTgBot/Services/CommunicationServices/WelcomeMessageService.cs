using System.Text.Json;
using CandidateTgBot.Helpers;
using CandidateTgBot.Types.Callbacks;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;

namespace CandidateTgBot.Services.CommunicationServices;

public class WelcomeMessageService
{
    private readonly ITelegramBotClient _botClient;
    private readonly IProvideAvailablePositions _availablePositions;

    public WelcomeMessageService(
        ITelegramBotClient botClient,
        IProvideAvailablePositions availablePositions)
    {
        _botClient = botClient;
        _availablePositions = availablePositions;
    }

    public async Task SendWelcomeMessageAsync(
        long chatId, 
        CancellationToken cancellationToken)
    {
        var welcomeMessage = 
            $"Hello, welcome to the Candidate Bot! \n" +
            "This bot will help you apply for the position. ";
        
        await _botClient.SendMessage(
            chatId: chatId,
            text: welcomeMessage,
            cancellationToken: cancellationToken
        );
        
        var positions = await _availablePositions
            .GetAvailablePositions(cancellationToken);

        if (positions.Count == 0)
        {
            await _botClient.SendMessage(
                chatId: chatId,
                text: "Unfortunately, there are no available positions at the moment available for fast-applying.\n" +
                      "You can visit our chanel to check if we have some other positions.",
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
                )).Chunk(2)
            .Concat([
                [
                    InlineKeyboardButton.WithCallbackData(
                        text: "Update list",
                        callbackData: new UpdateVacancyListCallback()
                            .ToTgString().ToString()
                    )
                ]
            ])
            .ToArray();
        await _botClient.SendMessage(
            chatId: chatId,
            text: "Please select the position you'd like to apply for:",
            replyMarkup: keyboard,
            cancellationToken: cancellationToken
        );
    }
}
