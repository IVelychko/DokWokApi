using AutoMapper;
using DokWokApi.BLL.Interfaces;
using DokWokApi.Exceptions;
using Microsoft.AspNetCore.Mvc;
using DokWokApi.BLL.Models.User;
using DokWokApi.BLL;
using Microsoft.AspNetCore.Authorization;

namespace DokWokApi.Controllers;

[ApiController]
[Route("api/users")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    private readonly ILogger<UsersController> _logger;

    public UsersController(IUserService userService, ILogger<UsersController> logger)
    {
        _userService = userService;
        _logger = logger;
    }

    [Authorize(Roles = $"{UserRoles.Admin}")]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserModel>>> GetAllUsers()
    {
        try
        {
            var users = await _userService.GetAllAsync();
            return Ok(users);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Server error.");
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

    [Authorize(Roles = $"{UserRoles.Admin}")]
    [HttpGet("customers")]
    public async Task<ActionResult<IEnumerable<UserModel>>> GetAllCustomers()
    {
        try
        {
            var customers = await _userService.GetAllCustomersAsync();
            return Ok(customers);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Server error.");
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

    [Authorize(Roles = $"{UserRoles.Admin},{UserRoles.Customer}")]
    [HttpGet("username/{userName}")]
    public async Task<ActionResult<UserModel>> GetUserByUserName(string userName, [FromServices] IUserService userService)
    {
        var userId = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "id")?.Value;
        if (userId is null)
        {
            return Unauthorized();
        }

        var authorizedUser = await userService.GetByIdAsync(userId);
        if (authorizedUser is null)
        {
            return Unauthorized();
        }

        var roles = await userService.GetUserRolesAsync(authorizedUser.Id!);
        if (!roles.Contains(UserRoles.Admin) && authorizedUser.UserName != userName)
        {
            return new ForbidResult();
        }

        try
        {
            var user = await _userService.GetByUserNameAsync(userName);
            if (user is null)
            {
                return NotFound();
            }
                
            return Ok(user);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Server error.");
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

    [Authorize(Roles = $"{UserRoles.Admin},{UserRoles.Customer}")]
    [HttpGet("id/{id}")]
    public async Task<ActionResult<UserModel>> GetUserById(string id, [FromServices] IUserService userService)
    {
        var userId = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "id")?.Value;
        if (userId is null)
        {
            return Unauthorized();
        }

        var authorizedUser = await userService.GetByIdAsync(userId);
        if (authorizedUser is null)
        {
            return Unauthorized();
        }

        var roles = await userService.GetUserRolesAsync(authorizedUser.Id!);
        if (!roles.Contains(UserRoles.Admin) && authorizedUser.Id != id)
        {
            return new ForbidResult();
        }

        try
        {
            var user = await _userService.GetByIdAsync(id);
            if (user is null)
            {
                return NotFound();
            }
                
            return Ok(user);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Server error.");
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

    [Authorize(Roles = $"{UserRoles.Admin},{UserRoles.Customer}")]
    [HttpGet("customers/id/{id}")]
    public async Task<ActionResult<UserModel>> GetCustomerById(string id, [FromServices] IUserService userService)
    {
        var userId = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "id")?.Value;
        if (userId is null)
        {
            return Unauthorized();
        }

        var authorizedUser = await userService.GetByIdAsync(userId);
        if (authorizedUser is null)
        {
            return Unauthorized();
        }

        var roles = await userService.GetUserRolesAsync(authorizedUser.Id!);
        if (!roles.Contains(UserRoles.Admin) && authorizedUser.Id != id)
        {
            return new ForbidResult();
        }

        try
        {
            var user = await _userService.GetCustomerByIdAsync(id);
            if (user is null)
            {
                return NotFound();
            }

            return Ok(user);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Server error.");
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

    [Authorize(Roles = $"{UserRoles.Admin}")]
    [HttpPost]
    public async Task<ActionResult<UserModel>> AddUser(UserRegisterModel postModel, [FromServices] IMapper mapper)
    {
        try
        {
            var model = mapper.Map<UserModel>(postModel);
            var addedModel = await _userService.AddAsync(model, postModel.Password!);
            return CreatedAtAction(nameof(GetCustomerById), new { id = addedModel.Id }, addedModel);
        }
        catch (ArgumentNullException ex)
        {
            _logger.LogInformation(ex, "Bad request.");
            return BadRequest();
        }
        catch (UserException ex)
        {
            _logger.LogInformation(ex, "Bad request.");
            return BadRequest();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Server error.");
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

    [Authorize(Roles = $"{UserRoles.Admin},{UserRoles.Customer}")]
    [HttpPut]
    public async Task<ActionResult<UserModel>> UpdateUser(
        UserPutModel putModel, [FromServices] IUserService userService, [FromServices] IMapper mapper)
    {
        var userId = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "id")?.Value;
        if (userId is null)
        {
            return Unauthorized();
        }

        var authorizedUser = await userService.GetByIdAsync(userId);
        if (authorizedUser is null)
        {
            return Unauthorized();
        }

        var roles = await userService.GetUserRolesAsync(authorizedUser.Id!);
        if (!roles.Contains(UserRoles.Admin) && authorizedUser.Id != putModel.Id)
        {
            return new ForbidResult();
        }

        try
        {
            var model = mapper.Map<UserModel>(putModel);
            var updatedModel = await _userService.UpdateAsync(model);
            return Ok(updatedModel);
        }
        catch (ArgumentNullException ex)
        {
            _logger.LogInformation(ex, "Bad request.");
            return BadRequest();
        }
        catch (UserException ex)
        {
            _logger.LogInformation(ex, "Bad request.");
            return BadRequest();
        }
        catch (EntityNotFoundException ex)
        {
            _logger.LogInformation(ex, "Not found.");
            return NotFound();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Server error.");
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

    [Authorize(Roles = $"{UserRoles.Customer}")]
    [HttpPut("password")]
    public async Task<ActionResult> UpdateCustomerPassword(UserPasswordChangeModel model, [FromServices] IUserService userService)
    {
        var userId = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "id")?.Value;
        if (userId is null)
        {
            return Unauthorized();
        }

        var authorizedUser = await userService.GetByIdAsync(userId);
        if (authorizedUser is null)
        {
            return Unauthorized();
        }
        else if (authorizedUser.Id != model.UserId)
        {
            return new ForbidResult();
        }

        try
        {
            await _userService.UpdateCustomerPasswordAsync(model);
            return Ok();
        }
        catch (ArgumentNullException ex)
        {
            _logger.LogInformation(ex, "Bad request.");
            return BadRequest();
        }
        catch (UserException ex)
        {
            _logger.LogInformation(ex, "Bad request.");
            return BadRequest();
        }
        catch (EntityNotFoundException ex)
        {
            _logger.LogInformation(ex, "Not found.");
            return NotFound();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Server error.");
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

    [Authorize(Roles = $"{UserRoles.Admin}")]
    [HttpPut("password/asAdmin")]
    public async Task<ActionResult> UpdateCustomerPasswordAsAdmin(UserPasswordChangeAsAdminModel model)
    {
        try
        {
            await _userService.UpdateCustomerPasswordAsAdminAsync(model);
            return Ok();
        }
        catch (ArgumentNullException ex)
        {
            _logger.LogInformation(ex, "Bad request.");
            return BadRequest();
        }
        catch (UserException ex)
        {
            _logger.LogInformation(ex, "Bad request.");
            return BadRequest();
        }
        catch (EntityNotFoundException ex)
        {
            _logger.LogInformation(ex, "Not found.");
            return NotFound();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Server error.");
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

    [Authorize(Roles = $"{UserRoles.Admin}")]
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteUserById(string id)
    {
        try
        {
            await _userService.DeleteAsync(id);
            return Ok();
        }
        catch (EntityNotFoundException ex)
        {
            _logger.LogInformation(ex, "Not found.");
            return NotFound();
        }
        catch (UserException ex)
        {
            _logger.LogInformation(ex, "Bad request.");
            return BadRequest();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Server error.");
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

    [HttpPost("customers/login")]
    public async Task<ActionResult<AuthorizedUserModel>> LoginCustomer(UserLoginModel loginModel)
    {
        try
        {
            var loggedInUser = await _userService.AuthenticateCustomerLoginAsync(loginModel);
            return Ok(loggedInUser);
        }
        catch (ArgumentNullException ex)
        {
            _logger.LogInformation(ex, "Bad request.");
            return BadRequest();
        }
        catch (UserException ex)
        {
            _logger.LogInformation(ex, "Bad request.");
            return BadRequest();
        }
        catch (EntityNotFoundException ex)
        {
            _logger.LogInformation(ex, "Not found.");
            return NotFound();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Server error.");
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

    [HttpPost("admins/login")]
    public async Task<ActionResult<AuthorizedUserModel>> LoginAdmin(UserLoginModel loginModel)
    {
        try
        {
            var loggedInUser = await _userService.AuthenticateAdminLoginAsync(loginModel);
            return Ok(loggedInUser);
        }
        catch (ArgumentNullException ex)
        {
            _logger.LogInformation(ex, "Bad request.");
            return BadRequest();
        }
        catch (UserException ex)
        {
            _logger.LogInformation(ex, "Bad request.");
            return BadRequest();
        }
        catch (EntityNotFoundException ex)
        {
            _logger.LogInformation(ex, "Not found.");
            return NotFound();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Server error.");
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

    [HttpPost("register")]
    public async Task<ActionResult<AuthorizedUserModel>> RegisterUser(UserRegisterModel registerModel)
    {
        try
        {
            var registeredUser = await _userService.AuthenticateRegisterAsync(registerModel);
            return Ok(registeredUser);
        }
        catch (ArgumentNullException ex)
        {
            _logger.LogInformation(ex, "Bad request.");
            return BadRequest();
        }
        catch (UserException ex)
        {
            _logger.LogInformation(ex, "Not found.");
            return BadRequest();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Server error.");
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

    [Authorize(Roles = $"{UserRoles.Customer},{UserRoles.Admin}")]
    [HttpGet("customers/isloggedin")]
    public async Task<ActionResult<UserModel>> IsCustomerTokenValid([FromServices] IUserService userService)
    {
        try
        {
            var userId = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "id")?.Value;
            if (userId is null)
            {
                return Unauthorized();
            }

            var authorizedUser = await userService.GetByIdAsync(userId);
            if (authorizedUser is null)
            {
                return Unauthorized();
            }

            return Ok(authorizedUser);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Server error.");
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

    [Authorize(Roles = $"{UserRoles.Admin}")]
    [HttpGet("admins/isloggedin")]
    public async Task<ActionResult<UserModel>> IsAdminTokenValid([FromServices] IUserService userService)
    {
        try
        {
            var userId = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "id")?.Value;
            if (userId is null)
            {
                return Unauthorized();
            }

            var authorizedUser = await userService.GetByIdAsync(userId);
            if (authorizedUser is null)
            {
                return Unauthorized();
            }

            return Ok(authorizedUser);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Server error.");
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

    [Authorize(Roles = $"{UserRoles.Admin}")]
    [HttpGet("roles")]
    public async Task<ActionResult<IEnumerable<string>>> GetUserRoles(string userId)
    {
        try
        {
            var roles = await _userService.GetUserRolesAsync(userId);
            return Ok(roles);
        }
        catch (EntityNotFoundException ex)
        {
            _logger.LogInformation(ex, "Not found.");
            return NotFound();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Server error.");
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

    [HttpGet("customers/isUserNameTaken/{userName}")]
    public async Task<ActionResult> IsUserNameTaken(string userName)
    {
        try
        {
            var isTaken = await _userService.IsUserNameTaken(userName);
            return new JsonResult(new { isTaken }) { StatusCode = StatusCodes.Status200OK };
        }
        catch (ArgumentNullException ex)
        {
            _logger.LogInformation(ex, "Bad request.");
            return BadRequest();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Server error.");
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

    [HttpGet("customers/isEmailTaken/{email}")]
    public async Task<ActionResult> IsEmailTaken(string email)
    {
        try
        {
            var isTaken = await _userService.IsEmailTaken(email);
            return new JsonResult(new { isTaken }) { StatusCode = StatusCodes.Status200OK };
        }
        catch (ArgumentNullException ex)
        {
            _logger.LogInformation(ex, "Bad request.");
            return BadRequest();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Server error.");
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

    [HttpGet("customers/isPhoneTaken/{phoneNumber}")]
    public async Task<ActionResult> IsPhoneNumberTaken(string phoneNumber)
    {
        try
        {
            var isTaken = await _userService.IsPhoneNumberTaken(phoneNumber);
            return new JsonResult(new { isTaken }) { StatusCode = StatusCodes.Status200OK };
        }
        catch (ArgumentNullException ex)
        {
            _logger.LogInformation(ex, "Bad request.");
            return BadRequest();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Server error.");
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }
}
