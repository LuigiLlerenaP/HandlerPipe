namespace HandlerPipe.Abstractions;

/// <summary>
/// Define el contrato para manejar un comando que retorna <typeparamref name="TResult"/>.
/// </summary>
/// <typeparam name="TCommand">Tipo del comando, debe implementar <see cref="ICommand{TResult}"/>.</typeparam>
/// <typeparam name="TResult">Tipo del resultado producido por el comando.</typeparam>
public interface ICommandHandler<in TCommand, TResult> where TCommand : ICommand<TResult>
{
    /// <summary>
    /// Ejecuta la lógica del comando de forma asíncrona.
    /// </summary>
    /// <param name="command">Instancia del comando a ejecutar.</param>
    /// <param name="cancellationToken">Token para cancelar la operación.</param>
    /// <returns>El resultado producido por el comando.</returns>
    Task<TResult> HandleAsync(TCommand command, CancellationToken cancellationToken = default);
}

/// <summary>
/// Contrato para comandos que no retornan datos (<see cref="Unit"/>).
/// </summary>
/// <typeparam name="TCommand">Tipo del comando, debe implementar <see cref="ICommand"/>.</typeparam>
public interface ICommandHandler<in TCommand> : ICommandHandler<TCommand, Unit> where TCommand : ICommand
{
}

/// <summary>
/// Define el contrato para manejar una consulta que retorna <typeparamref name="TResult"/>.
/// </summary>
/// <typeparam name="TQuery">Tipo de la consulta, debe implementar <see cref="IQuery{TResult}"/>.</typeparam>
/// <typeparam name="TResult">Tipo del resultado producido por la consulta.</typeparam>
public interface IQueryHandler<in TQuery, TResult> where TQuery : IQuery<TResult>
{
    /// <summary>
    /// Ejecuta la lógica de consulta de forma asíncrona.
    /// </summary>
    /// <param name="query">Instancia de la consulta a ejecutar.</param>
    /// <param name="cancellationToken">Token para cancelar la operación.</param>
    /// <returns>El resultado de la consulta.</returns>
    Task<TResult> HandleAsync(TQuery query, CancellationToken cancellationToken = default);
}