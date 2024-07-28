﻿using Application.Abstractions.Messaging;
using Application.Mapping.Extensions;
using Domain.Abstractions.Services;
using Domain.Models.User;
using Domain.ResultType;

namespace Application.Operations.User.Commands.UpdateUser;

public class UpdateUserCommandHandler(IUserService userService) : ICommandHandler<UpdateUserCommand, Result<UserModel>>
{
    public async Task<Result<UserModel>> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        var model = request.ToModel();
        var result = await userService.UpdateAsync(model);
        return result;
    }
}
