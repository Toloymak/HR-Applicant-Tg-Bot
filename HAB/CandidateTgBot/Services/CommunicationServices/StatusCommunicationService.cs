using CandidateTgBot.Extensions;
using CandidateTgBot.Services.DataProviders;
using Shared.Models;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;

namespace CandidateTgBot.Services.CommunicationServices;

public class StatusCommunicationService
{
    private readonly ISendUnableToIdentifyMessage _sendUnableToIdentifyMessage;
    private readonly ApplicationStatusProvider _statusProvider;
    private readonly ITelegramBotClient _tgClient;

    public StatusCommunicationService(
        ISendUnableToIdentifyMessage sendUnableToIdentifyMessage,
        ApplicationStatusProvider statusProvider,
        ITelegramBotClient tgClient)
    {
        _sendUnableToIdentifyMessage = sendUnableToIdentifyMessage;
        _statusProvider = statusProvider;
        _tgClient = tgClient;
    }

    /// <summary>
    /// Sends application status information
    /// </summary>
    public async Task SendStatusInfo(
        long chatId,
        Guid? botUserId,
        CancellationToken ct)
    {
        if (botUserId.HasValue)
        {
            await SendHandlingMessageStatuses(chatId, botUserId.Value, ct);
        }
        else
        {
            await _sendUnableToIdentifyMessage.Send(
                chatId,
                ct);
        }
    }

    private async Task SendHandlingMessageStatuses(
        long chatId,
        Guid botUserId,
        CancellationToken ct)
    {
        var statuses = await _statusProvider
            .GetApplicationForStatusCheck(chatId, botUserId, ct);

        if (statuses.Any())
        {
            await ShowShortCompletedStatusAsync(
                chatId,
                statuses.ToList(),
                ct);
        }
        else
        {
            await ShowNoApplicationsMessageAsync(
                chatId,
                ct);
        }
    }
    
    private async Task ShowShortCompletedStatusAsync(
        long chatId,
        List<UserApplicationStatus> applications,
        CancellationToken cancellationToken)
    {
        var statusText = $"You have {applications.Count} completed application(s):\n\n";

        foreach (var application in applications.Take(3))
        {
            var completionDate = application.LastActivity.ToString("MMM dd, yyyy");
            var statusDisplay = application.State switch
            {
                ApplicationStatus.ReviewByHr => "✅ Submitted for Review",
                ApplicationStatus.Approved => "✅ Approved! We will contact you soon",
                ApplicationStatus.Rejected => "❌ Sorry, you were not selected for this role",
                ApplicationStatus.InProgress => "📝 In Progress",
                _ => "Unknown Status"
            };
            
            statusText += $"**📄 {application.VacancyTitle}**\n" +
                          $"Completed: {completionDate} | Status: {statusDisplay}\n\n";
        }

        if (applications.Count > 3)
        {
            statusText += $"*... and {applications.Count - 3} more application(s)*\n\n";
        }

        statusText += "💡 *Ready to start a new application?*";

        var keyboard = (applications.Any(x => x.State is ApplicationStatus.InProgress)
                ? TgButtonProvider.Applications.ContinueWorkWithApplication
                : TgButtonProvider.Applications.StartNew)
            .ToMarkupKeyboard();

        if (applications.Any(x => x.State is ApplicationStatus.ReviewByHr))
            keyboard.AddButtons(TgButtonProvider.Applications.ShowRevokeApplicationMenu);

        await _tgClient.SendMessage(
            chatId: chatId,
            text: statusText,
            parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
            replyMarkup: keyboard,
            cancellationToken: cancellationToken
        );
    }
    
    private async Task ShowNoApplicationsMessageAsync(
        long chatId,
        CancellationToken cancellationToken)
    {
        var message = "📭 **No Applications Found**\n\n" +
                      "You don't have any applications yet.\n\n" +
                      "💡 *Ready to start your first application?*";

        var keyboard = TgButtonProvider
            .Applications.StartNew
            .ToArray()
            .ToMarkupKeyboard();

        await _tgClient.SendMessage(
            chatId: chatId,
            text: message,
            parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
            replyMarkup: keyboard,
            cancellationToken: cancellationToken
        );
    }
}