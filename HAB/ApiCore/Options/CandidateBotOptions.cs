using System.Diagnostics.CodeAnalysis;

namespace Domain.Options;

[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global",
    Justification = "Used by the DI container")]
public class CandidateBotOptions
{
    public required string Token { get; init; }
}