﻿using Application.Abstractions.Messaging;
using Domain.ResultType;

namespace Application.Operations.User.Commands.LoginAdmin;

public sealed record LoginAdminCommand(string UserName, string Password) : ICommand<Result<AuthorizedUserResponse>>;
