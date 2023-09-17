using static MadMicro.Web.Utility.StaticDetail;

namespace MadMicro.Web.Models
{
    public class RequestDTO
    {
        public ApiType ApiType { get; set; } = ApiType.GET;
        public string? Url { get; set; }
        public string? AccessToken { get; set; }
        public object? Data { get; set; }

    }
}
