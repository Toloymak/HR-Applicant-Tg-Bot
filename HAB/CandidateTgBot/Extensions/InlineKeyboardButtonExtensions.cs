using Telegram.Bot.Types.ReplyMarkups;

namespace CandidateTgBot.Extensions;

public static class InlineKeyboardButtonExtensions
{
    public static InlineKeyboardButton[] ToArray(
        this InlineKeyboardButton button)
        => [button];
    
    public static IList<InlineKeyboardButton> ToList(
        this InlineKeyboardButton button)
        => [button];
    
    public static InlineKeyboardMarkup ToMarkupKeyboard(
        this InlineKeyboardButton[] buttons)
        => new(buttons);
}