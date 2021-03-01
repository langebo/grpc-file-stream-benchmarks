using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace FileServer
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services) => services.AddGrpc();

        public void Configure(IApplicationBuilder app)
        {
            app.UseRouting();
            app.UseEndpoints(ep => ep.MapGrpcService<DownloadService>());
        }
    }
}
