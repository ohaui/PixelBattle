using Microsoft.AspNetCore.Mvc;

namespace PixelBattleAPI.Controllers;

public class PixelBattleController : Controller
{
    // GET
    public IActionResult Index()
    {
        return View();
    }
}