using System;
using System.Collections.Generic;

namespace Meetup.Domain
{
    public delegate bool LocationValidator(string location);

    public class ValidatedLocation : ValueObject
    {
        public ValidatedLocation(LocationValidator isValidLocation, string location)
        {
            if (string.IsNullOrEmpty(location) ||
            string.IsNullOrWhiteSpace(location) ||
            !isValidLocation(location))
            {
                throw new ArgumentException(nameof(location));
            }
            Value = location;
        }
        private ValidatedLocation(string location) => Value = location;

        public string Value { get; private set; }
        public static ValidatedLocation None => From(string.Empty);

        public static implicit operator string(ValidatedLocation id) => id.Value;
        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return Value;
        }

        public static ValidatedLocation From(string location) => new ValidatedLocation(location);
    }
}