using MediatR;

namespace Domain.Abstractions.Messaging;

public interface IQuery : IRequest;

public interface IQuery<out TResponse> : IRequest<TResponse>;
