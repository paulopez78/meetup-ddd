using System;
using System.Collections.Generic;

namespace Meetup.Domain
{
    public class MeetupId : ValueObject
    {
        public MeetupId(Guid meetupId)
        {
            if (meetupId == default) throw new ArgumentException(nameof(meetupId));
            Value = meetupId;
        }
        private MeetupId() { }

        public Guid Value { get; private set; }
        public static MeetupId None => new MeetupId { Value = default };

        public static implicit operator Guid(MeetupId id) => id.Value;

        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return Value;
        }

        public static MeetupId From(Guid meetupId) => new MeetupId(meetupId);
    }
}