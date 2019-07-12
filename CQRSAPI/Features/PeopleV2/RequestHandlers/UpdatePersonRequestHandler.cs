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

    public class UpdatePersonRequestHandler : IRequestHandler<UpdatePersonRequest, UpdatePersonResponse>
    {

        private readonly IRepository<Person> _peopleRepository;
        private readonly IPersonValidator _personValidator;
        private readonly IMessageTransport _messageTransport;

        public UpdatePersonRequestHandler(
            IRepository<Person> peopleRepository,
            IPersonValidator personValidator,
            IMessageTransport messageTransport)
        {
            _peopleRepository = peopleRepository;
            _personValidator = personValidator;
            _messageTransport = messageTransport;
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
                await _messageTransport.SendAsync(new PersonEventMessage() { Op = PersonEventMessage.Operation.UpdatedPerson, Id = request.Person.Id });
                return (new UpdatePersonResponse() { Result = ApiResponse<bool>.ResponseType.Ok });
            }
            else
            {
                return (new UpdatePersonResponse());
            }
        }

    }

}
