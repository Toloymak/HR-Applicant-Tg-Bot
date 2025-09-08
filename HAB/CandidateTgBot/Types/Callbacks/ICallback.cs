namespace CandidateTgBot.Types.Callbacks;

public interface ICallback : IHasCommandName, IHasDebugString
{
    TgCallbackData ToTgString();
}

public interface IHasCommandName
{
    string Command { get; }
}

public interface IHasConstantCommandName
{
    static abstract string CommandName { get; }
}

public interface IHasDebugString
{
    string GetDebugString();
}

public interface IParsableCallback<T> where T : ICallback
{
    static abstract T? Parse(string? data);
}