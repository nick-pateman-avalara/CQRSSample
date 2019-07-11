using CQRSAPI.Features.PeopleV2.Models;
using CQRSAPI.Features.PeopleV2.Responses;
using MediatR;

namespace CQRSAPI.Features.PeopleV2.Requests
{

    public class UpdatePersonRequest : IRequest<UpdatePersonResponse>
    {

        public Person Person { get; set; }

    }

}
