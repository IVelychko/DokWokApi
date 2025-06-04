using Application.Mapping.Extensions;
using Domain.Abstractions.Repositories;
using Domain.Abstractions.Services;
using Domain.Abstractions.Validation;
using Domain.Constants;
using Domain.DTOs.Responses.Users;
using Domain.Entities;
using Domain.Shared;
using Application.Extensions;
using Domain.DTOs.Requests.Users;
using Domain.DTOs.Responses;
using Domain.Specifications.Users;

namespace Application.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IUserRoleRepository _userRoleRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IUserServiceValidator _validator;

    public UserService(
        IUserRepository userRepository,
        IUserServiceValidator validator,
        IUserRoleRepository userRoleRepository,
        IUnitOfWork unitOfWork,
        IPasswordHasher passwordHasher)
    {
        _userRepository = userRepository;
        _validator = validator;
        _userRoleRepository = userRoleRepository;
        _unitOfWork = unitOfWork;
        _passwordHasher = passwordHasher;
    }

    public async Task<UserResponse> AddAsync(AddUserRequest request)
    {
        await ValidateAddUserRequestAsync(request);
        var user = request.ToEntity();
        user = await AddAsync(user, request.Password);
        return user.ToResponse();
    }

    public async Task DeleteAsync(long id)
    {
        await ValidateDeleteUserRequestAsync(id);
        var entityToDelete = await GetUserWithCustomerRoleByIdAsync(id);
        _userRepository.Delete(entityToDelete);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<IList<UserResponse>> GetAllUsersAsync()
    {
        var entities = await _userRepository.GetAllBySpecificationAsync(UserSpecification.IncludeAll);
        return entities.Select(p => p.ToResponse()).ToList();
    }

    public async Task<IList<UserResponse>> GetAllUsersByRoleNameAsync(string roleName)
    {
        Ensure.ArgumentNotNullOrWhiteSpace(roleName, nameof(roleName));
        var userRole = await GetUserRoleByNameAsync(roleName);
        UserSpecification specification = new()
        {
            IncludeRole = true,
            RoleId = userRole.Id,
        };
        var entities = await _userRepository.GetAllBySpecificationAsync(specification);
        return entities.Select(p => p.ToResponse()).ToList();
    }

    public async Task<IList<UserResponse>> GetAllCustomersAsync()
    {
        var customerRole = await GetUserRoleByNameAsync(UserRoles.Customer);
        UserSpecification specification = new()
        {
            IncludeRole = true,
            RoleId = customerRole.Id,
        };
        var entities = await _userRepository.GetAllBySpecificationAsync(specification);
        return entities.Select(p => p.ToResponse()).ToList();
    }

    public async Task<UserResponse> GetUserByUserNameAsync(string userName)
    {
        Ensure.ArgumentNotNullOrWhiteSpace(userName, nameof(userName));
        var entity = await GetUserByUserNameInternalAsync(userName);
        entity.UserRole = await GetUserRoleByIdAsync(entity.UserRoleId);
        return entity.ToResponse();
    }

    public async Task<UserResponse> GetUserByIdAsync(long id)
    {
        var entity = await GetUserByIdInternalAsync(id);
        entity.UserRole = await GetUserRoleByIdAsync(entity.UserRoleId);
        return entity.ToResponse();
    }
    
    public async Task<UserResponse> GetUserByRoleNameAndUserIdAsync(string roleName, long userId)
    {
        Ensure.ArgumentNotNullOrWhiteSpace(roleName, nameof(roleName));
        var userRole = await GetUserRoleByNameAsync(roleName);
        UserSpecification specification = new()
        {
            Id = userId,
            IncludeRole = true,
            RoleId = userRole.Id
        };
        var entity = await _userRepository.GetBySpecificationAsync(specification);
        entity = Ensure.EntityExists(entity, "The user was not found");
        return entity.ToResponse();
    }

    public async Task<UserResponse> GetCustomerByIdAsync(long id)
    {
        var customerRole = await GetUserRoleByNameAsync(UserRoles.Customer);
        UserSpecification specification = new()
        {
            Id = id,
            IncludeRole = true,
            RoleId = customerRole.Id
        };
        var entity = await _userRepository.GetBySpecificationAsync(specification);
        entity = Ensure.EntityExists(entity, "The customer was not found");
        return entity.ToResponse();
    }

    public async Task<UserResponse> UpdateAsync(UpdateUserRequest request)
    {
        await ValidateUpdateUserRequestAsync(request);
        var entityToUpdate = await GetUserByIdInternalAsync(request.Id);
        ChangeState(entityToUpdate, request);
        await _unitOfWork.SaveChangesAsync();
        entityToUpdate.UserRole = await GetUserRoleByIdAsync(entityToUpdate.UserRoleId);
        return entityToUpdate.ToResponse();
    }

    public async Task UpdateCustomerPasswordAsync(UpdatePasswordRequest request)
    {
        await ValidateUpdateCustomerPasswordRequestAsync(request);
        await UpdatePasswordAsync(request.UserId, request.NewPassword);
    }
    
    public async Task UpdateCustomerPasswordAsAdminAsync(UpdatePasswordAsAdminRequest request)
    {
        await ValidateUpdateCustomerPasswordAsAdminRequestAsync(request);
        await UpdatePasswordAsync(request.UserId, request.NewPassword);
    }

    public async Task<IsTakenResponse> IsUserNameTakenAsync(string userName)
    {
        Ensure.ArgumentNotNullOrWhiteSpace(userName, nameof(userName));
        var isUnique = await _userRepository.IsUserNameUniqueAsync(userName);
        return new IsTakenResponse(!isUnique);
    }

    public async Task<IsTakenResponse> IsEmailTakenAsync(string email)
    {
        Ensure.ArgumentNotNullOrWhiteSpace(email, nameof(email));
        var isUnique = await _userRepository.IsEmailUniqueAsync(email);
        return new IsTakenResponse(!isUnique);
    }

    public async Task<IsTakenResponse> IsPhoneNumberTakenAsync(string phoneNumber)
    {
        Ensure.ArgumentNotNullOrWhiteSpace(phoneNumber, nameof(phoneNumber));
        var isUnique = await _userRepository.IsPhoneNumberUniqueAsync(phoneNumber);
        return new IsTakenResponse(!isUnique);
    }
    
    private async Task<User> AddAsync(User user, string password)
    {
        var customerRole = await GetUserRoleByNameAsync(UserRoles.Customer);
        user.UserRoleId = customerRole.Id;
        user.PasswordHash = _passwordHasher.Hash(password);
        await _userRepository.AddAsync(user);
        await _unitOfWork.SaveChangesAsync();
        user.UserRole = customerRole;
        return user;
    }

    private static void ChangeState(User existingTrackedEntity, UpdateUserRequest updateUserRequest)
    {
        existingTrackedEntity.Id = updateUserRequest.Id;
        existingTrackedEntity.UserName = updateUserRequest.UserName;
        existingTrackedEntity.Email = updateUserRequest.Email;
        existingTrackedEntity.PhoneNumber = updateUserRequest.PhoneNumber;
        existingTrackedEntity.FirstName = updateUserRequest.FirstName;
    }
    
    private async Task UpdatePasswordAsync(long userId, string newPassword)
    {
        var entityToUpdate = await GetUserWithCustomerRoleByIdAsync(userId);
        entityToUpdate.PasswordHash = _passwordHasher.Hash(newPassword);
        await _unitOfWork.SaveChangesAsync();
    }
    
    private async Task ValidateAddUserRequestAsync(AddUserRequest request)
    {
        Ensure.ArgumentNotNull(request);
        var validationResult = await _validator.ValidateAddUserAsync(request);
        validationResult.ThrowIfValidationFailed();
    }
    
    private async Task ValidateUpdateUserRequestAsync(UpdateUserRequest request)
    {
        Ensure.ArgumentNotNull(request);
        var validationResult = await _validator.ValidateUpdateUserAsync(request);
        validationResult.ThrowIfValidationFailed();
    }
    
    private async Task ValidateUpdateCustomerPasswordRequestAsync(UpdatePasswordRequest request)
    {
        Ensure.ArgumentNotNull(request);
        var validationResult = await _validator.ValidateUpdatePasswordAsync(request);
        validationResult.ThrowIfValidationFailed();
    }
    
    private async Task ValidateUpdateCustomerPasswordAsAdminRequestAsync(UpdatePasswordAsAdminRequest request)
    {
        Ensure.ArgumentNotNull(request);
        var validationResult = await _validator.ValidateUpdatePasswordAsAdminAsync(request);
        validationResult.ThrowIfValidationFailed();
    }
    
    private async Task ValidateDeleteUserRequestAsync(long id)
    {
        DeleteUserRequest request = new(id);
        var validationResult = await _validator.ValidateDeleteUserAsync(request);
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

    private async Task<User> GetUserWithCustomerRoleByIdAsync(long id)
    {
        var customerRole = await GetUserRoleByNameAsync(UserRoles.Customer);
        UserSpecification specification = new()
        {
            Id = id,
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
