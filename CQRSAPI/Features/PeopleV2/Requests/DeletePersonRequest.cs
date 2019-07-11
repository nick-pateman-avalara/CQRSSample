using CQRSAPI.Features.PeopleV2.Responses;
using MediatR;

namespace CQRSAPI.Features.PeopleV2.Requests
{

    public class DeletePersonRequest : IRequest<DeletePersonResponse>
    {

        public int Id { get; set; }

    }

}
