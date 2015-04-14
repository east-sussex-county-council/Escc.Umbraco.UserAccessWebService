namespace ESCC.Umbraco.UserAccessWebService.Services.Interfaces
{
    public interface IHashService
    {
        string HashPassword(string password);
    }
}