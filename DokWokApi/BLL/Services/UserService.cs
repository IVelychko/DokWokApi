using AutoMapper;
using DokWokApi.BLL.Interfaces;
using DokWokApi.BLL.Models.User;
using DokWokApi.DAL;
using DokWokApi.DAL.Entities;
using DokWokApi.Exceptions;
using DokWokApi.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;

namespace DokWokApi.BLL.Services;

public class UserService : IUserService
{
    private readonly UserManager<ApplicationUser> _userManager;

    private readonly IMapper _mapper;

    private readonly ISecurityTokenService<UserModel, JwtSecurityToken> _securityTokenService;

    private readonly ISession? _session;

    public UserService(UserManager<ApplicationUser> userManager, IMapper mapper, 
        ISecurityTokenService<UserModel, JwtSecurityToken> securityTokenService,
        IHttpContextAccessor httpContextAccessor)
    {
        _userManager = userManager;
        _mapper = mapper;
        _securityTokenService = securityTokenService;
        _session = httpContextAccessor.HttpContext?.Session;
    }

    public async Task<UserModel> AddAsync(UserModel model, string password)
    {
        model = ServiceHelper.ThrowIfNull(model, "The passed model is null.");
        var user = _mapper.Map<ApplicationUser>(model);
        user.Id = Guid.NewGuid().ToString();
        var result = await _userManager.CreateAsync(user, password);
        ServiceHelper.ThrowIfNotSucceeded(result.Succeeded, result.Errors);

        var addedUser = await _userManager.FindByNameAsync(user.UserName!);
        await _userManager.AddToRoleAsync(addedUser!, UserRoles.Customer);
        return _mapper.Map<UserModel>(addedUser!);
    }

    public async Task DeleteAsync(string userName)
    {
        var user = await _userManager.FindByNameAsync(userName);
        user = RepositoryHelper.ThrowEntityNotFoundIfNull(user, "There is no user with this user name in the database.");

        var isAdmin = await _userManager.IsInRoleAsync(user, UserRoles.Admin);
        ServiceHelper.ThrowUserExcepyionIfTrue(isAdmin, "Action is not allowed");

        var result = await _userManager.DeleteAsync(user);
        ServiceHelper.ThrowIfNotSucceeded(result.Succeeded, result.Errors);
    }

    public async Task<IEnumerable<UserModel>> GetAllAsync()
    {
        var queryable = _userManager.Users.AsNoTracking();
        var entities = await queryable.ToListAsync();
        var models = _mapper.Map<IEnumerable<UserModel>>(entities);
        return models;
    }

    public async Task<IEnumerable<UserModel>> GetAllCustomersAsync()
    {
        var entities = await _userManager.GetUsersInRoleAsync(UserRoles.Customer);
        var models = _mapper.Map<IEnumerable<UserModel>>(entities);
        return models;
    }

    public async Task<UserModel?> GetByUserNameAsync(string userName)
    {
        var entity = await _userManager.FindByNameAsync(userName);
        if (entity is null)
        {
            return null;
        }

        var model = _mapper.Map<UserModel>(entity);
        return model;
    }

    public async Task<UserModel?> GetByIdAsync(string id)
    {
        var entity = await _userManager.FindByIdAsync(id);
        if (entity is null)
        {
            return null;
        }

        var model = _mapper.Map<UserModel>(entity);
        return model;
    }

    public async Task<UserModel?> GetCustomerByIdAsync(string id)
    {
        var entity = await _userManager.FindByIdAsync(id);
        if (entity is null)
        {
            return null;
        }

        var isCustomer = await _userManager.IsInRoleAsync(entity, UserRoles.Customer);
        if (!isCustomer)
        {
            return null;
        }

        var model = _mapper.Map<UserModel>(entity);
        return model;
    }

    public async Task<UserModel> UpdateAsync(UserModel model)
    {
        model = ServiceHelper.ThrowIfNull(model, "The passed model is null.");
        var user = await _userManager.FindByIdAsync(model.Id!);
        user = RepositoryHelper.ThrowEntityNotFoundIfNull(user, "There is no user with this ID in the database.");

        user.FirstName = model.FirstName;
        user.UserName = model.UserName;
        user.Email = model.Email;
        user.PhoneNumber = model.PhoneNumber;
        var result = await _userManager.UpdateAsync(user);
        ServiceHelper.ThrowIfNotSucceeded(result.Succeeded, result.Errors);

        var updatedUser = await _userManager.FindByNameAsync(user.UserName!);
        return _mapper.Map<UserModel>(updatedUser);
    }

    public async Task UpdateCustomerPasswordAsync(UserPasswordChangeModel model)
    {
        model = ServiceHelper.ThrowIfNull(model, "The passed model is null.");
        var user = await _userManager.FindByIdAsync(model.UserId!);
        user = RepositoryHelper.ThrowEntityNotFoundIfNull(user, "There is no user with this ID in the database.");
        bool isAdmin = await _userManager.IsInRoleAsync(user, UserRoles.Admin);
        ServiceHelper.ThrowUserExcepyionIfTrue(isAdmin, "Action is not allowed");
        bool isOldPasswordValid = await _userManager.CheckPasswordAsync(user, model.OldPassword!);
        ServiceHelper.ThrowUserExcepyionIfTrue(!isOldPasswordValid, "The old password is not valid.");

        await _userManager.RemovePasswordAsync(user);
        var passwordResult = await _userManager.AddPasswordAsync(user, model.NewPassword!);
        ServiceHelper.ThrowIfNotSucceeded(passwordResult.Succeeded, passwordResult.Errors);
    }

    public async Task<IEnumerable<string>> GetUserRolesAsync(string userId)
    {   
        var user = await _userManager.FindByIdAsync(userId);
        user = RepositoryHelper.ThrowEntityNotFoundIfNull(user, "There is no user with this id.");
        var roles = await _userManager.GetRolesAsync(user);
        return roles;
    }

    public async Task<UserModel> AuthenticateLoginAsync(UserLoginModel model)
    {
        model = ServiceHelper.ThrowIfNull(model, "The passed model is null.");

        var user = await _userManager.FindByNameAsync(model.UserName);
        user = RepositoryHelper.ThrowEntityNotFoundIfNull(user, "The credentials are wrong.");

        var isValidPassword = await _userManager.CheckPasswordAsync(user, model.Password);
        ServiceHelper.ThrowIfUserIsNotValid(isValidPassword, "The credentials are wrong.");

        var userModel = _mapper.Map<UserModel>(user);
        var token = _securityTokenService.CreateToken(userModel);
        if (_session is null)
        {
            throw new SessionException(nameof(_session), "The session is null");
        }

        await _session.SetStringAsync("userToken", token);
        return userModel;
    }

    public async Task<UserModel> AuthenticateRegisterAsync(UserRegisterModel model)
    {
        model = ServiceHelper.ThrowIfNull(model, "The passed model is null.");

        var userModel = _mapper.Map<UserModel>(model);
        userModel = await AddAsync(userModel, model.Password);

        var token = _securityTokenService.CreateToken(userModel);
        if (_session is null)
        {
            throw new SessionException(nameof(_session), "The session is null");
        }

        await _session.SetStringAsync("userToken", token);
        return userModel;
    }

    public async Task<UserModel?> IsCustomerLoggedInAsync()
    {
        if (_session is null)
        {
            throw new SessionException(nameof(_session), "The session is null");
        }

        var token = await _session.GetStringAsync("userToken");
        if (token is null)
        {
            return null;
        }

        var jwtSecurityToken = _securityTokenService.ValidateToken(token);
        var idClaim = jwtSecurityToken.Claims.FirstOrDefault(c => c.Type == "id");
        var userId = idClaim?.Value;
        if (userId is null)
        {
            return null;
        }

        var user = await _userManager.FindByIdAsync(userId);
        if (user is null)
        {
            return null;
        }

        bool isCustomer = await _userManager.IsInRoleAsync(user, UserRoles.Customer);
        bool isAdmin = await _userManager.IsInRoleAsync(user, UserRoles.Admin);
        if (!isCustomer && !isAdmin)
        {
            return null;
        }

        return _mapper.Map<UserModel>(user);
    }

    public async Task<UserModel?> IsAdminLoggedInAsync()
    {
        if (_session is null)
        {
            throw new SessionException(nameof(_session), "The session is null");
        }

        var token = await _session.GetStringAsync("userToken");
        if (token is null)
        {
            return null;
        }

        var jwtSecurityToken = _securityTokenService.ValidateToken(token);
        var idClaim = jwtSecurityToken.Claims.FirstOrDefault(c => c.Type == "id");
        var userId = idClaim?.Value;
        if (userId is null)
        {
            return null;
        }

        var user = await _userManager.FindByIdAsync(userId);
        if (user is null)
        {
            return null;
        }

        bool isAdmin = await _userManager.IsInRoleAsync(user, UserRoles.Admin);
        if (!isAdmin)
        {
            return null;
        }

        return _mapper.Map<UserModel>(user);
    }

    public async Task LogOutAsync()
    {
        if (_session is null)
        {
            throw new SessionException(nameof(_session), "The session is null");
        }

        await _session.RemoveAsync("userToken");
    }
}
