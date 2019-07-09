using System;
using System.Linq;
using System.Threading.Tasks;
using Marten;
using Meetup.Domain;

namespace Meetup.Api
{
    public class MeetupAppService
    {
        private readonly LocationValidator _validateLocation;
        private readonly IDocumentStore _eventStore;

        public MeetupAppService(IDocumentStore eventStore, LocationValidator validateLocation)
        {
            _validateLocation = validateLocation;
            _eventStore = eventStore;
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

        public async Task<MeetupAggregate> Get(Guid id)
        {
            using var session = _eventStore.OpenSession();
            var events = (await session.Events.FetchStreamAsync(id)).Select(x => x.Data);
            return MeetupAggregate.From(id, events);
        }

        private async Task ExecuteCommand(Guid id, Action<MeetupAggregate> command)
        {
            var meetup = await Get(id);
            command(meetup);
            await ExecuteTransaction(meetup);
        }

        private async Task ExecuteTransaction(MeetupAggregate meetup)
        {
            using var session = _eventStore.OpenSession();
            session.Events.Append(meetup.Id, meetup.Events.ToArray());

            var readModel = (await session.LoadAsync<AttendantsReadModel>(meetup.Id)) ?? new AttendantsReadModel();
            var newReadModel = new AttendantsProjection().Project(readModel, meetup.Events.ToArray());
            session.Store(newReadModel);

            await session.SaveChangesAsync();
        }
    }
}