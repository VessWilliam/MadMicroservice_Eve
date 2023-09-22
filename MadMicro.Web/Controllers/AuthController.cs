using MadMicro.Web.Models;
using MadMicro.Web.Services.IService;
using Microsoft.AspNetCore.Mvc;

namespace MadMicro.Web.Controllers;

public class AuthController : Controller
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpGet]
    public IActionResult Login()
    {
        UserLoginDTO userLoginDTO = new();
        return View(userLoginDTO);
    }

    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }

    public IActionResult Logout()
    {
        return View();
    }
}
