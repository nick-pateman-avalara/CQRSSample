using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CQRSAPI.Feature
{
    public class ApiContollerFeatureProvider : IApplicationFeatureProvider<ControllerFeature>
    {

        public void PopulateFeature(IEnumerable<ApplicationPart> parts, ControllerFeature feature)
        {
            feature.Controllers.Clear();
            foreach (Type entityType in typeof(ApiContollerFeatureProvider).GetTypeInfo().Assembly.GetTypes())
            {
                if(!entityType.IsInterface && typeof(IFeature).IsAssignableFrom(entityType))
                {
                    IFeature featurePart = (IFeature)Activator.CreateInstance(entityType);
                    if (featurePart.Enabled)
                    {
                        feature.Controllers.Add(featurePart.ControllerType.GetTypeInfo());
                    }
                }
            }
        }

    }
}
