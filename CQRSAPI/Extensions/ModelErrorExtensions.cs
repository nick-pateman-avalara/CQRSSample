using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Collections.Generic;
using System.Linq;

namespace CQRSAPI.Extensions
{

    public static class ModelErrorExtensions
    {

        public static List<string> ToStringList( this List<ModelError> errorList)
        {
            return (errorList
                .Select(me => me.ErrorMessage)
                .ToList());
        }

    }

}
