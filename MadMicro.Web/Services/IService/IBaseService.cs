using MadMicro.Web.Models;

namespace MadMicro.Web.Services.IService
{
    public interface IBaseService
    {
        Task<ResponseDTO?> SendAsync(RequestDTO requestDTO, bool withBearer = true);
    }
}
