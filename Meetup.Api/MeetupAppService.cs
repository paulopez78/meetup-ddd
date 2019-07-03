using System;
using System.Threading.Tasks;
using Meetup.Domain;

namespace Meetup.Api
{
    public class MeetupAppService
    {
        private readonly MeetupRepository _repo;
        private readonly LocationValidator _validateLocation;

        public MeetupAppService(MeetupRepository repo, LocationValidator validateLocation)
        {
            _repo = repo;
            _validateLocation = validateLocation;
        }

        public Task Handle(object command) => command switch
        {
            Meetup.V1.Create create =>
                _repo.Save(new MeetupAggregate(
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

            _ => throw new ArgumentException($"Invalid request {nameof(command)}")
        };

        public async Task<MeetupAggregate> Get(Guid id) => await _repo.Get(id);

        private async Task ExecuteCommand(Guid id, Action<MeetupAggregate> command)
        {
            var meetup = await Get(id);
            command(meetup);
            //Dispatch Events Before?

            await _repo.Save(meetup);

            //Dispatch Events After?
        }
    }
}