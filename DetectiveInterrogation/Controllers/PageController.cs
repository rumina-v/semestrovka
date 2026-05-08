using Microsoft.AspNetCore.Mvc;

namespace DetectiveInterrogation.Controllers;

public class PageController : Controller
{
    private readonly IWebHostEnvironment _environment;

    public PageController(IWebHostEnvironment environment)
    {
        _environment = environment;
    }

    [HttpGet("auth/login")]
    public IActionResult Login()
    {
        return HtmlPage("login.html");
    }

    [HttpGet("auth/register")]
    public IActionResult Register()
    {
        return HtmlPage("register.html");
    }

    [HttpGet("cases")]
    public IActionResult Cases()
    {
        return HtmlPage("desk.html");
    }

    [HttpGet("desk")]
    public IActionResult Desk()
    {
        return HtmlPage("desk.html");
    }

    [HttpGet("cases/details")]
    public IActionResult CaseDetails()
    {
        return HtmlPage("desk.html");
    }

    [HttpGet("interrogation/room")]
    public IActionResult InterrogationRoom()
    {
        return HtmlPage("room.html");
    }

    [HttpGet("interrogation/result")]
    public IActionResult InterrogationResult()
    {
        return HtmlPage("ending.html");
    }

    [HttpGet("ending")]
    public IActionResult Ending()
    {
        return HtmlPage("ending.html");
    }

    [HttpGet("achievements")]
    public IActionResult Achievements()
    {
        return HtmlPage("profile.html");
    }

    [HttpGet("profile")]
    public IActionResult Profile()
    {
        return HtmlPage("profile.html");
    }

    [HttpGet("admin")]
    public IActionResult Admin()
    {
        return HtmlPage("admin.html");
    }

    private PhysicalFileResult HtmlPage(string fileName)
    {
        var path = Path.Combine(_environment.WebRootPath, "pages", fileName);
        return PhysicalFile(path, "text/html; charset=utf-8");
    }
}
