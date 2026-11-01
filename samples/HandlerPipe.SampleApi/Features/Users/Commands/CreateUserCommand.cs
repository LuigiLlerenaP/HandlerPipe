using HandlerPipe.Abstractions;

namespace HandlerPipe.SampleApi.Features.Users;

public sealed record CreateUserCommand(string Name, string Email) : ICommand<UserResponse>;
