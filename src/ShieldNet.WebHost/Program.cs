using ShieldNet.WebHost.Extensions;


namespace ShieldNet.WebHost
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Host.ConfigureHost();
           
            builder.Services.RegisterAppServices(builder.Configuration);
            var app = builder.Build();

            app.UseAppPipelines();

            await app.RunAsync();
        }
    }
}
