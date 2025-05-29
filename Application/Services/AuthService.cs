using System.IdentityModel.Tokens.Jwt;
using Application.Extensions;
using Application.Mapping.Extensions;
using Domain.Abstractions.Repositories;
using Domain.Abstractions.Services;
using Domain.Abstractions.Validation;
using Domain.Constants;
using Domain.DTOs.Requests.Users;
using Domain.DTOs.Responses.Users;
using Domain.Entities;
using Domain.Exceptions;
using Domain.Shared;
using Domain.Specifications.Users;

namespace Application.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IUserRoleRepository _userRoleRepository;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ISecurityTokenService<User, JwtSecurityToken> _securityTokenService;
    private readonly IPasswordHasher _passwordHasher;
    private readonly TokenValidationParametersAccessor _tokenValidationParametersAccessor;
    private readonly IAuthServiceValidator _validator;

    public AuthService(
        IUserRepository userRepository,
        IUserRoleRepository userRoleRepository,
        IRefreshTokenRepository refreshTokenRepository,
        IUnitOfWork unitOfWork,
        ISecurityTokenService<User, JwtSecurityToken> securityTokenService,
        IPasswordHasher passwordHasher,
        TokenValidationParametersAccessor tokenValidationParametersAccessor,
        IAuthServiceValidator validator)
    {
        _userRepository = userRepository;
        _userRoleRepository = userRoleRepository;
        _refreshTokenRepository = refreshTokenRepository;
        _unitOfWork = unitOfWork;
        _securityTokenService = securityTokenService;
        _passwordHasher = passwordHasher;
        _tokenValidationParametersAccessor = tokenValidationParametersAccessor;
        _validator = validator;
    }
    
    public async Task<AuthorizedUserResponse> LoginCustomerAsync(LoginCustomerRequest request)
    {
        await ValidateLoginCustomerRequestAsync(request);
        var user = await GetUserWithCustomerRoleByUserNameAsync(request.UserName);
        var role = await GetUserRoleByIdAsync(user.UserRoleId);
        var token = _securityTokenService.CreateToken(user, role.Name);
        var refreshToken = await AddRefreshTokenAsync(user.Id, token);
        user.UserRole = role;
        return CreateAuthorizedUserResponse(user, token, refreshToken);
    }
    
    public async Task<AuthorizedUserResponse> LoginAdminAsync(LoginAdminRequest request)
    {
        await ValidateLoginAdminRequestAsync(request);
        var user = await GetUserByUserNameInternalAsync(request.UserName);
        var role = await GetUserRoleByIdAsync(user.UserRoleId);
        var token = _securityTokenService.CreateToken(user, role.Name);
        var refreshToken = await AddRefreshTokenAsync(user.Id, token);
        user.UserRole = role;
        return CreateAuthorizedUserResponse(user, token, refreshToken);
    }
    
    public async Task<AuthorizedUserResponse> RegisterAsync(RegisterUserRequest request)
    {
        await ValidateRegisterUserRequestAsync(request);
        var user = request.ToEntity();
        var customerRole = await GetUserRoleByNameAsync(UserRoles.Customer);
        user = await AddAsync(user, request.Password, customerRole.Id);
        var token = _securityTokenService.CreateToken(user, customerRole.Name);
        var refreshToken = await AddRefreshTokenAsync(user.Id, token);
        user.UserRole = customerRole;
        return CreateAuthorizedUserResponse(user, token, refreshToken);
    }
    
    public async Task<AuthorizedUserResponse> RefreshTokenAsync(RefreshUserTokenRequest request)
    {
        await ValidateRefreshUserTokenRequestAsync(request);
        await MarkRefreshTokenAsUsedAsync(request.RefreshToken);
        var jwtSecurityToken = ValidateTokenToRefresh(request.Token);
        var userId = long.Parse(jwtSecurityToken.Claims.First(c => c.Type == "id").Value);
        var user = await GetUserByIdInternalAsync(userId);
        var role = await GetUserRoleByIdAsync(user.UserRoleId);
        var newToken = _securityTokenService.CreateToken(user, role.Name);
        var refreshToken = await AddRefreshTokenAsync(user.Id, newToken);
        user.UserRole = role;
        return CreateAuthorizedUserResponse(user, newToken, refreshToken);
    }
    
    public async Task LogOutAsync(string refreshToken)
    {
        await ValidateLogOutUserRequestAsync(refreshToken);
        await MarkRefreshTokenAsUsedAsync(refreshToken);
        await _unitOfWork.SaveChangesAsync();
    }

    private async Task MarkRefreshTokenAsUsedAsync(string refreshToken)
    {
        var storedRefreshToken = await _refreshTokenRepository.GetByTokenAsync(refreshToken);
        storedRefreshToken = Ensure.EntityExists(storedRefreshToken, "The refresh token was not found");
        storedRefreshToken.Used = true;
    }

    private JwtSecurityToken ValidateTokenToRefresh(string token)
    {
        try
        {
            return _securityTokenService.ValidateToken(token, _tokenValidationParametersAccessor.Refresh);
        }
        catch
        {
            throw new ValidationException("SecurityToken", "The token is not valid");
        }
    }
    
    private async Task<User> AddAsync(User user, string password, long roleId)
    {
        user.UserRoleId = roleId;
        user.PasswordHash = _passwordHasher.Hash(password);
        await _userRepository.AddAsync(user);
        await _unitOfWork.SaveChangesAsync();
        return user;
    }

    private async Task<RefreshToken> AddRefreshTokenAsync(long userId, JwtSecurityToken token)
    {
        var jwtId = token.Claims.First(c => c.Type == JwtRegisteredClaimNames.Jti).Value;
        var refreshToken = CreateRefreshToken(jwtId, userId, DateTime.UtcNow.AddMonths(6));
        await _refreshTokenRepository.AddAsync(refreshToken);
        await _unitOfWork.SaveChangesAsync();
        return refreshToken;
    }
    
    private static AuthorizedUserResponse CreateAuthorizedUserResponse(
        User user, JwtSecurityToken token, RefreshToken refreshToken)
    {
        JwtSecurityTokenHandler jwtTokenHandler = new();
        var serializedJwtToken = jwtTokenHandler.WriteToken(token);
        return user.ToAuthorizedResponse(serializedJwtToken, refreshToken);
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
    
    private async Task ValidateLoginCustomerRequestAsync(LoginCustomerRequest request)
    {
        Ensure.ArgumentNotNull(request);
        var validationResult = await _validator.ValidateLoginCustomerUserAsync(request);
        validationResult.ThrowIfValidationFailed();
    }
    
    private async Task ValidateLoginAdminRequestAsync(LoginAdminRequest request)
    {
        Ensure.ArgumentNotNull(request);
        var validationResult = await _validator.ValidateLoginAdminUserAsync(request);
        validationResult.ThrowIfValidationFailed();
    }
    
    private async Task ValidateLogOutUserRequestAsync(string refreshToken)
    {
        Ensure.ArgumentNotNullOrWhiteSpace(refreshToken, nameof(refreshToken));
        LogOutUserRequest request = new(refreshToken);
        var validationResult = await _validator.ValidateLogOutUserAsync(request);
        validationResult.ThrowIfValidationFailed();
    }
    
    private async Task ValidateRegisterUserRequestAsync(RegisterUserRequest request)
    {
        Ensure.ArgumentNotNull(request);
        var validationResult = await _validator.ValidateRegisterUserAsync(request);
        validationResult.ThrowIfValidationFailed();
    }
    
    private async Task ValidateRefreshUserTokenRequestAsync(RefreshUserTokenRequest request)
    {
        Ensure.ArgumentNotNull(request);
        var validationResult = await _validator.ValidateRefreshUserTokenAsync(request);
        validationResult.ThrowIfValidationFailed();
    }
    
    private async Task<UserRole> GetUserRoleByIdAsync(long id)
    {
        var role = await _userRoleRepository.GetByIdAsync(id);
        return Ensure.EntityExists(role, "The role was not found");
    }
    
    private async Task<UserRole> GetUserRoleByNameAsync(string roleName)
    {
        var role = await _userRoleRepository.GetByNameAsync(roleName);
        return Ensure.EntityExists(role, "The role was not found");
    }
    
    private async Task<User> GetUserWithCustomerRoleByUserNameAsync(string userName)
    {
        Ensure.ArgumentNotNullOrWhiteSpace(userName, nameof(userName));
        var customerRole = await GetUserRoleByNameAsync(UserRoles.Customer);
        UserSpecification specification = new()
        {
            UserName = userName,
            RoleId = customerRole.Id
        };
        var customer = await _userRepository.GetBySpecificationAsync(specification);
        return Ensure.EntityExists(customer, "The customer was not found");
    }
    
    private async Task<User> GetUserByIdInternalAsync(long id)
    {
        var user = await _userRepository.GetByIdAsync(id);
        return Ensure.EntityExists(user, "The user was not found");
    }
    
    private async Task<User> GetUserByUserNameInternalAsync(string userName)
    {
        var user = await _userRepository.GetByUserNameAsync(userName);
        return Ensure.EntityExists(user, "The user was not found");
    }
}