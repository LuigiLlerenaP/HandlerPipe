using HandlerPipe.Abstractions;

namespace HandlerPipe.SampleApi.Features.Users;

public sealed record DeleteUserCommand(Guid Id) : ICommand;
