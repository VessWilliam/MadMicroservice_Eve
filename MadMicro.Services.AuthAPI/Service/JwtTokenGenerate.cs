using MadMicro.Services.AuthAPI.Models;
using MadMicro.Services.AuthAPI.Service.IService;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MadMicro.Services.AuthAPI.Service;

public class JwtTokenGenerate : IJwtTokenGenerate
{
    private readonly JwtOptions _jwtOptions;

    public JwtTokenGenerate(IOptions<JwtOptions> jwtOptions)
    {
        _jwtOptions = jwtOptions.Value;
    }
    public string GenerateToken(AppUser appUser)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_jwtOptions.Secret);
        var claims = new List<Claim>
        { 
            new Claim(JwtRegisteredClaimNames.Email, appUser.Email), 
            new Claim(JwtRegisteredClaimNames.Sub, appUser.Id), 
            new Claim(JwtRegisteredClaimNames.Name, appUser.UserName.ToString()), 
        };
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Audience = _jwtOptions.Audience,
            Issuer = _jwtOptions.Issuer,
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.Now.AddDays(7),
            SigningCredentials = new(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}
