using CQRSAPI.Features.People.Models;
using CQRSAPI.Features.People.Responses;
using MediatR;

namespace CQRSAPI.Features.People.Requests
{

    public class UpdatePersonRequest : IRequest<UpdatePersonResponse>
    {

        public Person Person { get; set; }

    }

}
