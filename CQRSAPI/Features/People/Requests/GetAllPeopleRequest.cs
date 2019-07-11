using System.Collections.Generic;
using CQRSAPI.Features.People.Responses;
using MediatR;

namespace CQRSAPI.Features.People.Requests
{

    public class GetAllPeopleRequest : IRequest<GetAllPeopleResponse>
    {

        public int PageSize { get; set; }

        public int PageNumber { get; set; }

        public List<KeyValuePair<string, string>> QueryParams { get; set; }

    }

}
