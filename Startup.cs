using Manatee.Trello;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace RedLeg.Coaching
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            // Get an app key via https://trello.com/app-key
            // Then get a 'server token' since we are too lazy to bother with OAuth flow

            TrelloAuthorization.Default.AppKey = configuration.GetValue<string>("Trello:AppKey");
            TrelloAuthorization.Default.UserToken = configuration.GetValue<string>("Trello:ApiKey");
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddOptions<TrelloConfiguration>("Trello");

            services.AddRazorPages();

            services.AddSingleton<ITrelloFactory, TrelloFactory>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
            });
        }
    }
}