using FluentAssertions;
using RetoScheduler.Configurations.Limits;
using RetoScheduler.Configurations;
using RetoScheduler.Enums;
using RetoScheduler.Exceptions;
using RetoScheduler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RetoScheduler.Extensions;

namespace RetoSchedulerTest.Extensions
{
    public class DateTimeExtensionsTests
    {
        [Fact]
        public void Should_Be_Next_Day_Of_Week_Execution( )
        {
            var dateTime1= DateTime.Now;
            var dateTime2= DateTime.Now;
            var dateTime3= DateTime.Now;

            var nextDayOfWeek1 = DayOfWeek.Monday;
            var nextDayOfWeek2 = DayOfWeek.Thursday;
            var nextDayOfWeek3 = DayOfWeek.Sunday;
            var res1 = dateTime1.NextDayOfWeek(nextDayOfWeek1);
            var res2 = dateTime1.NextDayOfWeek(nextDayOfWeek2);
            var res3 = dateTime1.NextDayOfWeek(nextDayOfWeek3);
            res1.Should().Be(new DateTime(2024, 4, 15));
            res2.Should().Be(new DateTime(2024, 4, 11));
            res3.Should().Be(new DateTime(2024, 4, 14));
        }
    }
}
