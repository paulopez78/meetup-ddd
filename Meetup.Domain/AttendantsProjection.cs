using System;
using System.Collections.Generic;
using System.Linq;

namespace Meetup.Domain
{
    public class AttendantsProjection
    {
        public AttendantsProjection()
        {
        }

        public AttendantsReadModel Project(params object[] events) => Project(new AttendantsReadModel(), events);
        public AttendantsReadModel Project(AttendantsReadModel readModel, params object[] events) => events.Aggregate(readModel, When);

        public AttendantsReadModel When(AttendantsReadModel state, object @event)
        {
            switch (@event)
            {
                case Events.NumberOfSeatsUpdated seats:
                    state.Capacity = seats.NumberOfSeats;
                    break;
                case Events.RSVPAccepted accepted:
                    if (state.Capacity > state.Going.Count)
                    {
                        state.Going.Add(accepted.MemberId);
                    }
                    else
                    {
                        state.Waiting.Add(accepted.MemberId);
                    }
                    break;
                case Events.RSVPDeclined declined:
                    state.Going.Remove(declined.MemberId);
                    state.NotGoing.Add(declined.MemberId);
                    UpdateWaitingList();
                    break;
            }

            void UpdateWaitingList()
            {
                if (state.Capacity > state.Going.Count)
                {
                    var first = state.Waiting.First();
                    state.Going.Add(first);
                    state.Waiting.Remove(first);
                }
            }

            return state;
        }
    }

    public class AttendantsReadModel
    {
        public int Capacity { get; set; }
        public List<Guid> Waiting { get; internal set; } = new List<Guid>();
        public List<Guid> Going { get; internal set; } = new List<Guid>();
        public List<Guid> NotGoing { get; internal set; } = new List<Guid>();
    }
}