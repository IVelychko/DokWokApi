using DokWokApi.BLL.Models.User;

namespace DokWokApi.BLL.Interfaces;

public interface IUserService
{
    Task<IEnumerable<UserModel>> GetAllAsync();

    Task<IEnumerable<UserModel>> GetAllCustomersAsync();

    Task<UserModel?> GetByUserNameAsync(string userName);

    Task<UserModel?> GetByIdAsync(string id);

    Task<UserModel?> GetCustomerByIdAsync(string id);

    Task<UserModel> AddAsync(UserModel model, string password);

    Task<UserModel> UpdateAsync(UserModel model);

    Task UpdateCustomerPasswordAsync(UserPasswordChangeModel model);

    Task UpdateCustomerPasswordAsAdminAsync(UserPasswordChangeAsAdminModel model);

    Task DeleteAsync(string id);

    Task<IEnumerable<string>> GetUserRolesAsync(string userId);

    Task<AuthorizedUserModel> AuthenticateCustomerLoginAsync(UserLoginModel model);

    Task<AuthorizedUserModel> AuthenticateAdminLoginAsync(UserLoginModel model);

    Task<AuthorizedUserModel> AuthenticateRegisterAsync(UserRegisterModel model);

    Task<UserModel?> IsCustomerTokenValidAsync(string token);

    Task<UserModel?> IsAdminTokenValidAsync(string token);

    Task<bool> IsUserNameTaken(string userName);

    Task<bool> IsEmailTaken(string email);

    Task<bool> IsPhoneNumberTaken(string phoneNumber);

    Task<UserModel?> GetUserFromToken(string token);
}
