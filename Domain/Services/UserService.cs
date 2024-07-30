using Domain.Abstractions.Repositories;
using Domain.Abstractions.Services;
using Domain.Abstractions.Validation;
using Domain.Entities;
using Domain.Errors;
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
            List<string> errors = [];
            if (model is null)
            {
                errors.Add("The passed user is null");
            }

            if (string.IsNullOrEmpty(password))
            {
                errors.Add("The passed password is null or empty");
            }

            var error = new ValidationError(errors);
            return Result<UserModel>.Failure(error);
        }

        var user = model.ToEntity();
        user.Id = Guid.NewGuid().ToString();
        var result = await _userRepository.AddAsync(user, password);
        return result.Match(u => u.ToModel(), Result<UserModel>.Failure);
    }

    public async Task<bool?> DeleteAsync(string id)
    {
        return await _userRepository.DeleteAsync(id);
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
        return entity?.ToModel();
    }

    public async Task<UserModel?> GetUserByIdAsync(string id)
    {
        var entity = await _userRepository.GetUserByIdAsync(id);
        return entity?.ToModel();
    }

    public async Task<UserModel?> GetCustomerByIdAsync(string id)
    {
        var entity = await _userRepository.GetCustomerByIdAsync(id);
        return entity?.ToModel();
    }

    public async Task<Result<UserModel>> UpdateAsync(UserModel model)
    {
        if (model is null)
        {
            var error = new ValidationError("The passed model is null");
            return Result<UserModel>.Failure(error);
        }

        var entity = model.ToEntity();
        var result = await _userRepository.UpdateAsync(entity);
        return result.Match(u => u.ToModel(), Result<UserModel>.Failure);
    }

    public async Task<Result<bool>> UpdateCustomerPasswordAsync(string userId, string oldPassword, string newPassword)
    {
        if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(oldPassword) || string.IsNullOrEmpty(newPassword))
        {
            List<string> errors = [];
            if (string.IsNullOrEmpty(userId))
            {
                errors.Add("The passed user id is null or empty");
            }

            if (string.IsNullOrEmpty(oldPassword))
            {
                errors.Add("The passed old password is null or empty");
            }

            if (string.IsNullOrEmpty(newPassword))
            {
                errors.Add("The passed new password is null or empty");
            }

            var error = new ValidationError(errors);
            return Result<bool>.Failure(error);
        }

        var result = await _userRepository.UpdateCustomerPasswordAsync(userId, oldPassword, newPassword);
        return result;
    }

    public async Task<Result<bool>> UpdateCustomerPasswordAsAdminAsync(string userId, string newPassword)
    {
        if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(newPassword))
        {
            List<string> errors = [];
            if (string.IsNullOrEmpty(userId))
            {
                errors.Add("The passed user id is null or empty");
            }

            if (string.IsNullOrEmpty(newPassword))
            {
                errors.Add("The passed new password is null or empty");
            }
            var error = new ValidationError(errors);
            return Result<bool>.Failure(error);
        }

        var result = await _userRepository.UpdateCustomerPasswordAsAdminAsync(userId, newPassword);
        return result;
    }

    public async Task<Result<IEnumerable<string>>> GetUserRolesAsync(string userId)
    {
        if (string.IsNullOrEmpty(userId))
        {
            var error = new ValidationError("The passed user id is null or empty");
            return Result<IEnumerable<string>>.Failure(error);
        }

        var result = await _userRepository.GetUserRolesAsync(userId);
        return result;
    }

    public async Task<Result<AuthorizedUserModel>> CustomerLoginAsync(string userName, string password)
    {
        var validationResult = await _validator.ValidateCustomerLoginAsync(userName, password);
        if (!validationResult.IsValid)
        {
            var error = new ValidationError(validationResult.Errors);
            return Result<AuthorizedUserModel>.Failure(error);
        }

        var user = await _userRepository.GetUserByUserNameAsync(userName);
        if (user is null)
        {
            var error = new EntityNotFoundError("The user was not found");
            return Result<AuthorizedUserModel>.Failure(error);
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
            var error = new ValidationError(validationResult.Errors);
            return Result<AuthorizedUserModel>.Failure(error);
        }

        var user = await _userRepository.GetUserByUserNameAsync(userName);
        if (user is null)
        {
            var error = new EntityNotFoundError("The user was not found");
            return Result<AuthorizedUserModel>.Failure(error);
        }

        var userModel = user.ToModel();
        var authorizedUserResult = await CreateAuthorizedUserModel(userModel);
        return authorizedUserResult;
    }

    public async Task<Result<AuthorizedUserModel>> RegisterAsync(UserModel model, string password)
    {
        if (model is null || string.IsNullOrEmpty(password))
        {
            List<string> errors = [];
            if (model is null)
            {
                errors.Add("The passed model is null");
            }

            if (string.IsNullOrEmpty(password))
            {
                errors.Add("The passed password is null or empty");
            }

            var error = new ValidationError(errors);
            return Result<AuthorizedUserModel>.Failure(error);
        }

        var result = await AddAsync(model, password);
        if (result.IsFaulted)
        {
            return Result<AuthorizedUserModel>.Failure(result.Error);
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
            var error = new ValidationError(validationResult.Errors);
            return Result<AuthorizedUserModel>.Failure(error);
        }

        try
        {
            var jwtSecurityToken = _securityTokenService.ValidateToken(securityToken, _tokenValidationParametersAccessor.Refresh);
            var isAlgorithmValid = _securityTokenService.IsTokenSecurityAlgorithmValid(jwtSecurityToken);
            var jwtValidationResult = _validator.ValidateExpiredJwt(jwtSecurityToken, isAlgorithmValid);
            if (!jwtValidationResult.IsValid)
            {
                var error = new ValidationError(jwtValidationResult.Errors);
                return Result<AuthorizedUserModel>.Failure(error);
            }

            var storedRefreshToken = await _refreshTokenRepository.GetByTokenWithDetailsAsync(refreshToken);
            var jti = jwtSecurityToken.Claims.First(c => c.Type == JwtRegisteredClaimNames.Jti).Value;
            var refreshTokenValidationResult = _validator.ValidateRefreshToken(storedRefreshToken, jti);
            if (!refreshTokenValidationResult.IsValid)
            {
                var error = new ValidationError(refreshTokenValidationResult.Errors);
                return Result<AuthorizedUserModel>.Failure(error);
            }

            storedRefreshToken!.Used = true;
            var refreshTokenUpdateResult = await _refreshTokenRepository.UpdateAsync(storedRefreshToken);
            if (refreshTokenUpdateResult.IsFaulted)
            {
                return Result<AuthorizedUserModel>.Failure(refreshTokenUpdateResult.Error);
            }

            var userId = jwtSecurityToken.Claims.First(c => c.Type == "id").Value;
            var user = await _userRepository.GetUserByIdAsync(userId);
            if (user is null)
            {
                var error = new ValidationError("The user does not exist");
                return Result<AuthorizedUserModel>.Failure(error);
            }

            var userModel = user.ToModel();
            var authorizedUserResult = await CreateAuthorizedUserModel(userModel);
            return authorizedUserResult;
        }
        catch
        {
            var error = new ValidationError("The token is not valid");
            return Result<AuthorizedUserModel>.Failure(error);
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

            var isCustomerResult = await _userRepository.IsInRoleAsync(user, UserRoles.Customer);
            if (isCustomerResult.IsFaulted)
            {
                return null;
            }

            var isAdminResult = await _userRepository.IsInRoleAsync(user, UserRoles.Admin);
            if (isAdminResult.IsFaulted)
            {
                return null;
            }

            if (!isCustomerResult.Value && !isAdminResult.Value)
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

            var isAdminResult = await _userRepository.IsInRoleAsync(user, UserRoles.Admin);
            if (isAdminResult.IsFaulted)
            {
                return null;
            }

            if (!isAdminResult.Value)
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
        if (string.IsNullOrEmpty(userName))
        {
            var error = new ValidationError("The passed user name is null or empty");
            return Result<bool>.Failure(error);
        }

        var isTakenResult = await _userRepository.IsUserNameTakenAsync(userName);
        return isTakenResult;
    }

    public async Task<Result<bool>> IsEmailTakenAsync(string email)
    {
        if (string.IsNullOrEmpty(email))
        {
            var error = new ValidationError("The passed email is null or empty");
            return Result<bool>.Failure(error);
        }

        var isTakenResult = await _userRepository.IsEmailTakenAsync(email);
        return isTakenResult;
    }

    public async Task<Result<bool>> IsPhoneNumberTakenAsync(string phoneNumber)
    {
        if (string.IsNullOrEmpty(phoneNumber))
        {
            var error = new ValidationError("The passed phone number is null or empty");
            return Result<bool>.Failure(error);
        }

        var isTakenResult = await _userRepository.IsPhoneNumberTakenAsync(phoneNumber);
        return isTakenResult;
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
            return Result<AuthorizedUserModel>.Failure(rolesResult.Error);
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
            return Result<AuthorizedUserModel>.Failure(refreshTokenAddResult.Error);
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
