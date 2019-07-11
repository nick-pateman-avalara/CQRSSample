using CQRSAPI.Features.People.Responses;
using MediatR;

namespace CQRSAPI.Features.People.Requests
{

    public class GetPersonRequest : IRequest<GetPersonResponse>
    {

        public int Id { get; set; }

    }

}
