using MadMicro.Services.AuthAPI.Models.DTO;

namespace MadMicro.Services.AuthAPI.Service.IService;

public interface IAuthService
{
    Task<string> Register(UserRegisterDTO userRegisterDTO);
    Task<LoginResponseDto> Login(UserLoginDTO userLoginDTO);
}
