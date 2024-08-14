﻿using Domain.Models;
using Domain.ResultType;

namespace Domain.Abstractions.Services;

public interface IProductService : ICrud<ProductModel>
{
    Task<IEnumerable<ProductModel>> GetAllByCategoryIdAsync(long categoryId);

    Task<IEnumerable<ProductModel>> GetAllByCategoryIdAndPageAsync(long categoryId, int pageNumber, int pageSize);

    Task<Result<bool>> IsNameTakenAsync(string name);
}
