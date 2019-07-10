using System.Reflection;
using CQRSAPI.Data;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using CQRSAPI.Extensions;
using CQRSAPI.People.Data;
using CQRSAPI.People.Messages;
using CQRSAPI.People.Models;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using CQRSAPI.Feature;

namespace CQRSAPI
{
    public class Startup
    {

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public static IConfiguration Configuration { get; private set; }

        public static string LocalTestConnectionString
        {
            get
            {
                string connectionString = Configuration.GetSection("ConnectionStrings").GetValue<string>("LocalTest");
                return (connectionString);
            }
        }

        public async void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc()
                .ConfigureApplicationPartManager(apm => apm.FeatureProviders.Add(new ApiContollerFeatureProvider()))
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            services.AddScoped<IApplicationFeatureProvider<ControllerFeature>, ApiContollerFeatureProvider>();

            services.AddTransient<ServiceFactory>(p => p.GetService);
            services.AddScoped<IMediator, Mediator>();
            services.AddMediatorHandlers(typeof(Startup).GetTypeInfo().Assembly);

            People.Feature.Services.AddServices(services);

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "People API", Version = "v1" });
            });

            //await PeopleRabbitMqMessageTransport.InitialiseAsync(Configuration.GetSection("ConnectionStrings").GetValue<string>("RabbitMQ"));
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "People API V1");
            });

            //app.UseHttpsRedirection();

            app.UseMvc();
        }

    }
}
