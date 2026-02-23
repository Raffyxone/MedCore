using Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Shared.Results;

namespace Application.Features.User.Handlers.Commands;

public sealed class VerifyUserCommandHandler : IRequestHandler<VerifyUserCommand, Result>
{
    private readonly IApplicationDbContext _context;

    public VerifyUserCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result> Handle(VerifyUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.VerificationToken == request.Token, cancellationToken);

        if (user is null)
        {
            return Result.Failure(new Error("User.InvalidToken", "The verification token is invalid."));
        }

        if (user.VerificationTokenExpires < DateTime.UtcNow)
        {
            return Result.Failure(new Error("User.ExpiredToken", "The verification token has expired."));
        }

        user.IsEmailConfirmed = true;
        user.VerificationToken = null;
        user.VerificationTokenExpires = null;

        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
