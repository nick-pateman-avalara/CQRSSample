using MediatR;

namespace CQRSAPI.Requests
{

    public class DeletePersonRequest : IRequest<bool>
    {

        public int Id { get; set; }

    }

}
