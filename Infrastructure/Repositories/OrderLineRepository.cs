﻿using Domain.Abstractions.Repositories;
using Domain.Entities;
using Domain.Errors;
using Domain.Helpers;
using Domain.Models;
using FluentValidation;
using Infrastructure.Specification;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class OrderLineRepository : IOrderLineRepository
{
    private readonly StoreDbContext _context;

    public OrderLineRepository(StoreDbContext context)
    {
        _context = context;
    }

    public async Task<Result<Unit>> AddAsync(OrderLine entity)
    {
        if (entity is null)
        {
            var error = new ValidationError("orderLine", "The passed entity is null");
            return Result<Unit>.Failure(error);
        }

        await _context.AddAsync(entity);
        return Unit.Default;
    }

    public async Task DeleteByIdAsync(long id)
    {
        var entity = await _context.OrderLines.FirstAsync(ol => ol.Id == id);
        _context.Remove(entity);
    }

    public async Task<IEnumerable<OrderLine>> GetAllBySpecificationAsync(Specification<OrderLine> specification)
    {
        var query = SpecificationEvaluator.ApplySpecification(_context.OrderLines, specification);
        return await query.ToListAsync();
    }

    public async Task<IEnumerable<OrderLine>> GetAllAsync(PageInfo? pageInfo = null)
    {
        IQueryable<OrderLine> query = _context.OrderLines;
        if (pageInfo is not null)
        {
            query = SpecificationEvaluator.ApplyPagination(query, pageInfo);
        }

        return await query.ToListAsync();
    }

    public async Task<IEnumerable<OrderLine>> GetAllByOrderIdAsync(long orderId, PageInfo? pageInfo = null)
    {
        IQueryable<OrderLine> query = _context.OrderLines.Where(ol => ol.OrderId == orderId);
        if (pageInfo is not null)
        {
            query = SpecificationEvaluator.ApplyPagination(query, pageInfo);
        }

        return await query.ToListAsync();
    }

    public async Task<IEnumerable<OrderLine>> GetAllWithDetailsAsync(PageInfo? pageInfo = null)
    {
        IQueryable<OrderLine> query = _context.OrderLines
            .Include(ol => ol.Order)
            .Include(ol => ol.Product)
                .ThenInclude(p => p!.Category);

        if (pageInfo is not null)
        {
            query = SpecificationEvaluator.ApplyPagination(query, pageInfo);
        }

        return await query.ToListAsync();
    }

    public async Task<IEnumerable<OrderLine>> GetAllWithDetailsByOrderIdAsync(long orderId, PageInfo? pageInfo = null)
    {
        IQueryable<OrderLine> query = _context.OrderLines
            .Include(ol => ol.Order)
            .Include(ol => ol.Product)
                .ThenInclude(p => p!.Category)
            .Where(ol => ol.OrderId == orderId);

        if (pageInfo is not null)
        {
            query = SpecificationEvaluator.ApplyPagination(query, pageInfo);
        }

        return await query.ToListAsync();
    }

    public async Task<OrderLine?> GetByIdAsync(long id)
    {
        return await _context.OrderLines.FirstOrDefaultAsync(ol => ol.Id == id);
    }

    public async Task<OrderLine?> GetByIdWithDetailsAsync(long id)
    {
        return await _context.OrderLines
            .Include(ol => ol.Order)
            .Include(ol => ol.Product)
                .ThenInclude(p => p!.Category)
            .FirstOrDefaultAsync(ol => ol.Id == id);
    }

    public async Task<OrderLine?> GetByOrderAndProductIdsAsync(long orderId, long productId)
    {
        return await _context.OrderLines
            .FirstOrDefaultAsync(ol => ol.OrderId == orderId && ol.ProductId == productId);
    }

    public async Task<OrderLine?> GetByOrderAndProductIdsWithDetailsAsync(long orderId, long productId)
    {
        return await _context.OrderLines
            .Include(ol => ol.Order)
            .Include(ol => ol.Product)
                .ThenInclude(p => p!.Category)
            .FirstOrDefaultAsync(ol => ol.OrderId == orderId && ol.ProductId == productId);
    }

    public Result<Unit> Update(OrderLine entity)
    {
        if (entity is null)
        {
            var error = new ValidationError("orderLine", "The passed entity is null");
            return Result<Unit>.Failure(error);
        }

        _context.Update(entity);
        return Unit.Default;
    }
}
