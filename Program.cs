using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

var version = typeof(Program).Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion;
if (version is not null)
{
    version = version.Split('+')[0]; // Remove the commit hash.
}

if (args.Length != 2 || args[0] != "mcp" || args[1] != "start")
{
    Console.Error.WriteLine($"Usage: dnx Knapcode.SampleMcpServer@{version} --yes -- mcp start");
    return 1;
}

var builder = Host.CreateApplicationBuilder(args);

// Configure all logs to go to stderr (stdout is used for the MCP protocol messages).
builder.Logging.AddConsole(o => o.LogToStandardErrorThreshold = LogLevel.Trace);

// Add the MCP services: the transport to use (stdio) and the tools to register.
builder.Services
    .AddMcpServer()
    .WithStdioServerTransport()
    .WithTools<RandomNumberTools>();

await builder.Build().RunAsync();
return 0;
