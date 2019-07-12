using System.Net;
using System.Threading.Tasks;
using CQRSAPI.Controllers;
using CQRSAPI.Feature;
using CQRSAPI.Features.PeopleV2.Feature;
using CQRSAPI.Features.PeopleV2.Models;
using CQRSAPI.Features.PeopleV2.Requests;
using CQRSAPI.Features.PeopleV2.Responses;
using CQRSAPI.Helpers;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace CQRSAPI.Features.PeopleV2.Controllers
{

    [Route("api/v2/people")]
    public class PeopleController : ApiControllerBase
    {

        private readonly IMediator _mediator;

        public sealed override ILogger Logger { get; protected set; }

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
            if (ModelState.IsValid)
            {
                GetAllPeopleRequest request = new GetAllPeopleRequest()
                {
                    PageSize = pageSize,
                    PageNumber = pageNumber,
                    QueryParams = QueryHelpers.ExtractQueryParamsFromRequest(HttpContext.Request, "pageSize", "pageNumber")
                };

                GetAllPeopleResponse getAllPeopleResponse = await _mediator.Send(request);
                return (ProcessApiResponse(getAllPeopleResponse));
            }
            else
            {
                return (InvalidModel(ModelState));
            }
        }

        [HttpGet("{id}"), 
         ProducesResponseType(typeof(Person), (int)HttpStatusCode.OK), 
         ProducesResponseType((int)HttpStatusCode.BadRequest), 
         ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> GetPerson(int id)
        {
            if (ModelState.IsValid)
            {
                GetPersonResponse response = await _mediator.Send(new GetPersonRequest() { Id = id });
                return (ProcessApiResponse(response));
            }
            else
            {
                return (InvalidModel(ModelState));
            }
        }

        [HttpPost, 
         ProducesResponseType(typeof(Person), (int)HttpStatusCode.OK), 
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
                return (InvalidModel(ModelState));
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
            if (ModelState.IsValid)
            {
                UpdatePersonResponse response = await _mediator.Send(new UpdatePersonRequest() { Person = person });
                return (ProcessApiResponse(response));
            }
            else
            {
                return (InvalidModel(ModelState));
            }
        }

        [HttpDelete("{id}"),
         ProducesResponseType((int)HttpStatusCode.OK),
         ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            if (ModelState.IsValid)
            {
                DeletePersonResponse response = await _mediator.Send(new DeletePersonRequest() { Id = id });
                return (ProcessApiResponse(response));
            }
            else
            {
                return (InvalidModel(ModelState));
            }
        }

    }
}
