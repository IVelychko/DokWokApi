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
    private readonly IUserService userService;

    public UsersController(IUserService userService)
    {
        this.userService = userService;
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType<string>(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<UserModel>>> GetAllUsers()
    {
        try
        {
            var users = await userService.GetAllAsync();

            return Ok(users);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }

    [Authorize("Customer")]
    [HttpGet("username/{userName}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType<string>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<string>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<string>(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<UserModel>> GetUserByUserName(string userName)
    {
        if (HttpContext.Items["User"] is not UserModel authenticatedUser || authenticatedUser.UserName != userName)
        {
            return Unauthorized("Unauthorized access.");
        }

        try
        {
            var user = await userService.GetByUserNameAsync(userName);
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

    [HttpGet("id/{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType<string>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<string>(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<UserModel>> GetUserById(string id)
    {
        try
        {
            var user = await userService.GetByIdAsync(id);
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
            var addedModel = await userService.AddAsync(model, postModel.Password);
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
            var updatedModel = await userService.UpdateAsync(model, putModel.Password);
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
            await userService.DeleteAsync(userName);
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

    [HttpGet("login")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType<string>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<string>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<string>(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<AuthenticationResponse>> LoginUser(UserLoginModel loginModel)
    {
        try
        {
            var response = await userService.AuthenticateLoginAsync(loginModel);
            return Ok(response);
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

    [HttpGet("register")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType<string>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<string>(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<AuthenticationResponse>> RegisterUser(UserRegisterModel registerModel)
    {
        try
        {
            var response = await userService.AuthenticateRegisterAsync(registerModel);
            return Ok(response);
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
}
