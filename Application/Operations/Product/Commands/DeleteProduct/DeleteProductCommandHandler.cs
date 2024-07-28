using Application.Abstractions.Messaging;
using Domain.Abstractions.Services;

namespace Application.Operations.Product.Commands.DeleteProduct;

public class DeleteProductCommandHandler(IProductService productService) : ICommandHandler<DeleteProductCommand, bool?>
{
    public async Task<bool?> Handle(DeleteProductCommand request, CancellationToken cancellationToken) =>
        await productService.DeleteAsync(request.Id);
}
