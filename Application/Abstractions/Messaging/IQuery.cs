using MediatR;

namespace Application.Abstractions.Messaging;

public interface IQuery : IRequest;

public interface IQuery<out TResponse> : IRequest<TResponse>;
