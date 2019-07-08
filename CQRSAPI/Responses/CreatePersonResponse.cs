using CQRSAPI.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Collections.Generic;

namespace CQRSAPI.Responses
{

    public class CreatePersonResponse
    {

        public bool Success { get; set; }

        public Person Result { get; set; }

        public List<ModelError> Errors { get; set; }

    }

}
