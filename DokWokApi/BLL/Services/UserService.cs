using AutoMapper;
using DokWokApi.BLL.Interfaces;
using DokWokApi.BLL.Models.User;
using DokWokApi.DAL.Entities;
using DokWokApi.Exceptions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace DokWokApi.BLL.Services;

public class UserService : IUserService
{
    private readonly UserManager<ApplicationUser> userManager;

    private readonly IMapper mapper;

    private readonly IConfiguration config;

    private readonly ISecurityTokenService<ApplicationUser> securityTokenService;

    public UserService(
        UserManager<ApplicationUser> userManager, 
        IMapper mapper, 
        IConfiguration configuration, 
        ISecurityTokenService<ApplicationUser> securityTokenService)
    {
        this.userManager = userManager;
        this.mapper = mapper;
        config = configuration;
        this.securityTokenService = securityTokenService;

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
        var user = mapper.Map<ApplicationUser>(model);
        user.Id = Guid.NewGuid().ToString();
        var result = await userManager.CreateAsync(user, password);
        ThrowIfNotSucceeded(result.Succeeded, result.Errors);

        var addedUser = await userManager.FindByNameAsync(user.UserName!);
        await userManager.AddToRoleAsync(addedUser!, "Customer");
        return mapper.Map<UserModel>(addedUser!);
    }

    public async Task DeleteAsync(string userName)
    {
        var user = await userManager.FindByNameAsync(userName);
        user = CheckRetrievedUser(user, "There is no user with this user name in the database.");

        var result = await userManager.DeleteAsync(user);
        ThrowIfNotSucceeded(result.Succeeded, result.Errors);
    }

    public async Task<IEnumerable<UserModel>> GetAllAsync()
    {
        var queryable = userManager.Users;
        var entities = await queryable.ToListAsync();
        var models = mapper.Map<IEnumerable<UserModel>>(entities);
        return models;
    }

    public async Task<UserModel?> GetByUserNameAsync(string userName)
    {
        var entity = await userManager.FindByNameAsync(userName);
        if (entity is null)
        {
            return null;
        }

        var model = mapper.Map<UserModel>(entity);
        return model;
    }

    public async Task<UserModel?> GetByIdAsync(string id)
    {
        var entity = await userManager.FindByIdAsync(id);
        if (entity is null)
        {
            return null;
        }

        var model = mapper.Map<UserModel>(entity);
        return model;
    }

    public async Task<UserModel> UpdateAsync(UserModel model, string? password = null)
    {
        model = CheckForNull(model, "The passed model is null.");
        var user = await userManager.FindByIdAsync(model.Id!);
        user = CheckRetrievedUser(user, "There is no user with this ID in the database.");

        user.FirstName = model.FirstName;
        user.UserName = model.UserName;
        user.Email = model.Email;
        user.PhoneNumber = model.PhoneNumber;
        var result = await userManager.UpdateAsync(user);
        ThrowIfNotSucceeded(result.Succeeded, result.Errors);

        if (password is not null)
        {
            await userManager.RemovePasswordAsync(user);
            var passwordResult = await userManager.AddPasswordAsync(user, password);
            ThrowIfNotSucceeded(passwordResult.Succeeded, passwordResult.Errors);
        }

        var updatedUser = await userManager.FindByNameAsync(user.UserName!);
        return mapper.Map<UserModel>(updatedUser);
    }

    public async Task<IEnumerable<string>> GetUserRolesAsync(UserModel model)
    {
        model = CheckForNull(model, "The passed model is null.");
        
        var user = mapper.Map<ApplicationUser>(model);
        var roles = await userManager.GetRolesAsync(user);
        return roles;
    }

    public async Task<AuthenticationResponse> AuthenticateLoginAsync(UserLoginModel model)
    {
        model = CheckForNull(model, "The passed model is null.");

        var user = await userManager.FindByNameAsync(model.UserName);
        user = CheckRetrievedUser(user, "The credentials are wrong.");

        var isValidPassword = await userManager.CheckPasswordAsync(user, model.Password);
        ThrowIfNotValid(isValidPassword, "The credentials are wrong.");

        var token = securityTokenService.CreateToken(user);
        var response = new AuthenticationResponse
        {
            UserName = user.UserName ?? string.Empty,
            Email = user.Email ?? string.Empty,
            FirstName = user.FirstName ?? string.Empty,
            PhoneNumber = user.PhoneNumber ?? string.Empty,
            Token = token,
        };
        return response;
    }

    public async Task<AuthenticationResponse> AuthenticateRegisterAsync(UserRegisterModel model)
    {
        model = CheckForNull(model, "The passed model is null.");

        var userModel = mapper.Map<UserModel>(model);
        userModel = await AddAsync(userModel, model.Password);
        var user = mapper.Map<ApplicationUser>(userModel);

        var token = securityTokenService.CreateToken(user);
        var response = new AuthenticationResponse
        {
            UserName = user.UserName ?? string.Empty,
            Email = user.Email ?? string.Empty,
            FirstName = user.FirstName ?? string.Empty,
            PhoneNumber = user.PhoneNumber ?? string.Empty,
            Token = token,
        };
        return response;
    } 
}
