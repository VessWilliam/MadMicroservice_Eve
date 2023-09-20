using MadMicro.Services.AuthAPI.DataContext;
using MadMicro.Services.AuthAPI.Migrations;
using MadMicro.Services.AuthAPI.Models;
using MadMicro.Services.AuthAPI.Models.DTO;
using MadMicro.Services.AuthAPI.Service.IService;
using Microsoft.AspNetCore.Identity;

namespace MadMicro.Services.AuthAPI.Service;

public class AuthService : IAuthService
{
    private readonly AppDbContext _db;
    private readonly UserManager<AppUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    public AuthService(AppDbContext db, UserManager<AppUser> userManager,
        RoleManager<IdentityRole> roleManager)
    {
        _db = db;
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task<LoginResponseDto> Login(UserLoginDTO userLoginDTO)
    {


        var user = _db.AppUsers.FirstOrDefault(u => u.UserName.ToLower() == userLoginDTO.UserName.ToLower());
        bool isValid = await _userManager.CheckPasswordAsync(user, userLoginDTO.Password);
        
        if (user == null ||  isValid == false) 
        {
            return new LoginResponseDto() { User = null, Token = ""};
        }

        

        UserDTO userDTO = new()
        {
            Email = user.Email,
            ID = user.Id,
            Name = user.Name,
            PhoneNumber = user.PhoneNumber
        };

        LoginResponseDto loginRes = new() 
        {
            User = userDTO,
            Token = ""
        };


        return loginRes;
    }

    public async Task<string> Register(UserRegisterDTO userRegisterDTO)
    {
        AppUser users = new()
        {
            UserName = userRegisterDTO.Email,
            Email = userRegisterDTO.Email,
            NormalizedUserName = userRegisterDTO.Email.ToUpper(),
            Name = userRegisterDTO.Name,
            PhoneNumber = userRegisterDTO.PhoneNumber,
        };

        try
        {
            var result = await _userManager.CreateAsync(users,userRegisterDTO.Password);
            if(result.Succeeded)
            {
                var userToReturn =  _db.AppUsers.First(u => u.UserName == userRegisterDTO.Email);

                UserDTO userDTO = new()
                {
                    Email = userToReturn.Email,
                    ID = userToReturn.Id,
                    Name = userToReturn.Name,
                    PhoneNumber = userToReturn.PhoneNumber
                };
                return "";
            }
            else
            {
                return result.Errors.FirstOrDefault().Description;
            }
        }
        catch (Exception ex)
        {

            throw;
        }
    }
}
