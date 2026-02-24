using Application.Features.User.Dtos;
using Application.Features.User.Handlers.Commands;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Api.Endpoints;

public static class UserEndpoints
{
    public static void MapUserEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("api/users").WithTags("Auth");

        group.MapPost("register", async (RegisterUserRequest request, ISender sender) =>
        {
            var command = new RegisterUserCommand(
                request.FirstName,
                request.LastName,
                request.Email,
                request.Password,
                request.ConfirmPassword,
                request.UserType);

            var result = await sender.Send(command);

            if (result.IsFailure)
            {
                return Results.BadRequest(result.Error);
            }

            return Results.Ok(new { Id = result.Value, Message = "Registration successful. Please check your email to verify your account." });
        })
        .WithName("RegisterUser")
        .Produces(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status400BadRequest);

        group.MapGet("verify", async ([FromQuery] string token, ISender sender, IConfiguration configuration) =>
        {
            var command = new VerifyUserCommand(token);
            var result = await sender.Send(command);

            var frontendUrl = configuration.GetValue<string>("FrontendUrl") ?? "http://localhost:4200";

            if (result.IsFailure)
            {
                return Results.Redirect($"{frontendUrl}/login?verified=false&error=invalid_token");
            }

            return Results.Redirect($"{frontendUrl}/login?verified=true");
        })
        .WithName("VerifyUserEmail")
        .Produces(StatusCodes.Status302Found);
    }
}
