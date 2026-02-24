using MediatR;
using Shared.Entities;
using Shared.Results;

namespace Application.Features.User.Handlers.Commands;

public sealed record RegisterUserCommand(
    string FirstName,
    string LastName,
    string Email,
    string Password,
    string ConfirmPassword,
    UserType UserType
) : IRequest<Result<Guid>>;
