using System.Diagnostics.CodeAnalysis;
using CandidateTgBot.Services;
using CandidateTgBot.Services.CommunicationServices;
using CandidateTgBot.Services.DataProviders;
using CandidateTgBot.Types.Callbacks;
using DataLayer.Contexts;
using DataLayer.Dals;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shared.Models;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace CandidateTgBot.Handlers.CallbackHandlers;

public class ApplyForVacancyCallbackHandler : ICallbackHandler<ApplyForVacancyCallback>
{
    private readonly ApplicationService _applicationService;
    private readonly HrBotContext _context;
    private readonly ILogger<ApplyForVacancyCallbackHandler> _logger;
    private readonly IProvideUserFromCallback _userProvider;
    private readonly ISendUnableToIdentifyMessage _sendUnableToIdentifyMessage;
    private readonly ApplyVacancyCommunicationService _applyVacancyCS;

    public ApplyForVacancyCallbackHandler(
        ITelegramBotClient tg,
        BotUserService botUserService,
        ApplicationService applicationService,
        HrBotContext context,
        CurrentQuestionService questionService,
        ILogger<ApplyForVacancyCallbackHandler> logger,
        IProvideUserFromCallback userProvider,
        ISendUnableToIdentifyMessage sendUnableToIdentifyMessage,
        ApplyVacancyCommunicationService applyVacancyCS)
    {
        _applicationService = applicationService;
        _context = context;
        _logger = logger;
        _userProvider = userProvider;
        _sendUnableToIdentifyMessage = sendUnableToIdentifyMessage;
        _applyVacancyCS = applyVacancyCS;
    }

    public async Task Handle(
        long chatId,
        ApplyForVacancyCallback command,
        CallbackQuery query,
        CancellationToken ct)
    {
        var botUserId = await _userProvider.GetBotUserId(query, ct);
        if (botUserId is null)
        {
            await _sendUnableToIdentifyMessage.Send(chatId, ct);
            return;
        }

        // Get vacancy information
        var vacancy = await _context.Vacancies
            .FirstOrDefaultAsync(v => v.Id == command.VacancyId && v.IsActive, ct);

        if (vacancy == null)
        {
            await _applyVacancyCS.SendVacancyUnavailableMessage(chatId, ct);
            return;
        }

        // Check if user already applied
        var hasApplied = await _applicationService.HasUserAppliedToVacancyAsync(
            botUserId.Value, command.VacancyId, ct);

        if (hasApplied)
        {
            await _applyVacancyCS.SendApplicationReviewMessage(chatId, ct, vacancy);
            return;
        }

        // Create the application
        var application = await _applicationService.CreateApplicationAsync(
            botUserId.Value, command.VacancyId, chatId, ct);

        // Update application status to InProgress since user is starting
        await _applicationService.UpdateApplicationStatus(
            application.Id, ApplicationStatus.InProgress, ct);

        // Send confirmation message
        await _applyVacancyCS.SendApplicationStartedMessage(chatId, ct, vacancy);

        _logger.LogInformation(
            "User {TgId} started application {ApplicationId} for vacancy '{VacancyTitle}' ({VacancyId})",
            botUserId, application.Id, vacancy.Title, vacancy.Id);

        // Start the question flow
        await _applyVacancyCS.SendCurrentQuestion(chatId, ct, application, botUserId.Value);
    }

    
}

