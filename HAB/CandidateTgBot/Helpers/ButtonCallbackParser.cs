using System.Dynamic;
using System.Reflection;
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
        CommandParsers = BuildCommandParsers();

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

    /// <summary>
    /// Automatically discovers and registers all callback types that implement ICallback, IHasConstantCommandName, and IParsableCallback<T>
    /// </summary>
    private static Dictionary<string, Func<string?, ICallback?>> BuildCommandParsers()
    {
        var parsers = new Dictionary<string, Func<string?, ICallback?>>();
        
        // Get all types in the current assembly that implement the required interfaces
        var callbackTypes = Assembly.GetExecutingAssembly()
            .GetTypes()
            .Where(type => type.IsClass 
                          && !type.IsAbstract 
                          && typeof(ICallback).IsAssignableFrom(type)
                          && typeof(IHasConstantCommandName).IsAssignableFrom(type)
                          && HasParsableCallbackInterface(type))
            .ToList();

        foreach (var callbackType in callbackTypes)
        {
            try
            {
                // Get the CommandName static property
                var commandNameProperty = callbackType.GetProperty(nameof(IHasConstantCommandName.CommandName), 
                    BindingFlags.Public | BindingFlags.Static);
                
                if (commandNameProperty?.GetValue(null) is not string commandName)
                    continue;

                // Get the Parse static method from IParsableCallback<T>
                var parseMethod = callbackType.GetMethod("Parse", 
                    BindingFlags.Public | BindingFlags.Static, 
                    null,
                    [typeof(string)], 
                    null);

                if (parseMethod == null)
                    continue;

                // Create a delegate for the Parse method
                var parseDelegate = (Func<string?, ICallback?>)Delegate.CreateDelegate(
                    typeof(Func<string?, ICallback?>), 
                    parseMethod);

                parsers[commandName] = parseDelegate;
            }
            catch (Exception)
            {
                // Skip types that don't conform to the expected pattern
                continue;
            }
        }

        return parsers;
    }

    /// <summary>
    /// Checks if a type implements IParsableCallback<T> where T is the type itself
    /// </summary>
    private static bool HasParsableCallbackInterface(Type type)
    {
        var interfaces = type.GetInterfaces();
        return interfaces.Any(i => i.IsGenericType 
                                  && i.GetGenericTypeDefinition() == typeof(IParsableCallback<>)
                                  && i.GetGenericArguments()[0] == type);
    }
}
