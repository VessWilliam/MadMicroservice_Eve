using MadMicro.Web.Models;

namespace MadMicro.Web.Services.IService;

public interface IAuthService
{
    Task<ResponseDTO?> LoginAsync(UserLoginDTO loginRequestDto);
    Task<ResponseDTO?> RegisterAsync(UserRegisterDTO registerRequestDto);
    Task<ResponseDTO?> AssignRoleLoginAsync(UserRegisterDTO registerRequestDto);

}
