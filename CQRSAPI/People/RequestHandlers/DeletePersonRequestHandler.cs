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

    public class DeletePersonRequestHandler : IRequestHandler<DeletePersonRequest, DeletePersonResponse>
    {

        private readonly IRepository<Person> _peopleRepository;

        public DeletePersonRequestHandler(IRepository<Person> peopleRepository)
        {
            _peopleRepository = peopleRepository;
        }
  
        public async Task<DeletePersonResponse> Handle(DeletePersonRequest request, CancellationToken cancellationToken)
        {
            if(await _peopleRepository.Exists(request.Id, cancellationToken))
            {
                int rowsAffected = await _peopleRepository.DeleteAsync(request.Id, cancellationToken);
                bool success = (rowsAffected == 1);
                if(success) await PeopleRabbitMqMessageTransport.SendIfInitialisedAsync(new PersonEventMessage() { Op = PersonEventMessage.Operation.DeletedPerson, Id = request.Id });
                return (new DeletePersonResponse() { Result = ApiResponse<bool>.ResponseType.Ok, Value =  success});
            }
            else
            {
                return (new DeletePersonResponse() { Result = ApiResponse<bool>.ResponseType.NotFound, Value = false});
            }
        }

    }

}
