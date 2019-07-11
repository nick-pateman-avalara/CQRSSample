using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Collections.Generic;

namespace CQRSAPI.Features.PeopleV2.Models
{

    public interface IPersonValidator
    {
        bool IsValid(
            Person person,
            out List<ModelError> errors);
    }

}
