using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace CandidateTgBot.Handlers;

public class MessageHandler
{
    private readonly ILogger<MessageHandler> _logger;
    
    public async Task HandleUpdateAsync(
        ITelegramBotClient bot,
        Update update,
        CancellationToken token)
    {
        var message = update.Message;
        var callback = update.CallbackQuery;
        var chatId = message?.Chat.Id ?? callback?.Message?.Chat.Id;
        if (chatId == null) return;

        
    }
}