using DokWokApi.Endpoints;

namespace DokWokApi.Extensions;

public static class EndpointRouterExtensions
{
    public static void AddAllEndpoints(this IEndpointRouteBuilder app)
    {
        app.AddCategoriesEndpoints();
        app.AddOrderLinesEndpoints();
        app.AddOrdersEndpoints();
        app.AddProductsEndpoints();
        app.AddShopsEndpoints();
        app.AddUsersEndpoints();
    }
}
