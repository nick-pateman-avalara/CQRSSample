using System;
using System.Threading.Tasks;
using CQRSAPI.Data;
using CQRSAPI.Feature;
using CQRSAPI.Features.People.Controllers;
using CQRSAPI.Features.People.Data;
using CQRSAPI.Features.People.Models;
using CQRSAPI.Messages;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CQRSAPI.Features.People.Feature
{
    public class PeopleFeature : IFeature
    {

        public string Name => "People";

        public bool Enabled { get; set; }

        public Type ControllerType => typeof(PeopleController);

        public void AddServices(IServiceCollection services)
        {
            services.AddScoped<IRepository<Person>, CqrsApiPeopleSqlRepository>();
            services.AddScoped<IPersonValidator, PersonValidator>();
        }

    }

}
