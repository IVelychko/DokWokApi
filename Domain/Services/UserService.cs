using Domain.Abstractions.Repositories;
using Domain.Abstractions.Services;
using Domain.Abstractions.Validation;
using Domain.Entities;
using Domain.Exceptions;
using Domain.Exceptions.Base;
using Domain.Helpers;
using Domain.Mapping.Extensions;
using Domain.Models.User;
using Domain.ResultType;
using System.IdentityModel.Tokens.Jwt;

namespace Domain.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly ISecurityTokenService<UserModel, JwtSecurityToken> _securityTokenService;
    private readonly IUserServiceValidator _validator;
    private readonly TokenValidationParametersAccessor _tokenValidationParametersAccessor;

    public UserService(IUserRepository userRepository,
        IRefreshTokenRepository refreshTokenRepository,
        ISecurityTokenService<UserModel, JwtSecurityToken> securityTokenService,
        IUserServiceValidator validator,
        TokenValidationParametersAccessor tokenValidationParametersAccessor)
    {
        _userRepository = userRepository;
        _refreshTokenRepository = refreshTokenRepository;
        _securityTokenService = securityTokenService;
        _validator = validator;
        _tokenValidationParametersAccessor = tokenValidationParametersAccessor;
    }

    public async Task<Result<UserModel>> AddAsync(UserModel model, string password)
    {
        if (model is null || string.IsNullOrEmpty(password))
        {
            var exception = new ValidationException("The passed data is null");
            return new Result<UserModel>(exception);
        }

        var user = model.ToEntity();
        user.Id = Guid.NewGuid().ToString();
        var result = await _userRepository.AddAsync(user, password);
        return result.Match(u => u.ToModel(),
            e => new Result<UserModel>(e));
    }

    public async Task<bool?> DeleteAsync(string id)
    {
        var result = await _userRepository.DeleteAsync(id);
        return result;
    }

    public async Task<IEnumerable<UserModel>> GetAllUsersAsync()
    {
        var entities = await _userRepository.GetAllUsersAsync();
        var models = entities.Select(u => u.ToModel());
        return models;
    }

    public async Task<IEnumerable<UserModel>> GetAllCustomersAsync()
    {
        var entities = await _userRepository.GetAllCustomersAsync();
        var models = entities.Select(u => u.ToModel());
        return models;
    }

    public async Task<UserModel?> GetUserByUserNameAsync(string userName)
    {
        var entity = await _userRepository.GetUserByUserNameAsync(userName);
        if (entity is null)
        {
            return null;
        }

        var model = entity.ToModel();
        return model;
    }

    public async Task<UserModel?> GetUserByIdAsync(string id)
    {
        var entity = await _userRepository.GetUserByIdAsync(id);
        if (entity is null)
        {
            return null;
        }

        var model = entity.ToModel();
        return model;
    }

    public async Task<UserModel?> GetCustomerByIdAsync(string id)
    {
        var entity = await _userRepository.GetCustomerByIdAsync(id);
        if (entity is null)
        {
            return null;
        }

        var model = entity.ToModel();
        return model;
    }

    public async Task<Result<UserModel>> UpdateAsync(UserModel model)
    {
        if (model is null)
        {
            var exception = new ValidationException("The passed model is null");
            return new Result<UserModel>(exception);
        }

        var entity = model.ToEntity();
        var result = await _userRepository.UpdateAsync(entity);
        return result.Match(u => u.ToModel(),
            e => new Result<UserModel>(e));
    }

    public async Task<Result<bool>> UpdateCustomerPasswordAsync(string userId, string oldPassword, string newPassword)
    {
        if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(oldPassword) || string.IsNullOrEmpty(newPassword))
        {
            var exception = new ValidationException("The passed data is null");
            return new Result<bool>(exception);
        }

        var result = await _userRepository.UpdateCustomerPasswordAsync(userId, oldPassword, newPassword);
        return result;
    }

    public async Task<Result<bool>> UpdateCustomerPasswordAsAdminAsync(string userId, string newPassword)
    {
        if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(newPassword))
        {
            var exception = new ValidationException("The passed data is null");
            return new Result<bool>(exception);
        }

        var result = await _userRepository.UpdateCustomerPasswordAsAdminAsync(userId, newPassword);
        return result;
    }

    public async Task<Result<IEnumerable<string>>> GetUserRolesAsync(string userId)
    {
        if (userId is null)
        {
            var exception = new ValidationException("The passed data is null");
            return new Result<IEnumerable<string>>(exception);
        }

        var result = await _userRepository.GetUserRolesAsync(userId);
        return result;
    }

    public async Task<Result<AuthorizedUserModel>> CustomerLoginAsync(string userName, string password)
    {
        var validationResult = await _validator.ValidateCustomerLoginAsync(userName, password);
        if (!validationResult.IsValid)
        {
            Exception exception = !validationResult.IsFound ? new NotFoundException(validationResult.Error)
                : new ValidationException(validationResult.Error);

            return new Result<AuthorizedUserModel>(exception);
        }

        var user = await _userRepository.GetUserByUserNameAsync(userName);
        if (user is null)
        {
            var exception = new NotFoundException("The user was not found");
            return new Result<AuthorizedUserModel>(exception);
        }

        var userModel = user.ToModel();
        var authorizedUserResult = await CreateAuthorizedUserModel(userModel);
        return authorizedUserResult;
    }

    public async Task<Result<AuthorizedUserModel>> AdminLoginAsync(string userName, string password)
    {
        var validationResult = await _validator.ValidateAdminLoginAsync(userName, password);
        if (!validationResult.IsValid)
        {
            Exception exception = !validationResult.IsFound ? new NotFoundException(validationResult.Error)
                : new ValidationException(validationResult.Error);

            return new Result<AuthorizedUserModel>(exception);
        }

        var user = await _userRepository.GetUserByUserNameAsync(userName);
        if (user is null)
        {
            var exception = new NotFoundException("The user was not found");
            return new Result<AuthorizedUserModel>(exception);
        }

        var userModel = user.ToModel();
        var authorizedUserResult = await CreateAuthorizedUserModel(userModel);
        return authorizedUserResult;
    }

    public async Task<Result<AuthorizedUserModel>> RegisterAsync(UserModel model, string password)
    {
        if (model is null || string.IsNullOrEmpty(password))
        {
            var exception = new ValidationException("The passed data is null.");
            return new Result<AuthorizedUserModel>(exception);
        }

        var result = await AddAsync(model, password);
        if (result.IsFaulted)
        {
            var exception = result.Exception;
            return new Result<AuthorizedUserModel>(exception);
        }

        var addedUser = result.Value;
        var authorizedUserResult = await CreateAuthorizedUserModel(addedUser);
        return authorizedUserResult;
    }

    public async Task<Result<AuthorizedUserModel>> RefreshTokenAsync(string securityToken, string refreshToken)
    {
        var validationResult = _validator.ValidateRefreshTokenModel(securityToken, refreshToken);
        if (!validationResult.IsValid)
        {
            var exception = new ValidationException(validationResult.Error);
            return new Result<AuthorizedUserModel>(exception);
        }

        try
        {
            var jwtSecurityToken = _securityTokenService.ValidateToken(securityToken, _tokenValidationParametersAccessor.Refresh);
            var isAlgorithmValid = _securityTokenService.IsTokenSecurityAlgorithmValid(jwtSecurityToken);
            var jwtValidationResult = _validator.ValidateExpiredJwt(jwtSecurityToken, isAlgorithmValid);
            if (!jwtValidationResult.IsValid)
            {
                var exception = new ValidationException(jwtValidationResult.Error);
                return new Result<AuthorizedUserModel>(exception);
            }

            var storedRefreshToken = await _refreshTokenRepository.GetByTokenWithDetailsAsync(refreshToken);
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
            var user = await _userRepository.GetUserByIdAsync(userId);
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

            var user = await _userRepository.GetUserByIdAsync(userId);
            if (user is null)
            {
                return null;
            }

            bool isCustomer = await _userRepository.IsInRoleAsync(user, UserRoles.Customer);
            bool isAdmin = await _userRepository.IsInRoleAsync(user, UserRoles.Admin);
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

            var user = await _userRepository.GetUserByIdAsync(userId);
            if (user is null)
            {
                return null;
            }

            bool isAdmin = await _userRepository.IsInRoleAsync(user, UserRoles.Admin);
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

    public async Task<Result<bool>> IsUserNameTakenAsync(string userName)
    {
        if (userName is null)
        {
            var exception = new ValidationException("The passed user name is null");
            return new Result<bool>(exception);
        }

        var isTaken = await _userRepository.IsUserNameTakenAsync(userName);
        return isTaken;
    }

    public async Task<Result<bool>> IsEmailTakenAsync(string email)
    {
        if (email is null)
        {
            var exception = new ValidationException("The passed email is null");
            return new Result<bool>(exception);
        }

        var isTaken = await _userRepository.IsEmailTakenAsync(email);
        return isTaken;
    }

    public async Task<Result<bool>> IsPhoneNumberTakenAsync(string phoneNumber)
    {
        if (phoneNumber is null)
        {
            var exception = new ValidationException("The passed phone number is null");
            return new Result<bool>(exception);
        }

        var isTaken = await _userRepository.IsPhoneNumberTakenAsync(phoneNumber);
        return isTaken;
    }

    public async Task<UserModel?> GetUserFromTokenAsync(string token)
    {
        var jwtSecurityToken = _securityTokenService.ValidateToken(token, _tokenValidationParametersAccessor.Regular);
        var userId = jwtSecurityToken.Claims.FirstOrDefault(x => x.Type == "id")?.Value;
        var user = userId is not null ? await _userRepository.GetUserByIdAsync(userId) : null;
        return user?.ToModel();
    }

    private async Task<Result<AuthorizedUserModel>> CreateAuthorizedUserModel(UserModel userModel)
    {
        var rolesResult = await _userRepository.GetUserRolesAsync(userModel.Id);
        if (rolesResult.IsFaulted)
        {
            return new Result<AuthorizedUserModel>(rolesResult.Exception);
        }

        var roles = rolesResult.Value;
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
