namespace Application.Policies;

public static partial class Policy
{
    public static RoleBasedPolicy Manage = new()
    {
        Name = "Manage",
        Roles = ["Manager"]
    };
}