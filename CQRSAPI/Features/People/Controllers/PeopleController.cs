using System.Net;
using System.Threading.Tasks;
using CQRSAPI.Controllers;
using CQRSAPI.Features.People.Messages;
using CQRSAPI.Features.People.Models;
using CQRSAPI.Features.People.Requests;
using CQRSAPI.Features.People.Responses;
using CQRSAPI.Helpers;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CQRSAPI.Features.People.Controllers
{

    [Route("api/people"),
     ApiController]
    public class PeopleController : ApiControllerBase
    {

        private static IMediator _mediator;

        public PeopleController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet, 
         ProducesResponseType((int)HttpStatusCode.OK),
         ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> GetAllPeople(
            int pageSize = 50,
            int pageNumber = 1)
        {
            await PeopleRabbitMqMessageTransport.SendIfInitialisedAsync(new PersonEventMessage());

            GetAllPeopleRequest request = new GetAllPeopleRequest()
            {
                PageSize = pageSize,
                PageNumber = pageNumber,
                QueryParams = QueryHelpers.ExtractQueryParamsFromRequest(HttpContext.Request, "pageSize", "pageNumber")
            };

            GetAllPeopleResponse getAllPeopleResponse = await _mediator.Send(request);
            return (ProcessApiResponse(getAllPeopleResponse));
        }

        [HttpGet("{id}"), 
         ProducesResponseType((int)HttpStatusCode.OK), 
         ProducesResponseType((int)HttpStatusCode.BadRequest), 
         ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> GetPerson(int? id)
        {
            if (!id.HasValue)
            {
                return (BadRequest("Missing id parameter."));
            }

            GetPersonResponse response = await _mediator.Send(new GetPersonRequest() { Id = id.Value });
            return (ProcessApiResponse(response));
        }

        [HttpPost, 
         ProducesResponseType((int)HttpStatusCode.OK), 
         ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> CreatePerson([FromBody] Person person)
        {
            if (ModelState.IsValid)
            {
                CreatePersonResponse response = await _mediator.Send(new CreatePersonRequest() { Person = person });
                return (ProcessApiResponse(response));
            }
            else
            {
                return (BadRequest("Request body Json data could not be deserialised to Person."));
            }
        }

        [HttpPost("{id}"), 
         ProducesResponseType((int)HttpStatusCode.OK), 
         ProducesResponseType((int)HttpStatusCode.BadRequest), 
         ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> UpdatePerson(
            int id,
            [FromBody] Person person)
        {
            if(person.Id != id)
            {
                return(BadRequest("Missing id parameter."));
            }

            if (ModelState.IsValid)
            {
                UpdatePersonResponse response = await _mediator.Send(new UpdatePersonRequest() { Person = person });
                return (ProcessApiResponse(response));
            }
            else
            {
                return (BadRequest("Request body Json data could not be deserialised to Person."));
            }
        }

        [HttpDelete("{id}"),
         ProducesResponseType((int)HttpStatusCode.OK),
         ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            DeletePersonResponse response = await _mediator.Send(new DeletePersonRequest() { Id = id });
            return (ProcessApiResponse(response));
        }

    }
}
