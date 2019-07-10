using CQRSAPI.People.Models;
using CQRSAPI.People.Responses;
using MediatR;

namespace CQRSAPI.People.Requests
{

    public class UpdatePersonRequest : IRequest<UpdatePersonResponse>
    {

        public Person Person { get; set; }

    }

}
