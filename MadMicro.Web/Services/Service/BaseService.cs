using MadMicro.Web.Models;
using MadMicro.Web.Services.IService;
using Newtonsoft.Json;
using System.Net;
using System.Text;
using static MadMicro.Web.Utility.StaticDetail;

namespace MadMicro.Web.Services.Service
{
    public class BaseService : IBaseService
    {

        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ITokenProvider _tokenProvider;

        public BaseService(IHttpClientFactory httpClientFactory, ITokenProvider tokenProvider)
        {
            _httpClientFactory = httpClientFactory;
            _tokenProvider = tokenProvider;
        }

        public async Task<ResponseDTO?> SendAsync(RequestDTO requestDTO, bool withBearer = true)
        {
            try
            {
                var client = _httpClientFactory.CreateClient("MadMicroAPI");
                HttpRequestMessage message = new();

                switch (requestDTO.ContentType)
                {
                    case ContentType.Json:
                        message.Headers.Add("Accept", "application/json");
                        break;
                    case ContentType.MultipartFromData:
                        message.Headers.Add("Accept", "*/*");
                        break;
                    default:
                        break;
                }


                //token
                if (withBearer)
                {
                    var token = _tokenProvider.GetToken();
                    message.Headers.Add("Authorization", $"Bearer {token}");
                }

                message.RequestUri = new Uri(requestDTO.Url);

                switch (requestDTO.ContentType)
                {
                    case ContentType.Json:

                        if (requestDTO.Data is null) message.Content = null;

                        message.Content = new StringContent(JsonConvert.SerializeObject(requestDTO.Data), Encoding.UTF8, "application/json");

                        break;
                    case ContentType.MultipartFromData:
                        var content = new MultipartFormDataContent();

                        foreach (var item in requestDTO.Data.GetType().GetProperties())
                        {
                            var value = item.GetValue(requestDTO.Data);

                            switch (value)
                            {
                                case FormFile:
                                    var file = (FormFile)value;

                                    if (file is null) break;

                                    content.Add(new StreamContent(file.OpenReadStream()), item.Name, file.FileName);
                                    break;

                                default:
                                    content.Add(new StringContent(value is null ? string.Empty : value.ToString()!), item.Name);
                                    break;
                            }
                        }
                        message.Content = content;
                        break;
                }

                HttpResponseMessage? apiResponse = null;

                switch (requestDTO.ApiType)
                {
                    case ApiType.POST:
                        message.Method = HttpMethod.Post;
                        break;
                    case ApiType.DELETE:
                        message.Method = HttpMethod.Delete;
                        break;
                    case ApiType.PUT:
                        message.Method = HttpMethod.Put;
                        break;
                    default:
                        message.Method = HttpMethod.Get;
                        break;
                }

                apiResponse = await client.SendAsync(message);

                switch (apiResponse.StatusCode)
                {
                    case HttpStatusCode.NotFound:
                        return new() { IsSuccess = false, Message = "Not Found" };
                    case HttpStatusCode.Forbidden:
                        return new() { IsSuccess = false, Message = "Access Denied" };
                    case HttpStatusCode.Unauthorized:
                        return new() { IsSuccess = false, Message = "Unauthorized" };
                    case HttpStatusCode.InternalServerError:
                        return new() { IsSuccess = false, Message = "Internal Server Error" };
                    default:
                        var apiContent = await apiResponse.Content.ReadAsStringAsync();
                        var apiResponseDto = JsonConvert.DeserializeObject<ResponseDTO>(apiContent);
                        return apiResponseDto;
                }
            }
            catch (Exception ex)
            {
                var dto = new ResponseDTO()
                {
                    IsSuccess = false,
                    Message = ex.Message.ToString(),
                };
                return dto;
            }
        }
    }
}
