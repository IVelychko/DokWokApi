using Application.Abstractions.Messaging;
using Domain.Abstractions.Services;
using Domain.ResultType;

namespace Application.Operations.User.Queries.IsUserPhoneNumberTaken;

public class IsUserPhoneNumberTakenQueryHandler(IUserService userService) : IQueryHandler<IsUserPhoneNumberTakenQuery, Result<bool>>
{
    public async Task<Result<bool>> Handle(IsUserPhoneNumberTakenQuery request, CancellationToken cancellationToken) =>
        await userService.IsPhoneNumberTakenAsync(request.PhoneNumber);
}
