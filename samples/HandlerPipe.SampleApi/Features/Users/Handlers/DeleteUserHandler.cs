using HandlerPipe.Abstractions;

namespace HandlerPipe.SampleApi.Features.Users;

public sealed class DeleteUserHandler(UserStore store)
    : ICommandHandler<DeleteUserCommand>
{
    public Task<Unit> HandleAsync(
        DeleteUserCommand command,
        CancellationToken cancellationToken = default)
    {
        store.Delete(command.Id);
        return Unit.Task;
    }
}
