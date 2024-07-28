using Application.Abstractions.Messaging;

namespace Application.Operations.Shop.Commands.DeleteShop;

public sealed record DeleteShopCommand(long Id) : ICommand<bool?>;
