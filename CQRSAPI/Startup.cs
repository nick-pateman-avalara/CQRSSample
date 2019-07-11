using System.Reflection;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using CQRSAPI.Extensions;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using CQRSAPI.Feature;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc.Formatters;

namespace CQRSAPI
{
    public class Startup
    {

        //public Startup(IConfiguration configuration)
        //{
        //    Configuration = configuration;
        //}

        private ILoggerFactory _loggerFactory;

        public static IConfiguration Configuration { get; private set; }
        public static ApiContollerFeatureProvider ApiFeatureController { get; private set; }

        public static string LocalTestConnectionString
        {
            get
            {
                string connectionString = Configuration.GetSection("ConnectionStrings").GetValue<string>("LocalTest");
                return (connectionString);
            }
        }

        public Startup(IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            _loggerFactory = loggerFactory;
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", true, true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            ApiFeatureController = new ApiContollerFeatureProvider(
                Configuration,
                services);

            services.AddMvc(
                options =>
                {
                    options.OutputFormatters.Add(new XmlSerializerOutputFormatter());
                })
                .ConfigureApplicationPartManager(apm => apm.FeatureProviders.Add(ApiFeatureController))
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            services.AddTransient<ServiceFactory>(p => p.GetService);
            services.AddScoped<IApplicationFeatureProvider<ControllerFeature>, ApiContollerFeatureProvider>();
            services.AddScoped<IMediator, Mediator>();
            services.AddMediatorHandlers(typeof(Startup).GetTypeInfo().Assembly);

            ApiFeatureController.AddServices();

            services.ConfigureSwaggerGen(options =>
            {
                // UseFullTypeNameInSchemaIds replacement for .NET Core
                options.CustomSchemaIds(x => x.FullName);
            });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "People API", Version = "v1" });
            });
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
