using HandlerPipe.Abstractions;

namespace HandlerPipe.SampleApi.Features.Users;

public sealed class CreateUserHandler(UserStore store)
    : ICommandHandler<CreateUserCommand, UserResponse>
{
    public Task<UserResponse> HandleAsync(
        CreateUserCommand command,
        CancellationToken cancellationToken = default)
    {
        var user = store.Add(command.Name, command.Email);
        return Task.FromResult(user);
    }
}
