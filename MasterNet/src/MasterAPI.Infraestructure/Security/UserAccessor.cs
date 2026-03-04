using System.Security.Claims;
using MasterAPI.Application.Interfaces;
using Microsoft.AspNetCore.Http;

namespace MasterAPI.Insfraestructure.Security
{
    public class UserAccessor(
        IHttpContextAccessor httpContextAccessor
    ) : IUserAccessor
    {
        public string GetEmail()
        {
            return httpContextAccessor.HttpContext!.User.FindFirstValue(ClaimTypes.Email)!;
        }

        public string GetUserName()
        {
            return httpContextAccessor.HttpContext!.User.FindFirstValue(ClaimTypes.Name)!;
        }
    }
}