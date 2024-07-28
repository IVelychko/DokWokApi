using Application.Abstractions.Messaging;
using Domain.Abstractions.Services;

namespace Application.Operations.ProductCategory.Commands.DeleteProductCategory;

public class DeleteProductCategoryCommandHandler(IProductCategoryService productCategoryService)
    : ICommandHandler<DeleteProductCategoryCommand, bool?>
{
    public async Task<bool?> Handle(DeleteProductCategoryCommand request, CancellationToken cancellationToken) =>
        await productCategoryService.DeleteAsync(request.Id);
}
