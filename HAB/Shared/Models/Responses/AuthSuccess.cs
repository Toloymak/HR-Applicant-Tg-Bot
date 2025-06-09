namespace Shared.Models.Responses;

public class AuthSuccess
{
    public required string Token { get; init; }
    public required DateTimeOffset ExpiredAt { get; init; }
}