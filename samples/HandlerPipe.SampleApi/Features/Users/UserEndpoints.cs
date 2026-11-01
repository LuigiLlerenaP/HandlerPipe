using HandlerPipe.Abstractions;

namespace HandlerPipe.SampleApi.Features.Users;

public static class UserEndpoints
{
    public static IEndpointRouteBuilder MapUserEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var users = endpoints.MapGroup("/users").WithTags("Users");

        users.MapGet("/", async (
            IQueryHandler<GetAllUsersQuery, IReadOnlyList<UserResponse>> handler,
            CancellationToken ct) =>
        {
            var result = await handler.HandleAsync(new GetAllUsersQuery(), ct);
            return Results.Ok(result);
        })
        .WithName("GetAllUsers")
        .WithSummary("Lista todos los usuarios")
        .Produces<IReadOnlyList<UserResponse>>();

        users.MapGet("/{id:guid}", async (
            Guid id,
            IQueryHandler<GetUserByIdQuery, UserResponse?> handler,
            CancellationToken ct) =>
        {
            var result = await handler.HandleAsync(new GetUserByIdQuery(id), ct);
            return result is null
                ? Results.NotFound(new { Message = $"Usuario '{id}' no encontrado." })
                : Results.Ok(result);
        })
        .WithName("GetUserById")
        .WithSummary("Obtiene un usuario por Id")
        .Produces<UserResponse>()
        .Produces(StatusCodes.Status404NotFound);

        users.MapPost("/", async (
            CreateUserRequest request,
            ICommandHandler<CreateUserCommand, UserResponse> handler,
            CancellationToken ct) =>
        {
            if (string.IsNullOrWhiteSpace(request.Name))
                return Results.ValidationProblem(new Dictionary<string, string[]>
                {
                    [nameof(request.Name)] = ["El nombre es obligatorio."]
                });

            if (string.IsNullOrWhiteSpace(request.Email) || !request.Email.Contains('@'))
                return Results.ValidationProblem(new Dictionary<string, string[]>
                {
                    [nameof(request.Email)] = ["El email no tiene un formato válido."]
                });

            var command = new CreateUserCommand(request.Name.Trim(), request.Email.Trim().ToLowerInvariant());
            var result = await handler.HandleAsync(command, ct);

            return Results.Created($"/users/{result.Id}", result);
        })
        .WithName("CreateUser")
        .WithSummary("Crea un nuevo usuario")
        .WithDescription("Registra un usuario. Los commands pasan automáticamente por el LoggingCommandHandlerDecorator.")
        .Accepts<CreateUserRequest>("application/json")
        .Produces<UserResponse>(StatusCodes.Status201Created)
        .Produces(StatusCodes.Status400BadRequest);

        users.MapDelete("/{id:guid}", async (
            Guid id,
            ICommandHandler<DeleteUserCommand> handler,
            CancellationToken ct) =>
        {
            await handler.HandleAsync(new DeleteUserCommand(id), ct);
            return Results.NoContent();
        })
        .WithName("DeleteUser")
        .WithSummary("Elimina un usuario")
        .WithDescription("Demuestra ICommand (sin resultado). El handler retorna Unit internamente.")
        .Produces(StatusCodes.Status204NoContent);

        return endpoints;
    }
}
