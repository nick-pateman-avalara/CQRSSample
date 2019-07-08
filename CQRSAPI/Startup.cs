using System.Data.Common;
using System.Reflection;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using CQRSAPI.Extensions;
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

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            services.AddScoped<IMediator, Mediator>();
            services.AddScoped<IPersonValidator, PersonValidator>();
            services.AddTransient<ServiceFactory>(p => p.GetService);
            services.AddMediatorHandlers(typeof(Startup).GetTypeInfo().Assembly);

            DbConnectionStringBuilder dbConnectionStringBuilder = new DbConnectionStringBuilder
            {
                { "Data Source", "." },
                { "Initial Catalog", "CQRSAPI" },
                { "Integrated Security", "true" }
            };
            string connectionString = dbConnectionStringBuilder.ToString();
            services.AddDbContext<PeopleContext>(options => options.UseSqlServer(connectionString));

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
