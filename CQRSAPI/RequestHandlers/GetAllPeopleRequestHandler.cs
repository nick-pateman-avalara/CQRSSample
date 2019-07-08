using MediatR;
using CQRSAPI.Models;
using CQRSAPI.Requests;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace CQRSAPI.RequestHandlers
{

    public class GetAllPeopleRequestHandler : IRequestHandler<GetAllPeopleRequest, List<Person>>
    {

        private readonly PeopleContext _peopleContext;

        public GetAllPeopleRequestHandler(PeopleContext peopleContext)
        {
            _peopleContext = peopleContext;
        }

        public async Task<List<Person>> Handle(GetAllPeopleRequest request, CancellationToken cancellationToken)
        {
            await Task.Yield();
            return (_peopleContext.Set<Person>().ToList());
        }

    }

}
