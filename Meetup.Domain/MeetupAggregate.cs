#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;

namespace Meetup.Domain
{
    public class MeetupAggregate
    {
        public MeetupId Id { get; private set; } = MeetupId.None;
        public MeetupTitle Title { get; private set; } = MeetupTitle.None;
        public ValidatedLocation Location { get; private set; } = ValidatedLocation.None;
        public NumberOfSeats NumberOfSeats { get; private set; } = NumberOfSeats.None;
        public MeetupState State { get; private set; } = MeetupState.Created;
        private Dictionary<MemberId, DateTime> _going { get; } = new Dictionary<MemberId, DateTime>();
        public IReadOnlyDictionary<MemberId, DateTime> Going => _going;
        private Dictionary<MemberId, DateTime> _notGoing { get; } = new Dictionary<MemberId, DateTime>();
        public IReadOnlyDictionary<MemberId, DateTime> NotGoing => _notGoing;
        private List<object> _events { get; } = new List<object>();
        public IEnumerable<object> Events => _events;

        public MeetupAggregate(
            MeetupId id,
            MeetupTitle title,
            ValidatedLocation location,
            NumberOfSeats seats,
            MeetupState state,
            Dictionary<MemberId, DateTime> going,
            Dictionary<MemberId, DateTime> notGoing)
        {
            Id = id;
            Title = title;
            Location = location;
            NumberOfSeats = seats;
            State = state;
            _going = going;
            _notGoing = notGoing;
        }

        public MeetupAggregate(MeetupId id, MeetupTitle title, ValidatedLocation location) =>
            Apply(new Events.MeetupCreated { MeetupId = id, Title = title, Location = location });

        public void UpdateNumberOfSeats(NumberOfSeats numberOfSeats) =>
            Apply(new Events.NumberOfSeatsUpdated { MeetupId = Id, NumberOfSeats = numberOfSeats });

        public void Publish() => Apply(new Events.MeetupPublished { MeetupId = Id });

        public void Cancel() => Apply(new Events.MeetupCanceled { MeetupId = Id });

        public void Closed() => Apply(new Events.MeetupClosed { MeetupId = Id });

        public void AcceptRSVP(MemberId memberId, DateTime acceptedAt) =>
            Apply(new Events.RSVPAccepted { MeetupId = Id, MemberId = memberId, AcceptedAt = acceptedAt });

        public void DeclineRSVP(MemberId memberId, DateTime declinedAt) =>
            Apply(new Events.RSVPDeclined { MeetupId = Id, MemberId = memberId, DeclinedAt = declinedAt });

        private void EnforceInvariants(object @event)
        {
            var valid = @event switch
            {
                Events.MeetupPublished _ => State == MeetupState.Published && NumberOfSeats != NumberOfSeats.None,
                Events.RSVPDeclined _ => State == MeetupState.Published,
                Events.RSVPAccepted _ => State == MeetupState.Published,
                _ => true
            };

            if (!valid)
            {
                throw new ArgumentException("MeetupAggregate invalid state");
            }
        }

        private void Apply(object @event)
        {
            When(@event);
            EnforceInvariants(@event);
            _events.Add(@event);
        }

        private void When(object @event)
        {
            switch (@event)
            {
                case Events.MeetupCreated created:
                    Id = MeetupId.From(created.MeetupId);
                    Title = new MeetupTitle(created.Title);
                    Location = ValidatedLocation.Parse(created.Location);
                    State = MeetupState.Created;
                    break;
                case Events.NumberOfSeatsUpdated seatsUpdated:
                    NumberOfSeats = NumberOfSeats.From(seatsUpdated.NumberOfSeats);
                    break;
                case Events.MeetupPublished published:
                    State = State.TransitionTo(MeetupState.Published);
                    break;
                case Events.MeetupCanceled canceled:
                    State = State.TransitionTo(MeetupState.Canceled);
                    break;
                case Events.MeetupClosed closed:
                    State = State.TransitionTo(MeetupState.Closed);
                    break;
                case Events.RSVPAccepted accepted:
                    _going.Add(new MemberId(accepted.MemberId), accepted.AcceptedAt);
                    break;
                case Events.RSVPDeclined declined:
                    _notGoing.Add(new MemberId(declined.MemberId), declined.DeclinedAt);
                    break;
            }
        }
    }

    public class MeetupState
    {
        public static readonly MeetupState Canceled = new MeetupState(nameof(Canceled));
        public static readonly MeetupState Closed = new MeetupState(nameof(Closed));
        private static readonly MeetupState published = new MeetupState(nameof(Published), Canceled, Closed);
        public static readonly MeetupState Created = new MeetupState(nameof(Created), Published, Canceled, Closed);

        private string _name;
        private MeetupState[] _allowedTransitions;

        public static MeetupState Published => published;

        private MeetupState(string name, params MeetupState[] allowedTransitions)
        {
            _name = name;
            _allowedTransitions = allowedTransitions ?? new MeetupState[] { };
        }

        public MeetupState TransitionTo(MeetupState toState)
        {
            if (_allowedTransitions.All(x => x != toState))
            {
                throw new ArgumentException($"Invalid transition from {_name} to {toState._name}");
            }

            return toState;
        }

        public override string ToString() => _name;
    }
}
