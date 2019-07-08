using System;
using AutoFixture;
using Xunit;
using Meetup.Domain;
using System.Collections.Generic;

#nullable enable
namespace Meetup.Domain.Tests
{
    public class ValueObjectsTests
    {
        [Theory]
        [InlineData(-1)]
        [InlineData(-0)]
        [InlineData(1001)]
        public void Given_InvalidNumberOfSeats_When_Create_Then_Throws(int numberOfSeats)
        {
            Action meetup = () => NumberOfSeats.From(numberOfSeats);
            Assert.Throws<ArgumentException>(meetup);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(1000)]
        public void Given_ValidNumberOfSeats_When_Create_Then_Throws(int numberOfSeats)
        {
            var seats = NumberOfSeats.From(numberOfSeats);
            Assert.Equal(numberOfSeats, seats);
        }

        [Fact]
        public void None_Tests()
        {
            var noneSeats = NumberOfSeats.None;
            Assert.Equal(0, noneSeats);
        }

        [Fact]
        public void Test_NumberOfSeats_Equality()
        {
            var numberOfSeats = 4;
            var number1 = NumberOfSeats.From(numberOfSeats);
            var number2 = NumberOfSeats.From(numberOfSeats);
            Assert.Equal(number1, number2);
            Assert.True(number1 == number2);
        }

        [Fact]
        public void Test_Location()
        {
            var numberOfSeats = 4;
            var number1 = NumberOfSeats.From(numberOfSeats);
            var number2 = NumberOfSeats.From(numberOfSeats);
            Assert.Equal(number1, number2);
            Assert.True(number1 == number2);
        }
    }
}
