using MediatR;
using CQRSAPI.Models;
using CQRSAPI.Responses;

namespace CQRSAPI.Requests
{

    public class CreatePersonRequest : IRequest<CreatePersonResponse>
    {

        public Person Person { get; set; }

    }

}
