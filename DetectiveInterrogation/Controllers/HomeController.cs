using Microsoft.AspNetCore.Mvc;

namespace DetectiveInterrogation.Controllers;

public class HomeController : Controller
{
    private readonly IWebHostEnvironment _environment;

    public HomeController(IWebHostEnvironment environment)
    {
        _environment = environment;
    }

    public IActionResult Index()
    {
        return HtmlPage("home.html");
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return HtmlPage("home.html");
    }

    private PhysicalFileResult HtmlPage(string fileName)
    {
        var path = Path.Combine(_environment.WebRootPath, "pages", fileName);
        return PhysicalFile(path, "text/html; charset=utf-8");
    }
}
