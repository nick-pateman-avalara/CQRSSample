using System.Threading;
using System.Threading.Tasks;
using CQRSAPI.Data;
using CQRSAPI.People.Messages;
using CQRSAPI.People.Models;
using CQRSAPI.People.Requests;
using CQRSAPI.People.Responses;
using CQRSAPI.Responses;
using MediatR;

namespace CQRSAPI.People.RequestHandlers
{

    public class CreatePersonRequestHandler : IRequestHandler<CreatePersonRequest, CreatePersonResponse>
    {

        private readonly IRepository<Person> _peopleRepository;
        private readonly IPersonValidator _personValidator;

        public CreatePersonRequestHandler(
            IRepository<Person> peopleRepository,
            IPersonValidator personValidator)
        {
            _peopleRepository = peopleRepository;
            _personValidator = personValidator;
        }
  
        public async Task<CreatePersonResponse> Handle(CreatePersonRequest request, CancellationToken cancellationToken)
        {
            if(!_personValidator.IsValid(request.Person, out var errors))
            {
                return new CreatePersonResponse() { Result = ApiResponse<Person>.ResponseType.BadRequest, Errors = errors };
            }

            Person addPerson = await _peopleRepository.AddAsync(request.Person, cancellationToken);
            await PeopleRabbitMqMessageTransport.SendIfInitialisedAsync(new PersonEventMessage() { Op = PersonEventMessage.Operation.CreatedPerson, Id = addPerson.Id});
            return (new CreatePersonResponse() { Result = ApiResponse<Person>.ResponseType.Ok, Value = addPerson });
        }

    }

}
