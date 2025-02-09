using Domain.Abstractions.Messaging;
using Domain.Abstractions.Services;
using Domain.DTOs.Queries.Users;
using Domain.DTOs.Responses;
using Domain.Shared;

namespace Application.Operations.User.Queries.IsUserNameTaken;

public class IsUserNameTakenQueryHandler(IUserService userService) : IQueryHandler<IsUserNameTakenQuery, Result<IsTakenResponse>>
{
    public async Task<Result<IsTakenResponse>> Handle(IsUserNameTakenQuery request, CancellationToken cancellationToken)
    {
        var result = await userService.IsUserNameTakenAsync(request.UserName);
        Result<IsTakenResponse> isTakenResponseResult = result.Match(isTaken => new IsTakenResponse(isTaken),
            Result<IsTakenResponse>.Failure);

        return isTakenResponseResult;
    }
}
