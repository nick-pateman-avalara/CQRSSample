using System.Collections.Specialized;
using MediatR;
using CQRSAPI.Responses;

namespace CQRSAPI.Requests
{

    public class GetAllPeopleRequest : IRequest<GetAllPeopleResponse>
    {

        public int PageSize { get; set; }

        public int PageNumber { get; set; }

        public NameValueCollection QueryParams { get; set; }

    }

}
