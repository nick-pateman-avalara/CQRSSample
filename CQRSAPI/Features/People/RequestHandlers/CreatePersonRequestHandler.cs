using System.Threading;
using System.Threading.Tasks;
using CQRSAPI.Data;
using CQRSAPI.Features.People.Messages;
using CQRSAPI.Features.People.Models;
using CQRSAPI.Features.People.Requests;
using CQRSAPI.Features.People.Responses;
using CQRSAPI.Messages;
using CQRSAPI.Responses;
using MediatR;

namespace CQRSAPI.Features.People.RequestHandlers
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

            Person addPerson = await _peopleRepository.AddAsync(request.Person, cancellationToken);
            await _messageTransport.SendAsync(new PersonEventMessage() { Op = PersonEventMessage.Operation.CreatedPerson, Id = addPerson.Id});
            return (new CreatePersonResponse() { Result = ApiResponse<Person>.ResponseType.Ok, Value = addPerson });
        }

    }

}
