using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
