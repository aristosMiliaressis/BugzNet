using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net;
using System.Threading.Tasks;
using BugzNet.Application.MediatR.Requests.Identity.Commands;
using MediatR;

namespace BugzNet.Web.Controllers
{
    [AllowAnonymous]
    [Route("api/token")]
    [ApiController]
    public class TokenController : ControllerBase
    {
        private readonly IMediator _mediator;

        public TokenController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Get bearer authentication token
        /// </summary>
        /// <param name="request" type="GenerateTokenCommand"></param>
        /// <response code="200" type="TokenGrantResponse">authentication succeeded</response>
        /// <response code="400">authentication failed</response>
        [HttpPost]
        [AllowAnonymous]
        [ProducesResponseType(typeof(TokenResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
        [ActionName("")]
        public async Task<IActionResult> Token([FromBody] GenerateTokenCommand request)
        {
            var response = await _mediator.Send(request);

            if (response.IsSuccess)
                return Content(JsonConvert.SerializeObject(response.Result), "application/json");
            
            return BadRequest();
        }
    }
}
