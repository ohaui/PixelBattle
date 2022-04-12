using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PixelBattleAPI.Data;
using PixelBattleAPI.Model;

namespace PixelBattleAPI.Controllers;

public class UsersController : Controller
{
    private readonly PixelContext _context;

    public UsersController(PixelContext context)
    {
        _context = context;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<User>> GetUserById(int id)
    {
        return (await _context.Users.FirstOrDefaultAsync(x => x.Id == id))!;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<User>>> GetUsers()
    {
        return await _context.Users.ToListAsync();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUserById(int id)
    {
        try
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == id);
            _context.Users.Remove(user!);
            await _context.SaveChangesAsync();
            return Ok();
        }
        catch (Exception e)
        {
            return Unauthorized();
        }
    }
}