namespace HandlerPipe.SampleApi.Features.Users;

public sealed class UserStore
{
    private readonly Dictionary<Guid, UserResponse> _users = [];

    public UserResponse Add(string name, string email)
    {
        var user = new UserResponse(Guid.NewGuid(), name, email, DateTimeOffset.UtcNow);
        _users[user.Id] = user;
        return user;
    }

    public bool Delete(Guid id) => _users.Remove(id);

    public UserResponse? FindById(Guid id) =>
        _users.TryGetValue(id, out var user) ? user : null;

    public IReadOnlyList<UserResponse> GetAll() =>
        [.. _users.Values.OrderBy(u => u.CreatedAt)];
}
