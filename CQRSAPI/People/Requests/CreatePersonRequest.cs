using CQRSAPI.People.Models;
using CQRSAPI.People.Responses;
using MediatR;

namespace CQRSAPI.People.Requests
{

    public class CreatePersonRequest : IRequest<CreatePersonResponse>
    {

        public Person Person { get; set; }

    }

}
