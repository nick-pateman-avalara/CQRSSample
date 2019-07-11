using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CQRSAPI.Data;
using CQRSAPI.Features.PeopleV2.Models;
using CQRSAPI.Features.PeopleV2.Requests;
using CQRSAPI.Features.PeopleV2.Responses;
using CQRSAPI.Responses;
using MediatR;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace CQRSAPI.Features.PeopleV2.RequestHandlers
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
                    request.QueryParams,
                    cancellationToken);
                return (new GetAllPeopleResponse() { Result = ApiResponse<List<Person>>.ResponseType.Ok, Value = allPeople });
            }
            else
            {
                return (new GetAllPeopleResponse() { Result = ApiResponse<List<Person>>.ResponseType.BadRequest, Errors = errors });
            }
        }

    }

}
