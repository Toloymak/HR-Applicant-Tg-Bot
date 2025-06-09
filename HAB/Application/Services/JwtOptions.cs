namespace Application.Services;

public class JwtOptions
{
    /// Key to sign the JWT token
    public required string SigningKey { get; init; }
}