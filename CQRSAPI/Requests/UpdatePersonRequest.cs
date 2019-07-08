using MediatR;
using CQRSAPI.Models;
using CQRSAPI.Responses;

namespace CQRSAPI.Requests
{

    public class UpdatePersonRequest : IRequest<UpdatePersonResponse>
    {

        public Person Person { get; set; }

    }

}
