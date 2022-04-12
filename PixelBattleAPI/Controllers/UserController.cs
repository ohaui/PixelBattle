using Microsoft.AspNetCore.Mvc;

namespace PixelBattleAPI.Controllers;

public class UserController : Controller
{
    // GET
    public IActionResult Index()
    {
        return View();
    }
}