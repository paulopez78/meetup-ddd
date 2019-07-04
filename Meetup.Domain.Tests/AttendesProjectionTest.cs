using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Meetup.Domain.Tests
{
    public class AttendesProjectionTest
    {
        [Fact]
        public void CreateProjectionTest()
        {
            var projection = new AttendantsProjection();

            var jon = Guid.NewGuid();
            var carla = Guid.NewGuid();
            var susan = Guid.NewGuid();

            var readModel = projection.Project(
                new Events.MeetupCreated { Title = "EventSourcing a tope" },
                new Events.NumberOfSeatsUpdated { NumberOfSeats = 2 },
                new Events.MeetupPublished { },
                new Events.RSVPAccepted { MemberId = jon, AcceptedAt = DateTime.UtcNow },
                new Events.RSVPAccepted { MemberId = carla, AcceptedAt = DateTime.UtcNow.AddMinutes(1) },
                new Events.RSVPAccepted { MemberId = susan, AcceptedAt = DateTime.UtcNow.AddMinutes(2) });

            readModel.Going.AssertEqual(jon, carla);
            readModel.Waiting.AssertEqual(susan);
        }
    }

    public static class AttendantsProjectionTestExtensions
    {
        public static void AssertEqual(this List<Guid> list, params Guid[] members) => Assert.Equal(members.Select(x => x), list);
    }

    public class AttendantsProjection
    {
        public AttendantsProjection()
        {
        }

        public AttendantsReadModel Project(params object[] events) => events.Aggregate(new AttendantsReadModel(), When);

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
                    state.NotGoing.Add(declined.MemberId);
                    break;
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