using Domain.Abstractions.Messaging;
using Domain.DTOs.Responses.ProductCategories;

namespace Domain.DTOs.Commands.ProductCategories;

public sealed record AddProductCategoryCommand(string Name) : ICommand<ProductCategoryResponse>;
