using CQRSAPI.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Collections.Generic;

namespace CQRSAPI.Responses
{

    public class GetAllPeopleResponse
    {

        public bool Success { get; set; }

        public List<Person> Result { get; set; }

        public List<ModelError> Errors { get; set; }

    }

}
