using System.Diagnostics.CodeAnalysis;
using Shared.Contracts;

namespace ApiCore.Options;

[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global",
    Justification = "Used by the DI container")]
public class CandidateBotOptions : IHasSectionName
{
    public static string SectionName => "CandidateBot";

    public required string Token { get; init; }
    public required string TgAddress { get; init; }
}