using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using RabbitMQ.Client;

namespace ifttthandler
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "ifttthandler", Version = "v1" });
            });
            services.Configure<RabbitMqOptions>(opts => Configuration.GetSection("RabbitMqOptions").Bind(opts));
            services.AddSingleton<IConnectionFactory, ConnectionFactory>(provider => 
            {
                var opts = provider.GetRequiredService<IOptions<RabbitMqOptions>>();

                return new ConnectionFactory 
                { 
                    HostName = opts.Value.HostName,
                    UserName = opts.Value.UserName,
                    Password = opts.Value.Password,
                    VirtualHost = opts.Value.VirtualHost
                };
            })
                .AddSingleton(provider => provider.GetRequiredService<IConnectionFactory>().CreateConnection())
                .AddSingleton(provider => provider.GetRequiredService<IConnection>().CreateModel());
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "ifttthandler v1"));
            }

            app.UseRouting();
            app.UseAuthorization();
            //app.UseMiddleware<AuthMiddleware>();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
