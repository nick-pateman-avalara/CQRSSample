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
using Microsoft.AspNetCore.Mvc.Formatters;
using CQRSAPI.Messages;
using Microsoft.AspNetCore.ResponseCompression;
using System.IO.Compression;
using CQRSAPI.Middleware;
using CQRSAPI.Providers;
using Microsoft.EntityFrameworkCore.Design.Internal;

namespace CQRSAPI
{
    public class Startup
    {

        public static IConfiguration Configuration { get; private set; }

        public static ApiControllerFeatureProvider ApiFeatureController { get; private set; }

        public static string LocalTestConnectionString
        {
            get
            {
                IConfigurationSection configurationSection = Configuration.GetSection("ConnectionStrings");
                return (configurationSection != null ? configurationSection.GetValue("LocalTest", string.Empty) : string.Empty);
            }
        }

        public Startup(IHostingEnvironment env)
        {
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

            ConfigureAppSettings(services);

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
            IMessageTransport messageTransport = RabbitMqMessageTransport.CreateAsync(Configuration)
                .GetAwaiter()
                .GetResult();
            services.AddSingleton(s => messageTransport);
        }

        private void ConfigureCompressionServices(IServiceCollection services)
        {
            services.Configure<GzipCompressionProviderOptions>(options => options.Level = CompressionLevel.Optimal);
            services.AddResponseCompression(options => { options.Providers.Add<GzipCompressionProvider>(); });
        }

        private void ConfigureMvcServices(IServiceCollection services)
        {
            ApiFeatureController = new ApiControllerFeatureProvider(
                Configuration,
                services);
            services.AddScoped<IApplicationFeatureProvider<ControllerFeature>>(s => ApiFeatureController);

            services.AddMvc(
                options =>
                {
                    options.OutputFormatters.Add(new XmlSerializerOutputFormatter());
                })
                .ConfigureApplicationPartManager(apm => apm.FeatureProviders.Add(ApiFeatureController))
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_2)
                .AddJsonOptions(options =>
                {
                    options.SerializerSettings.Converters.Add(new Newtonsoft.Json.Converters.StringEnumConverter());
                    options.SerializerSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
                });

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

        private void ConfigureAppSettings(IServiceCollection services)
        {
            services.AddSingleton<AppSettings>(x => new AppSettings
            {
                ConnectionString = LocalTestConnectionString
            });
        }

        public void Configure(
            IApplicationBuilder app, 
            IHostingEnvironment env, 
            IApplicationLifetime applicationLifetime)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

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
