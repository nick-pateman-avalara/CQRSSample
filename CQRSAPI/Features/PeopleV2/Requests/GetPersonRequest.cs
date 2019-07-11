using CQRSAPI.Features.PeopleV2.Responses;
using MediatR;

namespace CQRSAPI.Features.PeopleV2.Requests
{

    public class GetPersonRequest : IRequest<GetPersonResponse>
    {

        public int Id { get; set; }

    }

}
