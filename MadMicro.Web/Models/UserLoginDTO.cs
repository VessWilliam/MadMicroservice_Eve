using System.ComponentModel.DataAnnotations;

namespace MadMicro.Web.Models;

public class UserLoginDTO
{
    [Required]
    public string UserName { get; set; }
    [Required]
    public string Password { get; set; }
}
