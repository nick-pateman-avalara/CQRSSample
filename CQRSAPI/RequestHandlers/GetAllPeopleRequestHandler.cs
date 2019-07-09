using MediatR;
using CQRSAPI.Models;
using CQRSAPI.Requests;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using CQRSAPI.Data;
using CQRSAPI.Responses;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace CQRSAPI.RequestHandlers
{

    public class GetAllPeopleRequestHandler : IRequestHandler<GetAllPeopleRequest, GetAllPeopleResponse>
    {

        private readonly IRepository<Person> _peopleRepository;

        public GetAllPeopleRequestHandler(IRepository<Person> peopleRepository)
        {
            _peopleRepository = peopleRepository;
        }

        public async Task<GetAllPeopleResponse> Handle(GetAllPeopleRequest request, CancellationToken cancellationToken)
        {
            List<ModelError> errors = new List<ModelError>();
            if (request.PageSize <= 0) errors.Add(new ModelError("PageSize must be greater or equal to 1."));
            if (request.PageNumber <= 0) errors.Add(new ModelError("PageNumber must be greater or equal to 1."));

            if (errors.Count == 0)
            {
                List<Person> allPeople = await _peopleRepository.FindAllAsync(
                    request.PageSize,
                    request.PageNumber,
                    cancellationToken);
                return (new GetAllPeopleResponse() { Success = true, Result = allPeople });
            }
            else
            {
                return (new GetAllPeopleResponse() { Success = false, Errors = errors });
            }
        }

    }

}
