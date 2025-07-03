namespace Application.Client.Constants.Links;

public static class VacancyLinks
{
    public const string List = "/vacancies";
    
    public const string Create = "/vacancies/create";
    
    public static string Edit(Guid id) => $"/vacancies/edit/{id}";
}