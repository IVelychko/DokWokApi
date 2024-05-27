using AutoMapper;
using DokWokApi.BLL.Interfaces;
using DokWokApi.BLL.Models.User;
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

    private static T CheckForNull<T>(T? model, string errorMessage)
    {
        if (model is null)
        {
            throw new ArgumentNullException(nameof(model), errorMessage);
        }

        return model;
    }

    private static ApplicationUser CheckRetrievedUser(ApplicationUser? user, string errorMessage)
    {
        if (user is null)
        {
            throw new EntityNotFoundException(nameof(user), errorMessage);
        }

        return user;
    }

    private static void ThrowIfNotSucceeded(bool succeeded, IEnumerable<IdentityError> errors)
    {
        if (!succeeded)
        {
            var error = errors.Select(e => e.Description)
                .Aggregate((e1, e2) => $"{e1}\n{e2}");

            throw new UserException(nameof(succeeded), error);
        }
    }

    private static void ThrowIfNotValid(bool isValid, string errorMessage)
    {
        if (!isValid)
        {
            throw new UserException(nameof(isValid), errorMessage);
        }
    }

    public async Task<UserModel> AddAsync(UserModel model, string password)
    {
        model = CheckForNull(model, "The passed model is null.");
        var user = _mapper.Map<ApplicationUser>(model);
        user.Id = Guid.NewGuid().ToString();
        var result = await _userManager.CreateAsync(user, password);
        ThrowIfNotSucceeded(result.Succeeded, result.Errors);

        var addedUser = await _userManager.FindByNameAsync(user.UserName!);
        await _userManager.AddToRoleAsync(addedUser!, "Customer");
        return _mapper.Map<UserModel>(addedUser!);
    }

    public async Task DeleteAsync(string userName)
    {
        var user = await _userManager.FindByNameAsync(userName);
        user = CheckRetrievedUser(user, "There is no user with this user name in the database.");

        var result = await _userManager.DeleteAsync(user);
        ThrowIfNotSucceeded(result.Succeeded, result.Errors);
    }

    public async Task<IEnumerable<UserModel>> GetAllAsync()
    {
        var queryable = _userManager.Users;
        var entities = await queryable.ToListAsync();
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

    public async Task<UserModel> UpdateAsync(UserModel model, string? password = null)
    {
        model = CheckForNull(model, "The passed model is null.");
        var user = await _userManager.FindByIdAsync(model.Id!);
        user = CheckRetrievedUser(user, "There is no user with this ID in the database.");

        user.FirstName = model.FirstName;
        user.UserName = model.UserName;
        user.Email = model.Email;
        user.PhoneNumber = model.PhoneNumber;
        var result = await _userManager.UpdateAsync(user);
        ThrowIfNotSucceeded(result.Succeeded, result.Errors);

        if (password is not null)
        {
            await _userManager.RemovePasswordAsync(user);
            var passwordResult = await _userManager.AddPasswordAsync(user, password);
            ThrowIfNotSucceeded(passwordResult.Succeeded, passwordResult.Errors);
        }

        var updatedUser = await _userManager.FindByNameAsync(user.UserName!);
        return _mapper.Map<UserModel>(updatedUser);
    }

    public async Task<IEnumerable<string>> GetUserRolesAsync(UserModel model)
    {
        model = CheckForNull(model, "The passed model is null.");
        
        var user = _mapper.Map<ApplicationUser>(model);
        var roles = await _userManager.GetRolesAsync(user);
        return roles;
    }

    public async Task AuthenticateLoginAsync(UserLoginModel model)
    {
        model = CheckForNull(model, "The passed model is null.");

        var user = await _userManager.FindByNameAsync(model.UserName);
        user = CheckRetrievedUser(user, "The credentials are wrong.");

        var isValidPassword = await _userManager.CheckPasswordAsync(user, model.Password);
        ThrowIfNotValid(isValidPassword, "The credentials are wrong.");

        var userModel = _mapper.Map<UserModel>(user);
        var token = _securityTokenService.CreateToken(userModel);
        if (_session is null)
        {
            throw new SessionException(nameof(_session), "The session is null");
        }

        await _session.SetStringAsync("userToken", token);
    }

    public async Task AuthenticateRegisterAsync(UserRegisterModel model)
    {
        model = CheckForNull(model, "The passed model is null.");

        var userModel = _mapper.Map<UserModel>(model);
        userModel = await AddAsync(userModel, model.Password);

        var token = _securityTokenService.CreateToken(userModel);
        if (_session is null)
        {
            throw new SessionException(nameof(_session), "The session is null");
        }

        await _session.SetStringAsync("userToken", token);
    }

    public async Task<UserModel?> IsLoggedInAsync()
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
