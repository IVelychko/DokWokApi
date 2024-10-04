using Domain.Abstractions.Repositories;
using Domain.Abstractions.Services;
using Domain.Abstractions.Validation;
using Domain.Entities;
using Domain.Errors;
using Domain.Exceptions;
using Domain.Helpers;
using Domain.Mapping.Extensions;
using Domain.Models;
using Domain.Models.User;
using Domain.ResultType;
using System.IdentityModel.Tokens.Jwt;

namespace Domain.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IUserRoleRepository _userRoleRepository;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ISecurityTokenService<UserModel, JwtSecurityToken> _securityTokenService;
    private readonly IUserServiceValidator _validator;
    private readonly TokenValidationParametersAccessor _tokenValidationParametersAccessor;

    public UserService(IUserRepository userRepository,
        IRefreshTokenRepository refreshTokenRepository,
        ISecurityTokenService<UserModel, JwtSecurityToken> securityTokenService,
        IUserServiceValidator validator,
        TokenValidationParametersAccessor tokenValidationParametersAccessor,
        IUserRoleRepository userRoleRepository,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _refreshTokenRepository = refreshTokenRepository;
        _securityTokenService = securityTokenService;
        _validator = validator;
        _tokenValidationParametersAccessor = tokenValidationParametersAccessor;
        _userRoleRepository = userRoleRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<UserModel>> AddAsync(UserModel model, string password)
    {
        if (model is null || string.IsNullOrEmpty(password))
        {
            Dictionary<string, string[]> errors = [];
            if (model is null)
            {
                errors.Add(nameof(model), ["The passed user is null"]);
            }

            if (string.IsNullOrEmpty(password))
            {
                errors.Add(nameof(password), ["The passed password is null or empty"]);
            }

            var error = new ValidationError(errors);
            return Result<UserModel>.Failure(error);
        }

        var user = model.ToEntity();
        var customerRole = await _userRoleRepository.GetByNameAsync(UserRoles.Customer)
            ?? throw new DbException("The customer role doesn't exist");
        user.UserRoleId = customerRole.Id;
        await _userRepository.AddAsync(user, password);
        await _unitOfWork.SaveChangesAsync();
        var createdEntity = await _userRepository.GetUserByIdWithDetailsAsync(user.Id) ?? throw new DbException("There was a database error");
        return createdEntity.ToModel();
    }

    public async Task DeleteAsync(long id)
    {
        await _userRepository.DeleteByIdAsync(id);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<IEnumerable<UserModel>> GetAllUsersAsync(PageInfo? pageInfo = null)
    {
        var entities = await _userRepository.GetAllUsersWithDetailsAsync(pageInfo);
        var models = entities.Select(u => u.ToModel());
        return models;
    }

    public async Task<IEnumerable<UserModel>> GetAllCustomersAsync(PageInfo? pageInfo = null)
    {
        var entities = await _userRepository.GetAllCustomersWithDetailsAsync(pageInfo);
        var models = entities.Select(u => u.ToModel());
        return models;
    }

    public async Task<UserModel?> GetUserByUserNameAsync(string userName)
    {
        var entity = await _userRepository.GetUserByUserNameWithDetailsAsync(userName);
        return entity?.ToModel();
    }

    public async Task<UserModel?> GetUserByIdAsync(long id)
    {
        var entity = await _userRepository.GetUserByIdWithDetailsAsync(id);
        return entity?.ToModel();
    }

    public async Task<UserModel?> GetCustomerByIdAsync(long id)
    {
        var entity = await _userRepository.GetCustomerByIdWithDetailsAsync(id);
        return entity?.ToModel();
    }

    public async Task<Result<UserModel>> UpdateAsync(UserModel model)
    {
        if (model is null)
        {
            var error = new ValidationError(nameof(model), "The passed model is null");
            return Result<UserModel>.Failure(error);
        }

        var entityToUpdate = await _userRepository.GetUserByIdAsync(model.Id);
        if (entityToUpdate is null)
        {
            var error = new EntityNotFoundError(nameof(entityToUpdate), "The entity to update was not found");
            return Result<UserModel>.Failure(error);
        }

        var entity = model.ToEntity();
        entity.PasswordHash = entityToUpdate.PasswordHash;
        entity.IsEmailConfirmed = entityToUpdate.IsEmailConfirmed;
        _userRepository.Update(entity);
        await _unitOfWork.SaveChangesAsync();
        var updatedEntity = await _userRepository.GetUserByIdWithDetailsAsync(entity.Id)
            ?? throw new DbException("There was a database error");
        return updatedEntity.ToModel();
    }

    public async Task<Result<Unit>> UpdateCustomerPasswordAsync(long userId, string oldPassword, string newPassword)
    {
        if (string.IsNullOrEmpty(oldPassword) || string.IsNullOrEmpty(newPassword))
        {
            Dictionary<string, string[]> errors = [];
            if (string.IsNullOrEmpty(oldPassword))
            {
                errors.Add(nameof(oldPassword), ["The passed old password is null or empty"]);
            }

            if (string.IsNullOrEmpty(newPassword))
            {
                errors.Add(nameof(newPassword), ["The passed new password is null or empty"]);
            }

            var error = new ValidationError(errors);
            return Result<Unit>.Failure(error);
        }

        var result = await _userRepository.UpdateCustomerPasswordAsync(userId, oldPassword, newPassword);
        if (result.IsSuccess)
        {
            await _unitOfWork.SaveChangesAsync();
        }

        return result;
    }

    public async Task<Result<Unit>> UpdateCustomerPasswordAsAdminAsync(long userId, string newPassword)
    {
        if (string.IsNullOrEmpty(newPassword))
        {
            ValidationError error = new(nameof(newPassword), "The passed new password is null or empty");
            return Result<Unit>.Failure(error);
        }

        var result = await _userRepository.UpdateCustomerPasswordAsAdminAsync(userId, newPassword);
        if (result.IsSuccess)
        {
            await _unitOfWork.SaveChangesAsync();
        }

        return result;
    }

    public async Task<Result<AuthorizedUserModel>> LoginAsync(string userName, string password)
    {
        if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(password))
        {
            var error = new ValidationError("userData", "The passed userName or password is null");
            return Result<AuthorizedUserModel>.Failure(error);
        }

        var user = await _userRepository.GetUserByUserNameWithDetailsAsync(userName);
        if (user is null)
        {
            var error = new EntityNotFoundError(nameof(user), "The user was not found");
            return Result<AuthorizedUserModel>.Failure(error);
        }

        var isPasswordValid = _userRepository.CheckUserPassword(user, password);
        if (!isPasswordValid)
        {
            var error = new ValidationError("userData", "The passed credentials are not valid");
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
            Dictionary<string, string[]> errors = [];
            if (model is null)
            {
                errors.Add(nameof(model), ["The passed model is null"]);
            }

            if (string.IsNullOrEmpty(password))
            {
                errors.Add(nameof(password), ["The passed password is null or empty"]);
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
        if (securityToken is null || refreshToken is null)
        {
            Dictionary<string, string[]> errors = [];
            if (securityToken is null)
            {
                errors.Add(nameof(securityToken), ["The passed security token is null"]);
            }

            if (refreshToken is null)
            {
                errors.Add(nameof(refreshToken), ["The passed refresh token is null"]);
            }

            var error = new ValidationError(errors);
            return Result<AuthorizedUserModel>.Failure(error);
        }

        try
        {
            var jwtSecurityToken = _securityTokenService.ValidateToken(securityToken, _tokenValidationParametersAccessor.Refresh);
            var isAlgorithmValid = _securityTokenService.IsTokenSecurityAlgorithmValid(jwtSecurityToken);
            var jwtValidationResult = _validator.ValidateExpiredJwt(jwtSecurityToken, isAlgorithmValid);
            if (!jwtValidationResult.IsValid)
            {
                var error = new ValidationError(jwtValidationResult.ToDictionary());
                return Result<AuthorizedUserModel>.Failure(error);
            }

            var storedRefreshToken = await _refreshTokenRepository.GetByTokenAsync(refreshToken);
            var jti = jwtSecurityToken.Claims.First(c => c.Type == JwtRegisteredClaimNames.Jti).Value;
            var refreshTokenValidationResult = _validator.ValidateRefreshToken(storedRefreshToken!, jti);
            if (!refreshTokenValidationResult.IsValid)
            {
                var error = new ValidationError(refreshTokenValidationResult.ToDictionary());
                return Result<AuthorizedUserModel>.Failure(error);
            }

            storedRefreshToken!.Used = true;
            _refreshTokenRepository.Update(storedRefreshToken);
            await _unitOfWork.SaveChangesAsync();

            var userId = long.Parse(jwtSecurityToken.Claims.First(c => c.Type == "id").Value);
            var user = await _userRepository.GetUserByIdWithDetailsAsync(userId);
            if (user is null)
            {
                var error = new ValidationError(nameof(user), "The user does not exist");
                return Result<AuthorizedUserModel>.Failure(error);
            }

            var userModel = user.ToModel();
            var authorizedUserResult = await CreateAuthorizedUserModel(userModel);
            return authorizedUserResult;
        }
        catch
        {
            var error = new ValidationError(nameof(securityToken), "The token is not valid");
            return Result<AuthorizedUserModel>.Failure(error);
        }
    }

    public async Task<bool> LogOutAsync(string refreshToken)
    {
        if (string.IsNullOrEmpty(refreshToken))
        {
            return false;
        }

        var storedRefreshToken = await _refreshTokenRepository.GetByTokenAsync(refreshToken);
        if (storedRefreshToken is null)
        {
            return false;
        }

        storedRefreshToken.Used = true;
        _refreshTokenRepository.Update(storedRefreshToken);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }

    public async Task<Result<bool>> IsUserNameTakenAsync(string userName)
    {
        if (string.IsNullOrEmpty(userName))
        {
            var error = new ValidationError(nameof(userName), "The passed user name is null or empty");
            return Result<bool>.Failure(error);
        }

        var isTakenResult = await _userRepository.IsUserNameTakenAsync(userName);
        return isTakenResult;
    }

    public async Task<Result<bool>> IsEmailTakenAsync(string email)
    {
        if (string.IsNullOrEmpty(email))
        {
            var error = new ValidationError(nameof(email), "The passed email is null or empty");
            return Result<bool>.Failure(error);
        }

        var isTakenResult = await _userRepository.IsEmailTakenAsync(email);
        return isTakenResult;
    }

    public async Task<Result<bool>> IsPhoneNumberTakenAsync(string phoneNumber)
    {
        if (string.IsNullOrEmpty(phoneNumber))
        {
            var error = new ValidationError(nameof(phoneNumber), "The passed phone number is null or empty");
            return Result<bool>.Failure(error);
        }

        var isTakenResult = await _userRepository.IsPhoneNumberTakenAsync(phoneNumber);
        return isTakenResult;
    }

    public async Task<UserModel?> GetUserFromTokenAsync(string token)
    {
        var jwtSecurityToken = _securityTokenService.ValidateToken(token, _tokenValidationParametersAccessor.Regular);
        var result = long.TryParse(jwtSecurityToken.Claims.FirstOrDefault(x => x.Type == "id")?.Value, out long userId);
        var user = result ? await _userRepository.GetUserByIdWithDetailsAsync(userId) : null;
        return user?.ToModel();
    }

    private async Task<Result<AuthorizedUserModel>> CreateAuthorizedUserModel(UserModel userModel)
    {
        if (string.IsNullOrEmpty(userModel.UserRole))
        {
            var role = await _userRoleRepository.GetByIdAsync(userModel.UserRoleId);
            if (role is null)
            {
                ValidationError error = new("role", "The user role was not found");
                return Result<AuthorizedUserModel>.Failure(error);
            }

            userModel.UserRole = role.Name;
        }

        var token = _securityTokenService.CreateToken(userModel, userModel.UserRole);
        var jwtId = token.Claims.First(c => c.Type == JwtRegisteredClaimNames.Jti).Value;
        var refreshToken = CreateRefreshToken(jwtId, userModel.Id, DateTime.UtcNow.AddMonths(6));
        JwtSecurityTokenHandler jwtTokenHandler = new();
        var serializedJwtToken = jwtTokenHandler.WriteToken(token);
        await _refreshTokenRepository.AddAsync(refreshToken);
        await _unitOfWork.SaveChangesAsync();

        var authorizedUser = userModel.ToAuthorizedModel(serializedJwtToken, refreshToken);
        return authorizedUser;
    }

    private static RefreshToken CreateRefreshToken(string jwtId, long userId, DateTime expiryDate)
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
