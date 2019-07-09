using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using CQRSAPI.Models;
using CQRSAPI.Requests;
using CQRSAPI.Responses;

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
                PageNumber = pageNumber
            };

            GetAllPeopleResponse getAllPeopleResponse = await _mediator.Send(request);
            if(getAllPeopleResponse.Success)
            {
                return (Ok(getAllPeopleResponse.Result));
            }
            else
            {
                List<string> errors = getAllPeopleResponse.Errors
                    .Select(me => me.ErrorMessage)
                    .ToList();
                return (BadRequest(errors));
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
                return (BadRequest());
            }

            Person person = await _mediator.Send(new GetPersonRequest() { Id = id.Value });
            if (person == null)
            {
                return (NotFound());
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
                    List<string> errors = createResponse.Errors
                        .Select(me => me.ErrorMessage)
                        .ToList();
                    return (BadRequest(errors));
                }
            }

            return (BadRequest());
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
                    if (updateResponse.Errors != null)
                    {
                        List<string> errors = updateResponse.Errors
                            .Select(me => me.ErrorMessage)
                            .ToList();
                        return(BadRequest(errors));
                    }
                    else
                    {
                        return(NotFound());
                    }
                }
            }
            else
            {
                return (BadRequest());
            }
        }

        [HttpDelete("{id}"),
         ProducesResponseType((int)HttpStatusCode.OK),
         ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            bool deleted = await _mediator.Send(new DeletePersonRequest() { Id = id });
            return (deleted ? (IActionResult)Ok() : NotFound());
        }

    }
}
