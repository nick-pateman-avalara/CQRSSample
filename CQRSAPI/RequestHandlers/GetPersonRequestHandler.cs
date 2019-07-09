using MediatR;
using CQRSAPI.Requests;
using System.Threading;
using System.Threading.Tasks;
using CQRSAPI.Models;
using CQRSAPI.Data;

namespace CQRSAPI.RequestHandlers
{

    public class GetPersonRequestHandler : IRequestHandler<GetPersonRequest, Person>
    {

        private readonly IRepository<Person> _peopleRepository;

        public GetPersonRequestHandler(IRepository<Person> peopleRepository)
        {
            _peopleRepository = peopleRepository;
        }

        public async Task<Person> Handle(GetPersonRequest request, CancellationToken cancellationToken)
        {
            return await _peopleRepository.FindAsync(request.Id, cancellationToken);
        }

    }

}
