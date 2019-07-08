using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Meetup.Api
{
    [Route("api/meetup")]
    [ApiController]
    public class MeetupQueryController : ControllerBase
    {

        public MeetupQueryController()
        {
        }

        [HttpGet("attendants/{id}")]
        public async Task<ActionResult> Get(Guid id)
        {
            await Task.Delay(0);
            return Ok(null);
        }
    }
}
