using System.Reflection;
using CandidateTgBot.Helpers;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot.Types;

namespace CandidateTgBot.Handlers.Commands;

internal static class MenuBotConfig
{
    public static IReadOnlyCollection<BotCommand> GetMenuCommands()
    {
        var allMenuCommandTypes = typeof(IMenuBotCommand).Assembly.GetTypes()
            .Where(t => t.IsClass
                        && !t.IsAbstract
                        && typeof(IMenuBotCommand).IsAssignableFrom(t));

        var commands = new List<BotCommand>();

        foreach (var type in allMenuCommandTypes)
        {
            var commandProp = type.GetProperty(nameof(IMenuBotCommand.Command), 
                BindingFlags.Public | BindingFlags.Static);
            var descriptionProp = type.GetProperty(nameof(IMenuBotCommand.Description),
                BindingFlags.Public | BindingFlags.Static);

            if (commandProp is null || descriptionProp is null)
                continue;

            var command = commandProp.GetValue(null) as string;
            var description = descriptionProp.GetValue(null) as string;

            if (!string.IsNullOrWhiteSpace(command)
                && !string.IsNullOrWhiteSpace(description))
            {
                commands.Add(new BotCommand
                {
                    Command = command,
                    Description = description
                });
            }
        }

        return commands;
    }
    
    public static IDictionary<string, Type> BuildCommandTypeMap()
    {
        var types = typeof(IMenuBotCommand).Assembly.GetTypes()
            .Where(t => t.IsClass
                        && !t.IsAbstract
                        && typeof(IMenuBotCommand).IsAssignableFrom(t));

        var result = new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase);

        foreach (var type in types)
        {
            var commandProp = type.GetProperty("Command",
                BindingFlags.Public | BindingFlags.Static);
            if (commandProp == null) continue;

            var command = commandProp.GetValue(null) as string;
            if (string.IsNullOrWhiteSpace(command)) continue;

            result[CommandHelper.NormalizeCommandToText(command)] = type;
        }

        return result;
    }
    
    public static void RegisterMenuCommands(
        this IServiceCollection services)
    {
        var commandTypes = typeof(IMenuBotCommand).Assembly.GetTypes()
            .Where(t => 
                t.IsClass 
                && !t.IsAbstract && typeof(IMenuBotCommand).IsAssignableFrom(t));

        foreach (var type in commandTypes)
        {
            services.AddTransient(typeof(IMenuBotCommand), type);
            services.AddTransient(type);
        }
    }
}