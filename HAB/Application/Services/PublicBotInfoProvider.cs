using ApiCore.Options;
using Application.Client.Services.Bot;
using LanguageExt;
using Shared.Models;

namespace Application.Services;

internal class ProvidePublicBotInfo : IProvidePublicBotInfo
{
    private readonly CandidateBotOptions _candidateBotOptions;

    public ProvidePublicBotInfo(
        CandidateBotOptions candidateBotOptions)
    {
        _candidateBotOptions = candidateBotOptions;
    }

    public EitherAsync<Exception, BotPublicInfo> GetPublicBotInfo(
        CancellationToken ct)
    {
        return new BotPublicInfo(
            new TgUserName(_candidateBotOptions.TgAddress));
    }
}