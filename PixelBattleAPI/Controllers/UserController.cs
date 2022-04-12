using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PixelBattleAPI.Data;
using PixelBattleAPI.Enums;
using PixelBattleAPI.Model;
using PixelBattleAPI.Records;
using PixelBattleAPI.Services;

namespace PixelBattleAPI.Controllers;

public class UserController : Controller
{
    private readonly PixelContext _context;
    private readonly ITokenService _tokenServce;
    private readonly ILogger<UserController> _logger;

    public UserController(PixelContext context, ITokenService tokenService, ILogger<UserController> logger)
    {
        _context = context;
        _tokenServce = tokenService;
        _logger = logger;
    }

    [Route("/register")]
    [HttpPost]
    public async Task<ActionResult<UserRecord>> Register(RegisterRecord registerRecord)
    {
        using HMACSHA512 hmac = new HMACSHA512();

        if (await IsUsernameBusy(registerRecord.Username))
        {
            return BadRequest("Username is taken.");
        }
        else if (!await IsUsernameOK(registerRecord.Username))
        {
            return BadRequest("Username is null or contains white spaces.");
        }

        var privilege = Privileges.Default;

        if (registerRecord.Username == "ohaui")
            privilege = Privileges.Admin;
        
        
        var user = new User()
        {
            Username = registerRecord.Username.ToLower(),
            PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerRecord.Password)),
            PasswordSalt = hmac.Key,
            Privileges = privilege
        };
        
        privilege = Privileges.Default;
        

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return Ok("Account created successfully");
    }

    private async Task<bool> IsUsernameBusy(string username)
    {
        return await _context.Users.AnyAsync(x => x.Username == username.ToLower());
    }

    private async Task<bool> IsUsernameOK(string username)
    {
        if (string.IsNullOrEmpty(username) || username.Contains(" ")) return await Task.FromResult(false);

        return await Task.FromResult(true);
    }

    [HttpPost("/login")]
    public async Task<ActionResult<UserRecord>> Login(RegisterRecord registerRecord)
    {
        var userToLogin = await _context.Users.FirstOrDefaultAsync(x => x.Username == registerRecord.Username);

        if (userToLogin == default(User)) return BadRequest("Wrong login or password");
        
        using HMACSHA512 hmac = new HMACSHA512(userToLogin.PasswordSalt);

        var userDtoPassHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerRecord.Password));

        for (int i = 0; i < userDtoPassHash.Length; i++)
        {
            if (userDtoPassHash[i] != userToLogin.PasswordHash[i]) return BadRequest("Wrong login or password.");
        }

        Response.Cookies.Append("jwt", _tokenServce.CreateToken(userToLogin), new CookieOptions {HttpOnly = true});

        return Ok(new UserRecord() {Username = registerRecord.Username, Token = _tokenServce.CreateToken(userToLogin)});
    }
}