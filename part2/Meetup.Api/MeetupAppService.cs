using System;
using System.Linq;
using System.Threading.Tasks;
using EasyNetQ;
using Meetup.Domain;

namespace Meetup.Api
{
    public class MeetupAppService
    {
        private readonly LocationValidator _validateLocation;

        public MeetupAppService(LocationValidator validateLocation)
        {
            _validateLocation = validateLocation;
        }

        public Task Handle(object command) => command switch
        {
            Meetup.V1.Create create =>
                ExecuteTransaction(new MeetupAggregate(
                    new MeetupId(create.MeetupId),
                    new MeetupTitle(create.Title),
                    new ValidatedLocation(_validateLocation, create.Location))),

            Meetup.V1.UpdateNumberOfSeats updateSeats =>
                ExecuteCommand(
                    updateSeats.MeetupId,
                    meetup => meetup.UpdateNumberOfSeats(NumberOfSeats.From(updateSeats.NumberOfSeats))),

            Meetup.V1.Publish publish =>
                 ExecuteCommand(
                     publish.MeetupId,
                     meetup => meetup.Publish()),

            Meetup.V1.Close close =>
                 ExecuteCommand(
                     close.MeetupId,
                     meetup => meetup.Close()),

            Meetup.V1.Cancel cancel =>
                 ExecuteCommand(
                     cancel.MeetupId,
                     meetup => meetup.Close()),

            Meetup.V1.AcceptRSVP accept =>
                 ExecuteCommand(
                     accept.MeetupId,
                     meetup => meetup.AcceptRSVP(new MemberId(accept.MemberId), accept.AcceptedAt)),

            Meetup.V1.DeclineRSVP decline =>
                 ExecuteCommand(
                     decline.MeetupId,
                     meetup => meetup.AcceptRSVP(new MemberId(decline.MemberId), decline.DeclinedAt)),

            _ => throw new ArgumentException($"Invalid request {nameof(command)}")
        };

        public Task<MeetupAggregate> Get(Guid id) => throw new NotImplementedException();

        private async Task ExecuteCommand(Guid id, Action<MeetupAggregate> command)
        {
            var meetup = await Get(id);
            command(meetup);
            await ExecuteTransaction(meetup);
        }

        private Task ExecuteTransaction(MeetupAggregate meetup)
        {
            return Task.CompletedTask;
        }
    }
}