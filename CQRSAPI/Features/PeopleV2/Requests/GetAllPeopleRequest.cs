using System.Collections.Generic;
using CQRSAPI.Features.PeopleV2.Responses;
using MediatR;

namespace CQRSAPI.Features.PeopleV2.Requests
{

    public class GetAllPeopleRequest : IRequest<GetAllPeopleResponse>
    {

        public int PageSize { get; set; }

        public int PageNumber { get; set; }

        public List<KeyValuePair<string, string>> QueryParams { get; set; }

    }

}
