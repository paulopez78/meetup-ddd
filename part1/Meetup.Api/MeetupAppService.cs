using System;
using System.Linq;
using System.Threading.Tasks;
using EasyNetQ;
using Meetup.Domain;

namespace Meetup.Api
{
    public class MeetupAppService
    {
        private readonly MeetupRepository _repo;
        private readonly AttendantsRepository _attendantsRepo;
        private readonly LocationValidator _validateLocation;
        private readonly IBus _bus;

        public MeetupAppService(MeetupRepository repo, AttendantsRepository attendantsRepo, IBus bus, LocationValidator validateLocation)
        {
            _repo = repo;
            _attendantsRepo = attendantsRepo;
            _validateLocation = validateLocation;
            _bus = bus;
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

        public async Task<MeetupAggregate> Get(Guid id) => await _repo.Get(id);

        private async Task ExecuteCommand(Guid id, Action<MeetupAggregate> command)
        {
            var meetup = await Get(id);
            command(meetup);
            await ExecuteTransaction(meetup);
        }

        private async Task ExecuteTransaction(MeetupAggregate meetup)
        {
            // Inline Projection (Read your writes)
            var newState = await Project(meetup);

            await _repo.Save(meetup);
            await _attendantsRepo.Save(newState);

            // Dispatch Events After
            foreach (var @event in meetup.Events)
            {
                await _bus.PublishAsync((dynamic)@event);
            }
        }

        private async Task<AttendantsReadModel> Project(MeetupAggregate meetup)
        {
            var state = await _attendantsRepo.Get(meetup.Id);
            if (state == null) state = new AttendantsReadModel();
            var newState = new AttendantsProjection().Project(state, meetup.Events.ToArray());
            return newState;
        }
    }
}