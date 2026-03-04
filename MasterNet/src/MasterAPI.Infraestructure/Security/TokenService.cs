using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using MasterAPI.Application.Interfaces;
using MasterAPI.Domain.Models;
using MasterAPI.Persistence;
using MasterAPI.Persistence.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace MasterAPI.Insfraestructure.Security
{
    //Es fundamental inyectar IConfiguration en esta clase porque esta interfaz permite poder leer el token
    //Ademas inyecto una instancia del contexto para tener acceso a la base de datos y comprobar
    //el rol y claim de cada usuario que accede a la aplicación.
    public class TokenService(
        MasterAPIDbContext context,
        IConfiguration configuration
    ) : ITokenService
    {
        public async Task<string> CreateToken(UserApp user)
        {
            //En primer lugar, antes de generar el token, necesito conocer todos los roles y claims que existen en la base de datos y que estan
            //asignados a un usuario en especifico.
            var policies = await context.Database.SqlQuery<string>($@"
                SELECT
                    aspr.ClaimValue
                FROM AspNetUsers a
                    LEFT JOIN AspNetUserRoles ar
                        ON a.Id = ar.UserId
                    LEFT JOIN AspNetRoleClaims aspr
                        ON ar.RoleId = aspr.RoleId
                WHERE a.Id = {user.Id}
            ").ToListAsync();
            
            //Una vez que obtengo las politicas o policies(reglas o permisos) del usuario

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName!),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Email, user.Email!)  
            };

            foreach (var item in policies)
            {
                if(item is not null)
                {
                    claims.Add(new (CustomClaims.POLICIES, item));
                }
            }

            var credentials = new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["TokenKey"]!)), 
                SecurityAlgorithms.HmacSha256
            );

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = credentials
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
    }
}