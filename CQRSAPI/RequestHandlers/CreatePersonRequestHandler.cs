using MediatR;
using CQRSAPI.Requests;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using CQRSAPI.Responses;
using CQRSAPI.Models;

namespace CQRSAPI.RequestHandlers
{

    public class CreatePersonRequestHandler : IRequestHandler<CreatePersonRequest, CreatePersonResponse>
    {

        private readonly PeopleContext _peopleContext;
        private readonly IPersonValidator _personValidator;

        public CreatePersonRequestHandler(
            PeopleContext peopleContext,
            IPersonValidator personValidator)
        {
            _peopleContext = peopleContext;
            _personValidator = personValidator;
        }
  
        public async Task<CreatePersonResponse> Handle(CreatePersonRequest request, CancellationToken cancellationToken)
        {
            if(!_personValidator.IsValid(request.Person, out var errors))
            {
                return new CreatePersonResponse() { Success = false, Errors = errors };
            }

            EntityEntry<Person> addPerson = await _peopleContext.AddAsync(request.Person, cancellationToken);
            await _peopleContext.SaveChangesAsync(cancellationToken);
            return (new CreatePersonResponse() { Success = true, Result = addPerson.Entity });
        }

    }

}
