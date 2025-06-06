﻿namespace Domain.DTOs.Responses.Products;

public class ProductResponse : BaseResponse
{
    public required string Name { get; set; }

    public required decimal Price { get; set; }

    public required decimal Weight { get; set; }

    public required string MeasurementUnit { get; set; }

    public required string Description { get; set; }

    public required long CategoryId { get; set; }

    public required string CategoryName { get; set; }
}
