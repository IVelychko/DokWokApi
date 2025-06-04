using DokWokApi.Constants;
using DokWokApi.Filters;
using Domain.Abstractions.Services;
using Domain.Constants;
using Domain.DTOs.Requests.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DokWokApi.Controllers;

[ApiController]
[Route(ApiRoutes.Users.Group)]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IOrderService _orderService;
    private const string GetUserByIdRouteName = nameof(GetUserById);

    public UsersController(
        IUserService userService,
        IOrderService orderService)
    {
        _userService = userService;
        _orderService = orderService;
    }
    
    [HttpGet]
    [Authorize(Roles = UserRoles.Admin)]
    public async Task<IActionResult> GetAllUsers()
    {
        var users = await _userService.GetAllUsersAsync();
        return Ok(users);
    }
    
    [HttpGet(ApiRoutes.Users.GetUserById, Name = GetUserByIdRouteName)]
    [Authorize(Roles = $"{UserRoles.Admin},{UserRoles.Customer}")]
    [AuthorizeUserRetrievalById]
    public async Task<IActionResult> GetUserById(long id)
    {
        var user = await _userService.GetUserByIdAsync(id);
        return Ok(user);
    }
    
    // [HttpGet(ApiRoutes.Users.GetUserByUserName)]
    // [Authorize(Roles = $"{UserRoles.Admin},{UserRoles.Customer}")]
    // [AuthorizeUserRetrievalByName]
    // public async Task<IActionResult> GetUserByUserName(string userName)
    // {
    //     var user = await _userService.GetUserByUserNameAsync(userName);
    //     return Ok(user);
    // }
    
    [HttpPost]
    [Authorize(Roles = UserRoles.Admin)]
    public async Task<IActionResult> AddUser(AddUserRequest request)
    {
        var user = await _userService.AddAsync(request);
        return CreatedAtAction(nameof(GetUserById), new { id = user.Id }, user);
    }
    
    [HttpPut]
    [Authorize(Roles = $"{UserRoles.Admin},{UserRoles.Customer}")]
    [AuthorizeUserUpdate]
    public async Task<IActionResult> UpdateUser(UpdateUserRequest request)
    {
        var user = await _userService.UpdateAsync(request);
        return Ok(user);
    }
    
    [HttpPut(ApiRoutes.Users.UpdateCustomerPassword)]
    [Authorize(Roles = $"{UserRoles.Admin},{UserRoles.Customer}")]
    [AuthorizeUserUpdatePassword]
    public async Task<IActionResult> UpdateCustomerPassword(UpdatePasswordRequest request)
    {
        await _userService.UpdateCustomerPasswordAsync(request);
        return Ok();
    }
    
    [HttpPut(ApiRoutes.Users.UpdateCustomerPasswordAsAdmin)]
    [Authorize(Roles = UserRoles.Admin)]
    public async Task<IActionResult> UpdateCustomerPasswordAsAdmin(UpdatePasswordAsAdminRequest request)
    {
        await _userService.UpdateCustomerPasswordAsAdminAsync(request);
        return Ok();
    }
    
    [HttpDelete(ApiRoutes.Users.DeleteUserById)]
    [Authorize(Roles = UserRoles.Admin)]
    public async Task<IActionResult> DeleteUserById(long id)
    {
        await _userService.DeleteAsync(id);
        return Ok();
    }
    
    [HttpGet(ApiRoutes.Users.IsCustomerUserNameTaken)]
    public async Task<IActionResult> IsUserNameTaken(string userName)
    {
        var response = await _userService.IsUserNameTakenAsync(userName);
        return Ok(response);
    }
    
    [HttpGet(ApiRoutes.Users.IsCustomerEmailTaken)]
    public async Task<IActionResult> IsEmailTaken(string email)
    {
        var response = await _userService.IsEmailTakenAsync(email);
        return Ok(response);
    }
    
    [HttpGet(ApiRoutes.Users.IsCustomerPhoneNumberTaken)]
    public async Task<IActionResult> IsPhoneNumberTaken(string phoneNumber)
    {
        var response = await _userService.IsPhoneNumberTakenAsync(phoneNumber);
        return Ok(response);
    }

    [HttpGet(ApiRoutes.Users.GetAllOrdersByUserId)]
    [Authorize(Roles = $"{UserRoles.Admin},{UserRoles.Customer}")]
    public async Task<IActionResult> GetAllOrdersByUserId(long id)
    {
        var orders = await _orderService.GetAllByUserIdAsync(id);
        return Ok(orders);
    }
}