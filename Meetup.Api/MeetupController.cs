using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Meetup.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class MeetupController : ControllerBase
    {
        private readonly MeetupAppService _appService;

        public MeetupController(MeetupAppService appService)
        {
            _appService = appService;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult> Get(Guid id)
        {
            var meetup = await _appService.Get(id);
            return Ok(new
            {
                Id = meetup.Id,
                Title = meetup.Title,
                Location = meetup.Location,
                NumberOfSeats = meetup.NumberOfSeats,
                State = meetup.State
            });
        }

        [HttpPost]
        public async Task<ActionResult> Post(Meetup.V1.Create request)
        {
            await _appService.Handle(request);
            return Ok();
        }

        [HttpPut("seats")]
        public async Task<ActionResult> Put(Meetup.V1.UpdateNumberOfSeats request)
        {
            await _appService.Handle(request);
            return Ok();
        }
    }
}
