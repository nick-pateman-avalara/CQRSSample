using System;

namespace CQRSAPI.Feature
{

    public interface IFeature
    {

        bool Enabled { get; set; }

        Type ControllerType { get; }

    }

}
