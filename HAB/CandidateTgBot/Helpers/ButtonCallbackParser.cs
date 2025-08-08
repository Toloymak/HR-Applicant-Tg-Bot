using System.Dynamic;
using System.Text.Json;
using System.Text.RegularExpressions;
using CandidateTgBot.Types.Callbacks;
using MessagePack;
using Microsoft.Extensions.Logging;

namespace CandidateTgBot.Helpers;

public class ButtonCallbackParser
{
    private readonly ILogger<ButtonCallbackParser> _logger;

    private static readonly Dictionary<string, Func<string?, ICallback?>>
        CommandParsers = new()
    {
        { VacancyInfoCallback.CommandName, VacancyInfoCallback.Parse },
        { UpdateVacancyListCallback.CommandName, UpdateVacancyListCallback.Parse }
    };

    public ButtonCallbackParser(ILogger<ButtonCallbackParser> logger)
    {
        _logger = logger;
    }

    public ICallback? ParseCallback(string callback)
    {
        if (GetCommand(callback) is not {} tgCommand)
            return null;

        if (CommandParsers.TryGetValue(tgCommand.Command, out var commandParser))
            return commandParser.Invoke(tgCommand.CallbackData);
        
        _logger.LogWarning(
            "No command parser found for callback: {Callback}",
            callback);
        
        return null;
    }
    
    private static TgCallbackData? GetCommand(string callback)
        => TgCallbackData.Parse(callback);
}