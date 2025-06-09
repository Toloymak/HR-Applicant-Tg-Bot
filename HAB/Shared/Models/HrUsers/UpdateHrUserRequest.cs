namespace Shared.Models.HrUsers;

public record UpdateHrUserRequest
{
    public required int Id { get; init; }
    public required string TgName { get; init; }
    public required string Alias { get; init; }
    public required string Position { get; init; }
}