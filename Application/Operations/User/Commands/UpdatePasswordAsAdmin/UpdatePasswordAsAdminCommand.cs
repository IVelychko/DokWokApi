using Application.Abstractions.Messaging;
using Domain.Helpers;

namespace Application.Operations.User.Commands.UpdatePasswordAsAdmin;

public sealed record UpdatePasswordAsAdminCommand(long UserId, string NewPassword) : ICommand<Result<Unit>>;
