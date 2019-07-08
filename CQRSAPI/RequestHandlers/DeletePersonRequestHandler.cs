using MediatR;
using CQRSAPI.Requests;
using System.Threading;
using System.Threading.Tasks;
using CQRSAPI.Models;

namespace CQRSAPI.RequestHandlers
{

    public class DeletePersonRequestHandler : IRequestHandler<DeletePersonRequest, bool>
    {

        private readonly PeopleContext _peopleContext;

        public DeletePersonRequestHandler(PeopleContext peopleContext)
        {
            _peopleContext = peopleContext;
        }
  
        public async Task<bool> Handle(DeletePersonRequest request, CancellationToken cancellationToken)
        {
            if(_peopleContext.PersonExists(request.PersonId))
            {
                Person person = await _peopleContext.People.FindAsync(request.PersonId);
                _peopleContext.People.Remove(person);
                await _peopleContext.SaveChangesAsync(cancellationToken);
                return (true);
            }
            else
            {
                return (false);
            }
        }

    }

}
