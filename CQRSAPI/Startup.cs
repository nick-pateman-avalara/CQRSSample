using System.Reflection;
using CQRSAPI.Data;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using CQRSAPI.Extensions;
using CQRSAPI.Messages;
using CQRSAPI.Models;
using Microsoft.OpenApi.Models;

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
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            services.AddScoped<IRepository<Person>, CqrsApiPeopleSqlRepository>();
            services.AddScoped<IMediator, Mediator>();
            services.AddScoped<IPersonValidator, PersonValidator>();
            services.AddTransient<ServiceFactory>(p => p.GetService);
            services.AddMediatorHandlers(typeof(Startup).GetTypeInfo().Assembly);

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "People API", Version = "v1" });
            });

            await PeopleRabbitMQMessageTransport.InitialiseAsync(Configuration.GetSection("ConnectionStrings").GetValue<string>("RabbitMQ"));
            await PeopleRabbitMQMessageTransport.Instance.PublishAsync(new PersonEventMessage());
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
