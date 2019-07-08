using MediatR;
using Microsoft.EntityFrameworkCore;
using CQRSAPI.Requests;
using System.Threading;
using System.Threading.Tasks;
using CQRSAPI.Models;

namespace CQRSAPI.RequestHandlers
{

    public class GetPersonRequestHandler : IRequestHandler<GetPersonRequest, Person>
    {

        private readonly PeopleContext _peopleContext;

        public GetPersonRequestHandler(PeopleContext peopleContext)
        {
            _peopleContext = peopleContext;
        }
  
        public async Task<Person> Handle(GetPersonRequest request, CancellationToken cancellationToken)
        {
            return await _peopleContext.People
                .FirstOrDefaultAsync(m => m.PersonId == request.PersonId, cancellationToken);
        }

    }

}
