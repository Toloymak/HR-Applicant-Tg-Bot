namespace Shared.Models.HrUsers;

public record CreateHrUserRequest
{
    public required string TgName { get; init; }
    public required string Alias { get; init; }
    public required string Position { get; init; }
}