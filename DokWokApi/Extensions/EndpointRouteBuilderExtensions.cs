using DokWokApi.Endpoints;

namespace DokWokApi.Extensions;

public static class EndpointRouteBuilderExtensions
{
    public static void MapEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapShopsEndpoints();
        app.MapProductsEndpoints();
        app.MapProductCategoriesEndpoints();
        app.MapUsersEndpoints();
        app.MapOrdersEndpoints();
        app.MapOrderLinesEndpoints();
    }
}
