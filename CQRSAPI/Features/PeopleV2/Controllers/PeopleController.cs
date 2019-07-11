using System;
using System.Net;
using System.Threading.Tasks;
using CQRSAPI.Controllers;
using CQRSAPI.Feature;
using CQRSAPI.Features.PeopleV2.Feature;
using CQRSAPI.Features.PeopleV2.Messages;
using CQRSAPI.Features.PeopleV2.Models;
using CQRSAPI.Features.PeopleV2.Requests;
using CQRSAPI.Features.PeopleV2.Responses;
using CQRSAPI.Helpers;
using CQRSAPI.Messages;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace CQRSAPI.Features.PeopleV2.Controllers
{

    [Route("api/v2/people")]
    public class PeopleController : ApiControllerBase
    {

        private readonly IMediator _mediator;

        public override ILogger Logger { get; protected set; }

        public PeopleController(
            ILogger<PeopleController> logger,
            IMediator mediator)
        {
            Logger = logger;
            _mediator = mediator;
        }

        [HttpGet("featureInfo")]
        public ActionResult GetFeature()
        {
            IFeature feature = Startup.ApiFeatureController.GetFeatureByName(PeopleFeature.GetName());
            return (feature != null ? (ActionResult)Ok(feature) : NotFound());
        }

        [HttpGet, 
         ProducesResponseType((int)HttpStatusCode.OK),
         ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> GetAllPeople(
            int pageSize = 50,
            int pageNumber = 1)
        {
            await RabbitMqMessageTransport.SendIfInitialisedAsync(new PersonEventMessage());

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
         ProducesResponseType((int)HttpStatusCode.BadRequest),
         ProducesResponseType((int)HttpStatusCode.Conflict)]
        public async Task<IActionResult> CreatePerson([FromBody] Person person)
        {
            if (ModelState.IsValid)
            {
                CreatePersonResponse response = await _mediator.Send(new CreatePersonRequest() { Person = person });
                return (ProcessApiResponse(
                    response,
                    conflictReturnValue: $"Person already exists with the name '{person.FirstName} {person.LastName}'"));
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
