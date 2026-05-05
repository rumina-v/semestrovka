using Microsoft.AspNetCore.Mvc;

namespace DetectiveInterrogation.Controllers;

public class PageController : Controller
{
    [HttpGet("auth/login")]
    public IActionResult Login()
    {
        return View("~/Views/Auth/Login.cshtml");
    }

    [HttpGet("auth/register")]
    public IActionResult Register()
    {
        return View("~/Views/Auth/Register.cshtml");
    }

    [HttpGet("cases")]
    public IActionResult Cases()
    {
        return View("~/Views/Case/Index.cshtml");
    }

    [HttpGet("desk")]
    public IActionResult Desk()
    {
        return View("~/Views/Case/Index.cshtml");
    }

    [HttpGet("cases/details")]
    public IActionResult CaseDetails()
    {
        return View("~/Views/Case/Details.cshtml");
    }

    [HttpGet("interrogation/room")]
    public IActionResult InterrogationRoom()
    {
        return View("~/Views/Interrogation/Room.cshtml");
    }

    [HttpGet("interrogation/result")]
    public IActionResult InterrogationResult()
    {
        return View("~/Views/Interrogation/Result.cshtml");
    }

    [HttpGet("ending")]
    public IActionResult Ending()
    {
        return View("~/Views/Interrogation/Result.cshtml");
    }

    [HttpGet("achievements")]
    public IActionResult Achievements()
    {
        return View("~/Views/Achievement/Index.cshtml");
    }

    [HttpGet("profile")]
    public IActionResult Profile()
    {
        return View("~/Views/Achievement/Index.cshtml");
    }

    [HttpGet("admin")]
    public IActionResult Admin()
    {
        return View("~/Views/Admin/Index.cshtml");
    }
}
