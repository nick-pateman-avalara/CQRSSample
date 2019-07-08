using MediatR;

namespace CQRSAPI.Requests
{

    public class DeletePersonRequest : IRequest<bool>
    {

        public int PersonId { get; set; }

    }

}
