using AutoMapper;
using DokWokApi.BLL.Interfaces;
using DokWokApi.Exceptions;
using Microsoft.AspNetCore.Mvc;
using DokWokApi.BLL.Models.User;
using DokWokApi.BLL;
using Microsoft.AspNetCore.Authorization;
using DokWokApi.BLL.Services;

namespace DokWokApi.Controllers;

[ApiController]
[Route("api/users")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    [Authorize(Roles = $"{UserRoles.Admin}")]
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

    [Authorize(Roles = $"{UserRoles.Admin}")]
    [HttpGet("customers")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType<string>(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<UserModel>>> GetAllCustomers()
    {
        try
        {
            var customers = await _userService.GetAllCustomersAsync();
            return Ok(customers);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }

    [Authorize(Roles = $"{UserRoles.Admin},{UserRoles.Customer}")]
    [HttpGet("username/{userName}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType<string>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<string>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<string>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<string>(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<UserModel>> GetUserByUserName(string userName, [FromServices] IUserService userService, bool isAdmin = false)
    {
        if (!isAdmin)
        {
            var token = HttpContext.Request.Headers.Authorization.FirstOrDefault()?.Split(" ")[^1];
            if (token is null)
                return Unauthorized();

            var authorizedUser = await userService.GetUserFromToken(token);
            if (authorizedUser is null)
            {
                return Unauthorized();
            }
            else if (authorizedUser.UserName != userName)
            {
                return StatusCode(StatusCodes.Status403Forbidden);
            }
        }

        try
        {
            var user = await _userService.GetByUserNameAsync(userName);
            if (user is null)
                return NotFound();

            return Ok(user);
        }
        catch (Exception)
        {
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

    [Authorize(Roles = $"{UserRoles.Admin},{UserRoles.Customer}")]
    [HttpGet("id/{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType<string>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<string>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<string>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<string>(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<UserModel>> GetUserById(string id, [FromServices] IUserService userService, bool isAdmin = false)
    {
        if (!isAdmin)
        {
            var token = HttpContext.Request.Headers.Authorization.FirstOrDefault()?.Split(" ")[^1];
            if (token is null)
                return Unauthorized();

            var authorizedUser = await userService.GetUserFromToken(token);
            if (authorizedUser is null)
            {
                return Unauthorized();
            }
            else if (authorizedUser.Id != id)
            {
                return StatusCode(StatusCodes.Status403Forbidden);
            }
        }

        try
        {
            var user = await _userService.GetByIdAsync(id);
            if (user is null)
                return NotFound();

            return Ok(user);
        }
        catch (Exception)
        {
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

    [Authorize(Roles = $"{UserRoles.Admin},{UserRoles.Customer}")]
    [HttpGet("customers/id/{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType<string>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<string>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<string>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<string>(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<UserModel>> GetCustomerById(string id, [FromServices] IUserService userService, bool isAdmin = false)
    {
        if (!isAdmin)
        {
            var token = HttpContext.Request.Headers.Authorization.FirstOrDefault()?.Split(" ")[^1];
            if (token is null)
                return Unauthorized();

            var authorizedUser = await userService.GetUserFromToken(token);
            if (authorizedUser is null)
            {
                return Unauthorized();
            }
            else if (authorizedUser.Id != id)
            {
                return Forbid();
            }
        }

        try
        {
            var user = await _userService.GetCustomerByIdAsync(id);
            if (user is null)
                return NotFound();

            return Ok(user);
        }
        catch (Exception)
        {
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

    [Authorize(Roles = $"{UserRoles.Admin}")]
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType<string>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<string>(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<UserModel>> AddUser(UserRegisterModel postModel, [FromServices] IMapper mapper)
    {
        try
        {
            var model = mapper.Map<UserModel>(postModel);
            var addedModel = await _userService.AddAsync(model, postModel.Password!);
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

    [Authorize(Roles = $"{UserRoles.Admin},{UserRoles.Customer}")]
    [HttpPut]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType<string>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<string>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<string>(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<UserModel>> UpdateUser(
        UserPutModel putModel, [FromServices] IUserService userService, [FromServices] IMapper mapper, bool isAdmin = false)
    {
        if (!isAdmin)
        {
            var token = HttpContext.Request.Headers.Authorization.FirstOrDefault()?.Split(" ")[^1];
            if (token is null)
                return Unauthorized();

            var authorizedUser = await userService.GetUserFromToken(token);
            if (authorizedUser is null)
            {
                return Unauthorized();
            }
            else if (authorizedUser.Id != putModel.Id)
            {
                return Forbid();
            }
        }

        try
        {
            var model = mapper.Map<UserModel>(putModel);
            var updatedModel = await _userService.UpdateAsync(model);
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
        catch (EntityNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }

    [Authorize(Roles = $"{UserRoles.Customer}")]
    [HttpPut("password")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType<string>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<string>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<string>(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> UpdateCustomerPassword(UserPasswordChangeModel model, [FromServices] IUserService userService)
    {
        var token = HttpContext.Request.Headers.Authorization.FirstOrDefault()?.Split(" ")[^1];
        if (token is null)
            return Unauthorized();

        var authorizedUser = await userService.GetUserFromToken(token);
        if (authorizedUser is null)
        {
            return Unauthorized();
        }
        else if (authorizedUser.Id != model.UserId)
        {
            return Forbid();
        }

        try
        {
            await _userService.UpdateCustomerPasswordAsync(model);
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

    [Authorize(Roles = $"{UserRoles.Admin}")]
    [HttpPut("password/asAdmin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType<string>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<string>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<string>(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> UpdateCustomerPasswordAsAdmin(UserPasswordChangeAsAdminModel model)
    {
        try
        {
            await _userService.UpdateCustomerPasswordAsAdminAsync(model);
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

    [Authorize(Roles = $"{UserRoles.Admin}")]
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType<string>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<string>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<string>(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> DeleteUserById(string id)
    {
        try
        {
            await _userService.DeleteAsync(id);
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

    [HttpPost("customers/login")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType<string>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<string>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<string>(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<AuthorizedUserModel>> LoginCustomer(UserLoginModel loginModel)
    {
        try
        {
            var loggedInUser = await _userService.AuthenticateCustomerLoginAsync(loginModel);
            return Ok(loggedInUser);
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

    [HttpPost("admins/login")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType<string>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<string>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<string>(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<AuthorizedUserModel>> LoginAdmin(UserLoginModel loginModel)
    {
        try
        {
            var loggedInUser = await _userService.AuthenticateAdminLoginAsync(loginModel);
            return Ok(loggedInUser);
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
    public async Task<ActionResult<AuthorizedUserModel>> RegisterUser(UserRegisterModel registerModel)
    {
        try
        {
            var registeredUser = await _userService.AuthenticateRegisterAsync(registerModel);
            return Ok(registeredUser);
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

    [HttpGet("customers/isloggedin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType<string>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<string>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<string>(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<UserModel>> IsCustomerTokenValid()
    {
        try
        {
            var token = HttpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ")[^1];
            if (token is null)
            {
                return BadRequest("There was no token provided");
            }

            var user = await _userService.IsCustomerTokenValidAsync(token);
            if (user is null)
            {
                return Unauthorized();
            }

            return Ok(user);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }

    [HttpGet("admins/isloggedin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType<string>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<string>(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<UserModel>> IsAdminTokenValid()
    {
        try
        {
            var token = HttpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ")[^1];
            if (token is null)
            {
                return BadRequest("There was no token provided");
            }

            var user = await _userService.IsAdminTokenValidAsync(token);
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

    [Authorize(Roles = $"{UserRoles.Admin}")]
    [HttpGet("roles")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType<string>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<string>(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<string>>> GetUserRoles(string userId)
    {
        try
        {
            var roles = await _userService.GetUserRolesAsync(userId);
            return Ok(roles);
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

    [HttpGet("customers/isUserNameTaken/{userName}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType<string>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<string>(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> IsUserNameTaken(string userName)
    {
        try
        {
            var isTaken = await _userService.IsUserNameTaken(userName);
            return new JsonResult(new { isTaken }) { StatusCode = StatusCodes.Status200OK };
        }
        catch (ArgumentNullException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }

    [HttpGet("customers/isEmailTaken/{email}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType<string>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<string>(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> IsEmailTaken(string email)
    {
        try
        {
            var isTaken = await _userService.IsEmailTaken(email);
            return new JsonResult(new { isTaken }) { StatusCode = StatusCodes.Status200OK };
        }
        catch (ArgumentNullException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }

    [HttpGet("customers/isPhoneTaken/{phoneNumber}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType<string>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<string>(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> IsPhoneNumberTaken(string phoneNumber)
    {
        try
        {
            var isTaken = await _userService.IsPhoneNumberTaken(phoneNumber);
            return new JsonResult(new { isTaken }) { StatusCode = StatusCodes.Status200OK };
        }
        catch (ArgumentNullException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }
}
