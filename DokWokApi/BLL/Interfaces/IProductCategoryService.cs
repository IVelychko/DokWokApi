﻿using DokWokApi.BLL.Models.ProductCategory;

namespace DokWokApi.BLL.Interfaces;

public interface IProductCategoryService : ICrud<ProductCategoryModel>
{
    Task<bool> IsNameTaken(string name);
}
