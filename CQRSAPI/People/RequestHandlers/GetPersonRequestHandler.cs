using System.Threading;
using System.Threading.Tasks;
using CQRSAPI.Data;
using CQRSAPI.People.Models;
using CQRSAPI.People.Requests;
using CQRSAPI.People.Responses;
using CQRSAPI.Responses;
using MediatR;

namespace CQRSAPI.People.RequestHandlers
{

    public class GetPersonRequestHandler : IRequestHandler<GetPersonRequest, GetPersonResponse>
    {

        private readonly IRepository<Person> _peopleRepository;

        public GetPersonRequestHandler(IRepository<Person> peopleRepository)
        {
            _peopleRepository = peopleRepository;
        }

        public async Task<GetPersonResponse> Handle(GetPersonRequest request, CancellationToken cancellationToken)
        {
            Person person = await _peopleRepository.FindAsync(request.Id, cancellationToken);
            GetPersonResponse response = new GetPersonResponse()
            {
                Result = person != null ? ApiResponse<Person>.ResponseType.Ok : ApiResponse<Person>.ResponseType.NotFound,
                Value = person
            };
            return (response);
        }

    }

}
