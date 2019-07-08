using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace Meetup.IntegrationTests
{
    public class AttendantsTest : IClassFixture<MeetupClientFixture>
    {
        private readonly MeetupClient _client;

        public AttendantsTest(MeetupClientFixture fixture)
        {
            _client = fixture.MeetupClient;
        }

        [Fact]
        public async Task Attendants_Test()
        {
            var meetupId = Guid.NewGuid();
            var title = "EventSourcing CQRS";
            var location = "Seattle, Redmond, Microsoft";
            var numberOfSeats = 2;
            var jon = Guid.NewGuid();
            var carla = Guid.NewGuid();
            var susan = Guid.NewGuid();

            await _client.Create(meetupId, title, location);
            await _client.UpdateSeats(meetupId, numberOfSeats);
            await _client.Publish(meetupId);
            await _client.AcceptRSVP(meetupId, jon, DateTime.UtcNow);
            await _client.AcceptRSVP(meetupId, carla, DateTime.UtcNow.AddSeconds(1));
            await _client.AcceptRSVP(meetupId, susan, DateTime.UtcNow.AddSeconds(1));

            var attendants = await _client.GetAttendants(meetupId);
            attendants.Going.AssertEqual(jon, carla);
            attendants.Waiting.AssertEqual(susan);
            Assert.Empty(attendants.NotGoing);
        }
    }

    public static class AttendantsTestExtensions
    {
        public static void AssertEqual(this List<Guid> list, params Guid[] members) =>
            Assert.Equal(members.Select(x => x).ToList(), list);
    }
}
