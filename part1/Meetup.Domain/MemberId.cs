using System;
using System.Collections.Generic;

namespace Meetup.Domain
{
    public class MemberId : ValueObject
    {
        public MemberId(Guid memberId) => Value = memberId;

        public Guid Value { get; }

        public static implicit operator Guid(MemberId id) => id.Value;
        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return Value;
        }
    }
}