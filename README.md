# HandlerPipe

Librería ligera para manejo explícito de comandos y queries con pipeline mediante decoradores en .NET.

## Instalación

```bash
dotnet add package HandlerPipe
```

## Características

- Contratos CQRS simples: `ICommand`, `IQuery`, `ICommandHandler`, `IQueryHandler`
- Registro automático de handlers por ensamblado
- Decoradores para commands y queries
- Decorador de logging listo para usar

## Uso rápido

```csharp
builder.Services
	.AddHandlerPipe(typeof(Program).Assembly)
	.AddCommandDecorator(typeof(LoggingCommandHandlerDecorator<,>));
```

## Licencia

MIT. Ver [LICENSE](LICENSE).
