using System;

namespace Meetup.Domain
{
    public class Events
    {
        public class MeetupCreated
        {
            public Guid MeetupId { get; set; }
            public string Title { get; set; }
            public string Location { get; set; }
        }

        public class NumberOfSeatsUpdated
        {
            public Guid MeetupId { get; set; }
            public int NumberOfSeats { get; set; }
        }

        public class MeetupPublished
        {
            public Guid MeetupId { get; set; }
        }
        public class MeetupCanceled
        {
            public Guid MeetupId { get; set; }
        }

        public class MeetupClosed
        {
            public Guid MeetupId { get; set; }
        }

        public class RSVPAccepted
        {
            public Guid MeetupId { get; set; }
            public Guid MemberId { get; set; }
            public DateTime AcceptedAt { get; set; }
        }

        public class RSVPDeclined
        {
            public Guid MeetupId { get; set; }
            public Guid MemberId { get; set; }
            public DateTime DeclinedAt { get; set; }
        }
    }
}