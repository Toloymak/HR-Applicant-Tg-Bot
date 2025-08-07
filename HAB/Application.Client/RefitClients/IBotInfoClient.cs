using Refit;
using Shared.Models;

namespace Application.Client.RefitClients;

public interface IBotInfoClient
{
    /// <summary>
    /// Get public bot info
    /// </summary>
    /// <param name="ct">Cancellation token</param>
    /// <returns>Bot public info</returns>
    [Get("/api/bots/info")]
    Task<IApiResponse<BotPublicInfo>> GetPublicBotInfo(
        CancellationToken ct);
    
}