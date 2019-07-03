using System;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace Meetup.IntegrationTests
{
    public class MeetupTests
    {
        [Fact]
        public async Task Meetup_Create_Test()
        {
            var client = HttpClientFactory.Create();
            client.BaseAddress = new Uri("http://localhost:5000/api/meetup/");

            var meetupId = Guid.NewGuid();
            var title = "EventSourcing CQRS";
            var location = "Seattle, Redmond, Microsoft";

            await client.PostAsJsonAsync("", new
            {
                MeetupId = meetupId,
                Title = title,
                Location = location
            });

            var response = await client.GetAsync(meetupId.ToString());
            var meetup = await response.Content.ReadAsAsync<Meetup>();
            Assert.Equal(meetupId, meetup.Id);
            Assert.Equal(location, meetup.Location);
            Assert.Equal(title, meetup.Title);
            Assert.Equal(Meetup.MeetupState.Created, meetup.State);
        }
    }
    public class Meetup
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Location { get; set; }
        public int NumberOfSeats { get; set; }
        public MeetupState State { get; set; }

        public enum MeetupState
        {
            Created,
            Published,
            Canceled,
            Closed
        }
    }
}
