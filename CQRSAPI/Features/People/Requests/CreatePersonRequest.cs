using CQRSAPI.Features.People.Models;
using CQRSAPI.Features.People.Responses;
using MediatR;

namespace CQRSAPI.Features.People.Requests
{

    public class CreatePersonRequest : IRequest<CreatePersonResponse>
    {

        public Person Person { get; set; }

    }

}
