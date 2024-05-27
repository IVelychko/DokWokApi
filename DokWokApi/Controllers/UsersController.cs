using AutoMapper;
using DokWokApi.BLL.Interfaces;
using DokWokApi.Exceptions;
using Microsoft.AspNetCore.Mvc;
using DokWokApi.BLL.Models.User;
using DokWokApi.Attributes;

namespace DokWokApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType<string>(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<UserModel>>> GetAllUsers()
    {
        try
        {
            var users = await _userService.GetAllAsync();

            return Ok(users);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }

    [Authorize("Customer")]
    [HttpGet("username/{userName?}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType<string>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<string>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<string>(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<UserModel>> GetUserByUserName(string? userName)
    {
        if (HttpContext.Items["User"] is not UserModel authenticatedUser)
        {
            return Unauthorized("Unauthorized access.");
        }
        else if (userName is not null && authenticatedUser.UserName != userName)
        {
            return Unauthorized("Unauthorized access.");
        }
        userName ??= authenticatedUser.UserName ?? string.Empty;

        try
        {
            var user = await _userService.GetByUserNameAsync(userName);
            if (user is null)
            {
                return NotFound("The user was not found.");
            }

            return Ok(user);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }

    [Authorize("Customer")]
    [HttpGet("id/{id?}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType<string>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<string>(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<UserModel>> GetUserById(string? id)
    {
        if (HttpContext.Items["User"] is not UserModel authenticatedUser)
        {
            return Unauthorized("Unauthorized access.");
        }
        else if (id is not null && authenticatedUser.Id != id)
        {
            return Unauthorized("Unauthorized access.");
        }
        id ??= authenticatedUser.Id ?? string.Empty;

        try
        {
            var user = await _userService.GetByIdAsync(id);
            if (user is null)
            {
                return NotFound("The user was not found.");
            }

            return Ok(user);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType<string>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<string>(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<UserModel>> AddUser(UserRegisterModel postModel, [FromServices] IMapper mapper)
    {
        try
        {
            var model = mapper.Map<UserModel>(postModel);
            var addedModel = await _userService.AddAsync(model, postModel.Password);
            return Ok(addedModel);
        }
        catch (ArgumentNullException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (UserException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }

    [HttpPut]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType<string>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<string>(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<UserModel>> UpdateUser(UserPutModel putModel, [FromServices] IMapper mapper)
    {
        try
        {
            var model = mapper.Map<UserModel>(putModel);
            var updatedModel = await _userService.UpdateAsync(model, putModel.Password);
            return Ok(updatedModel);
        }
        catch (ArgumentNullException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (UserException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }

    [HttpDelete("{userName}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType<string>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<string>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<string>(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> DeleteUser(string userName)
    {
        try
        {
            await _userService.DeleteAsync(userName);
            return Ok();
        }
        catch (EntityNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (UserException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }

    [HttpPost("login")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType<string>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<string>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<string>(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> LoginUser(UserLoginModel loginModel)
    {
        try
        {
            await _userService.AuthenticateLoginAsync(loginModel);
            return Ok();
        }
        catch (ArgumentNullException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (UserException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (EntityNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }

    [HttpPost("register")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType<string>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<string>(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> RegisterUser(UserRegisterModel registerModel)
    {
        try
        {
            await _userService.AuthenticateRegisterAsync(registerModel);
            return Ok();
        }
        catch (ArgumentNullException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (UserException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }

    [HttpGet("isloggedin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType<string>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<string>(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<UserModel>> IsLoggedIn()
    {
        try
        {
            var user = await _userService.IsLoggedInAsync();
            if (user is null)
            {
                return Unauthorized("Unauthorized.");
            }

            return Ok(user);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }

    [HttpGet("logout")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType<string>(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> LogOutUser()
    {
        try
        {
            await _userService.LogOutAsync();
            return Ok();
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }
}
