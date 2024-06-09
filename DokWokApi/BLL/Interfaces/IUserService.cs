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

    Task DeleteAsync(string id);

    Task<IEnumerable<string>> GetUserRolesAsync(string userId);

    Task<UserModel> AuthenticateLoginAsync(UserLoginModel model);

    Task<UserModel> AuthenticateRegisterAsync(UserRegisterModel model);

    Task<UserModel?> IsCustomerLoggedInAsync();

    Task<UserModel?> IsAdminLoggedInAsync();

    Task LogOutAsync();

    Task<bool> IsUserNameTaken(string userName);

    Task<bool> IsEmailTaken(string email);
}
