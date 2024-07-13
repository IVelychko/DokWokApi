using AutoMapper;
using DokWokApi.BLL.Interfaces;
using DokWokApi.BLL.Models.User;
using DokWokApi.DAL;
using DokWokApi.DAL.Entities;
using DokWokApi.Exceptions;
using DokWokApi.Validation;
using LanguageExt;
using LanguageExt.Common;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;

namespace DokWokApi.BLL.Services;

public class UserService : IUserService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IMapper _mapper;
    private readonly ISecurityTokenService<UserModel, JwtSecurityToken> _securityTokenService;
    private readonly IUserServiceValidator _validator;

    public UserService(UserManager<ApplicationUser> userManager, IMapper mapper, 
        ISecurityTokenService<UserModel, JwtSecurityToken> securityTokenService,
        IHttpContextAccessor httpContextAccessor,
        IUserServiceValidator validator)
    {
        _userManager = userManager;
        _mapper = mapper;
        _securityTokenService = securityTokenService;
        _validator = validator;
    }

    public async Task<Result<UserModel>> AddAsync(UserModel model, string password)
    {
        var validationResult = await _validator.ValidateAddAsync(model);
        if (!validationResult.IsValid)
        {
            Exception exception = !validationResult.IsFound ? new EntityNotFoundException(validationResult.Error)
                : new ValidationException(validationResult.Error);

            return new Result<UserModel>(exception);
        }

        var user = _mapper.Map<ApplicationUser>(model);
        user.Id = Guid.NewGuid().ToString();
        var result = await _userManager.CreateAsync(user, password);
        if (!result.Succeeded)
        {
            var exception = new ValidationException(result.Errors.Select(e => e.Description)
                .Aggregate((e1, e2) => $"{e1}\n{e2}"));
            return new Result<UserModel>(exception);
        }

        var addedUser = await _userManager.FindByNameAsync(user.UserName!);
        if (addedUser is not null)
        {
            await _userManager.AddToRoleAsync(addedUser, UserRoles.Customer);
            return _mapper.Map<UserModel>(addedUser);
        }
        else
        {
            var exception = new DbException("There was the database error");
            return new Result<UserModel>(exception);
        }
    }

    public async Task<bool?> DeleteAsync(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user is null)
        {
            return null;
        }

        var isAdmin = await _userManager.IsInRoleAsync(user, UserRoles.Admin);
        if (isAdmin)
        {
            return false;
        }

        var result = await _userManager.DeleteAsync(user);
        if (result.Succeeded)
        {
            return true;
        }

        return false;
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

    public async Task<Result<UserModel>> UpdateAsync(UserModel model)
    {
        var validationResult = await _validator.ValidateUpdateAsync(model);
        if (!validationResult.IsValid)
        {
            Exception exception = !validationResult.IsFound ? new EntityNotFoundException(validationResult.Error)
                : new ValidationException(validationResult.Error);

            return new Result<UserModel>(exception);
        }

        var user = (await _userManager.FindByIdAsync(model.Id!))!;
        user.FirstName = model.FirstName;
        user.UserName = model.UserName;
        user.Email = model.Email;
        user.PhoneNumber = model.PhoneNumber;
        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
        {
            var exception = new ValidationException(result.Errors.Select(e => e.Description)
                .Aggregate((e1, e2) => $"{e1}\n{e2}"));
            return new Result<UserModel>(exception);
        }

        var updatedUser = await _userManager.FindByNameAsync(user.UserName!);
        if (updatedUser is not null)
        {
            return _mapper.Map<UserModel>(updatedUser);
        }
        else
        {
            var exception = new DbException("There was the database error");
            return new Result<UserModel>(exception);
        }
    }

    public async Task<Result<bool>> UpdateCustomerPasswordAsync(UserPasswordChangeModel model)
    {
        var validationResult = await _validator.ValidateUpdateCustomerPasswordAsync(model);
        if (!validationResult.IsValid)
        {
            Exception exception = !validationResult.IsFound ? new EntityNotFoundException(validationResult.Error)
                : new ValidationException(validationResult.Error);

            return new Result<bool>(exception);
        }

        var user = (await _userManager.FindByIdAsync(model.UserId!))!;
        var oldPasswordResult = await _userManager.RemovePasswordAsync(user);
        if (!oldPasswordResult.Succeeded)
        {
            var exception = new DbException(oldPasswordResult.Errors.Select(e => e.Description)
                .Aggregate((e1, e2) => $"{e1}\n{e2}"));
            return new Result<bool>(exception);
        }

        var passwordResult = await _userManager.AddPasswordAsync(user, model.NewPassword!);
        if (!passwordResult.Succeeded)
        {
            var exception = new DbException(passwordResult.Errors.Select(e => e.Description)
                .Aggregate((e1, e2) => $"{e1}\n{e2}"));
            return new Result<bool>(exception);
        }

        return true;
    }

    public async Task<Result<bool>> UpdateCustomerPasswordAsAdminAsync(UserPasswordChangeAsAdminModel model)
    {
        var validationResult = await _validator.ValidateUpdateCustomerPasswordAsAdminAsync(model);
        if (!validationResult.IsValid)
        {
            Exception exception = !validationResult.IsFound ? new EntityNotFoundException(validationResult.Error)
                : new ValidationException(validationResult.Error);

            return new Result<bool>(exception);
        }

        var user = (await _userManager.FindByIdAsync(model.UserId!))!;
        var oldPasswordResult = await _userManager.RemovePasswordAsync(user);
        if (!oldPasswordResult.Succeeded)
        {
            var exception = new DbException(oldPasswordResult.Errors.Select(e => e.Description)
                .Aggregate((e1, e2) => $"{e1}\n{e2}"));
            return new Result<bool>(exception);
        }

        var passwordResult = await _userManager.AddPasswordAsync(user, model.NewPassword!);
        if (!passwordResult.Succeeded)
        {
            var exception = new DbException(passwordResult.Errors.Select(e => e.Description)
                .Aggregate((e1, e2) => $"{e1}\n{e2}"));
            return new Result<bool>(exception);
        }

        return true;
    }

    public async Task<Result<IEnumerable<string>>> GetUserRolesAsync(string userId)
    {   
        var user = await _userManager.FindByIdAsync(userId);
        if (user is null)
        {
            var exception = new EntityNotFoundException("There is no user with this id.");
            return new Result<IEnumerable<string>>(exception);
        }

        var roles = await _userManager.GetRolesAsync(user);
        return new Result<IEnumerable<string>>(roles);
    }

    public async Task<Result<AuthorizedUserModel>> AuthenticateCustomerLoginAsync(UserLoginModel model)
    {
        var validationResult = await _validator.ValidateCustomerLoginAsync(model);
        if (!validationResult.IsValid)
        {
            Exception exception = !validationResult.IsFound ? new EntityNotFoundException(validationResult.Error)
                : new ValidationException(validationResult.Error);

            return new Result<AuthorizedUserModel>(exception);
        }

        var user = (await _userManager.FindByNameAsync(model.UserName))!;
        var userModel = _mapper.Map<UserModel>(user);
        var roles = await _userManager.GetRolesAsync(user);
        var token = _securityTokenService.CreateToken(userModel, roles);
        var authUserModel = _mapper.Map<AuthorizedUserModel>(userModel);
        authUserModel.Token = token;
        return authUserModel;
    }

    public async Task<Result<AuthorizedUserModel>> AuthenticateAdminLoginAsync(UserLoginModel model)
    {
        var validationResult = await _validator.ValidateAdminLoginAsync(model);
        if (!validationResult.IsValid)
        {
            Exception exception = !validationResult.IsFound ? new EntityNotFoundException(validationResult.Error)
                : new ValidationException(validationResult.Error);

            return new Result<AuthorizedUserModel>(exception);
        }

        var user = (await _userManager.FindByNameAsync(model.UserName))!;
        var userModel = _mapper.Map<UserModel>(user);
        var roles = await _userManager.GetRolesAsync(user);
        var token = _securityTokenService.CreateToken(userModel, roles);
        var authUserModel = _mapper.Map<AuthorizedUserModel>(userModel);
        authUserModel.Token = token;

        return authUserModel;
    }

    public async Task<Result<AuthorizedUserModel>> AuthenticateRegisterAsync(UserRegisterModel model)
    {
        if (model is null)
        {
            var exception = new ValidationException("The passed model is null.");
            return new Result<AuthorizedUserModel>(exception);
        }

        var userModel = _mapper.Map<UserModel>(model);
        var result = await AddAsync(userModel, model.Password!);
        var authResult = await result.MapAsync(async user =>
        {
            var roles = await _userManager.GetRolesAsync(_mapper.Map<ApplicationUser>(user));
            var token = _securityTokenService.CreateToken(user, roles);
            var authUserModel = _mapper.Map<AuthorizedUserModel>(user);
            authUserModel.Token = token;
            return authUserModel;
        });

        return authResult.Match(au => au,
            e => new Result<AuthorizedUserModel>(e));
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

    public async Task<Result<bool>> IsUserNameTaken(string userName)
    {
        if (userName is null)
        {
            var exception = new ValidationException("The passed user name is null");
            return new Result<bool>(exception);
        }

        var user = await _userManager.FindByNameAsync(userName);
        return user is not null;
    }

    public async Task<Result<bool>> IsEmailTaken(string email)
    {
        if (email is null)
        {
            var exception = new ValidationException("The passed email is null");
            return new Result<bool>(exception);
        }

        var user = await _userManager.FindByEmailAsync(email);
        return user is not null;
    }

    public async Task<Result<bool>> IsPhoneNumberTaken(string phoneNumber)
    {
        if (phoneNumber is null)
        {
            var exception = new ValidationException("The passed phone number is null");
            return new Result<bool>(exception);
        }

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
