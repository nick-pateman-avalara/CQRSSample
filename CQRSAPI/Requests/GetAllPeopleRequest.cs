using System.Collections.Generic;
using MediatR;
using CQRSAPI.Responses;

namespace CQRSAPI.Requests
{

    public class GetAllPeopleRequest : IRequest<GetAllPeopleResponse>
    {

        public int PageSize { get; set; }

        public int PageNumber { get; set; }

        public List<KeyValuePair<string, string>> QueryParams { get; set; }

    }

}
