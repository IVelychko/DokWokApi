using Application.Abstractions.Messaging;
using Domain.Abstractions.Services;
using Domain.ResultType;

namespace Application.Operations.User.Queries.IsUserEmailTaken;

public class IsUserEmailTakenQueryHandler(IUserService userService) : IQueryHandler<IsUserEmailTakenQuery, Result<bool>>
{
    public async Task<Result<bool>> Handle(IsUserEmailTakenQuery request, CancellationToken cancellationToken) =>
        await userService.IsEmailTakenAsync(request.Email);
}
