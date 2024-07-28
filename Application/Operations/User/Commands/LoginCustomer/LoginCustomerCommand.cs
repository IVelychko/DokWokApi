using Application.Abstractions.Messaging;
using Domain.Models.User;
using Domain.ResultType;

namespace Application.Operations.User.Commands.LoginCustomer;

public sealed record LoginCustomerCommand(string UserName, string Password) : ICommand<Result<AuthorizedUserModel>>;
