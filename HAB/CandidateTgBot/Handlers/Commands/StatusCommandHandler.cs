using CandidateTgBot.Services;
using CandidateTgBot.Services.CommunicationServices;
using Microsoft.Extensions.Logging;
using Telegram.Bot;

namespace CandidateTgBot.Handlers.Commands;

public class StatusCommandHandler : IMenuBotCommand
{
    private readonly StatusCommunicationService _statusService;

    public StatusCommandHandler(
        StatusCommunicationService statusService)
    {
        _statusService = statusService;
    }

    public async Task HandleCommand(long chatId, CancellationToken ct)
    {
        await _statusService.SendStatusInfo(chatId, null, ct);
    }

    public static string Command => "status";
    public static string Description => "Show your application status";
}
