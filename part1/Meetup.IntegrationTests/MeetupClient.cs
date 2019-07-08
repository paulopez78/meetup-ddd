using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace Meetup.IntegrationTests
{
    public class MeetupClient
    {
        private HttpClient _client;

        public MeetupClient(HttpClient client, IConfiguration configuration)
        {
            _client = client;
            client.BaseAddress = new Uri(configuration["meetupUrl"] ?? "http://localhost:5000/api/meetup/");
        }

        public Task<HttpResponseMessage> Create(Guid id, string title, string location) =>
            _client.PostAsJsonAsync("", new
            {
                MeetupId = id,
                Title = title,
                Location = location
            });


        public Task<HttpResponseMessage> UpdateSeats(Guid id, int seats) =>
            _client.PutAsJsonAsync("seats", new
            {
                MeetupId = id,
                NumberOfSeats = seats
            });

        public Task<HttpResponseMessage> Publish(Guid id) =>
            _client.PutAsJsonAsync("publish", new
            {
                MeetupId = id,
            });

        public Task<HttpResponseMessage> AcceptRSVP(Guid id, Guid memberId, DateTime acceptedAt) =>
            _client.PutAsJsonAsync("acceptrsvp", new
            {
                MeetupId = id,
                MemberId = memberId,
                AcceptedAt = acceptedAt
            });

        public Task<HttpResponseMessage> RejectRSVP(Guid id, Guid memberId, DateTime rejectedAt) =>
            _client.PutAsJsonAsync("rejectrsvp", new
            {
                MeetupId = id,
                MemberId = memberId,
                RejectedAt = rejectedAt
            });

        public Task<HttpResponseMessage> Cancel(Guid id) =>
            _client.PutAsJsonAsync("cancel", new
            {
                MeetupId = id,
            });

        public Task<HttpResponseMessage> Close(Guid id) =>
            _client.PutAsJsonAsync("close", new
            {
                MeetupId = id,
            });

        public async Task<Meetup> Get(Guid id)
        {
            var response = await _client.GetAsync($"{id}");
            var log = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Get RESPONSE: {log}");
            var content = await response.Content.ReadAsStringAsync();
            return await response.Content.ReadAsAsync<Meetup>();
        }

        public async Task<Attendants> GetAttendants(Guid id)
        {
            var response = await _client.GetAsync($"attendants/{id}");
            var log = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Get RESPONSE: {log}");

            var content = await response.Content.ReadAsStringAsync();
            return await response.Content.ReadAsAsync<Attendants>();
        }
    }

    public class Meetup
    {
        public Guid MeetupId { get; set; }
        public string Title { get; set; }
        public string Location { get; set; }
        public int NumberOfSeats { get; set; }
        public MeetupState State { get; set; }
        public Dictionary<Guid, DateTime> Going { get; set; }
        public Dictionary<Guid, DateTime> NotGoing { get; set; }

        public enum MeetupState
        {
            Created,
            Published,
            Canceled,
            Closed
        }
    }

    public class Attendants
    {
        public Guid MeetupId { get; set; }
        public List<Guid> Going { get; set; }
        public List<Guid> NotGoing { get; set; }
        public List<Guid> Waiting { get; set; }
    }
}