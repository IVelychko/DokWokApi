﻿using AutoMapper;
using DokWokApi.BLL.Interfaces;
using DokWokApi.BLL.Models.Product;
using DokWokApi.DAL.Entities;
using DokWokApi.DAL.Interfaces;
using DokWokApi.Exceptions;
using LanguageExt.Common;
using Microsoft.EntityFrameworkCore;

namespace DokWokApi.BLL.Services;

public class ProductService : IProductService
{
    private readonly IProductRepository _repository;
    private readonly IMapper _mapper;

    public ProductService(IProductRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<Result<ProductModel>> AddAsync(ProductModel model)
    {
        if (model is null)
        {
            var exception = new ValidationException("The passed model is null.");
            return new Result<ProductModel>(exception);
        }

        var entity = _mapper.Map<Product>(model);
        var result = await _repository.AddAsync(entity);

        return result.Match(p => _mapper.Map<ProductModel>(p),
            e => new Result<ProductModel>(e));
    }

    public async Task<bool?> DeleteAsync(long id)
    {
        return await _repository.DeleteByIdAsync(id);
    }

    public async Task<IEnumerable<ProductModel>> GetAllAsync()
    {
        var queryable = _repository.GetAllWithDetails().OrderBy(p => p.Id);
        var entities = await queryable.ToListAsync();
        var models = _mapper.Map<IEnumerable<ProductModel>>(entities);
        return models;
    }

    public async Task<IEnumerable<ProductModel>> GetAllByCategoryIdAsync(long categoryId)
    {
        var entities = _repository.GetAllWithDetails();
        var filteredEntities = entities.Where(p => p.CategoryId == categoryId);
        var list = await filteredEntities.ToListAsync();
        var models = _mapper.Map<IEnumerable<ProductModel>>(list);
        return models;
    }

    public async Task<ProductModel?> GetByIdAsync(long id)
    {
        var entity = await _repository.GetByIdWithDetailsAsync(id);
        if (entity is null)
        {
            return null;
        }

        var model = _mapper.Map<ProductModel>(entity);
        return model;
    }

    public async Task<Result<ProductModel>> UpdateAsync(ProductModel model)
    {
        if (model is null)
        {
            var exception = new ValidationException("The passed model is null.");
            return new Result<ProductModel>(exception);
        }

        var entity = _mapper.Map<Product>(model);
        var result = await _repository.UpdateAsync(entity);

        return result.Match(p => _mapper.Map<ProductModel>(p),
            e => new Result<ProductModel>(e));
    }

    public async Task<Result<bool>> IsNameTaken(string name)
    {
        if (name is null)
        {
            var exception = new ValidationException("The passed name is null");
            return new Result<bool>(exception);
        }

        var product = await _repository.GetAll().FirstOrDefaultAsync(p => p.Name == name);
        return product is not null;
    }
}
