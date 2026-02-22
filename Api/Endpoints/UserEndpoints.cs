using Application.Features.User.Dtos;
using Application.Features.User.Handlers.Commands;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Api.Endpoints;

public static class UserEndpoints
{
    public static void MapUserEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("api/users");

        group.MapPost("register", async (RegisterUserRequest request, ISender sender) =>
        {
            var command = new RegisterUserCommand(request.Email, request.Password);
            var result = await sender.Send(command);

            if (result.IsFailure)
            {
                return Results.BadRequest(result.Error);
            }

            return Results.Ok(new { Id = result.Value });
        });

        group.MapGet("verify", async ([FromQuery] string token, ISender sender) =>
        {
            var command = new VerifyUserCommand(token);
            var result = await sender.Send(command);

            if (result.IsFailure)
            {
                return Results.BadRequest(result.Error);
            }

            return Results.Ok(new { Message = "Email verified successfully." });
        });
    }
}
