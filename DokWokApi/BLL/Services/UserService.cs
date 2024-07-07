using AutoMapper;
using DokWokApi.BLL.Interfaces;
using DokWokApi.BLL.Models.User;
using DokWokApi.DAL;
using DokWokApi.DAL.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;

namespace DokWokApi.BLL.Services;

public class UserService : IUserService
{
    private readonly UserManager<ApplicationUser> _userManager;

    private readonly IMapper _mapper;

    private readonly ISecurityTokenService<UserModel, JwtSecurityToken> _securityTokenService;

    public UserService(UserManager<ApplicationUser> userManager, IMapper mapper, 
        ISecurityTokenService<UserModel, JwtSecurityToken> securityTokenService,
        IHttpContextAccessor httpContextAccessor)
    {
        _userManager = userManager;
        _mapper = mapper;
        _securityTokenService = securityTokenService;
    }

    public async Task<UserModel> AddAsync(UserModel model, string password)
    {
        model = ServiceHelper.ThrowArgumentNullExceptionIfNull(model, "The passed model is null.");
        ServiceHelper.ThrowUserExceptionIfTrue(model.PhoneNumber is null, "The phone number is null");
        bool isPhoneNumberTaken = await _userManager.Users.AnyAsync(u => u.PhoneNumber == model.PhoneNumber);
        ServiceHelper.ThrowUserExceptionIfTrue(isPhoneNumberTaken, "The phone number is already taken");

        var user = _mapper.Map<ApplicationUser>(model);
        user.Id = Guid.NewGuid().ToString();
        var result = await _userManager.CreateAsync(user, password);
        ServiceHelper.ThrowUserExceptionIfNotSucceeded(result.Succeeded, result.Errors);

        var addedUser = await _userManager.FindByNameAsync(user.UserName!);
        await _userManager.AddToRoleAsync(addedUser!, UserRoles.Customer);
        return _mapper.Map<UserModel>(addedUser!);
    }

    public async Task DeleteAsync(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        user = RepositoryHelper.ThrowEntityNotFoundExceptionIfNull(user, "There is no user with this user name in the database.");

        var isAdmin = await _userManager.IsInRoleAsync(user, UserRoles.Admin);
        ServiceHelper.ThrowUserExceptionIfTrue(isAdmin, "Action is not allowed");

        var result = await _userManager.DeleteAsync(user);
        ServiceHelper.ThrowUserExceptionIfNotSucceeded(result.Succeeded, result.Errors);
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
        model = ServiceHelper.ThrowArgumentNullExceptionIfNull(model, "The passed model is null.");
        var user = await _userManager.FindByIdAsync(model.Id!);
        user = RepositoryHelper.ThrowEntityNotFoundExceptionIfNull(user, "There is no user with this ID in the database.");
        ServiceHelper.ThrowUserExceptionIfTrue(model.PhoneNumber is null, "The phone number is null");
        if (model.PhoneNumber != user.PhoneNumber)
        {
            bool isPhoneNumberTaken = await _userManager.Users.AnyAsync(u => u.PhoneNumber == model.PhoneNumber);
            ServiceHelper.ThrowUserExceptionIfTrue(isPhoneNumberTaken, "The phone number is already taken");
        }

        user.FirstName = model.FirstName;
        user.UserName = model.UserName;
        user.Email = model.Email;
        user.PhoneNumber = model.PhoneNumber;
        var result = await _userManager.UpdateAsync(user);
        ServiceHelper.ThrowUserExceptionIfNotSucceeded(result.Succeeded, result.Errors);

        var updatedUser = await _userManager.FindByNameAsync(user.UserName!);
        return _mapper.Map<UserModel>(updatedUser);
    }

    public async Task UpdateCustomerPasswordAsync(UserPasswordChangeModel model)
    {
        model = ServiceHelper.ThrowArgumentNullExceptionIfNull(model, "The passed model is null.");
        var user = await _userManager.FindByIdAsync(model.UserId!);
        user = RepositoryHelper.ThrowEntityNotFoundExceptionIfNull(user, "There is no user with this ID in the database.");
        bool isAdmin = await _userManager.IsInRoleAsync(user, UserRoles.Admin);
        ServiceHelper.ThrowUserExceptionIfTrue(isAdmin, "Action is not allowed");
        bool isOldPasswordValid = await _userManager.CheckPasswordAsync(user, model.OldPassword!);
        ServiceHelper.ThrowUserExceptionIfTrue(!isOldPasswordValid, "The old password is not valid.");

        await _userManager.RemovePasswordAsync(user);
        var passwordResult = await _userManager.AddPasswordAsync(user, model.NewPassword!);
        ServiceHelper.ThrowUserExceptionIfNotSucceeded(passwordResult.Succeeded, passwordResult.Errors);
    }

    public async Task UpdateCustomerPasswordAsAdminAsync(UserPasswordChangeAsAdminModel model)
    {
        model = ServiceHelper.ThrowArgumentNullExceptionIfNull(model, "The passed model is null.");
        var user = await _userManager.FindByIdAsync(model.UserId!);
        user = RepositoryHelper.ThrowEntityNotFoundExceptionIfNull(user, "There is no user with this ID in the database.");
        bool isAdmin = await _userManager.IsInRoleAsync(user, UserRoles.Admin);
        ServiceHelper.ThrowUserExceptionIfTrue(isAdmin, "Action is not allowed");

        await _userManager.RemovePasswordAsync(user);
        var passwordResult = await _userManager.AddPasswordAsync(user, model.NewPassword!);
        ServiceHelper.ThrowUserExceptionIfNotSucceeded(passwordResult.Succeeded, passwordResult.Errors);
    }

    public async Task<IEnumerable<string>> GetUserRolesAsync(string userId)
    {   
        var user = await _userManager.FindByIdAsync(userId);
        user = RepositoryHelper.ThrowEntityNotFoundExceptionIfNull(user, "There is no user with this id.");
        var roles = await _userManager.GetRolesAsync(user);
        return roles;
    }

    public async Task<AuthorizedUserModel> AuthenticateCustomerLoginAsync(UserLoginModel model)
    {
        model = ServiceHelper.ThrowArgumentNullExceptionIfNull(model, "The passed model is null.");

        var user = await _userManager.FindByNameAsync(model.UserName);
        user = RepositoryHelper.ThrowEntityNotFoundExceptionIfNull(user, "The credentials are wrong.");

        bool isCustomer = await _userManager.IsInRoleAsync(user, UserRoles.Customer);
        bool isAdmin = await _userManager.IsInRoleAsync(user, UserRoles.Admin);
        ServiceHelper.ThrowUserExceptionIfUserIsNotValid(isCustomer || isAdmin, "The user is not allowed.");

        var isValidPassword = await _userManager.CheckPasswordAsync(user, model.Password);
        ServiceHelper.ThrowUserExceptionIfUserIsNotValid(isValidPassword, "The credentials are wrong.");

        var userModel = _mapper.Map<UserModel>(user);
        var roles = await _userManager.GetRolesAsync(user);
        var token = _securityTokenService.CreateToken(userModel, roles);
        var authUserModel = _mapper.Map<AuthorizedUserModel>(userModel);
        authUserModel.Token = token;
        return authUserModel;
    }

    public async Task<AuthorizedUserModel> AuthenticateAdminLoginAsync(UserLoginModel model)
    {
        model = ServiceHelper.ThrowArgumentNullExceptionIfNull(model, "The passed model is null.");

        var user = await _userManager.FindByNameAsync(model.UserName);
        user = RepositoryHelper.ThrowEntityNotFoundExceptionIfNull(user, "The credentials are wrong.");

        bool isAdmin = await _userManager.IsInRoleAsync(user, UserRoles.Admin);
        ServiceHelper.ThrowUserExceptionIfUserIsNotValid(isAdmin, "The user is not allowed.");

        var isValidPassword = await _userManager.CheckPasswordAsync(user, model.Password);
        ServiceHelper.ThrowUserExceptionIfUserIsNotValid(isValidPassword, "The credentials are wrong.");

        var userModel = _mapper.Map<UserModel>(user);
        var roles = await _userManager.GetRolesAsync(user);
        var token = _securityTokenService.CreateToken(userModel, roles);
        var authUserModel = _mapper.Map<AuthorizedUserModel>(userModel);
        authUserModel.Token = token;

        return authUserModel;
    }

    public async Task<AuthorizedUserModel> AuthenticateRegisterAsync(UserRegisterModel model)
    {
        model = ServiceHelper.ThrowArgumentNullExceptionIfNull(model, "The passed model is null.");

        var userModel = _mapper.Map<UserModel>(model);
        userModel = await AddAsync(userModel, model.Password!);
        var roles = await _userManager.GetRolesAsync(_mapper.Map<ApplicationUser>(userModel));
        var token = _securityTokenService.CreateToken(userModel, roles);
        var authUserModel = _mapper.Map<AuthorizedUserModel>(userModel);
        authUserModel.Token = token;
        return authUserModel;
    }

    public async Task<UserModel?> IsCustomerTokenValidAsync(string token)
    {
        if (token is null)
        {
            return null;
        }

        try
        {
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
        catch (Exception)
        {
            return null;
        }
    }

    public async Task<UserModel?> IsAdminTokenValidAsync(string token)
    {
        if (token is null)
        {
            return null;
        }

        try
        {
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
        catch (Exception)
        {
            return null;
        }
    }

    public async Task<bool> IsUserNameTaken(string userName)
    {
        ServiceHelper.ThrowArgumentNullExceptionIfNull(userName, "Username is null");
        var user = await _userManager.FindByNameAsync(userName);
        return user is not null;
    }

    public async Task<bool> IsEmailTaken(string email)
    {
        ServiceHelper.ThrowArgumentNullExceptionIfNull(email, "Email is null");
        var user = await _userManager.FindByEmailAsync(email);
        return user is not null;
    }

    public async Task<bool> IsPhoneNumberTaken(string phoneNumber)
    {
        ServiceHelper.ThrowArgumentNullExceptionIfNull(phoneNumber, "Phone number is null");
        var isTaken = await _userManager.Users.AnyAsync(u => u.PhoneNumber == phoneNumber);
        return isTaken;
    }

    public async Task<UserModel?> GetUserFromToken(string token)
    {
        var jwtSecurityToken = _securityTokenService.ValidateToken(token);
        var userId = jwtSecurityToken.Claims.FirstOrDefault(x => x.Type == "id")?.Value;
        var user = userId is not null ? await _userManager.FindByIdAsync(userId) : null;
        return user is null ? null : _mapper.Map<UserModel>(user);
    }
}
