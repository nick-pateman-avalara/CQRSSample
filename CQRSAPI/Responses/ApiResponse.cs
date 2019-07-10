using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CQRSAPI.Responses
{

    public class ApiResponse<T>
    {

        public enum ResponseType
        {
            None = 0,
            Ok = 1,
            NotFound = 2,
            Conflict = 3,
            BadRequest = 4
        }

        public ResponseType Result { get; set; }

        public T Value { get; set; }

        public List<ModelError> Errors { get; set; } = new List<ModelError>();

    }

}
