// See https://aka.ms/new-console-template for more information

using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

var botClient = new TelegramBotClient("");

var me = await botClient.GetMe();
Console.WriteLine($"Bot started: {me.Username}");

using var cts = new CancellationTokenSource();

var userStates = new Dictionary<long, UserState>();

botClient.StartReceiving(
    HandleUpdateAsync,
    HandleErrorAsync,
    cancellationToken: cts.Token
);


Console.ReadLine();
cts.Cancel();


async Task HandleUpdateAsync(ITelegramBotClient bot, Update update, CancellationToken token)
{
    var message = update.Message;
    var callback = update.CallbackQuery;
    var chatId = message?.Chat.Id ?? callback?.Message.Chat.Id;
    if (chatId == null) return;

    if (!userStates.ContainsKey(chatId.Value))
        userStates[chatId.Value] = new UserState();

    var state = userStates[chatId.Value];

    if (message?.Text == "/start" || message?.Text?.ToLower() == "рестарт")
    {
        userStates[chatId.Value] = new UserState(); // reset state

        var vacancies = new InlineKeyboardMarkup(new[]
        {
            new[] { InlineKeyboardButton.WithCallbackData("Backend Developer", "vacancy_backend") },
            new[] { InlineKeyboardButton.WithCallbackData("Frontend Developer", "vacancy_frontend") }
        });

        await bot.SendMessage(chatId, "Choose a vacancy:", replyMarkup: vacancies, cancellationToken: token);
        return;
    }

    if (callback != null && callback.Data.StartsWith("vacancy_"))
    {
        state.VacancyId = callback.Data;
        state.CurrentStep = Step.WaitingForAspNet;

        await bot.AnswerCallbackQuery(callback.Id, cancellationToken: token);
        await bot.SendMessage(chatId, "Do you have experience with ASP.NET? (Yes/No)", cancellationToken: token);
        return;
    }

    if (state.CurrentStep == Step.WaitingForAspNet && message != null)
    {
        state.AspNetExperience = message.Text?.Trim().ToLower();
        if (state.AspNetExperience == "yes")
        {
            state.CurrentStep = Step.WaitingForRussian;
            await bot.SendMessage(chatId, "Do you speak Russian? (Yes/No)", cancellationToken: token);
        }
        else
        {
            state.CurrentStep = Step.None;
            await bot.SendMessage(chatId, "Thanks for your response! We’ll be in touch.", cancellationToken: token);
        }
        return;
    }

    if (state.CurrentStep == Step.WaitingForRussian && message != null)
    {
        state.KnowsRussian = message.Text?.Trim().ToLower();
        state.CurrentStep = Step.None;

        await bot.SendMessage(chatId,
            $"Thanks! Summary:\n" +
            $"- Vacancy: {state.VacancyId?.Replace("vacancy_", "").ToUpper()}\n" +
            $"- ASP.NET experience: {state.AspNetExperience}\n" +
            $"- Speaks Russian: {state.KnowsRussian}",
            cancellationToken: token);
        return;
    }
}

Task HandleErrorAsync(ITelegramBotClient bot, Exception ex, CancellationToken token)
{
    Console.WriteLine($"Error: {ex.Message}");
    return Task.CompletedTask;
}

class UserState
{
    public Step CurrentStep { get; set; } = Step.None;
    public string? VacancyId { get; set; }
    public string? AspNetExperience { get; set; }
    public string? KnowsRussian { get; set; }
}

enum Step
{
    None,
    WaitingForAspNet,
    WaitingForRussian
}