using Application.Mapping.Extensions;
using Domain.Abstractions.Repositories;
using Domain.Abstractions.Services;
using Domain.Abstractions.Validation;
using Domain.DTOs.Commands.Users;
using Domain.DTOs.Responses.Users;
using Domain.Entities;
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
        Ensure.ArgumentNotNullOrWhiteSpace(userName);
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
        Ensure.ArgumentNotNullOrWhiteSpace(newPassword);
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
        Ensure.ArgumentNotNullOrWhiteSpace(userName);
        User? user = await _userRepository.GetUserByUserNameWithDetailsAsync(userName);
        user = Ensure.EntityFound(user, "The user was not found");
        return await CreateAuthorizedUserResponse(user);
    }

    // TODO: Add Transaction for user creation and refresh token creation
    public async Task<AuthorizedUserResponse> RegisterAsync(RegisterUserCommand command)
    {
        Ensure.ArgumentNotNull(command);
        User user = command.ToEntity();
        user = await AddAsync(user, command.Password);
        return await CreateAuthorizedUserResponse(user);
    }

    public async Task<AuthorizedUserResponse> RefreshTokenAsync(string securityToken, string refreshToken)
    {
        Ensure.ArgumentNotNullOrWhiteSpace(securityToken);
        Ensure.ArgumentNotNullOrWhiteSpace(refreshToken);

        try
        {
            var storedRefreshToken = await _refreshTokenRepository.GetByTokenAsync(refreshToken);
            storedRefreshToken = Ensure.EntityFound(storedRefreshToken, "The refresh token was not found");
            storedRefreshToken.Used = true;
            _refreshTokenRepository.Update(storedRefreshToken);

            var jwtSecurityToken = _securityTokenService.ValidateToken(securityToken, _tokenValidationParametersAccessor.Refresh);
            var userId = long.Parse(jwtSecurityToken.Claims.First(c => c.Type == "id").Value);
            var user = await _userRepository.GetUserByIdWithDetailsAsync(userId);
            user = Ensure.EntityFound(user, "The user was not found");
            
            var authorizedUserResponse = await CreateAuthorizedUserResponse(user);
            return authorizedUserResponse;
        }
        catch
        {
            throw new ValidationException(nameof(securityToken), "The token is not valid");
        }
    }

    public async Task LogOutAsync(string refreshToken)
    {
        Ensure.ArgumentNotNullOrWhiteSpace(refreshToken);

        var storedRefreshToken = await _refreshTokenRepository.GetByTokenAsync(refreshToken);
        storedRefreshToken = Ensure.EntityFound(storedRefreshToken, "The refresh token was not found");

        storedRefreshToken.Used = true;
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<bool> IsUserNameUniqueAsync(string userName)
    {
        Ensure.ArgumentNotNullOrWhiteSpace(userName);
        return await _userRepository.IsUserNameUniqueAsync(userName);
    }

    public async Task<bool> IsEmailUniqueAsync(string email)
    {
        Ensure.ArgumentNotNullOrWhiteSpace(email);
        return await _userRepository.IsEmailUniqueAsync(email);
    }

    public async Task<bool> IsPhoneNumberUniqueAsync(string phoneNumber)
    {
        Ensure.ArgumentNotNullOrWhiteSpace(phoneNumber);
        return await _userRepository.IsPhoneNumberUniqueAsync(phoneNumber);
    }
    
    private async Task<User> AddAsync(User user, string password)
    {
        Ensure.ArgumentNotNull(user);
        Ensure.ArgumentNotNullOrWhiteSpace(password);
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
