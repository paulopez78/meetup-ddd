using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using static Meetup.Domain.Tests.AttendantsProjectionTestExtensions;

namespace Meetup.Domain.Tests
{
    public class AttendesProjectionTest
    {
        [Fact]
        public void Test1() =>
            GivenPublishedMeetup(
                when: (project, readModel) =>
                {
                    project.RSVPAccepted(readModel, jon);
                    project.RSVPAccepted(readModel, carla);
                    project.RSVPAccepted(readModel, susan);
                },
                then: readModel =>
                {
                    readModel.Going.AssertEqual(jon, carla);
                    readModel.Waiting.AssertEqual(susan);
                }
            );

        [Fact]
        public void Test2() =>
            GivenPublishedMeetup(
                when: (project, readModel) =>
                {
                    project.RSVPAccepted(readModel, jon);
                    project.RSVPAccepted(readModel, carla);
                    project.RSVPDeclined(readModel, susan);
                },
                then: readModel =>
                {
                    readModel.Going.AssertEqual(jon, carla);
                    readModel.NotGoing.AssertEqual(susan);
                }
            );

        [Fact]
        public void Test3() =>
            GivenPublishedMeetup(
                when: (project, readModel) =>
                {
                    project.RSVPAccepted(readModel, jon);
                    project.RSVPAccepted(readModel, carla);
                    project.RSVPAccepted(readModel, susan);
                    project.RSVPDeclined(readModel, jon);
                },
                then: readModel =>
                {
                    readModel.Going.AssertEqual(carla, susan);
                    readModel.NotGoing.AssertEqual(jon);
                    Assert.Empty(readModel.Waiting);
                }
            );
    }

    public static class AttendantsProjectionTestExtensions
    {
        public static Guid jon = Guid.NewGuid();
        public static Guid carla = Guid.NewGuid();
        public static Guid susan = Guid.NewGuid();

        public static void GivenPublishedMeetup(Action<AttendantsProjection, AttendantsReadModel> when, Action<AttendantsReadModel> then)
        {
            var projection = new AttendantsProjection();
            var readModel = new AttendantsReadModel();

            projection.Project(
                readModel,
                new Events.MeetupCreated { Title = "EventSourcing a tope" },
                new Events.NumberOfSeatsUpdated { NumberOfSeats = 2 },
                new Events.MeetupPublished { });

            when(projection, readModel);
            then(readModel);
        }

        public static AttendantsProjection RSVPAccepted(this AttendantsProjection projection, AttendantsReadModel readModel, Guid memberId)
        {
            projection.Project(readModel, new Events.RSVPAccepted { MemberId = memberId, AcceptedAt = DateTime.UtcNow });
            return projection;
        }

        public static AttendantsProjection RSVPDeclined(this AttendantsProjection projection, AttendantsReadModel readModel, Guid memberId)
        {
            projection.Project(readModel, new Events.RSVPDeclined { MemberId = memberId, DeclinedAt = DateTime.UtcNow });
            return projection;
        }

        public static void AssertEqual(this List<Guid> list, params Guid[] members) => Assert.Equal(members.Select(x => x), list);
    }
}