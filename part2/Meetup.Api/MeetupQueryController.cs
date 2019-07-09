using System;
using System.Threading.Tasks;
using Marten;
using Meetup.Domain;
using Microsoft.AspNetCore.Mvc;

namespace Meetup.Api
{
    [Route("api/meetup")]
    [ApiController]
    public class MeetupQueryController : ControllerBase
    {
        private readonly IDocumentStore _store;

        public MeetupQueryController(IDocumentStore store)
        {
            _store = store;
        }

        [HttpGet("attendants/{id}")]
        public async Task<ActionResult> Get(Guid id)
        {
            var attendants = await _store.Query<AttendantsReadModel>(id);
            return Ok(attendants);
        }

        [HttpGet("v2/attendants/{id}")]
        public async Task<ActionResult> GetV2(Guid id)
        {
            var attendants = await _store.Query<AttendantsReadModel, AttendantsProjection>(id);
            return Ok(attendants);
        }
    }
}
