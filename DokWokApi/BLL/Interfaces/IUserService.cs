using DokWokApi.BLL.Models.User;

namespace DokWokApi.BLL.Interfaces;

public interface IUserService
{
    Task<IEnumerable<UserModel>> GetAllAsync();

    Task<UserModel?> GetByUserNameAsync(string userName);

    Task<UserModel?> GetByIdAsync(string id);

    Task<UserModel> AddAsync(UserModel model, string password);

    Task<UserModel> UpdateAsync(UserModel model, string? password = null);

    Task DeleteAsync(string userName);

    Task<IEnumerable<string>> GetUserRolesAsync(UserModel model);

    Task AuthenticateLoginAsync(UserLoginModel model);

    Task AuthenticateRegisterAsync(UserRegisterModel model);

    Task<UserModel?> IsLoggedInAsync();

    Task LogOutAsync();
}
