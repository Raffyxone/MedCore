using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Shared.Results;
using System.Text.RegularExpressions;

namespace Application.Features.User.Domain.ValueObjects;

public partial record Email
{
    public string Value { get; }

    private Email(string value) => Value = value;

    public static Result<Email> Create(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return Result.Failure<Email>(new Error("Email.Empty", "Email is required."));

        if (!EmailRegex().IsMatch(email))
            return Result.Failure<Email>(new Error("Email.Invalid", "Email format is invalid."));

        return Result.Success(new Email(email));
    }

    [GeneratedRegex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$")]
    private static partial Regex EmailRegex();
}
