using HandlerPipe.Abstractions;

namespace HandlerPipe.SampleApi.Features.Users;

public sealed class GetUserByIdHandler(UserStore store)
    : IQueryHandler<GetUserByIdQuery, UserResponse?>
{
    public Task<UserResponse?> HandleAsync(
        GetUserByIdQuery query,
        CancellationToken cancellationToken = default) =>
        Task.FromResult(store.FindById(query.Id));
}
