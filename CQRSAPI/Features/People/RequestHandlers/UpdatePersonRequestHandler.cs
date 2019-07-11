using System.Threading;
using System.Threading.Tasks;
using CQRSAPI.Data;
using CQRSAPI.Features.People.Messages;
using CQRSAPI.Features.People.Models;
using CQRSAPI.Features.People.Requests;
using CQRSAPI.Features.People.Responses;
using CQRSAPI.Responses;
using MediatR;

namespace CQRSAPI.Features.People.RequestHandlers
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
                return new UpdatePersonResponse() { Result = ApiResponse<bool>.ResponseType.BadRequest , Errors = errors };
            }

            int affectedRows = await _peopleRepository.UpdateAsync(request.Person, cancellationToken);
            if (affectedRows > 0)
            {
                await PeopleRabbitMqMessageTransport.SendIfInitialisedAsync(new PersonEventMessage() { Op = PersonEventMessage.Operation.UpdatedPerson, Id = request.Person.Id });
                return (new UpdatePersonResponse() { Result = ApiResponse<bool>.ResponseType.Ok });
            }
            else
            {
                return (new UpdatePersonResponse());
            }
        }

    }

}
