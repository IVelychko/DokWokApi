using Domain.Abstractions.Messaging;
using Domain.Abstractions.Services;
using Domain.DTOs.Queries.Users;
using Domain.DTOs.Responses;

namespace Application.Operations.Users.Queries.IsUserPhoneNumberTaken;

public class IsUserPhoneNumberTakenQueryHandler(IUserService userService)
    : IQueryHandler<IsUserPhoneNumberTakenQuery, IsTakenResponse>
{
    public async Task<IsTakenResponse> Handle(IsUserPhoneNumberTakenQuery request, CancellationToken cancellationToken)
    {
        var isUnique = await userService.IsPhoneNumberUniqueAsync(request.PhoneNumber);
        return new IsTakenResponse(!isUnique);
    }
}
