namespace MasterAPI.Application.Interfaces
{
    public interface IUserAccessor
    {
        string GetUserName();
        string GetEmail();
    }
}