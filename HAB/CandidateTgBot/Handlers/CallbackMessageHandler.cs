using CandidateTgBot.Helpers;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace CandidateTgBot.Handlers;

public class CallbackMessageHandler
{
    private readonly ITelegramBotClient _tgClient;
    private readonly ButtonCallbackParser _buttonCallbackParser;

    public CallbackMessageHandler(
        ITelegramBotClient tgClient,
        ButtonCallbackParser buttonCallbackParser)
    {
        _tgClient = tgClient;
        _buttonCallbackParser = buttonCallbackParser;
    }

    public async Task HandleCallback(
        long chatId,
        CallbackQuery callback,
        CancellationToken token)
    {
        if (_buttonCallbackParser.ParseCallback(callback) is { } command)
        {
            await _tgClient.SendMessage(
                chatId: chatId,
                text: $"Command received: {command.Command}, command: {command}",
                cancellationToken: token
            );
                
            await _tgClient.AnswerCallbackQuery(
                callbackQueryId: callback.Id,
                text: $"Callback received successfully! {callback} -{callback.Data}-",
                showAlert: false,
                cancellationToken: token
            );
            return;
        }

        await _tgClient.SendMessage(
            chatId: chatId,
            text: $"Error handling command ({callback.Data})",
            cancellationToken: token
        );
                
        await _tgClient.AnswerCallbackQuery(
            callbackQueryId: callback.Id,
            text: $"Sorry, I couldn't parse the command, contact support",
            showAlert: true,
            cancellationToken: token
        );
    }
}