namespace CandidateTgBot.Types.Callbacks;

public interface ICallback : IHasCommandName, IHasDebugString
{
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