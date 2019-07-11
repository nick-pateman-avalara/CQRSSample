using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Collections.Generic;

namespace CQRSAPI.Features.People.Models
{
    public interface IPersonValidator
    {

        bool IsValid(
            Person person,
            out List<ModelError> errors);

    }

}
