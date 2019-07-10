using System;
using CQRSAPI.Feature;
using CQRSAPI.People.Controllers;

namespace CQRSAPI.People.Feature
{
    public class PeopleFeature : IFeature
    {

        public bool Enabled { get; set; } = true;   //!!!

        public Type ControllerType => typeof(PeopleController);

    }

}
