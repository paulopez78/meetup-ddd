using System;
using AutoFixture;
using Xunit;
using Meetup.Domain;
using System.Collections.Generic;

namespace Meetup.Domain.Tests
{
    public class MeetupStateTests
    {
        [Theory]
        [MemberData(nameof(AllowedTransitions))]
        public void Given_State_When_TransitionAllowedState_Then_Transitioned(MeetupState from, MeetupState to) =>
            Assert.Equal(to, from.TransitionTo(to));

        [Theory]
        [MemberData(nameof(InvalidTransitions))]
        public void Given_State_When_TransitionInvalidState_Then_Throws(MeetupState from, MeetupState to) =>
            Assert.Throws<ArgumentException>(() => from.TransitionTo(to));

        public static IEnumerable<object[]> AllowedTransitions =>
            new List<object[]>
            {
                new object[] { MeetupState.Created, MeetupState.Published },
                new object[] { MeetupState.Created, MeetupState.Canceled},
                new object[] { MeetupState.Created, MeetupState.Closed},

                new object[] { MeetupState.Published, MeetupState.Canceled},
                new object[] { MeetupState.Published, MeetupState.Closed},
            };

        public static IEnumerable<object[]> InvalidTransitions =>
            new List<object[]>
            {
                new object[] { MeetupState.Canceled, MeetupState.Created},
                new object[] { MeetupState.Canceled, MeetupState.Published},

                new object[] { MeetupState.Closed, MeetupState.Created},
                new object[] { MeetupState.Closed, MeetupState.Published},

                new object[] { MeetupState.Published, MeetupState.Created},
            };
    }
}
