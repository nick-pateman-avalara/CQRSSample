using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace CQRSAPI.People.Models
{

    public interface IPersonValidator
    {
        bool IsValid(
            Person person,
            out List<ModelError> errors);
    }

    public class PersonValidator : IPersonValidator
    {

        public bool IsValid(
            Person person,
            out List<ModelError> errors)
        {
            errors = new List<ModelError>();

            if (string.IsNullOrEmpty(person.FirstName) || person.FirstName.Length > 100) errors.Add(new ModelError("FirstName is required and must be between 1 and 100 chars long."));
            if (person.LastName.Length > 100) errors.Add(new ModelError("LastName is optional but must be between 1 and 100 chars long if present."));
            if (person.Age < 1 || person.Age > 100) errors.Add(new ModelError("Age must be between 1 and 100."));

            return (errors.Count == 0);
        }

    }

}
