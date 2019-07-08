using MediatR;
using CQRSAPI.Models;

namespace CQRSAPI.Requests
{

    public class GetPersonRequest : IRequest<Person>
    {

        public int PersonId { get; set; }

    }

}
