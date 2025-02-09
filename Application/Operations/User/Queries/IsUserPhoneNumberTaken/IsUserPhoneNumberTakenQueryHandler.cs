using Domain.Abstractions.Messaging;
using Domain.Abstractions.Services;
using Domain.DTOs.Queries.Users;
using Domain.DTOs.Responses;
using Domain.Shared;

namespace Application.Operations.User.Queries.IsUserPhoneNumberTaken;

public class IsUserPhoneNumberTakenQueryHandler(IUserService userService)
    : IQueryHandler<IsUserPhoneNumberTakenQuery, Result<IsTakenResponse>>
{
    public async Task<Result<IsTakenResponse>> Handle(IsUserPhoneNumberTakenQuery request, CancellationToken cancellationToken)
    {
        var result = await userService.IsPhoneNumberTakenAsync(request.PhoneNumber);
        Result<IsTakenResponse> isTakenResponseResult = result.Match(isTaken => new IsTakenResponse(isTaken),
            Result<IsTakenResponse>.Failure);

        return isTakenResponseResult;
    }
}
