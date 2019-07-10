using CQRSAPI.People.Models;
using CQRSAPI.People.Responses;
using MediatR;

namespace CQRSAPI.People.Requests
{

    public class GetPersonRequest : IRequest<GetPersonResponse>
    {

        public int Id { get; set; }

    }

}
