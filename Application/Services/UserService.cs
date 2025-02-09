using Application.Mapping.Extensions;
using Domain.Abstractions.Repositories;
using Domain.Abstractions.Services;
using Domain.Abstractions.Validation;
using Domain.DTOs.Commands.Users;
using Domain.DTOs.Responses.Users;
using Domain.Entities;
using Domain.Errors;
using Domain.Exceptions;
using Domain.Mapping.Extensions;
using Domain.Models;
using Domain.Models.User;
using Domain.Shared;
using System.IdentityModel.Tokens.Jwt;

namespace Application.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IUserRoleRepository _userRoleRepository;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICacheService _cacheService;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ISecurityTokenService<User, JwtSecurityToken> _securityTokenService;
    private readonly IUserServiceValidator _validator;
    private readonly TokenValidationParametersAccessor _tokenValidationParametersAccessor;

    public UserService(IUserRepository userRepository,
        IRefreshTokenRepository refreshTokenRepository,
        ISecurityTokenService<User, JwtSecurityToken> securityTokenService,
        IUserServiceValidator validator,
        TokenValidationParametersAccessor tokenValidationParametersAccessor,
        IUserRoleRepository userRoleRepository,
        IUnitOfWork unitOfWork,
        ICacheService cacheService,
        IPasswordHasher passwordHasher)
    {
        _userRepository = userRepository;
        _refreshTokenRepository = refreshTokenRepository;
        _securityTokenService = securityTokenService;
        _validator = validator;
        _tokenValidationParametersAccessor = tokenValidationParametersAccessor;
        _userRoleRepository = userRoleRepository;
        _unitOfWork = unitOfWork;
        _cacheService = cacheService;
        _passwordHasher = passwordHasher;
    }

    public async Task<UserResponse> AddAsync(AddUserCommand command)
    {
        Ensure.ArgumentNotNull(command);
        User user = command.ToEntity();
        user = await AddAsync(user, command.Password);
        return user.ToResponse();
    }

    public async Task DeleteAsync(long id)
    {
        User? entityToDelete = await _userRepository.GetCustomerByIdAsync(id);
        entityToDelete = Ensure.EntityFound(entityToDelete, "The customer was not found");
        _userRepository.Delete(entityToDelete);
        await _unitOfWork.SaveChangesAsync();

        await _cacheService.RemoveAsync("allUsers");
        await _cacheService.RemoveAsync("allCustomers");
        await _cacheService.RemoveAsync($"userByUserName{entityToDelete.UserName}");
        await _cacheService.RemoveAsync($"userById{id}");
        await _cacheService.RemoveAsync($"customerById{id}");
        await _cacheService.RemoveByPrefixAsync("paginatedAllUsers");
        await _cacheService.RemoveByPrefixAsync("paginatedAllCustomers");
    }

    public async Task<IEnumerable<UserResponse>> GetAllUsersAsync(PageInfo? pageInfo = null)
    {
        string key = pageInfo is null ? "allUsers" :
            $"paginatedAllUsers-page{pageInfo.Number}-size{pageInfo.Size}";

        Specification<User> specification = new() { PageInfo = pageInfo };
        specification.IncludeExpressions.Add(new(u => u.UserRole));
        IList<User> entities = await Caching.GetCollectionFromCache(_cacheService,
            key, specification, _userRepository.GetAllUsersBySpecificationAsync);

        return entities.Select(p => p.ToResponse());
    }

    public async Task<IEnumerable<UserResponse>> GetAllCustomersAsync(PageInfo? pageInfo = null)
    {
        string key = pageInfo is null ? "allCustomers" :
            $"paginatedAllCustomers-page{pageInfo.Number}-size{pageInfo.Size}";

        Specification<User> specification = new() { PageInfo = pageInfo };
        specification.IncludeExpressions.Add(new(u => u.UserRole));
        IList<User> entities = await Caching.GetCollectionFromCache(_cacheService,
            key, specification, _userRepository.GetAllCustomersBySpecificationAsync);

        return entities.Select(p => p.ToResponse());
    }

    public async Task<UserResponse?> GetUserByUserNameAsync(string userName)
    {
        string key = $"userByUserName{userName}";
        Specification<User> specification = new() { Criteria = u => u.UserName == userName };
        specification.IncludeExpressions.Add(new(u => u.UserRole));
        User? entity = await Caching.GetEntityFromCache(_cacheService,
            key, specification, _userRepository.GetAllUsersBySpecificationAsync);

        return entity?.ToResponse();
    }

    public async Task<UserResponse?> GetUserByIdAsync(long id)
    {
        string key = $"userById{id}";
        Specification<User> specification = new() { Criteria = u => u.Id == id };
        specification.IncludeExpressions.Add(new(u => u.UserRole));
        User? entity = await Caching.GetEntityFromCache(_cacheService,
            key, specification, _userRepository.GetAllUsersBySpecificationAsync);

        return entity?.ToResponse();
    }

    public async Task<UserResponse?> GetCustomerByIdAsync(long id)
    {
        string key = $"customerById{id}";
        Specification<User> specification = new() { Criteria = u => u.Id == id };
        specification.IncludeExpressions.Add(new(u => u.UserRole));
        User? entity = await Caching.GetEntityFromCache(_cacheService,
            key, specification, _userRepository.GetAllCustomersBySpecificationAsync);

        return entity?.ToResponse();
    }

    public async Task<UserResponse> UpdateAsync(UpdateUserCommand command)
    {
        Ensure.ArgumentNotNull(command);
        User entity = command.ToEntity();
        User? entityToUpdate = await _userRepository.GetUserByIdWithDetailsAsync(entity.Id);
        entityToUpdate = Ensure.EntityFound(entityToUpdate, "The user was not found");

        await _cacheService.RemoveAsync($"userByUserName{entityToUpdate.UserName}");
        await _cacheService.RemoveAsync("allUsers");
        await _cacheService.RemoveAsync($"userById{entityToUpdate.Id}");
        await _cacheService.RemoveByPrefixAsync("paginatedAllUsers");
        if (entityToUpdate.UserRole?.Name == UserRoles.Customer)
        {
            await _cacheService.RemoveAsync("allCustomers");
            await _cacheService.RemoveAsync($"customerById{entityToUpdate.Id}");
            await _cacheService.RemoveByPrefixAsync("paginatedAllCustomers");
        }
        
        ChangeState(entityToUpdate, entity);
        await _unitOfWork.SaveChangesAsync();
        User updatedEntity = await _userRepository.GetUserByIdWithDetailsAsync(entity.Id)
                             ?? throw new DbException("There was a database error");

        return updatedEntity.ToResponse();
    }

    public async Task UpdateCustomerPasswordAsync(long userId, string newPassword)
    {
        Ensure.ArgumentNotNull(newPassword);
        User? entityToUpdate = await _userRepository.GetCustomerByIdAsync(userId);
        entityToUpdate = Ensure.EntityFound(entityToUpdate, "The customer was not found");
        entityToUpdate.PasswordHash = _passwordHasher.Hash(newPassword);
        await _unitOfWork.SaveChangesAsync();

        await _cacheService.RemoveByPrefixAsync("userBy");
        await _cacheService.RemoveAsync("allUsers");
        await _cacheService.RemoveAsync("allCustomers");
        await _cacheService.RemoveByPrefixAsync("customerBy");
        await _cacheService.RemoveByPrefixAsync("paginatedAllUsers");
        await _cacheService.RemoveByPrefixAsync("paginatedAllCustomers");
    }

    public async Task<AuthorizedUserResponse> LoginAsync(string userName)
    {
        Ensure.ArgumentNotNull(userName);
        User? user = await _userRepository.GetUserByUserNameWithDetailsAsync(userName);
        user = Ensure.EntityFound(user, "The user was not found");
        return await CreateAuthorizedUserResponse(user);
    }

    public async Task<AuthorizedUserResponse> RegisterAsync(RegisterUserCommand command)
    {
        Ensure.ArgumentNotNull(command);
        User user = command.ToEntity();
        user = await AddAsync(user, command.Password);
        return await CreateAuthorizedUserResponse(user);
    }

    public async Task<AuthorizedUserResponse> RefreshTokenAsync(string securityToken, string refreshToken)
    {
        Ensure.ArgumentNotNull(securityToken);
        Ensure.ArgumentNotNull(refreshToken);

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
    
    private async Task<User> AddAsync(User user, string password)
    {
        UserRole customerRole = await _userRoleRepository.GetByNameAsync(UserRoles.Customer)
                                ?? throw new DbException("The customer role doesn't exist");
        user.UserRoleId = customerRole.Id;
        user.PasswordHash = _passwordHasher.Hash(password);
        await _userRepository.AddAsync(user);
        await _unitOfWork.SaveChangesAsync();
        User createdEntity = await _userRepository.GetUserByIdWithDetailsAsync(user.Id) 
                             ?? throw new DbException("There was a database error");

        await _cacheService.RemoveAsync("allUsers");
        await _cacheService.RemoveAsync("allCustomers");
        await _cacheService.RemoveByPrefixAsync("paginatedAllUsers");
        await _cacheService.RemoveByPrefixAsync("paginatedAllCustomers");

        return createdEntity;
    }

    public async Task<User?> GetUserFromTokenAsync(string token)
    {
        var jwtSecurityToken = _securityTokenService.ValidateToken(token, _tokenValidationParametersAccessor.Regular);
        var result = long.TryParse(jwtSecurityToken.Claims.FirstOrDefault(x => x.Type == "id")?.Value, out long userId);
        var user = result ? await _userRepository.GetUserByIdWithDetailsAsync(userId) : null;
        return user;
    }

    private async Task<AuthorizedUserResponse> CreateAuthorizedUserResponse(User user)
    {
        JwtSecurityToken token = _securityTokenService.CreateToken(user, user.UserRole!.Name);
        string jwtId = token.Claims.First(c => c.Type == JwtRegisteredClaimNames.Jti).Value;
        RefreshToken refreshToken = CreateRefreshToken(jwtId, user.Id, DateTime.UtcNow.AddMonths(6));
        JwtSecurityTokenHandler jwtTokenHandler = new();
        string serializedJwtToken = jwtTokenHandler.WriteToken(token);
        await _refreshTokenRepository.AddAsync(refreshToken);
        await _unitOfWork.SaveChangesAsync();

        AuthorizedUserResponse authorizedUserResponse = user.ToAuthorizedResponse(serializedJwtToken, refreshToken);
        return authorizedUserResponse;
    }

    private static RefreshToken CreateRefreshToken(string jwtId, long userId, DateTime expiryDate)
    {
        return new RefreshToken
        {
            CreationDate = DateTime.UtcNow,
            ExpiryDate = expiryDate,
            JwtId = jwtId,
            Token = Guid.NewGuid().ToString(),
            UserId = userId
        };
    }

    private static void ChangeState(User existingTrackedEntity, User updatedEntity)
    {
        existingTrackedEntity.Id = updatedEntity.Id;
        existingTrackedEntity.UserName = updatedEntity.UserName;
        existingTrackedEntity.Email = updatedEntity.Email;
        existingTrackedEntity.PhoneNumber = updatedEntity.PhoneNumber;
        existingTrackedEntity.FirstName = updatedEntity.FirstName;
    }
}
