using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CQRSAPI.Data;
using CQRSAPI.Features.PeopleV2.Messages;
using CQRSAPI.Features.PeopleV2.Models;
using CQRSAPI.Features.PeopleV2.Requests;
using CQRSAPI.Features.PeopleV2.Responses;
using CQRSAPI.Messages;
using CQRSAPI.Responses;
using MediatR;

namespace CQRSAPI.Features.PeopleV2.RequestHandlers
{

    public class CreatePersonRequestHandler : IRequestHandler<CreatePersonRequest, CreatePersonResponse>
    {

        private readonly IRepository<Person> _peopleRepository;
        private readonly IPersonValidator _personValidator;
        private readonly IMessageTransport _messageTransport;

        public CreatePersonRequestHandler(
            IRepository<Person> peopleRepository,
            IPersonValidator personValidator,
            IMessageTransport messageTransport)
        {
            _peopleRepository = peopleRepository;
            _personValidator = personValidator;
            _messageTransport = messageTransport;
        }
  
        public async Task<CreatePersonResponse> Handle(CreatePersonRequest request, CancellationToken cancellationToken)
        {
            if(!_personValidator.IsValid(request.Person, out var errors))
            {
                return new CreatePersonResponse() { Result = ApiResponse<Person>.ResponseType.BadRequest, Errors = errors };
            }

            List<Person> existing = await _peopleRepository.FindAllAsync(
                50,
                1,
                new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("FirstName", $"eq.{request.Person.FirstName}"),
                    new KeyValuePair<string, string>("op", "and"),
                    new KeyValuePair<string, string>("LastName", $"eq.{request.Person.LastName}"),
                },
                cancellationToken);

            if (existing.Any())
            {
                return (new CreatePersonResponse() { Result = ApiResponse<Person>.ResponseType.Conflict });
            }
            else
            {
                Person addPerson = await _peopleRepository.AddAsync(request.Person, cancellationToken);
                await _messageTransport.SendAsync(new PersonEventMessage() { Op = PersonEventMessage.Operation.CreatedPerson, Id = addPerson.Id });
                return (new CreatePersonResponse() { Result = ApiResponse<Person>.ResponseType.Ok, Value = addPerson });
            }
        }

    }

}
