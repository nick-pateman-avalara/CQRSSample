using System.Net;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using CQRSAPI.Models;
using CQRSAPI.Requests;
using CQRSAPI.Responses;
using CQRSAPI.Extensions;
using Microsoft.AspNetCore.Http.Extensions;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Web;
using CQRSAPI.Helpers;

namespace CQRSAPI.Controllers
{

    [Route("api/people"),
     ApiController]
    public class PeopleController : Controller
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
            GetAllPeopleRequest request = new GetAllPeopleRequest()
            {
                PageSize = pageSize,
                PageNumber = pageNumber,
                QueryParams = QueryHelpers.ExtractQueryParamsFromRequest(HttpContext.Request, "pageSize", "pageNumber")
            };

            GetAllPeopleResponse getAllPeopleResponse = await _mediator.Send(request);
            if(getAllPeopleResponse.Success)
            {
                return (Ok(getAllPeopleResponse.Result));
            }
            else
            {
                return (BadRequest(getAllPeopleResponse.Errors.ToStringList()));
            }
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

            Person person = await _mediator.Send(new GetPersonRequest() { Id = id.Value });
            if (person == null)
            {
                return (NotFound($"Person with Id {id} not found."));
            }

            return (Ok(person));
        }

        [HttpPost, 
         ProducesResponseType((int)HttpStatusCode.OK), 
         ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> CreatePerson([FromBody] Person person)
        {
            if (ModelState.IsValid)
            {
                CreatePersonResponse createResponse = await _mediator.Send(new CreatePersonRequest() { Person = person });

                if (createResponse.Success)
                {
                    return (Ok(createResponse.Result));
                }
                else
                {
                    return (BadRequest(createResponse.Errors.ToStringList()));
                }
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
                UpdatePersonResponse updateResponse = await _mediator.Send(new UpdatePersonRequest() { Person = person });
                if(updateResponse.Success)
                {
                    return (Ok(person));
                }
                else
                {
                    if (updateResponse.Errors != null)
                    {
                        return(BadRequest(updateResponse.Errors.ToStringList()));
                    }
                    else
                    {
                        return(NotFound($"Person with Id {id} not found."));
                    }
                }
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
            bool deleted = await _mediator.Send(new DeletePersonRequest() { Id = id });
            return (deleted ? (IActionResult)Ok() : NotFound($"Person with Id {id} not found."));
        }

    }
}
