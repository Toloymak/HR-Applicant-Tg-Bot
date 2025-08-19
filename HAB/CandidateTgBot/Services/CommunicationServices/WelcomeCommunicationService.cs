using DataLayer.Contexts;
using Microsoft.EntityFrameworkCore;
using Shared.Models;
using Telegram.Bot;

namespace CandidateTgBot.Services.CommunicationServices;

public class WelcomeCommunicationService
{
    private readonly ITelegramBotClient _botClient;
    private readonly VacancyListCommunicationService _vacancyListService;
    private readonly HrBotContext _context;

    public WelcomeCommunicationService(
        ITelegramBotClient botClient,
        VacancyListCommunicationService vacancyListService,
        HrBotContext context)
    {
        _botClient = botClient;
        _vacancyListService = vacancyListService;
        _context = context;
    }

    public async Task SendWelcomeMessageAsync(
        long chatId, 
        CancellationToken cancellationToken)
    {
        await SendWelcomeMessageAsync(chatId, null, cancellationToken);
    }

    public async Task SendWelcomeMessageAsync(
        long chatId, 
        Guid? botUserId,
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
        
        await _vacancyListService.SendVacancyListAsync(chatId, botUserId, cancellationToken);
    }
}
