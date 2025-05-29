using Domain.Abstractions.Services;
using Domain.DTOs.Requests.Users;
using Domain.DTOs.Responses.Users;
using Domain.Shared;

namespace Infrastructure.Cache;

public class CacheUserService : IUserService
{
    private readonly IUserService _userService;
    private readonly ICacheService _cacheService;

    public CacheUserService(IUserService userService, ICacheService cacheService)
    {
        _userService = userService;
        _cacheService = cacheService;
    }

    public async Task<UserResponse> AddAsync(AddUserRequest request)
    {
        Ensure.ArgumentNotNull(request);
        var response = await _userService.AddAsync(request);
        await _cacheService.RemoveAsync("allUsers");
        await _cacheService.RemoveAsync("allCustomers");
        return response;
    }

    public async Task DeleteAsync(long id)
    {
        await _userService.DeleteAsync(id);
        await _cacheService.RemoveAsync("allUsers");
        await _cacheService.RemoveAsync("allCustomers");
        await _cacheService.RemoveByPrefixAsync("userBy");
        await _cacheService.RemoveByPrefixAsync("customerBy");
    }

    public async Task<IList<UserResponse>> GetAllUsersAsync()
    {
        const string key = "allUsers";
        var cachedResponses = await _cacheService.GetAsync<IList<UserResponse>>(key);
        if (cachedResponses is not null)
        {
            return cachedResponses;
        }

        var responses = await _userService.GetAllUsersAsync();
        await _cacheService.SetAsync(key, responses);
        return responses;
    }

    public async Task<IList<UserResponse>> GetAllCustomersAsync()
    {
        const string key = "allCustomers";
        var cachedResponses = await _cacheService.GetAsync<IList<UserResponse>>(key);
        if (cachedResponses is not null)
        {
            return cachedResponses;
        }

        var responses = await _userService.GetAllCustomersAsync();
        await _cacheService.SetAsync(key, responses);
        return responses;
    }

    public async Task<UserResponse> GetUserByUserNameAsync(string userName)
    {
        var key = $"userByUserName{userName}";
        var cachedResponse = await _cacheService.GetAsync<UserResponse>(key);
        if (cachedResponse is not null)
        {
            return cachedResponse;
        }

        var response = await _userService.GetUserByUserNameAsync(userName);
        await _cacheService.SetAsync(key, response);
        return response;
    }

    public async Task<UserResponse> GetUserByIdAsync(long id)
    {
        var key = $"userById{id}";
        var cachedResponse = await _cacheService.GetAsync<UserResponse>(key);
        if (cachedResponse is not null)
        {
            return cachedResponse;
        }

        var response = await _userService.GetUserByIdAsync(id);
        await _cacheService.SetAsync(key, response);
        return response;
    }

    public async Task<UserResponse> GetCustomerByIdAsync(long id)
    {
        var key = $"customerById{id}";
        var cachedResponse = await _cacheService.GetAsync<UserResponse>(key);
        if (cachedResponse is not null)
        {
            return cachedResponse;
        }

        var response = await _userService.GetCustomerByIdAsync(id);
        await _cacheService.SetAsync(key, response);
        return response;
    }

    public async Task<UserResponse> UpdateAsync(UpdateUserRequest request)
    {
        Ensure.ArgumentNotNull(request);
        var response = await _userService.UpdateAsync(request);
        await _cacheService.RemoveByPrefixAsync("userBy");
        await _cacheService.RemoveAsync("allUsers");
        await _cacheService.RemoveAsync("allCustomers");
        await _cacheService.RemoveByPrefixAsync("customerBy");
        return response;
    }

    public async Task UpdateCustomerPasswordAsync(long userId, string oldPassword, string newPassword)
    {
        Ensure.ArgumentNotNullOrWhiteSpace(oldPassword, nameof(oldPassword));
        Ensure.ArgumentNotNullOrWhiteSpace(newPassword, nameof(newPassword));
        await _userService.UpdateCustomerPasswordAsync(userId, oldPassword, newPassword);
        await _cacheService.RemoveByPrefixAsync("userBy");
        await _cacheService.RemoveAsync("allUsers");
        await _cacheService.RemoveAsync("allCustomers");
        await _cacheService.RemoveByPrefixAsync("customerBy");
    }

    public async Task UpdateCustomerPasswordAsAdminAsync(long userId, string newPassword)
    {
        Ensure.ArgumentNotNullOrWhiteSpace(newPassword, nameof(newPassword));
        await _userService.UpdateCustomerPasswordAsAdminAsync(userId, newPassword);
        await _cacheService.RemoveByPrefixAsync("userBy");
        await _cacheService.RemoveAsync("allUsers");
        await _cacheService.RemoveAsync("allCustomers");
        await _cacheService.RemoveByPrefixAsync("customerBy");
    }
    
    public async Task<bool> IsUserNameUniqueAsync(string userName)
    {
        Ensure.ArgumentNotNullOrWhiteSpace(userName, nameof(userName));
        return await _userService.IsUserNameUniqueAsync(userName);
    }
    
    public async Task<bool> IsEmailUniqueAsync(string email)
    {
        Ensure.ArgumentNotNullOrWhiteSpace(email, nameof(email));
        return await _userService.IsEmailUniqueAsync(email);
    }
    
    public async Task<bool> IsPhoneNumberUniqueAsync(string phoneNumber)
    {
        Ensure.ArgumentNotNullOrWhiteSpace(phoneNumber, nameof(phoneNumber));
        return await _userService.IsPhoneNumberUniqueAsync(phoneNumber);
    }
}