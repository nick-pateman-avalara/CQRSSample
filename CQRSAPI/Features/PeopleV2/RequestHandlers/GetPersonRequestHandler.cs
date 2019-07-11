using System.Threading;
using System.Threading.Tasks;
using CQRSAPI.Data;
using CQRSAPI.Features.PeopleV2.Models;
using CQRSAPI.Features.PeopleV2.Requests;
using CQRSAPI.Features.PeopleV2.Responses;
using CQRSAPI.Responses;
using MediatR;

namespace CQRSAPI.Features.PeopleV2.RequestHandlers
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
