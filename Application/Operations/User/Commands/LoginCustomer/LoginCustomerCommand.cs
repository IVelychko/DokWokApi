using Application.Abstractions.Messaging;
using Domain.ResultType;

namespace Application.Operations.User.Commands.LoginCustomer;

public sealed record LoginCustomerCommand(string UserName, string Password) : ICommand<Result<AuthorizedUserResponse>>;
