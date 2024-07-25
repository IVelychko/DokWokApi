using DokWokApi.DAL.Entities;
using DokWokApi.DAL.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace DokWokApi.DAL.Validation;

public class OrderRepositoryValidator : IValidator<Order>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly StoreDbContext _context;

    public OrderRepositoryValidator(UserManager<ApplicationUser> userManager, StoreDbContext context)
    {
        _userManager = userManager;
        _context = context;
    }

    public async Task<ValidationResult> ValidateAddAsync(Order? model)
    {
        ValidationResult result = new()
        {
            IsValid = true,
            IsFound = true,
        };
        if (model is null)
        {
            result.IsValid = false;
            result.Error = "The passed order is null.";
            return result;
        }

        if (model.UserId is not null && !await _userManager.Users.AsNoTracking().AnyAsync(u => u.Id == model.UserId))
        {
            result.IsValid = false;
            result.IsFound = false;
            result.Error = "There is no user with the ID specified in the UserId property of the Order entity.";
            return result;
        }

        if (model.ShopId is not null && !await _context.Shops.AsNoTracking().AnyAsync(s => s.Id == model.ShopId))
        {
            result.IsValid = false;
            result.IsFound = false;
            result.Error = "There is no shop with the ID specified in the ShopId property of the Order entity.";
            return result;
        }

        return result;
    }

    public async Task<ValidationResult> ValidateUpdateAsync(Order? model)
    {
        ValidationResult result = new()
        {
            IsValid = true,
            IsFound = true,
        };
        if (model is null)
        {
            result.IsValid = false;
            result.Error = "The passed order is null.";
            return result;
        }

        var entityToUpdate = await _context.Orders.AsNoTracking().FirstOrDefaultAsync(o => o.Id == model.Id);
        if (entityToUpdate is null)
        {
            result.IsValid = false;
            result.IsFound = false;
            result.Error = "There is no order with this ID in the database.";
            return result;
        }

        if (model.UserId is not null && !await _userManager.Users.AsNoTracking().AnyAsync(u => u.Id == model.UserId))
        {
            result.IsValid = false;
            result.IsFound = false;
            result.Error = "There is no user with the ID specified in the UserId property of the Order entity.";
            return result;
        }

        if (model.ShopId is not null && !await _context.Shops.AsNoTracking().AnyAsync(s => s.Id == model.ShopId))
        {
            result.IsValid = false;
            result.IsFound = false;
            result.Error = "There is no shop with the ID specified in the ShopId property of the Order entity.";
            return result;
        }

        return result;
    }
}
