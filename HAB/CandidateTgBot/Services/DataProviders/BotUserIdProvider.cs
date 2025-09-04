using DataLayer.Dals;
using Microsoft.Extensions.Logging;
using Telegram.Bot.Types;

namespace CandidateTgBot.Services.DataProviders;

public interface IProvideUserFromCallback
{
    Task<Guid?> GetBotUserId(CallbackQuery query, CancellationToken ct);

    Task<BotUserDal?> GetBotUser(
        CallbackQuery query,
        CancellationToken ct);
}

public class BotUserIdProvider
{
    private readonly BotUserService _botUserService;
    private readonly ILogger<BotUserIdProvider> _logger;

    public BotUserIdProvider(
        BotUserService botUserService,
        ILogger<BotUserIdProvider> logger)
    {
        _botUserService = botUserService;
        _logger = logger;
    }

    public async Task<Guid?> GetBotUserId(
        CallbackQuery query, CancellationToken ct)
    {
        try
        {
            var botUser = await _botUserService.CreateOrUpdateUserAsync(query.From, ct);
            return botUser?.Id;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting bot user for Telegram user {TgId}", query.From.Id);
            return null;
        }
    }
    
    
    public async Task<BotUserDal?> GetBotUser(
        CallbackQuery query, CancellationToken ct)
    {
        try
        {
            var botUser = await _botUserService.CreateOrUpdateUserAsync(query.From, ct);
            return botUser;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting bot user for Telegram user {TgId}", query.From.Id);
            return null;
        }
    }
}