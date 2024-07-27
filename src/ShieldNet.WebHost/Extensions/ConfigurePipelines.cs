namespace ShieldNet.WebHost.Extensions
{
    internal static class ConfigurePipelines
    {
        public static IApplicationBuilder UseAppPipelines(this WebApplication app)
        {
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseAuthentication();
            app.UseRouting()
                .UseAuthorization()
                .UseEndpoints(endpoints =>
                {

                    endpoints.MapRazorPages();
                    endpoints.MapControllers();
                    endpoints.MapControllerRoute(name: "default",
                                pattern: "{controller=Home}/{action=Index}/{id?}");
                });
            return app;
        }
    }
}
