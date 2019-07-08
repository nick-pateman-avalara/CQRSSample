using MediatR;
using CQRSAPI.Models;
using System.Collections.Generic;
using CQRSAPI.Queries;

namespace CQRSAPI.Requests
{

    public class GetAllPeopleRequest : IRequest<List<Person>>
    {

        public int PageSize { get; set; }

        public int PageNumber { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public IntegerQuery Age { get; set; }

    }

}
