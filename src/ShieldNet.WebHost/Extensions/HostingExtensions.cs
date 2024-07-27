using Serilog;
using Serilog.Sinks.SystemConsole.Themes;

namespace ShieldNet.WebHost.Extensions
{
    internal static class HostingExtensions
    {

        public static void ConfigureHost(this IHostBuilder hostBuilder)
        {
            hostBuilder.UseSerilog((hostingContext, loggerConfiguration) =>
            {
                loggerConfiguration
                    .ReadFrom.Configuration(hostingContext.Configuration)
                    .Enrich.FromLogContext()
                    .WriteTo.Console(
                        theme: AnsiConsoleTheme.Literate,
                        outputTemplate: "{NewLine}[{Timestamp:HH:mm:ss} {Level:u3}] {SourceContext}{NewLine} {Message:lj}{NewLine} {Exception}{NewLine}");
            });
        }
    }
}
