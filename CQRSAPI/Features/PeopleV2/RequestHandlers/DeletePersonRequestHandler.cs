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

    public class DeletePersonRequestHandler : IRequestHandler<DeletePersonRequest, DeletePersonResponse>
    {

        private readonly IRepository<Person> _peopleRepository;
        private readonly IMessageTransport _messageTransport;

        public DeletePersonRequestHandler(
            IRepository<Person> peopleRepository,
            IMessageTransport messageTransport)
        {
            _peopleRepository = peopleRepository;
            _messageTransport = messageTransport;
        }
  
        public async Task<DeletePersonResponse> Handle(DeletePersonRequest request, CancellationToken cancellationToken)
        {
            if(await _peopleRepository.Exists(request.Id, cancellationToken))
            {
                int rowsAffected = await _peopleRepository.DeleteAsync(request.Id, cancellationToken);
                bool success = (rowsAffected == 1);
                if(success) await _messageTransport.SendAsync(new PersonEventMessage() { Op = PersonEventMessage.Operation.DeletedPerson, Id = request.Id });
                return (new DeletePersonResponse() { Result = ApiResponse<bool>.ResponseType.Ok, Value =  success});
            }
            else
            {
                return (new DeletePersonResponse() { Result = ApiResponse<bool>.ResponseType.NotFound, Value = false});
            }
        }

    }

}
