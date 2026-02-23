using MediatR;
using Shared.Results;

namespace Application.Features.User.Handlers.Commands;

public sealed record RegisterUserCommand(string Email, string Password) : IRequest<Result<Guid>>;
