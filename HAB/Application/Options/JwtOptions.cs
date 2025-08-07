using Shared.Contracts;

namespace Application.Options;

public class JwtOptions : IHasSectionName
{
    public static string SectionName => "Jwt";
    
    /// Key to sign the JWT token
    public required string SigningKey { get; init; }
}