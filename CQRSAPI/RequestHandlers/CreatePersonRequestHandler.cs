using MediatR;
using CQRSAPI.Requests;
using System.Threading;
using System.Threading.Tasks;
using CQRSAPI.Data;
using CQRSAPI.Responses;
using CQRSAPI.Models;
using CQRSAPI.Messages;

namespace CQRSAPI.RequestHandlers
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
                return new CreatePersonResponse() { Success = false, Errors = errors };
            }

            Person addPerson = await _peopleRepository.AddAsync(request.Person, cancellationToken);
            await PeopleRabbitMQMessageTransport.Instance.SendLocalAsync(new PersonMessage() { Op = PersonMessage.Operation.CreatedPerson, Id = addPerson.Id});
            return (new CreatePersonResponse() { Success = true, Result = addPerson });
        }

    }

}
