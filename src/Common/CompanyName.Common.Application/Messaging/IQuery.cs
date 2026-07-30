using CompanyName.Common.Domain;
using MediatR;

namespace CompanyName.Common.Application.Messaging;

public interface IQuery<TResponse> : IRequest<Result<TResponse>>;