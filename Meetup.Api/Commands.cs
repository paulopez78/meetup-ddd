using System;

namespace Meetup.Api
{
    public static class Meetup
    {
        public static class V1
        {
            public class Create
            {
                public Guid MeetupId { get; set; }
                public string Title { get; set; }
                public string Location { get; set; }
            }

            public class UpdateNumberOfSeats
            {
                public Guid MeetupId { get; set; }
                public int NumberOfSeats { get; set; }
            }

            public class Publish
            {
                public Guid MeetupId { get; set; }
            }
        }
    }
}