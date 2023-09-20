using Microsoft.AspNetCore.Identity;

namespace MadMicro.Services.AuthAPI.Models;

public class AppUser : IdentityUser
{
    public string Name { get; set; }


}
