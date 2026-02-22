using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Application.Features.User.Domain.ValueObjects;

namespace Application.Features.User.Domain.Entities;

public class User
{
    public Guid Id { get; private set; }
    public Email Email { get; private set; }
    public string PasswordHash { get; private set; }
    public bool IsEmailConfirmed { get; private set; }
    public string? VerificationToken { get; private set; }
    public DateTime? VerificationTokenExpires { get; private set; }

    private User() { }

    public static User Create(Email email, string passwordHash, string verificationToken, DateTime expires)
    {
        return new User
        {
            Id = Guid.NewGuid(),
            Email = email,
            PasswordHash = passwordHash,
            IsEmailConfirmed = false,
            VerificationToken = verificationToken,
            VerificationTokenExpires = expires
        };
    }

    public void ConfirmEmail()
    {
        IsEmailConfirmed = true;
        VerificationToken = null;
        VerificationTokenExpires = null;
    }
}
