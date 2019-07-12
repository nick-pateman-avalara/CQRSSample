﻿using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace CQRSAPI.Feature
{

    public interface IFeature
    {

        string Name { get; }

        bool Enabled { get; set; }

        Type ControllerType { get; }

        void AddServices(IServiceCollection services);


    }

}
