namespace WebApi.Controllers;

using Microsoft.AspNetCore.Mvc;
using System.Reflection;
using WebApi.Authorization;
using WebApi.Entities;
using WebApi.Models;
using WebApi.Services;

[Authorize]
[ApiController]
[Route("[controller]")]
public class UsersController : ControllerBase
{
    private IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    [AllowAnonymous]
    [HttpPost("authenticate")]
    public async Task<IActionResult> Authenticate([FromBody]AuthenticateModel model)
    {
       
        try
        {
            var user = await _userService.Authenticate(model);
            return Ok(user);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }

    }
    
    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<IActionResult> Create([FromBody] CreateUserModel model)
    {
        try
        {
            var user = await _userService.Create(model);
            return CreatedAtAction(nameof(GetById), new { id = user.Id }, user);
            
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });

        }
    }
    //get user by id
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var user = (await _userService.GetAll()).FirstOrDefault(u => u.Id == id);

        if (user == null)
            return NotFound();

        return Ok(user);
    }

    //get all users
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var users = await _userService.GetAll();
        return Ok(users);
    }


    //update
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateUserModel model)
    {
        try 
        { 
            var user = await _userService.Update(id, model);
            return Ok(new { message = "Пользователь обновлен" });
        }
        catch(Exception ex){ return BadRequest(new { message = ex.Message }); }
        
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var delete = await _userService.Delete(id);
            return Ok(new { message = "Пользователь удален" });
        }
        catch (Exception ex) { return NotFound(new { message = ex.Message }); }
        


    }

}
