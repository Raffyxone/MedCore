using Application.Features.User.Domain;
using MediatR;
using Shared.Results;

namespace Application.Features.User.Handlers.Commands;

public class VerifyUserCommandHandler : IRequestHandler<VerifyUserCommand, Result>
{
    private readonly IUserRepository _userRepository;

    public VerifyUserCommandHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<Result> Handle(VerifyUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByVerificationTokenAsync(request.Token, cancellationToken);

        if (user is null)
        {
            return Result.Failure(new Error("User.InvalidToken", "The verification token is invalid."));
        }

        if (user.VerificationTokenExpires < DateTime.UtcNow)
        {
            return Result.Failure(new Error("User.ExpiredToken", "The verification token has expired."));
        }

        user.ConfirmEmail();

        await _userRepository.UpdateAsync(user, cancellationToken);

        return Result.Success();
    }
}
