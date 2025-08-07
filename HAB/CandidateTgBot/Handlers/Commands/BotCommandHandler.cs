using CandidateTgBot.Helpers;
using Microsoft.Extensions.Logging;

namespace CandidateTgBot.Handlers.Commands;

public class BotCommandHandler
{
    private readonly ILogger<BotCommandHandler> _logger;
    private readonly IServiceProvider _serviceProvider;
    
    private static readonly IDictionary<string, Type> CommandTypes
        = MenuBotConfig.BuildCommandTypeMap();

    public BotCommandHandler(
        IServiceProvider serviceProvider,
        ILogger<BotCommandHandler> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }
    
    public async Task<bool> TryExecuteCommand(
        string command,
        long chatId,
        CancellationToken ct)
    {
        var normalizedCommand = CommandHelper.NormalizeCommand(command);
        if (!CommandHelper.LooksLikeCommand(normalizedCommand))
            return false;
        
        if (!CommandTypes.TryGetValue(
                CommandHelper.NormalizeCommandToText(normalizedCommand), out var type))
            return false;

        if (_serviceProvider.GetService(type) is not IMenuBotCommand handler)
        {
            _logger.LogWarning(
                "Command handler for '{Command}' not found or not registered",
                normalizedCommand);
            return false;
        }

        await handler.HandleCommand(chatId, ct);
        _logger.LogInformation(
            "Executed command '{Command}' for chat ID: {ChatId}",
            command,
            chatId
        );
        return true;
    }
}