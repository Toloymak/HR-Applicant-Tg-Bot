using DataLayer.Dals;
using Telegram.Bot;

namespace CandidateTgBot.Services.CommunicationServices;

public class ApplyVacancyCommunicationService
{
    private readonly ITelegramBotClient _tg;
    private readonly CurrentQuestionService _questionService;
    
    public async Task SendCurrentQuestion(
        long chatId,
        CancellationToken ct,
        UserApplicationDal application,
        Guid botUserId)
    {
        var questionSent = await _questionService.SendCurrentQuestionAsync(
            chatId, application.Id, botUserId, ct);

        if (!questionSent)
        {
            await _tg.SendMessage(
                chatId: chatId,
                text: "❌ Unable to load questions for this vacancy. Please contact support.",
                cancellationToken: ct
            );
        }
    }

    public async Task SendApplicationStartedMessage(
        long chatId,
        CancellationToken ct,
        VacancyDal vacancy)
    {
        await _tg.SendMessage(
            chatId: chatId,
            text: $"🎯 **Application Started!**\n\n" +
                  $"You are now applying for: **{vacancy.Title}**\n\n" +
                  $"📝 Let's begin with the first question...",
            parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
            cancellationToken: ct
        );
    }

    public async Task SendApplicationReviewMessage(
        long chatId,
        CancellationToken ct,
        VacancyDal vacancy)
    {
        await _tg.SendMessage(
            chatId: chatId,
            text: $"📋 You have already applied to the **{vacancy.Title}** position. " +
                  "Your application is being reviewed by our HR team.",
            parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
            cancellationToken: ct
        );
    }

    public async Task SendVacancyUnavailableMessage(
        long chatId,
        CancellationToken ct)
    {
        await _tg.SendMessage(
            chatId: chatId,
            text: "❌ Sorry, this vacancy is no longer available.",
            cancellationToken: ct
        );
    }
}