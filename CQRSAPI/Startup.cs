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
using CQRSAPI.Messages;
using Microsoft.AspNetCore.ResponseCompression;
using System.IO.Compression;
using CQRSAPI.Middleware;

namespace CQRSAPI
{
    public class Startup
    {

        private ILoggerFactory _loggerFactory;

        public static IConfiguration Configuration { get; private set; }

        public static ApiContollerFeatureProvider ApiFeatureController { get; private set; }

        public static string LocalTestConnectionString
        {
            get
            {
                return (Configuration.GetSection("ConnectionStrings").GetValue<string>("LocalTest"));
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
            services.AddTransient<ServiceFactory>(p => p.GetService);

            ConfigureNServiceBusServices(services);

            ConfigureCompressionServices(services);

            //Caching?

            ConfigureMvcServices(services);

            //Node?

            ConfigureCorsServices(services);

            ConfigureMediatorServices(services);

            //Authentication?

            ConfigureSwaggerServices(services);

            //Database?

            //Aws?

            //Container?
        }

        private void ConfigureNServiceBusServices(IServiceCollection services)
        {
            services.AddSingleton<IMessageTransport>(s => RabbitMqMessageTransport.Create(LocalTestConnectionString));
        }

        private void ConfigureCompressionServices(IServiceCollection services)
        {
            services.Configure<GzipCompressionProviderOptions>(options => options.Level = CompressionLevel.Optimal);
            services.AddResponseCompression(options => { options.Providers.Add<GzipCompressionProvider>(); });
        }

        private void ConfigureMvcServices(IServiceCollection services)
        {
            ApiFeatureController = new ApiContollerFeatureProvider(
                Configuration,
                services);
            services.AddScoped<IApplicationFeatureProvider<ControllerFeature>>(s => ApiFeatureController);

            services.AddMvc(
                options =>
                {
                    options.OutputFormatters.Add(new XmlSerializerOutputFormatter());
                })
                .ConfigureApplicationPartManager(apm => apm.FeatureProviders.Add(ApiFeatureController))
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            ApiFeatureController.AddServices();
        }

        private void ConfigureCorsServices(IServiceCollection services)
        {
            // TODO: tie CORS down to specific origins
            services.AddCors(
                options =>
                {
                    options.AddPolicy("AllowAllOrigins", builder =>
                    {
                        builder.AllowAnyOrigin();
                        builder.AllowAnyHeader();
                        builder.AllowAnyMethod();
                        builder.AllowCredentials();
                    });

                });
        }

        private void ConfigureMediatorServices(IServiceCollection services)
        {
            services.AddScoped<IMediator, Mediator>();
            services.AddMediatorHandlers(typeof(Startup).GetTypeInfo().Assembly);
        }

        private void ConfigureSwaggerServices(IServiceCollection services)
        {
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

        public void Configure(
            IApplicationBuilder app, 
            IHostingEnvironment env, 
            IApplicationLifetime applicationLifetime)
        {
            //if (env.IsDevelopment())
            //{
            //    app.UseDeveloperExceptionPage();
            //}
            //else
            //{
            //    app.UseHsts();
            //}

            app.UseRequestLocalization();

            app.UseResponseCompression();

            //app.UseAuthentication();

            app.UseCors("AllowAllOrigins"); // must come before app.UserMvc()

            app.UseMiddleware(typeof(ErrorHandlingMiddleware));

            app.UseMvc();

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "People API V1");
            });
        }

    }
}
