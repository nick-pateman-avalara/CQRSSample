using CQRSAPI.Data;
using CQRSAPI.People.Data;
using CQRSAPI.People.Models;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CQRSAPI.People.Feature
{

    public static class Services
    {

        public static void AddServices(IServiceCollection services)
        {
            services.AddScoped<IRepository<Person>, CqrsApiPeopleSqlRepository>();
            services.AddScoped<IPersonValidator, PersonValidator>();
        }

    }

}
