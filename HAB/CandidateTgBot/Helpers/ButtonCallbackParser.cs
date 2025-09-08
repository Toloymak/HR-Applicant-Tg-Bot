using System.Dynamic;
using System.Text.Json;
using System.Text.RegularExpressions;
using CandidateTgBot.Types.Callbacks;
using MessagePack;
using Microsoft.Extensions.Logging;
using Telegram.Bot.Types;

namespace CandidateTgBot.Helpers;

public class ButtonCallbackParser
{
    private readonly ILogger<ButtonCallbackParser> _logger;

    private static readonly Dictionary<string, Func<string?, ICallback?>>
        CommandParsers = new()
    {
        { VacancyInfoCallback.CommandName, VacancyInfoCallback.Parse },
        { UpdateVacancyListCallback.CommandName, UpdateVacancyListCallback.Parse },
        { ApplyForVacancyCallback.CommandName, ApplyForVacancyCallback.Parse },
        { ShowOtherVacanciesCallback.CommandName, ShowOtherVacanciesCallback.Parse },
        { CancelApplicationCallback.CommandName, CancelApplicationCallback.Parse },
        { ConfirmCancelApplicationCallback.CommandName, ConfirmCancelApplicationCallback.Parse },
        { ConfirmResetCallback.CommandName, ConfirmResetCallback.Parse },
        { CancelRevokeApplicationCallback.CommandName, CancelRevokeApplicationCallback.Parse },
        { AnswerYesCallback.CommandName, AnswerYesCallback.Parse },
        { AnswerNoCallback.CommandName, AnswerNoCallback.Parse },
        { AnswerTextCallback.CommandName, AnswerTextCallback.Parse },
        { SendOneMoreApplicationCallback.CommandName, SendOneMoreApplicationCallback.Parse },
        { ShowStatusCallback.CommandName, ShowStatusCallback.Parse },
        { StartNewApplicationCallback.CommandName, StartNewApplicationCallback.Parse },
        // { RevokeApplicationListCallback.CommandName, RevokeApplicationListCallback.Parse },
        { RevokeSpecificApplicationCallback.CommandName, RevokeSpecificApplicationCallback.Parse },
        { ConfirmRevokeApplicationCallback.CommandName, ConfirmRevokeApplicationCallback.Parse },
        { ShowRevokeListCallback.CommandName, ShowRevokeListCallback.Parse }
    };

    public ButtonCallbackParser(ILogger<ButtonCallbackParser> logger)
    {
        _logger = logger;
    }

    public ICallback? ParseCallback(CallbackQuery callback)
    {
        if (callback.Data is not {} callbackData
            || GetCommand(callbackData) is not {} tgCommand)
            return null;

        if (CommandParsers.TryGetValue(tgCommand.Command, out var commandParser))
            return commandParser.Invoke(tgCommand.CallbackData);
        
        _logger.LogWarning(
            "No command parser found for callback: {Callback}",
            callbackData);
        
        return null;
    }
    
    private static TgCallbackData? GetCommand(string callback)
        => TgCallbackData.Parse(callback);
}
