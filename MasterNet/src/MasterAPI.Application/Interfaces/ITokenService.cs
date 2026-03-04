using MasterAPI.Persistence.Models;

namespace MasterAPI.Application.Interfaces
{
    public interface ITokenService
    {
        Task<string> CreateToken(UserApp user);
    }
}