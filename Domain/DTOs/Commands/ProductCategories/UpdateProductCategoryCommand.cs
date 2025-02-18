using Domain.Abstractions.Messaging;
using Domain.DTOs.Responses.ProductCategories;

namespace Domain.DTOs.Commands.ProductCategories;

public sealed record UpdateProductCategoryCommand(long Id, string Name) 
    : ICommand<ProductCategoryResponse>;
