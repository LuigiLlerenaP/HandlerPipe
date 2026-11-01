using HandlerPipe.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace HandlerPipe.DependencyInjection;

/// <summary>
/// Métodos de extensión para registrar HandlerPipe y sus decoradores en el contenedor de dependencias.
/// </summary>
public static class HandlerPipeExtensions
{
    private static readonly Type CommandHandlerOpenType = typeof(ICommandHandler<,>);
    private static readonly Type QueryHandlerOpenType = typeof(IQueryHandler<,>);

    /// <summary>
    /// Escanea los assemblies indicados y registra todas las implementaciones concretas de
    /// <see cref="ICommandHandler{TCommand,TResult}"/> e <see cref="IQueryHandler{TQuery,TResult}"/>.
    /// </summary>
    /// <param name="services">El contenedor de servicios.</param>
    /// <param name="lifetime">Ciclo de vida de los handlers (por defecto <see cref="ServiceLifetime.Scoped"/>).</param>
    /// <param name="assemblies">Assemblies a escanear.</param>
    /// <returns>La misma colección <see cref="IServiceCollection"/> para encadenamiento fluido.</returns>
    public static IServiceCollection AddHandlerPipe(
        this IServiceCollection services,
        ServiceLifetime lifetime = ServiceLifetime.Scoped,
        params Assembly[] assemblies)
    {
        ArgumentNullException.ThrowIfNull(services);

        if (assemblies is null || assemblies.Length == 0)
            throw new ArgumentException("Debe proporcionar al menos un assembly para escanear.", nameof(assemblies));

        foreach (var assembly in assemblies)
        {
            var candidates = GetLoadableTypes(assembly)
                .Where(t => t is { IsAbstract: false, IsInterface: false, IsGenericTypeDefinition: false });

            foreach (var type in candidates)
            {
                foreach (var iface in type.GetInterfaces())
                {
                    if (!iface.IsGenericType) continue;

                    var def = iface.GetGenericTypeDefinition();

                    if (def == CommandHandlerOpenType || def == QueryHandlerOpenType)
                    {
                        services.Add(new ServiceDescriptor(iface, type, lifetime));
                    }
                }
            }
        }

        return services;
    }

    /// <summary>
    /// Sobrecarga conveniente que asume ciclo de vida <see cref="ServiceLifetime.Scoped"/>.
    /// </summary>
    public static IServiceCollection AddHandlerPipe(
        this IServiceCollection services,
        params Assembly[] assemblies) =>
        services.AddHandlerPipe(ServiceLifetime.Scoped, assemblies);

    /// <summary>
    /// Envuelve todos los <see cref="ICommandHandler{TCommand,TResult}"/> registrados con el decorador genérico indicado.
    /// </summary>
    /// <param name="services">El contenedor de servicios.</param>
    /// <param name="openGenericDecoratorType">Tipo decorador genérico abierto (e.g. <c>typeof(LoggingCommandHandlerDecorator&lt;,&gt;)</c>).</param>
    public static IServiceCollection AddCommandDecorator(
        this IServiceCollection services,
        Type openGenericDecoratorType)
    {
        return ApplyDecorator(services, CommandHandlerOpenType, openGenericDecoratorType);
    }

    /// <summary>
    /// Envuelve todos los <see cref="IQueryHandler{TQuery,TResult}"/> registrados con el decorador genérico indicado.
    /// </summary>
    /// <param name="services">El contenedor de servicios.</param>
    /// <param name="openGenericDecoratorType">Tipo decorador genérico abierto.</param>
    public static IServiceCollection AddQueryDecorator(
        this IServiceCollection services,
        Type openGenericDecoratorType)
    {
        return ApplyDecorator(services, QueryHandlerOpenType, openGenericDecoratorType);
    }

    private static IServiceCollection ApplyDecorator(
        IServiceCollection services,
        Type openHandlerInterfaceType,
        Type openGenericDecoratorType)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(openGenericDecoratorType);

        if (!openGenericDecoratorType.IsGenericTypeDefinition || openGenericDecoratorType.GetGenericArguments().Length != 2)
        {
            throw new ArgumentException(
                $"El tipo {openGenericDecoratorType.Name} debe ser una definición genérica abierta de 2 argumentos (ej. Decorator<,>).",
                nameof(openGenericDecoratorType));
        }

        // Snapshot de los descriptores registrados antes de modificar la colección
        var descriptors = services
            .Where(d => d.ServiceType.IsGenericType && d.ServiceType.GetGenericTypeDefinition() == openHandlerInterfaceType)
            .ToList();

        foreach (var descriptor in descriptors)
        {
            services.Remove(descriptor);

            var genericArgs = descriptor.ServiceType.GetGenericArguments();
            var closedDecorator = openGenericDecoratorType.MakeGenericType(genericArgs);
            var capturedDescriptor = descriptor;

            services.Add(new ServiceDescriptor(
                serviceType: descriptor.ServiceType,
                factory: sp =>
                {
                    var inner = capturedDescriptor switch
                    {
                        { ImplementationInstance: not null } d => d.ImplementationInstance,
                        { ImplementationFactory: not null } d => d.ImplementationFactory(sp),
                        { ImplementationType: not null } d => ActivatorUtilities.CreateInstance(sp, d.ImplementationType),
                        _ => throw new InvalidOperationException($"No se pudo resolver la instancia previa para {capturedDescriptor.ServiceType.Name}.")
                    };

                    return ActivatorUtilities.CreateInstance(sp, closedDecorator, inner);
                },
                lifetime: descriptor.Lifetime));
        }

        return services;
    }

    private static IEnumerable<Type> GetLoadableTypes(Assembly assembly)
    {
        try
        {
            return assembly.GetTypes();
        }
        catch (ReflectionTypeLoadException ex)
        {
            return ex.Types.Where(t => t != null)!;
        }
    }
}