using MediatR;
using CQRSAPI.Models;
using System.Collections.Generic;

namespace CQRSAPI.Requests
{

    public class GetAllPeopleRequest : IRequest<List<Person>>
    {

    }

}
