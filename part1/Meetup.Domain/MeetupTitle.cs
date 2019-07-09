using System;
using System.Collections.Generic;

namespace Meetup.Domain
{
    public class MeetupTitle : ValueObject
    {
        public MeetupTitle(string title)
        {
            if (string.IsNullOrEmpty(title) || string.IsNullOrWhiteSpace(title) || title.Length > 50)
            {
                throw new ArgumentException($"Invalid title {nameof(title)}");
            }
            Value = title;
        }
        private MeetupTitle() { Value = ""; }

        public string Value { get; private set; }

        public static MeetupTitle None => new MeetupTitle { Value = string.Empty };

        public static implicit operator string(MeetupTitle title) => title.Value;

        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return Value;
        }
    }
}