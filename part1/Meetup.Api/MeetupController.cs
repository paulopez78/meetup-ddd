﻿using System;
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
                MeetupId = meetup.Id.Value,
                Title = meetup.Title.Value,
                Location = meetup.Location.Value,
                NumberOfSeats = meetup.NumberOfSeats.Value,
                State = meetup.State.ToString()
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

        [HttpPut("publish")]
        public async Task<ActionResult> Put(Meetup.V1.Publish request)
        {
            await _appService.Handle(request);
            return Ok();
        }

        [HttpPut("acceptrsvp")]
        public async Task<ActionResult> Put(Meetup.V1.AcceptRSVP request)
        {
            await _appService.Handle(request);
            return Ok();
        }

        [HttpPut("declinersvp")]
        public async Task<ActionResult> Put(Meetup.V1.DeclineRSVP request)
        {
            await _appService.Handle(request);
            return Ok();
        }


        [HttpPut("close")]
        public async Task<ActionResult> Put(Meetup.V1.Close request)
        {
            await _appService.Handle(request);
            return Ok();
        }

        [HttpPut("cancel")]
        public async Task<ActionResult> Put(Meetup.V1.Cancel request)
        {
            await _appService.Handle(request);
            return Ok();
        }
    }
}
