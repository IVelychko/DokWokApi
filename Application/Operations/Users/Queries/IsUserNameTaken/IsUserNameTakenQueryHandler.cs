using Domain.Abstractions.Messaging;
using Domain.Abstractions.Services;
using Domain.DTOs.Queries.Users;
using Domain.DTOs.Responses;

namespace Application.Operations.Users.Queries.IsUserNameTaken;

public class IsUserNameTakenQueryHandler(IUserService userService) 
    : IQueryHandler<IsUserNameTakenQuery, IsTakenResponse>
{
    public async Task<IsTakenResponse> Handle(IsUserNameTakenQuery request, CancellationToken cancellationToken)
    {
        var isUnique = await userService.IsUserNameUniqueAsync(request.UserName);
        return new IsTakenResponse(!isUnique);
    }
}
