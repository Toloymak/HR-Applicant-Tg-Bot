namespace Application.Client.Constants.Links;

public static class HrUsersLinks
{
    public const string List = "/hrs/";
    
    public const string Create = "/hrs/create";
    
    public static string Edit(int id) => $"/hrs/{id}";
}