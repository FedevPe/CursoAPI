using Microsoft.AspNetCore.Identity;

namespace MasterAPI.Persistence.Models
{
    public class UserApp : IdentityUser
    {
        public string? NombreCompleto { get; set; }
        public string? TituloProfesional { get; set; }
    }
}