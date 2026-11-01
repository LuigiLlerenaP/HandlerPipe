using HandlerPipe.Abstractions;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace HandlerPipe.Decorators;

/// <summary>
/// Decorador que envuelve cualquier <see cref="ICommandHandler{TCommand,TResult}"/> añadiendo:
/// <list type="bullet">
///   <item>Log de inicio con el nombre del comando.</item>
///   <item>Medición de tiempo de ejecución con cero alocaciones de memoria.</item>
///   <item>Manejo diferenciado de cancelación (<see cref="OperationCanceledException"/>).</item>
///   <item>Log de error con la excepción original si el handler falla.</item>
/// </list>
/// Se registra al llamar <c>AddCommandDecorator(typeof(LoggingCommandHandlerDecorator&lt;,&gt;))</c>.
/// </summary>
/// <typeparam name="TCommand">Tipo del comando manejado.</typeparam>
/// <typeparam name="TResult">Tipo del resultado producido.</typeparam>
public sealed class LoggingCommandHandlerDecorator<TCommand, TResult>(
    ICommandHandler<TCommand, TResult> inner,
    ILogger<LoggingCommandHandlerDecorator<TCommand, TResult>> logger)
    : ICommandHandler<TCommand, TResult>
    where TCommand : ICommand<TResult>
{
    private readonly ILogger<LoggingCommandHandlerDecorator<TCommand, TResult>> _logger = logger;

    /// <inheritdoc/>
    public async Task<TResult> HandleAsync(TCommand command, CancellationToken cancellationToken = default)
    {
        var commandName = typeof(TCommand).Name;

        _logger.LogInformation("→ Handling [{CommandName}]", commandName);

        var startTimestamp = Stopwatch.GetTimestamp();

        try
        {
            var result = await inner.HandleAsync(command, cancellationToken).ConfigureAwait(false);

            var elapsed = Stopwatch.GetElapsedTime(startTimestamp);
            _logger.LogInformation(
                "[{CommandName}] handled successfully in {ElapsedMs:F2}ms",
                commandName,
                elapsed.TotalMilliseconds);

            return result;
        }
        catch (OperationCanceledException)
        {
            var elapsed = Stopwatch.GetElapsedTime(startTimestamp);
            _logger.LogWarning(
                "[{CommandName}] was cancelled after {ElapsedMs:F2}ms",
                commandName,
                elapsed.TotalMilliseconds);
            throw;
        }
        catch (Exception ex)
        {
            var elapsed = Stopwatch.GetElapsedTime(startTimestamp);
            _logger.LogError(
                ex,
                "[{CommandName}] failed after {ElapsedMs:F2}ms",
                commandName,
                elapsed.TotalMilliseconds);
            throw;
        }
    }
}