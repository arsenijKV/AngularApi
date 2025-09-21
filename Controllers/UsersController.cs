namespace WebApi.Controllers;

using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using WebApi.Authorization;
using WebApi.Entities;
using WebApi.Models;
using WebApi.Services;

[Authorize] 
[ApiController]
[Route("[controller]")]
public class AuthController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IConfiguration _config;

    public AuthController(IUserService userService, IConfiguration config)
    {
        _userService = userService;
        _config = config;
    }

    
    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] CreateUserModel model)
    {
        try
        {
            var user = await _userService.Create(model);
            
            return CreatedAtAction(nameof(GetById), new { id = user.Id }, new { user.Id, user.Username, user.FirstName, user.LastName });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [AllowAnonymous]
    [HttpPost("authenticate")]
    public async Task<IActionResult> Authenticate([FromBody] AuthenticateModel model)
    {
        var user = await _userService.Authenticate(model);
        if (user == null) return BadRequest(new { message = "Неверный логин или пароль" });

        var token = GenerateJwtToken(user);
        return Ok(new
        {
            id = user.Id,
            username = user.Username,
            firstName = user.FirstName,
            lastName = user.LastName,
            token
        });
    }

    

     //auth/users
    [HttpGet("users")]
    public async Task<IActionResult> GetAll()
    {
        var users = await _userService.GetAll();
        
        var result = users.Select(u => new { u.Id, u.FirstName, u.LastName, u.Username });
        return Ok(result);
    }

    //auth/users/{id}
    [HttpGet("users/{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        
        var user = (await _userService.GetAll()).FirstOrDefault(u => u.Id == id);

        if (user == null)
            return NotFound();

        return Ok(new { user.Id, user.FirstName, user.LastName, user.Username });
    }

    //auth/users/{id}
    [HttpPut("users/{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateUserModel model)
    {
        try
        {
            var updated = await _userService.Update(id, model);
            if (updated == null)
                return NotFound();

            return Ok(new { message = "Пользователь обновлён" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    //auth/users/{id}
    [HttpDelete("users/{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var deleted = await _userService.Delete(id);
            if (deleted == null)
                return NotFound();

            return NoContent(); 
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    
    private string GenerateJwtToken(WebApi.Entities.User user)
    {
        var jwt = _config.GetSection("Jwt");
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt["Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Username)
        };

        var token = new JwtSecurityToken(
            issuer: jwt["Issuer"],
            audience: jwt["Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(int.Parse(jwt["ExpiresMinutes"]!)),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
