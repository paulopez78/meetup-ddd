using System.Threading.Tasks;
using Meetup.Domain;
using Microsoft.Extensions.Logging;

namespace Meetup.Api
{
    public class MeetupPublishedEventHandler : IMessageHandler<Events.MeetupPublished>
    {
        private readonly ILogger<MeetupPublishedEventHandler> _logger;

        public MeetupPublishedEventHandler(ILogger<MeetupPublishedEventHandler> logger) => _logger = logger;

        public Task Handle(Events.MeetupPublished message)
        {
            _logger.LogInformation($"SENDING EMAIL, Meetup {message.MeetupId} Published");
            return Task.CompletedTask;
        }
    }
}