using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Collections.Generic;

namespace CQRSAPI.Responses
{

    public class UpdatePersonResponse
    {

        public bool Success { get; set; }

        public List<ModelError> Errors { get; set; }

    }

}
