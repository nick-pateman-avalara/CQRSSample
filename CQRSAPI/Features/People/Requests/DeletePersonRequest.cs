using CQRSAPI.Features.People.Responses;
using MediatR;

namespace CQRSAPI.Features.People.Requests
{

    public class DeletePersonRequest : IRequest<DeletePersonResponse>
    {

        public int Id { get; set; }

    }

}
