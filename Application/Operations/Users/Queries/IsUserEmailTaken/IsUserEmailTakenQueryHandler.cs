using Domain.Abstractions.Messaging;
using Domain.Abstractions.Services;
using Domain.DTOs.Queries.Users;
using Domain.DTOs.Responses;

namespace Application.Operations.Users.Queries.IsUserEmailTaken;

public class IsUserEmailTakenQueryHandler(IUserService userService) 
    : IQueryHandler<IsUserEmailTakenQuery, IsTakenResponse>
{
    public async Task<IsTakenResponse> Handle(IsUserEmailTakenQuery request, CancellationToken cancellationToken)
    {
        var isUnique = await userService.IsEmailUniqueAsync(request.Email);
        return new IsTakenResponse(!isUnique);
    }
}
