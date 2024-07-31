using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using EasyBook.Identity;
using EasyBook.Models;
using Microsoft.IdentityModel.Tokens;

namespace EasyBook.Services;

public class AuthService{
    private static IConfigurationRoot bearer_config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional:false)
            .Build();

    public string Create(UserDTO data){
        var handler = new JwtSecurityTokenHandler();
        var private_key = Encoding.UTF8.GetBytes(bearer_config["JwtBearer:PrivateKey"]!);
        var credentials = new SigningCredentials(
            new SymmetricSecurityKey(private_key),
            SecurityAlgorithms.HmacSha256
        );

        var tokenDescriptor = new SecurityTokenDescriptor{
            SigningCredentials = credentials,
            Expires = DateTime.UtcNow.AddMinutes(30),
            Subject = GenerateClaim(data)
        };

        var token = handler.CreateToken(tokenDescriptor);
        return handler.WriteToken(token);
    }

    private static ClaimsIdentity GenerateClaim(UserDTO user){
        var ci = new ClaimsIdentity();

        ci.AddClaim(new Claim("id", user.Id.ToString()));
        ci.AddClaim(new Claim(ClaimTypes.Email, user.Email));
        ci.AddClaim(new Claim(ClaimTypes.Name, user.FirstName));
        ci.AddClaim(new Claim(ClaimTypes.Name, user.LastName));
        ci.AddClaim(new Claim(IdentityData.AdminUserClaim, user.IsAdmin.ToString()));
        
        ci.AddClaim(new Claim(
            JwtRegisteredClaimNames.Aud, 
            bearer_config["JwtBearer:Audience"]!
        ));

        ci.AddClaim(new Claim(
            JwtRegisteredClaimNames.Iss, 
            bearer_config["JwtBearer:Issuer"]!
        ));

        return ci;
    }
}