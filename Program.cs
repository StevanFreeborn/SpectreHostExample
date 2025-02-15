using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Spectre.Console;
using Spectre.Console.Cli;

await Host.CreateDefaultBuilder(args)
    .ConfigureServices(static (ctx, services) =>
    {
        _ = services.AddSingleton(AnsiConsole.Console);
    })
    .BuildApp()
    .RunAsync(args);

internal static class HostBuilderExtensions
{
    public static CommandApp<GreetingCommand> BuildApp(this IHostBuilder builder)
    {
        TypeRegistrar registrar = new(builder);
        CommandApp<GreetingCommand> app = new(registrar);
        return app;
    }
}

internal sealed class GreetingCommand(IAnsiConsole console) : Command
{
    private readonly IAnsiConsole _console = console;

    public override int Execute(CommandContext context)
    {
        _console.MarkupLine("Hello, [bold]World[/]!");
        return 0;
    }
}

internal sealed class TypeRegistrar(IHostBuilder builder) : ITypeRegistrar
{
    private readonly IHostBuilder _builder = builder;

    public ITypeResolver Build()
    {
        return new TypeResolver(_builder.Build());
    }

    public void Register(Type service, Type implementation)
    {
        _ = _builder.ConfigureServices((_, services) => services.AddSingleton(service, implementation));
    }

    public void RegisterInstance(Type service, object implementation)
    {
        _ = _builder.ConfigureServices((_, services) => services.AddSingleton(service, implementation));
    }

    public void RegisterLazy(Type service, Func<object> factory)
    {
        _ = _builder.ConfigureServices((_, services) => services.AddSingleton(service, _ => factory()));
    }
}

internal sealed class TypeResolver(IHost host) : ITypeResolver, IDisposable
{
    private readonly IHost _host = host;

    public object? Resolve(Type? type)
    {
        return type is not null ? _host.Services.GetRequiredService(type) : null;
    }

    public void Dispose()
    {
        _host.Dispose();
    }
}
