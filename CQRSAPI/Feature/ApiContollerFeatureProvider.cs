﻿using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.Configuration;

namespace CQRSAPI.Feature
{
    public class ApiContollerFeatureProvider : IApplicationFeatureProvider<ControllerFeature>
    {

        private readonly IConfiguration _configuration;
        private readonly List<IFeature> _features;

        public IServiceCollection Services { get; private set; }

        public ApiContollerFeatureProvider(
            IConfiguration configuration,
            IServiceCollection services)
        {
            _configuration = configuration;
            _features = new List<IFeature>();
            Services = services;
            EnumerateFeatures();
        }

        private void EnumerateFeatures()
        {
            foreach (Type entityType in typeof(ApiContollerFeatureProvider).GetTypeInfo().Assembly.GetTypes())
            {
                if (!entityType.IsInterface && typeof(IFeature).IsAssignableFrom(entityType))
                {
                    IFeature featurePart = (IFeature)Activator.CreateInstance(entityType);
                    bool enabled = _configuration.GetSection("Features").GetValue<bool>(featurePart.Name);
                    featurePart.Enabled = enabled;

                    if (featurePart.Enabled)
                    {
                        _features.Add(featurePart);
                    }
                }
            }
        }

        public void PopulateFeature(IEnumerable<ApplicationPart> parts, ControllerFeature feature)
        {
            feature.Controllers.Clear();
            foreach (IFeature curFeature in _features)
            {
                feature.Controllers.Add(curFeature.ControllerType.GetTypeInfo());
            }
        }

        public void AddServices()
        {
            foreach (IFeature feature in _features)
            {
                feature.AddServices(Services);
            }
        }

        public void StartupFeatures()
        {
            foreach (IFeature feature in _features)
            {
                feature.Startup(_configuration);
            }
        }

    }
}