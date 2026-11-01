using HandlerPipe.Abstractions;

namespace HandlerPipe.SampleApi.Features.Users;

public sealed class GetAllUsersHandler(UserStore store)
    : IQueryHandler<GetAllUsersQuery, IReadOnlyList<UserResponse>>
{
    public Task<IReadOnlyList<UserResponse>> HandleAsync(
        GetAllUsersQuery query,
        CancellationToken cancellationToken = default) =>
        Task.FromResult(store.GetAll());
}
