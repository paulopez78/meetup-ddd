using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Meetup.Api
{
    [Route("api/meetup")]
    [ApiController]
    public class MeetupQueryController : ControllerBase
    {
        private readonly AttendantsRepository _repo;

        public MeetupQueryController(AttendantsRepository repo)
        {
            _repo = repo;
        }

        [HttpGet("attendants/{id}")]
        public async Task<ActionResult> Get(Guid id)
        {
            var attendandts = await _repo.Get(id);
            return Ok(attendandts);
        }
    }
}
