using MediatR;
using CQRSAPI.Models;
using CQRSAPI.Requests;
using System.Threading;
using System.Threading.Tasks;
using CQRSAPI.Responses;
using CQRSAPI.Data;
using CQRSAPI.Messages;

namespace CQRSAPI.RequestHandlers
{

    public class UpdatePersonRequestHandler : IRequestHandler<UpdatePersonRequest, UpdatePersonResponse>
    {

        private readonly IRepository<Person> _peopleRepository;
        private readonly IPersonValidator _personValidator;

        public UpdatePersonRequestHandler(
            IRepository<Person> peopleRepository,
            IPersonValidator personValidator)
        {
            _peopleRepository = peopleRepository;
            _personValidator = personValidator;
        }

        public async Task<UpdatePersonResponse> Handle(UpdatePersonRequest request, CancellationToken cancellationToken)
        {
            if (!_personValidator.IsValid(request.Person, out var errors))
            {
                return new UpdatePersonResponse() { Success = false, Errors = errors };
            }

            int affectedRows = await _peopleRepository.UpdateAsync(request.Person, cancellationToken);
            if (affectedRows > 0)
            {
                await PeopleRabbitMQMessageTransport.Instance.PublishAsync(new PersonEventMessage() { Op = PersonEventMessage.Operation.UpdatedPerson, Id = request.Person.Id });
                return (new UpdatePersonResponse() { Success = true });
            }
            else
            {
                return (new UpdatePersonResponse());
            }
        }

    }

}
