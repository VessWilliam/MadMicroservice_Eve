using MadMicro.Web.Models;
using MadMicro.Web.Services.IService;
using static MadMicro.Web.Utility.StaticDetail;

namespace MadMicro.Web.Services.Service;

public class AuthService : IAuthService
{
    private readonly IBaseService _baseService;

    public AuthService(IBaseService baseService)
    {
        _baseService = baseService;
    }

    public async Task<ResponseDTO?> LoginAsync(UserLoginDTO loginRequestDto)
    {
        return await _baseService.SendAsync(new RequestDTO
        {
            ApiType = ApiType.POST,
            Data = loginRequestDto,
            Url = $"{AuthAPIBase}/api/auth/login"
        });
    }

    public async Task<ResponseDTO?> AssignRoleLoginAsync(UserRegisterDTO registerRequestDto)
    {
        return await _baseService.SendAsync(new RequestDTO
        {
            ApiType = ApiType.POST,
            Data = registerRequestDto,
            Url = $"{AuthAPIBase}/api/auth/AssignRole"
        });
    }

    public async Task<ResponseDTO?> RegisterAsync(UserRegisterDTO registerRequestDto)
    {
        return await _baseService.SendAsync(new RequestDTO
        {
            ApiType = ApiType.POST,
            Data = registerRequestDto,
            Url = $"{AuthAPIBase}/api/auth/register"
        });
    }
}
