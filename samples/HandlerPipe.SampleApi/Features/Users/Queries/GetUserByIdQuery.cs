using HandlerPipe.Abstractions;

namespace HandlerPipe.SampleApi.Features.Users;

public sealed record GetUserByIdQuery(Guid Id) : IQuery<UserResponse?>;
