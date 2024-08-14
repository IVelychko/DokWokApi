using Application.Abstractions.Messaging;

namespace Application.Operations.User.Queries.GetAllCustomersByPage;

public sealed record GetAllCustomersByPageQuery(int PageNumber, int PageSize) : IQuery<IEnumerable<UserResponse>>;
