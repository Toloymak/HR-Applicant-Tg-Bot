namespace CandidateTgBot.Types.Callbacks;

public interface ICallback : IHasCommandName
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