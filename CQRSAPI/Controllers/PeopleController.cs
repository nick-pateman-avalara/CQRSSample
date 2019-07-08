using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using CQRSAPI.Models;
using CQRSAPI.Requests;
using CQRSAPI.Responses;

namespace CQRSAPI.Controllers
{

    [Route("api/people")]
    [ApiController]
    public class PeopleController : Controller
    {

        private static IMediator _mediator;

        public PeopleController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet, 
         ProducesResponseType(200)]
        public async Task<IActionResult> GetAllPeople()
        {
            return Ok(await _mediator.Send(new GetAllPeopleRequest()));
        }

        [HttpGet("{id}"), 
         ProducesResponseType(200), 
         ProducesResponseType(400), 
         ProducesResponseType(401)]
        public async Task<IActionResult> GetPerson(int? id)
        {
            if (!id.HasValue)
            {
                return BadRequest();
            }

            Person person = await _mediator.Send(new GetPersonRequest() { PersonId = id.Value });
            if (person == null)
            {
                return NotFound();
            }

            return Ok(person);
        }

        [HttpPost, 
         ProducesResponseType(200), 
         ProducesResponseType(400)]
        public async Task<IActionResult> CreatePerson([FromBody] Person person)
        {
            if (ModelState.IsValid)
            {
                CreatePersonResponse createResponse = await _mediator.Send(new CreatePersonRequest() { Person = person });

                if (createResponse.Success)
                {
                    return Ok(createResponse.Result);
                }
                else
                {
                    List<string> errors = createResponse.Errors
                        .Select(me => me.ErrorMessage)
                        .ToList();
                    return BadRequest(errors);
                }
            }

            return BadRequest();
        }

        [HttpPost("{id}"), 
         ProducesResponseType(200), 
         ProducesResponseType(400), 
         ProducesResponseType(401)]
        public async Task<IActionResult> UpdatePerson(
            int id,
            [FromBody] Person person)
        {
            if(person.PersonId != id)
            {
                return(BadRequest());
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
                    List<string> errors = updateResponse.Errors
                        .Select(me => me.ErrorMessage)
                        .ToList();
                    return BadRequest(errors);
                }
            }
            else
            {
                return (BadRequest());
            }
        }

        [HttpDelete("{id}"),
         ProducesResponseType(200),
         ProducesResponseType(404)]
        public async Task<IActionResult> Delete(int id)
        {
            bool deleted = await _mediator.Send(new DeletePersonRequest() { PersonId = id });
            return (deleted ? (IActionResult)Ok() : NotFound());
        }

    }
}
