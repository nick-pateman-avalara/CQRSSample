using MediatR;
using CQRSAPI.Models;
using CQRSAPI.Requests;
using System.Threading;
using System.Threading.Tasks;
using CQRSAPI.Responses;
using Microsoft.EntityFrameworkCore;

namespace CQRSAPI.RequestHandlers
{

    public class UpdatePersonRequestHandler : IRequestHandler<UpdatePersonRequest, UpdatePersonResponse>
    {

        private readonly PeopleContext _peopleContext;
        private readonly IPersonValidator _personValidator;

        public UpdatePersonRequestHandler(
            PeopleContext peopleContext,
            IPersonValidator personValidator)
        {
            _peopleContext = peopleContext;
            _personValidator = personValidator;
        }

        public async Task<UpdatePersonResponse> Handle(UpdatePersonRequest request, CancellationToken cancellationToken)
        {
            if (!_personValidator.IsValid(request.Person, out var errors))
            {
                return new UpdatePersonResponse() { Success = false, Errors = errors };
            }

            Person person = await _peopleContext.People
                .FirstOrDefaultAsync(m => m.PersonId == request.Person.PersonId, cancellationToken);
            if (person != null)
            {
                _peopleContext.Entry(person).CurrentValues.SetValues(request.Person);
                return (new UpdatePersonResponse() { Success = await _peopleContext.SaveChangesAsync(cancellationToken) > 0 });
            }
            else
            {
                return (new UpdatePersonResponse());
            }
        }

    }

}
