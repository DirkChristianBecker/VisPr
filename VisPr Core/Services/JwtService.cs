using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using VisPrCore.Datamodel.Responses.User;
using Microsoft.Extensions.Configuration;

namespace VisPrCore.Services
{
    public class JwtService
    {
        private int Expiration_Minutes = 1;

        private readonly IConfiguration Configuration;

        public JwtService(IConfiguration configuration)
        {
            Configuration = configuration;
            Expiration_Minutes = int.Parse(Configuration["Jwt:Expiration_Minutes"] ?? "10");
        }

        public AuthenticationResponse CreateToken(IdentityUser user, IList<string>? roles)
        {
            var expiration = DateTime.UtcNow.AddMinutes(Expiration_Minutes);

            var token = CreateJwtToken(
                CreateClaims(user, roles),
                CreateSigningCredentials(),
                expiration
            );

            var tokenHandler = new JwtSecurityTokenHandler();

            return new AuthenticationResponse
            {
                Token = tokenHandler.WriteToken(token),
                Expiration = expiration
            };
        }

        private JwtSecurityToken CreateJwtToken(Claim[] claims, SigningCredentials credentials, DateTime expiration) =>
            new JwtSecurityToken(
                Configuration["Jwt:Issuer"],
                Configuration["Jwt:Audience"],
                claims,
                expires: expiration,
                signingCredentials: credentials
            );

        private Claim[] CreateClaims(IdentityUser user, IList<string>? roles)
        {
            var tmp = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, Configuration["Jwt:Subject"] ?? "JWT for vispr².de"),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, user.UserName ?? "No user"),
                new Claim(ClaimTypes.Email, user.Email ?? "No user")
            };

            if(roles != null)
            {
                foreach(var r in roles)
                {
                    tmp.Add(new Claim(ClaimTypes.Role, r ));
                }
            }

            return tmp.ToArray();
        }

        private SigningCredentials CreateSigningCredentials() =>
            new SigningCredentials(
                new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(Configuration["Jwt:Key"] ?? Guid.NewGuid().ToString())
                ),
                SecurityAlgorithms.HmacSha256
            );
    }
}
