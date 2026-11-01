using HandlerPipe.Abstractions;

namespace HandlerPipe.SampleApi.Features.Users;

public sealed record GetAllUsersQuery() : IQuery<IReadOnlyList<UserResponse>>;
