namespace Application.Policies;

public class RoleBasedPolicy
{
    public required string Name { get; init; }
    public required string[] Roles { get; init; }
}