using System;
using AutoFixture;
using Xunit;
using System.Collections.Generic;
using static Meetup.Domain.Tests.MeetupTestExtensions;
using System.Linq;

#nullable enable
namespace Meetup.Domain.Tests
{
    public class MeetupTests
    {
        [Fact]
        public void Given_None_Meetup_When_Create_Then_MeetupCreated()
        {
            var meetup = Given_Created_Meetup();

            meetup.Events.AssertLast<Events.MeetupCreated>(@event =>
            {
                Assert.Equal(title, @event.Title);
                Assert.Equal(validatedLocation, @event.Location);
            });

            Assert.Equal(title, meetup.Title);
            Assert.Equal(validatedLocation, meetup.Location);
            Assert.Equal(NumberOfSeats.None, meetup.NumberOfSeats);
            Assert.Equal(MeetupState.Created, meetup.State);
        }

        [Fact]
        public void Given_Created_Meetup_When_Publish_Then_Throws() =>
            Given_Created_Meetup<ArgumentException>(
                when: meetup => meetup.Publish()
            );

        [Fact]
        public void Given_Created_Meetup_When_Publish_WithSeats_Then_Published() =>
            Given_Created_Meetup(
                when: meetup => meetup.UpdateNumberOfSeats().Publish(),
                then: meetup =>
                {
                    Assert.Equal(MeetupState.Published, meetup.State);
                    meetup.Events.AssertLast<Events.MeetupPublished>();
                }
            );

        [Fact]
        public void Given_PublishedMeetup_WhenAcceptRSVP_Then_MemberGoing()
        {
            var memberId = Auto.Create<MemberId>();
            var acceptedAt = Auto.Create<DateTime>();

            Given_Published_Meetup(
                when: meetup => meetup.AcceptRSVP(memberId, acceptedAt),
                then: meetup =>
                {
                    meetup.Going.AssertContains((memberId, acceptedAt));
                    meetup.Events.AssertLast<Events.RSVPAccepted>(@event =>
                    {
                        Assert.Equal(memberId, @event.MemberId);
                        Assert.Equal(acceptedAt, @event.AcceptedAt);
                    });
                }
            );
        }

        [Fact]
        public void Given_CreatedMeetup_When_AcceptRSVP_Then_Throws()
        {
            var memberId = Auto.Create<MemberId>();
            var acceptedAt = Auto.Create<DateTime>();

            Given_Created_Meetup<ArgumentException>(
                when: meetup => meetup.AcceptRSVP(memberId, acceptedAt)
            );
        }

        [Fact]
        public void Given_PublishedMeetup_When_Decline_RSVP_Then_MemberNotGoing()
        {
            var memberId = Auto.Create<MemberId>();
            var declinedAt = Auto.Create<DateTime>();

            Given_Published_Meetup(
                when: meetup => meetup.DeclineRSVP(memberId, declinedAt),
                then: meetup =>
                {
                    meetup.NotGoing.AssertContains((memberId, declinedAt));
                    meetup.Events.AssertLast<Events.RSVPDeclined>(@event =>
                    {
                        Assert.Equal(memberId, @event.MemberId);
                        Assert.Equal(declinedAt, @event.DeclinedAt);
                    });
                }
            );
        }
    }

    public static class MeetupTestExtensions
    {
        public static void AssertContains(this IReadOnlyDictionary<MemberId, DateTime> @this, (MemberId id, DateTime at) member)
        {
            Assert.Equal(new Dictionary<MemberId, DateTime>
            {
                {member.id, member.at}
            }, @this);
        }

        public static void AssertLast<TEvent>(this IEnumerable<object> @this) =>
            @this.AssertLast<TEvent>(_ => { });

        public static void AssertLast<TEvent>(this IEnumerable<object> @this, Action<TEvent> assert)
        {
            var @event = @this.Last();
            Assert.IsType<TEvent>(@event);
            assert((TEvent)@event);
        }

        public static Fixture Auto { get; } = new Fixture();
        public static MeetupId id = Auto.Create<MeetupId>();
        public static MeetupTitle title = Auto.Create<MeetupTitle>();
        public static NumberOfSeats numberOfSeats = Auto.Create<NumberOfSeats>();
        public static ValidatedLocation validatedLocation = new ValidatedLocation(location => true, "Seattle, Redmond");

        public static MeetupAggregate Given_Published_Meetup(
            Action<MeetupAggregate> when,
            Action<MeetupAggregate> then)
        {
            var meetup = Given_Created_Meetup();
            meetup.UpdateNumberOfSeats(numberOfSeats);
            meetup.Publish();
            when(meetup);
            then(meetup);
            return meetup;
        }

        public static MeetupAggregate Given_NoneMeetup(Func<MeetupAggregate> when, Action<MeetupAggregate> then)
        {
            var meetup = when();
            then(meetup);
            return meetup;
        }

        public static MeetupAggregate Given_Created_Meetup(Action<MeetupAggregate> when, Action<MeetupAggregate> then)
        {
            var meetup = Given_Created_Meetup();
            when(meetup);
            then(meetup);
            return meetup;
        }

        public static MeetupAggregate Given_Created_Meetup<T>(Action<MeetupAggregate> when) where T : Exception
        {
            var meetup = Given_Created_Meetup();
            Assert.Throws<T>(() => when(meetup));
            return meetup;
        }

        public static MeetupAggregate Given_Published_Meetup()
        {
            var meetup = Given_Created_Meetup();
            meetup.UpdateNumberOfSeats(NumberOfSeats.From(10));
            meetup.Publish();
            return meetup;
        }

        public static MeetupAggregate Given_Created_Meetup() => new MeetupAggregate(id, title, validatedLocation);

        public static MeetupAggregate UpdateNumberOfSeats(this MeetupAggregate @this)
        {
            @this.UpdateNumberOfSeats(numberOfSeats);
            return @this;
        }

        public static MeetupAggregate Publish(this MeetupAggregate @this)
        {
            @this.Publish();
            return @this;
        }
    }
}
