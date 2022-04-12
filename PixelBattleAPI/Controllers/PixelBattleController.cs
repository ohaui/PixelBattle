using System.Drawing;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PixelBattleAPI.Data;
using PixelBattleAPI.Enums;
using PixelBattleAPI.Model;
using PixelBattleAPI.Records;

namespace PixelBattleAPI.Controllers;

public class PixelBattleController : Controller
{
    private readonly PixelContext _context;
    private readonly ILogger<PixelBattleController> _logger;

    public PixelBattleController(PixelContext context, ILogger<PixelBattleController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [Authorize]
    [Route("/changepixel")]
    [HttpPost]
    public async Task<IActionResult> ChangePixel([FromQuery] Pixel pixel)
    {
        var pixelToChange = await _context.Pixels.FirstOrDefaultAsync(x => x.Position == pixel.Position);
        pixelToChange!.Color = pixel.Color;
        pixelToChange.UserOwn = pixel.UserOwn;
        pixelToChange.Date = DateTime.Now;

        await _context.SaveChangesAsync();
        return Ok();
    }

    [Authorize]
    [Route("/changepixelcolor")]
    [HttpPost]
    public async Task<IActionResult> ChangePixel(string position, string color)
    {
        var claimsUser = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var user = await _context.Users.FirstOrDefaultAsync(x => x.Username == claimsUser);
        
        _logger.LogCritical(user.Username);

        if (user.Privileges != Privileges.Admin)
        {
            if (DateTime.Now < user.LatestChange.AddSeconds(30))
                return BadRequest($"You cannot change any pixels before this time {user.LatestChange.AddSeconds(30)}");
        }

        var pixelToChange = await _context.Pixels.FirstOrDefaultAsync(x => x.Position == int.Parse(position));
        pixelToChange!.Color = Color.FromName(color).ToArgb();
        pixelToChange!.UserOwn = user;

        user.LatestChange = DateTime.Now;

        await _context.SaveChangesAsync();
        return Ok();
    }

    [Authorize]
    [Route("/createnewtable")]
    public async Task<IActionResult> CreateNewTable()
    {
        var table = _context.Pixels;

        var claimsUser = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var user = await _context.Users.FirstOrDefaultAsync(x => x.Username == claimsUser);

        if (user.Privileges != Privileges.Admin) return Unauthorized();

        for (var i = 1; i <= 4004; i++)
        {
            var pixel = table.FirstOrDefaultAsync(x => x.Position == i).Result;

            if (pixel != null)
            {
                pixel.UserOwn = null;
                pixel.Color = Color.White.ToArgb();
                pixel.Date = DateTime.Now;
            }
            else
            {
                await table.AddAsync(new Pixel
                {
                    Position = i, UserOwn = null, Color = Color.White.ToArgb(), Date = DateTime.Now
                });
            }
        }

        await _context.SaveChangesAsync();

        return Ok();
    }

    [Route("/pixels")]
    [HttpGet]
    public async Task<ActionResult<Pixel[]>> GetPixels()
    {
        return await _context.Pixels.ToArrayAsync();
    }

    [Route("/pixel")]
    [HttpGet]
    public async Task<IActionResult> GetPixel(int position)
    {
        return Json(await _context.Pixels.FirstOrDefaultAsync(x => x.Position == position));
    }

    [Route("/init")]
    [HttpPost]
    public async Task<IActionResult> InitialTable()
    {
        if (_context.Pixels.ToArray()[0] != null)
        {
            return BadRequest();
        }
        var table = _context.Pixels;
        
        for (var i = 1; i <= 4004; i++)
        {
            var pixel = table.FirstOrDefaultAsync(x => x.Position == i).Result;

            if (pixel != null)
            {
                pixel.UserOwn = null;
                pixel.Color = Color.White.ToArgb();
                pixel.Date = DateTime.Now;
            }
            else
            {
                await table.AddAsync(new Pixel
                {
                    Position = i, UserOwn = null, Color = Color.White.ToArgb(), Date = DateTime.Now
                });
            }
        }

        await _context.SaveChangesAsync();

        return Ok();
    }
}