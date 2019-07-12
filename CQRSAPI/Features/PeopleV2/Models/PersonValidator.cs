using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;

namespace CQRSAPI.Features.PeopleV2.Models
{

    public class PersonValidator : IPersonValidator
    {

        private readonly ILogger<PersonValidator> _logger;

        public PersonValidator(ILogger<PersonValidator> logger)
        {
            _logger = logger;
        }

        public bool IsValid(
            Person person,
            out List<ModelError> errors)
        {
            errors = new List<ModelError>();

            if (string.IsNullOrEmpty(person.FirstName) || person.FirstName.Length > 100) errors.Add(new ModelError("FirstName is required and must be between 1 and 100 chars long."));
            if (person.LastName.Length > 100) errors.Add(new ModelError("LastName is optional but must be between 1 and 100 chars long if present."));
            if (person.Age < 1 || person.Age > 100) errors.Add(new ModelError("Age must be between 1 and 100."));

            if (errors.Count > 0)
            {
                _logger?.LogError("Validation of Person failed.");
            }
            return (errors.Count == 0);
        }

    }

}
