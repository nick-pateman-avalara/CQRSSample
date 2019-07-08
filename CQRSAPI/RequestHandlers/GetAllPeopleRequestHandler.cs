using MediatR;
using CQRSAPI.Models;
using CQRSAPI.Requests;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using CQRSAPI.Queries;

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
            IQueryable<Person> allPeople = _peopleContext.Set<Person>();

            if (!string.IsNullOrEmpty(request.FirstName))
            {
                allPeople = allPeople.Where(p => p.FirstName == request.FirstName);
            }

            if (!string.IsNullOrEmpty(request.LastName))
            {
                allPeople = allPeople.Where(p => p.LastName == request.LastName);
            }

            if (request.Age != null)
            {
                switch (request.Age.Operator)
                {
                    case IntegerQuery.ComparisonOperator.Equals:
                    {
                        allPeople = allPeople.Where(p => p.Age == request.Age.Value);
                        break;
                    }
                    case IntegerQuery.ComparisonOperator.LessThan:
                    {
                        allPeople = allPeople.Where(p => p.Age < request.Age.Value);
                        break;
                    }
                    case IntegerQuery.ComparisonOperator.GreaterThan:
                    {
                        allPeople = allPeople.Where(p => p.Age > request.Age.Value);
                        break;
                    }
                }
            }

            //Paging
            IQueryable<Person> currentPage = allPeople
                .Skip(request.PageSize * request.PageNumber)
                .Take(request.PageSize);
            return (currentPage.ToList());
        }

    }

}
