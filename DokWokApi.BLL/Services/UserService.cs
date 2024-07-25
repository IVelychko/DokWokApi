using DokWokApi.BLL.Interfaces;
using DokWokApi.BLL.Models.User;
using DokWokApi.BLL.Validation;
using DokWokApi.DAL.Entities;
using DokWokApi.DAL.Interfaces;
using DokWokApi.DAL.Exceptions;
using DokWokApi.BLL.Infrastructure;
using DokWokApi.DAL.ResultType;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using DokWokApi.BLL.Extensions;

namespace DokWokApi.BLL.Services;

public class UserService : IUserService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly ISecurityTokenService<UserModel, JwtSecurityToken> _securityTokenService;
    private readonly IUserServiceValidator _validator;
    private readonly TokenValidationParametersAccessor _tokenValidationParametersAccessor;

    public UserService(UserManager<ApplicationUser> userManager,
        IRefreshTokenRepository refreshTokenRepository,
        ISecurityTokenService<UserModel, JwtSecurityToken> securityTokenService,
        IUserServiceValidator validator,
        TokenValidationParametersAccessor tokenValidationParametersAccessor)
    {
        _userManager = userManager;
        _refreshTokenRepository = refreshTokenRepository;
        _securityTokenService = securityTokenService;
        _validator = validator;
        _tokenValidationParametersAccessor = tokenValidationParametersAccessor;
    }

    public async Task<Result<UserModel>> AddAsync(UserModel model, string password)
    {
        var validationResult = await _validator.ValidateAddAsync(model);
        if (!validationResult.IsValid)
        {
            Exception exception = !validationResult.IsFound ? new NotFoundException(validationResult.Error)
                : new ValidationException(validationResult.Error);

            return new Result<UserModel>(exception);
        }

        var user = model.ToEntity();
        user.Id = Guid.NewGuid().ToString();
        var result = await _userManager.CreateAsync(user, password);
        if (!result.Succeeded)
        {
            var exception = new ValidationException(result.Errors.Select(e => e.Description)
                .Aggregate((e1, e2) => $"{e1}\n{e2}"));
            return new Result<UserModel>(exception);
        }

        var addedUser = await _userManager.FindByIdAsync(user.Id);
        if (addedUser is not null)
        {
            await _userManager.AddToRoleAsync(addedUser, UserRoles.Customer);
            return addedUser.ToModel();
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
        var models = entities.Select(u => u.ToModel());
        return models;
    }

    public async Task<IEnumerable<UserModel>> GetAllCustomersAsync()
    {
        var entities = await _userManager.GetUsersInRoleAsync(UserRoles.Customer);
        var models = entities.Select(u => u.ToModel());
        return models;
    }

    public async Task<UserModel?> GetByUserNameAsync(string userName)
    {
        var entity = await _userManager.FindByNameAsync(userName);
        if (entity is null)
        {
            return null;
        }

        var model = entity.ToModel();
        return model;
    }

    public async Task<UserModel?> GetByIdAsync(string id)
    {
        var entity = await _userManager.FindByIdAsync(id);
        if (entity is null)
        {
            return null;
        }

        var model = entity.ToModel();
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

        var model = entity.ToModel();
        return model;
    }

    public async Task<Result<UserModel>> UpdateAsync(UserModel model)
    {
        var validationResult = await _validator.ValidateUpdateAsync(model);
        if (!validationResult.IsValid)
        {
            Exception exception = !validationResult.IsFound ? new NotFoundException(validationResult.Error)
                : new ValidationException(validationResult.Error);

            return new Result<UserModel>(exception);
        }

        var user = await _userManager.FindByIdAsync(model.Id);
        if (user is null)
        {
            var exception = new NotFoundException("The user was not found");
            return new Result<UserModel>(exception);
        }

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

        var updatedUser = await _userManager.FindByIdAsync(user.Id);
        if (updatedUser is not null)
        {
            return updatedUser.ToModel();
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
            Exception exception = !validationResult.IsFound ? new NotFoundException(validationResult.Error)
                : new ValidationException(validationResult.Error);

            return new Result<bool>(exception);
        }

        var user = await _userManager.FindByIdAsync(model.UserId!);
        if (user is null)
        {
            var exception = new NotFoundException("The user was not found");
            return new Result<bool>(exception);
        }

        var oldPasswordResult = await _userManager.RemovePasswordAsync(user);
        if (!oldPasswordResult.Succeeded)
        {
            var exception = new DbException(oldPasswordResult.Errors.Select(e => e.Description)
                .Aggregate((e1, e2) => $"{e1}\n{e2}"));
            return new Result<bool>(exception);
        }

        var newPasswordResult = await _userManager.AddPasswordAsync(user, model.NewPassword!);
        if (!newPasswordResult.Succeeded)
        {
            var exception = new DbException(newPasswordResult.Errors.Select(e => e.Description)
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
            Exception exception = !validationResult.IsFound ? new NotFoundException(validationResult.Error)
                : new ValidationException(validationResult.Error);

            return new Result<bool>(exception);
        }

        var user = await _userManager.FindByIdAsync(model.UserId!);
        if (user is null)
        {
            var exception = new NotFoundException("The user was not found");
            return new Result<bool>(exception);
        }

        var oldPasswordResult = await _userManager.RemovePasswordAsync(user);
        if (!oldPasswordResult.Succeeded)
        {
            var exception = new DbException(oldPasswordResult.Errors.Select(e => e.Description)
                .Aggregate((e1, e2) => $"{e1}\n{e2}"));
            return new Result<bool>(exception);
        }

        var newPasswordResult = await _userManager.AddPasswordAsync(user, model.NewPassword!);
        if (!newPasswordResult.Succeeded)
        {
            var exception = new DbException(newPasswordResult.Errors.Select(e => e.Description)
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
            var exception = new NotFoundException("There is no user with this id.");
            return new Result<IEnumerable<string>>(exception);
        }

        var roles = await _userManager.GetRolesAsync(user);
        return new Result<IEnumerable<string>>(roles);
    }

    public async Task<Result<AuthorizedUserModel>> CustomerLoginAsync(UserLoginModel model)
    {
        var validationResult = await _validator.ValidateCustomerLoginAsync(model);
        if (!validationResult.IsValid)
        {
            Exception exception = !validationResult.IsFound ? new NotFoundException(validationResult.Error)
                : new ValidationException(validationResult.Error);

            return new Result<AuthorizedUserModel>(exception);
        }

        var user = await _userManager.FindByNameAsync(model.UserName);
        if (user is null)
        {
            var exception = new NotFoundException("The user was not found");
            return new Result<AuthorizedUserModel>(exception);
        }

        var userModel = user.ToModel();
        var authorizedUserResult = await CreateAuthorizedUserModel(userModel);
        return authorizedUserResult;
    }

    public async Task<Result<AuthorizedUserModel>> AdminLoginAsync(UserLoginModel model)
    {
        var validationResult = await _validator.ValidateAdminLoginAsync(model);
        if (!validationResult.IsValid)
        {
            Exception exception = !validationResult.IsFound ? new NotFoundException(validationResult.Error)
                : new ValidationException(validationResult.Error);

            return new Result<AuthorizedUserModel>(exception);
        }

        var user = await _userManager.FindByNameAsync(model.UserName);
        if (user is null)
        {
            var exception = new NotFoundException("The user was not found");
            return new Result<AuthorizedUserModel>(exception);
        }

        var userModel = user.ToModel();
        var authorizedUserResult = await CreateAuthorizedUserModel(userModel);
        return authorizedUserResult;
    }

    public async Task<Result<AuthorizedUserModel>> RegisterAsync(UserRegisterModel model)
    {
        if (model is null)
        {
            var exception = new ValidationException("The passed model is null.");
            return new Result<AuthorizedUserModel>(exception);
        }

        var userModel = model.ToModel();
        var result = await AddAsync(userModel, model.Password!);
        if (result.IsFaulted)
        {
            var exception = result.Exception;
            return new Result<AuthorizedUserModel>(exception);
        }

        var addedUser = result.Value;
        var authorizedUserResult = await CreateAuthorizedUserModel(addedUser);
        return authorizedUserResult;
    }

    public async Task<Result<AuthorizedUserModel>> RefreshTokenAsync(RefreshTokenModel refreshTokenModel)
    {
        var validationResult = _validator.ValidateRefreshTokenModel(refreshTokenModel);
        if (!validationResult.IsValid)
        {
            var exception = new ValidationException(validationResult.Error);
            return new Result<AuthorizedUserModel>(exception);
        }

        try
        {
            var jwtSecurityToken = _securityTokenService.ValidateToken(refreshTokenModel.Token!, _tokenValidationParametersAccessor.Refresh);
            var isAlgorithmValid = _securityTokenService.IsTokenSecurityAlgorithmValid(jwtSecurityToken);
            var jwtValidationResult = _validator.ValidateExpiredJwt(jwtSecurityToken, isAlgorithmValid);
            if (!jwtValidationResult.IsValid)
            {
                var exception = new ValidationException(jwtValidationResult.Error);
                return new Result<AuthorizedUserModel>(exception);
            }

            var storedRefreshToken = await _refreshTokenRepository.GetByTokenWithDetailsAsync(refreshTokenModel.RefreshToken!);
            var jti = jwtSecurityToken.Claims.First(c => c.Type == JwtRegisteredClaimNames.Jti).Value;
            var refreshTokenValidationResult = _validator.ValidateRefreshToken(storedRefreshToken, jti);
            if (!refreshTokenValidationResult.IsValid)
            {
                var exception = new ValidationException(validationResult.Error);
                return new Result<AuthorizedUserModel>(exception);
            }

            storedRefreshToken!.Used = true;
            var refreshTokenUpdateResult = await _refreshTokenRepository.UpdateAsync(storedRefreshToken);
            if (refreshTokenUpdateResult.IsFaulted)
            {
                var exception = refreshTokenUpdateResult.Exception;
                return new Result<AuthorizedUserModel>(exception);
            }

            var userId = jwtSecurityToken.Claims.First(c => c.Type == "id").Value;
            var user = await _userManager.FindByIdAsync(userId);
            if (user is null)
            {
                var exception = new ValidationException("The user does not exist");
                return new Result<AuthorizedUserModel>(exception);
            }
            
            var userModel = user.ToModel();
            var authorizedUserResult = await CreateAuthorizedUserModel(userModel);
            return authorizedUserResult;
        }
        catch
        {
            var exception = new ValidationException("The token is not valid");
            return new Result<AuthorizedUserModel>(exception);
        }
    }

    public async Task<UserModel?> IsCustomerTokenValidAsync(string token)
    {
        if (token is null)
        {
            return null;
        }

        try
        {
            var jwtSecurityToken = _securityTokenService.ValidateToken(token, _tokenValidationParametersAccessor.Regular);
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

            return user.ToModel();
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
            var jwtSecurityToken = _securityTokenService.ValidateToken(token, _tokenValidationParametersAccessor.Regular);
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

            return user.ToModel();
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

        var isTaken = await _userManager.Users.AsNoTracking().AnyAsync(u => u.UserName == userName);
        return isTaken;
    }

    public async Task<Result<bool>> IsEmailTaken(string email)
    {
        if (email is null)
        {
            var exception = new ValidationException("The passed email is null");
            return new Result<bool>(exception);
        }

        var isTaken = await _userManager.Users.AsNoTracking().AnyAsync(u => u.Email == email);
        return isTaken;
    }

    public async Task<Result<bool>> IsPhoneNumberTaken(string phoneNumber)
    {
        if (phoneNumber is null)
        {
            var exception = new ValidationException("The passed phone number is null");
            return new Result<bool>(exception);
        }

        var isTaken = await _userManager.Users.AsNoTracking().AnyAsync(u => u.PhoneNumber == phoneNumber);
        return isTaken;
    }

    public async Task<UserModel?> GetUserFromToken(string token)
    {
        var jwtSecurityToken = _securityTokenService.ValidateToken(token, _tokenValidationParametersAccessor.Regular);
        var userId = jwtSecurityToken.Claims.FirstOrDefault(x => x.Type == "id")?.Value;
        var user = userId is not null ? await _userManager.FindByIdAsync(userId) : null;
        return user?.ToModel();
    }

    private async Task<Result<AuthorizedUserModel>> CreateAuthorizedUserModel(UserModel userModel)
    {
        var roles = await _userManager.GetRolesAsync(userModel.ToEntity());
        var token = _securityTokenService.CreateToken(userModel, roles);
        var jwtId = token.Claims.First(c => c.Type == JwtRegisteredClaimNames.Jti).Value;
        var refreshToken = CreateRefreshToken(jwtId, userModel.Id, DateTime.UtcNow.AddMonths(6));
        JwtSecurityTokenHandler jwtTokenHandler = new();
        var serializedJwtToken = jwtTokenHandler.WriteToken(token);
        var refreshTokenAddResult = await _refreshTokenRepository.AddAsync(refreshToken);
        if (refreshTokenAddResult.IsFaulted)
        {
            var exception = refreshTokenAddResult.Exception;
            return new Result<AuthorizedUserModel>(exception);
        }

        var authorizedUser = userModel.ToAuthorizedModel(serializedJwtToken, refreshToken);
        return authorizedUser;
    }

    private static RefreshToken CreateRefreshToken(string jwtId, string userId, DateTime expiryDate)
    {
        return new()
        {
            CreationDate = DateTime.UtcNow,
            ExpiryDate = expiryDate,
            JwtId = jwtId,
            Token = Guid.NewGuid().ToString(),
            UserId = userId
        };
    }
}
