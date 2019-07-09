using MediatR;
using CQRSAPI.Requests;
using System.Threading;
using System.Threading.Tasks;
using CQRSAPI.Models;
using CQRSAPI.Data;

namespace CQRSAPI.RequestHandlers
{

    public class DeletePersonRequestHandler : IRequestHandler<DeletePersonRequest, bool>
    {

        private readonly IRepository<Person> _peopleRepository;

        public DeletePersonRequestHandler(IRepository<Person> peopleRepository)
        {
            _peopleRepository = peopleRepository;
        }
  
        public async Task<bool> Handle(DeletePersonRequest request, CancellationToken cancellationToken)
        {
            if(await _peopleRepository.Exists(request.Id, cancellationToken))
            {
                int rowsAffected = await _peopleRepository.DeleteAsync(request.Id, cancellationToken);
                return (rowsAffected == 1);
            }
            else
            {
                return (false);
            }
        }

    }

}
