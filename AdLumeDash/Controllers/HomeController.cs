using Microsoft.AspNetCore.Mvc;

namespace AdLumeDash.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}
