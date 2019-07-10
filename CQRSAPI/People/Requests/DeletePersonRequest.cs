using CQRSAPI.People.Responses;
using MediatR;

namespace CQRSAPI.People.Requests
{

    public class DeletePersonRequest : IRequest<DeletePersonResponse>
    {

        public int Id { get; set; }

    }

}
