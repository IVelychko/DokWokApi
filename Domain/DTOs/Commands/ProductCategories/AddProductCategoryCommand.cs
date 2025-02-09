﻿using Domain.Abstractions.Messaging;
using Domain.DTOs.Responses.ProductCategories;
using Domain.Shared;

namespace Domain.DTOs.Commands.ProductCategories;

public sealed record AddProductCategoryCommand(string Name) : ICommand<Result<ProductCategoryResponse>>;
