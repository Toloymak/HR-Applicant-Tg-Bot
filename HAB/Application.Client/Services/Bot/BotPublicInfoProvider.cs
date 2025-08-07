using Application.Client.Extensions;
using Application.Client.RefitClients;
using LanguageExt;
using Shared.Models;

namespace Application.Client.Services.Bot;

public interface IProvidePublicBotInfo
{
    EitherAsync<Exception, BotPublicInfo> GetPublicBotInfo(
        CancellationToken ct);
}

internal sealed class BotPublicInfoProvider
    : IProvidePublicBotInfo
{
    private readonly IBotInfoClient _botInfoClient;

    public BotPublicInfoProvider(IBotInfoClient botInfoClient)
    {
        _botInfoClient = botInfoClient;
    }

    public EitherAsync<Exception, BotPublicInfo> 
        GetPublicBotInfo(CancellationToken ct)
    {
        return _botInfoClient
            .GetPublicBotInfo(ct)
            .ToEither();
    }
}