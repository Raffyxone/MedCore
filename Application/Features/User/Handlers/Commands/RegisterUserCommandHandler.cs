using Application.Interfaces;
using Application.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Shared.Entities;
using Shared.Results;
using System.Text.RegularExpressions;

namespace Application.Features.User.Handlers.Commands;

public sealed class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, Result<Guid>>
{
    private readonly IApplicationDbContext _context;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IEmailService _emailService;

    public RegisterUserCommandHandler(
        IApplicationDbContext context,
        IPasswordHasher passwordHasher,
        IEmailService emailService)
    {
        _context = context;
        _passwordHasher = passwordHasher;
        _emailService = emailService;
    }

    public async Task<Result<Guid>> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Email) || !Regex.IsMatch(request.Email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
        {
            return Result.Failure<Guid>(new Error("User.InvalidEmail", "The email format is invalid."));
        }

        bool emailExists = await _context.Users.AnyAsync(u => u.Email == request.Email, cancellationToken);

        if (emailExists)
        {
            return Result.Failure<Guid>(new Error("User.DuplicateEmail", "The email is already in use."));
        }

        string hashedPassword = _passwordHasher.Hash(request.Password);
        string verificationToken = Guid.NewGuid().ToString("N");

        var user = new Shared.Entities.User
        {
            Id = Guid.NewGuid(),
            Email = request.Email,
            PasswordHash = hashedPassword,
            IsEmailConfirmed = false,
            VerificationToken = verificationToken,
            VerificationTokenExpires = DateTime.UtcNow.AddHours(24)
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync(cancellationToken);

        string apiBaseUrl = "https://localhost:7018";
        string verificationLink = $"{apiBaseUrl}/api/users/verify?token={verificationToken}";

        await _emailService.SendVerificationEmailAsync(user.Email, verificationLink, cancellationToken);

        return Result.Success(user.Id);
    }
}
