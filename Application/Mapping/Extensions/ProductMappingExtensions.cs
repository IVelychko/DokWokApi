using Application.Operations.Product;
using Application.Operations.Product.Commands.AddProduct;
using Application.Operations.Product.Commands.UpdateProduct;
using Domain.DTOs.Commands.Products;
using Domain.DTOs.Requests.Products;
using Domain.DTOs.Responses.Products;
using Domain.Entities;
using Domain.Models;

namespace Application.Mapping.Extensions;

public static class ProductMappingExtensions
{
    public static AddProductCommand ToCommand(this AddProductRequest request) =>
        new(request.Name, request.Price, request.Weight, request.MeasurementUnit, request.Description, request.CategoryId);

    public static UpdateProductCommand ToCommand(this UpdateProductRequest request) =>
        new(request.Id, request.Name, request.Price, request.Weight, request.MeasurementUnit, request.Description, request.CategoryId);

    // public static ProductModel ToModel(this AddProductCommand command)
    // {
    //     return new()
    //     {
    //         Name = command.Name,
    //         CategoryId = command.CategoryId,
    //         Description = command.Description,
    //         Price = command.Price,
    //         Weight = command.Weight,
    //         MeasurementUnit = command.MeasurementUnit,
    //     };
    // }
    
    public static Product ToEntity(this AddProductCommand command)
    {
        return new Product
        {
            Name = command.Name,
            CategoryId = command.CategoryId,
            Description = command.Description,
            Price = command.Price,
            Weight = command.Weight,
            MeasurementUnit = command.MeasurementUnit,
        };
    }

    // public static ProductModel ToModel(this UpdateProductCommand command)
    // {
    //     return new()
    //     {
    //         Id = command.Id,
    //         Name = command.Name,
    //         CategoryId = command.CategoryId,
    //         Description = command.Description,
    //         Price = command.Price,
    //         Weight = command.Weight,
    //         MeasurementUnit = command.MeasurementUnit
    //     };
    // }
    
    public static Product ToEntity(this UpdateProductCommand command)
    {
        return new Product
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

    // public static ProductResponse ToResponse(this ProductModel model)
    // {
    //     return new()
    //     {
    //         Id = model.Id,
    //         CategoryId = model.CategoryId,
    //         CategoryName = model.CategoryName,
    //         Description = model.Description,
    //         MeasurementUnit = model.MeasurementUnit,
    //         Name = model.Name,
    //         Price = model.Price,
    //         Weight = model.Weight
    //     };
    // }
    
    public static ProductResponse ToResponse(this Product entity)
    {
        return new ProductResponse
        {
            Id = entity.Id,
            CategoryId = entity.CategoryId,
            CategoryName = entity.Category?.Name ?? string.Empty,
            Description = entity.Description,
            MeasurementUnit = entity.MeasurementUnit,
            Name = entity.Name,
            Price = entity.Price,
            Weight = entity.Weight
        };
    }
}
