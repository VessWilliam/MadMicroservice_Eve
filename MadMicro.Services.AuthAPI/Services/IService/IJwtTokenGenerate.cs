using MadMicro.Services.AuthAPI.Models;

namespace MadMicro.Services.AuthAPI.Service.IService;

public interface IJwtTokenGenerate
{
    string GenerateToken(AppUser appUser, IEnumerable<string> roles);
}
