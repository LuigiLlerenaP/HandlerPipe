using HandlerPipe.Abstractions;
using HandlerPipe.Decorators;
using HandlerPipe.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xunit;

namespace HandlerPipe.Tests;

public class HandlerPipeExtensionsTests
{
    [Fact]
    public async Task AddHandlerPipe_Registers_CommandHandler_And_Decorator_Wraps_Execution()
    {
        var services = new ServiceCollection();

        services.AddLogging();
        services.AddHandlerPipe(typeof(TestCommandHandler).Assembly);
        services.AddCommandDecorator(typeof(LoggingCommandHandlerDecorator<,>));

        using var provider = services.BuildServiceProvider();
        using var scope = provider.CreateScope();

        var handler = scope.ServiceProvider.GetRequiredService<ICommandHandler<TestCommand, string>>();
        var result = await handler.HandleAsync(new TestCommand("ok"));

        Assert.Equal("ok", result);
    }

    public sealed record TestCommand(string Value) : ICommand<string>;

    public sealed class TestCommandHandler : ICommandHandler<TestCommand, string>
    {
        public Task<string> HandleAsync(TestCommand command, CancellationToken cancellationToken = default)
            => Task.FromResult(command.Value);
    }
}
