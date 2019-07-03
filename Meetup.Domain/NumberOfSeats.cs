using System;
using System.Collections.Generic;

#nullable enable
namespace Meetup.Domain
{
    public class NumberOfSeats : ValueObject
    {
        public int Value { get; }
        public static NumberOfSeats None = new NumberOfSeats(0);

        public static NumberOfSeats From(int numberOfSeats)
        {
            if (numberOfSeats < 1 || numberOfSeats > 1000) throw new ArgumentException($"Number of seats must be from 1 to 999 {nameof(numberOfSeats)}");
            return new NumberOfSeats(numberOfSeats);
        }

        private NumberOfSeats(int numberOfSeats)
        {
            Value = numberOfSeats;
        }

        public static implicit operator int(NumberOfSeats number) => number.Value;

        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return this.Value;
        }
    }
}