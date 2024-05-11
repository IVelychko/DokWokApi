using AutoMapper;
using DokWokApi.BLL.Models;
using DokWokApi.BLL.Models.Post;
using DokWokApi.BLL.Models.Put;
using DokWokApi.DAL.Entities;

namespace DokWokApi.BLL;

public class AutomapperProfile : Profile
{
    public AutomapperProfile()
    {
        CreateMap<ProductCategory, ProductCategoryModel>()
            .ReverseMap();

        CreateMap<Product, ProductModel>()
            .ForMember(pm => pm.CategoryName, opt => opt.MapFrom(p => p.Category != null ? p.Category.Name : string.Empty));

        CreateMap<ProductModel, Product>();

        CreateMap<ProductCategoryModel, ProductCategoryPostModel>()
            .ReverseMap();

        CreateMap<ProductModel, ProductPostModel>()
            .ReverseMap();

        CreateMap<ProductCategoryModel, ProductCategoryPutModel>()
            .ReverseMap();

        CreateMap<ProductModel, ProductPutModel>()
            .ReverseMap();
    }
}
