using MadMicro.Web.Models;

namespace MadMicro.Web.Services.IService
{
    public interface IBaseService
    {
        Task<ResponseDTO?> SendAsync(RequestDTO responseDTO);
    }
}
