using Application.Operations.Product;
using Application.Operations.Product.Commands.AddProduct;
using Application.Operations.Product.Commands.UpdateProduct;
using Domain.Models;

namespace Application.Mapping.Extensions;

public static class ProductMappingExtensions
{
    public static AddProductCommand ToCommand(this AddProductRequest request) =>
        new(request.Name, request.Price, request.Weight, request.MeasurementUnit, request.Description, request.CategoryId);

    public static UpdateProductCommand ToCommand(this UpdateProductRequest request) =>
        new(request.Id, request.Name, request.Price, request.Weight, request.MeasurementUnit, request.Description, request.CategoryId);

    public static ProductModel ToModel(this AddProductCommand command)
    {
        return new()
        {
            Name = command.Name,
            CategoryId = command.CategoryId,
            Description = command.Description,
            Price = command.Price,
            Weight = command.Weight,
            MeasurementUnit = command.MeasurementUnit,
        };
    }

    public static ProductModel ToModel(this UpdateProductCommand command)
    {
        return new()
        {
            Id = command.Id,
            Name = command.Name,
            CategoryId = command.CategoryId,
            Description = command.Description,
            Price = command.Price,
            Weight = command.Weight,
            MeasurementUnit = command.MeasurementUnit
        };
    }

    public static ProductResponse ToResponse(this ProductModel model)
    {
        return new()
        {
            Id = model.Id,
            CategoryId = model.CategoryId,
            CategoryName = model.CategoryName,
            Description = model.Description,
            MeasurementUnit = model.MeasurementUnit,
            Name = model.Name,
            Price = model.Price,
            Weight = model.Weight
        };
    }
}
