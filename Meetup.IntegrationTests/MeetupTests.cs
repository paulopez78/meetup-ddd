using System;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace Meetup.IntegrationTests
{
    public class MeetupTests : IClassFixture<MeetupClientFixture>
    {
        private readonly MeetupClient _client;

        public MeetupTests(MeetupClientFixture fixture)
        {
            _client = fixture.MeetupClient;
        }

        [Fact]
        public async Task Meetup_Create_Test()
        {
            var meetupId = Guid.NewGuid();
            var title = "EventSourcing CQRS";
            var location = "Seattle, Redmond, Microsoft";

            await _client.Create(meetupId, title, location);

            var meetup = await _client.Get(meetupId);

            Assert.Equal(meetupId, meetup.MeetupId);
            Assert.Equal(location, meetup.Location);
            Assert.Equal(title, meetup.Title);
            Assert.Equal(Meetup.MeetupState.Created, meetup.State);
        }
    }
}
