using Application.Features.User.Domain;
using Application.Features.User.Domain.ValueObjects;
using Application.Services;
using MediatR;
using Shared.Results;
using UserEntity = Application.Features.User.Domain.Entities.User;

namespace Application.Features.User.Handlers.Commands;

public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, Result<Guid>>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IEmailService _emailService;

    public RegisterUserCommandHandler(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        IEmailService emailService)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _emailService = emailService;
    }

    public async Task<Result<Guid>> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        var emailResult = Email.Create(request.Email);

        if (emailResult.IsFailure)
        {
            return Result.Failure<Guid>(emailResult.Error);
        }

        bool emailExists = await _userRepository.EmailExistsAsync(emailResult.Value.Value, cancellationToken);

        if (emailExists)
        {
            return Result.Failure<Guid>(new Error("User.DuplicateEmail", "The email is already in use."));
        }

        string hashedPassword = _passwordHasher.Hash(request.Password);
        string verificationToken = Guid.NewGuid().ToString("N");
        DateTime tokenExpiration = DateTime.UtcNow.AddHours(24);

        var user = UserEntity.Create(
            emailResult.Value,
            hashedPassword,
            verificationToken,
            tokenExpiration);

        await _userRepository.AddAsync(user, cancellationToken);

        await _emailService.SendVerificationEmailAsync(user.Email.Value, verificationToken, cancellationToken);

        return Result.Success(user.Id);
    }
}
