namespace MethodFitness.Tests
{
    using System;

    using NUnit.Framework;

    public class Harness
    {
    }

    [TestFixture]
    public class when_performing_algorythm
    {

        [SetUp]
        public void Setup()
        {

        }

        [Test]
        public void should_return_correct_date()
        {
            var endDate = DateTime.Now;
            int diff = endDate.DayOfWeek - DayOfWeek.Sunday;
            if (diff < 0)
            {
                diff += 7;
            }

            endDate = endDate.AddDays(-1 * diff).Date;

            endDate.ToShortDateString().ShouldEqual("1/13/2013");
        }

    }
}