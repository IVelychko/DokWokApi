using Application.Abstractions.Messaging;
using Domain.Abstractions.Services;
using Domain.Helpers;

namespace Application.Operations.User.Queries.IsUserEmailTaken;

public class IsUserEmailTakenQueryHandler(IUserService userService) : IQueryHandler<IsUserEmailTakenQuery, Result<IsTakenResponse>>
{
    public async Task<Result<IsTakenResponse>> Handle(IsUserEmailTakenQuery request, CancellationToken cancellationToken)
    {
        var result = await userService.IsEmailTakenAsync(request.Email);
        Result<IsTakenResponse> isTakenResponseResult = result.Match(isTaken => new IsTakenResponse(isTaken),
            Result<IsTakenResponse>.Failure);

        return isTakenResponseResult;
    }
}
