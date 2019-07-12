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

        public static List<ModelError> ToErrorList(this ModelStateDictionary modelState)
        {
            List<ModelError> errors = new List<ModelError>();
            IEnumerable<ModelErrorCollection> allModelErrors = modelState.Values.Select(mse => mse.Errors);
            foreach (ModelErrorCollection curModelErrors in allModelErrors)
            {
                errors.AddRange(curModelErrors.ToList());
            }
            return (errors);
        }

    }

}
