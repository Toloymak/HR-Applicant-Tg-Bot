namespace Application.Policies;

public static partial class Policy
{
    public readonly static RoleBasedPolicy Manage = new()
    {
        Name = "Manage",
        Roles = ["hr"]
    };
}