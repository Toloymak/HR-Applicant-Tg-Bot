using Telegram.Bot;

namespace CandidateTgBot.Services.CommunicationServices;

public class WelcomeCommunicationService
{
    private readonly ITelegramBotClient _botClient;
    private readonly VacancyListCommunicationService _vacancyListService;

    public WelcomeCommunicationService(
        ITelegramBotClient botClient,
        VacancyListCommunicationService vacancyListService)
    {
        _botClient = botClient;
        _vacancyListService = vacancyListService;
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
        
        await _vacancyListService.SendVacancyListAsync(chatId, cancellationToken);
    }
}
