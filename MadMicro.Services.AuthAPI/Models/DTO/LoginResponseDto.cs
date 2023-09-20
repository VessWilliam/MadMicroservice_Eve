namespace MadMicro.Services.AuthAPI.Models.DTO;

public class LoginResponseDto
{
    public UserDTO User { get; set; }
    public string Token { get; set; }
}
