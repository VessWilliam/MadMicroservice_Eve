using MadMicro.Web.Services.IService;
using MadMicro.Web.Utility;

namespace MadMicro.Web.Services.Service;

public class TokenProvider : ITokenProvider
{
    private readonly IHttpContextAccessor _contextAccessor;

    public TokenProvider(IHttpContextAccessor contextAccessor)
    {
        _contextAccessor = contextAccessor;
    }

    public void ClearToken()
    {
        _contextAccessor.HttpContext?.Response.Cookies.Delete(StaticDetail.TokenCookie);
    }

    public string? GetToken()
    {
        string? token = null;

        bool? isHasToken = _contextAccessor.HttpContext?.Request.Cookies.TryGetValue(StaticDetail.TokenCookie, out token);

        return isHasToken is true ? token : null;
    }

    public void SetToken(string token)
    {
        _contextAccessor.HttpContext?.Response.Cookies.Append(StaticDetail.TokenCookie , token);
    }
}
