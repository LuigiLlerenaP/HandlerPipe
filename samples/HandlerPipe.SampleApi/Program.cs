using HandlerPipe.Decorators;
using HandlerPipe.DependencyInjection;
using HandlerPipe.SampleApi.Features.Users;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

// UserStore como Singleton simula una base de datos en memoria
builder.Services.AddSingleton<UserStore>();

builder.Services
    .AddHandlerPipe(typeof(Program).Assembly)                        // Escanea handlers del assembly
    .AddCommandDecorator(typeof(LoggingCommandHandlerDecorator<,>)); // Logging automático en commands

// ─── Pipeline HTTP ───────────────────────────────────────────────────────────

var app = builder.Build();

if (app.Environment.IsDevelopment())
    app.MapOpenApi();

app.UseHttpsRedirection();

app.MapUserEndpoints();

app.Run();
