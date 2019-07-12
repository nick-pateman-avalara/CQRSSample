using System;
using System.Threading.Tasks;
using CQRSAPI.Data;
using CQRSAPI.Feature;
using CQRSAPI.Features.PeopleV2.Controllers;
using CQRSAPI.Features.PeopleV2.Data;
using CQRSAPI.Features.PeopleV2.Messages;
using CQRSAPI.Features.PeopleV2.Models;
using CQRSAPI.Messages;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CQRSAPI.Features.PeopleV2.Feature
{
    public class PeopleFeature : IFeature
    {

        public string Name => "PeopleV2";

        public bool Enabled { get; set; }

        public Type ControllerType => typeof(PeopleController);

        public void AddServices(IServiceCollection services)
        {
            services.AddScoped<IRepository<Person>, CqrsApiPeopleSqlRepository>();
            services.AddScoped<IPersonValidator, PersonValidator>();
        }

        public static string GetName()
        {
            return (new PeopleFeature().Name);
        }

    }

}
