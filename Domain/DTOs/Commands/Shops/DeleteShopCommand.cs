using Domain.Abstractions.Messaging;

namespace Domain.DTOs.Commands.Shops;

public sealed record DeleteShopCommand(long Id) : ICommand;
