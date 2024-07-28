using Application.Abstractions.Messaging;
using Domain.Abstractions.Services;
using Domain.ResultType;

namespace Application.Operations.User.Queries.IsUserNameTaken;

public class IsUserNameTakenQueryHandler(IUserService userService) : IQueryHandler<IsUserNameTakenQuery, Result<bool>>
{
    public async Task<Result<bool>> Handle(IsUserNameTakenQuery request, CancellationToken cancellationToken) =>
        await userService.IsUserNameTakenAsync(request.UserName);
}
