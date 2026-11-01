namespace HandlerPipe.Abstractions;

/// <summary>
/// Representa un valor vacío, equivalente a <c>void</c> en contextos genéricos.
/// Se usa como resultado de comandos que no retornan datos (<see cref="ICommand"/>).
/// </summary>
public readonly struct Unit : IEquatable<Unit>, IComparable<Unit>, IComparable
{
    /// <summary>Instancia única de <see cref="Unit"/>.</summary>
    public static readonly Unit Value = new();

    /// <summary>Tarea completada que retorna <see cref="Unit"/>.</summary>
    public static readonly Task<Unit> Task = System.Threading.Tasks.Task.FromResult(Value);

    public int CompareTo(Unit other) => 0;

    int IComparable.CompareTo(object? obj) => obj is Unit ? 0 : 1;

    public bool Equals(Unit other) => true;

    public override bool Equals(object? obj) => obj is Unit;

    public override int GetHashCode() => 0;

    public override string ToString() => "()";

    public static bool operator ==(Unit left, Unit right) => true;

    public static bool operator !=(Unit left, Unit right) => false;
}

/// <summary>
/// Marca un tipo como un comando que produce un resultado de tipo <typeparamref name="TResult"/>.
/// Los comandos representan intenciones de cambiar el estado del sistema (write operations).
/// </summary>
/// <typeparam name="TResult">Tipo del resultado que retorna el comando.</typeparam>
public interface ICommand<out TResult> { }

/// <summary>
/// Marca un tipo como un comando que no retorna datos.
/// Internamente resuelve como <see cref="ICommand{TResult}"/> con <see cref="Unit"/>.
/// </summary>
public interface ICommand : ICommand<Unit> { }

/// <summary>
/// Marca un tipo como una consulta que produce un resultado de tipo <typeparamref name="TResult"/>.
/// Las queries son operaciones de sólo lectura; no deben modificar estado del sistema.
/// </summary>
/// <typeparam name="TResult">Tipo del resultado que retorna la consulta.</typeparam>
public interface IQuery<out TResult> { }