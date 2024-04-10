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
        public void Should_Be_Next_Day_Of_Week_Execution()
        {
            var res1 = new DateTime(2024, 4, 10).NextDayOfWeek(DayOfWeek.Monday);
            var res2 = new DateTime(2024, 4, 10).NextDayOfWeek(DayOfWeek.Tuesday);
            var res3 = new DateTime(2024, 4, 10).NextDayOfWeek(DayOfWeek.Wednesday);
            var res4 = new DateTime(2024, 4, 10).NextDayOfWeek(DayOfWeek.Thursday);
            var res5 = new DateTime(2024, 4, 10).NextDayOfWeek(DayOfWeek.Friday);
            var res6 = new DateTime(2024, 4, 10).NextDayOfWeek(DayOfWeek.Saturday);
            var res7 = new DateTime(2024, 4, 10).NextDayOfWeek(DayOfWeek.Sunday);
            res1.Should().Be(new DateTime(2024, 4, 15));
            res2.Should().Be(new DateTime(2024, 4, 16));
            res3.Should().Be(new DateTime(2024, 4, 10));
            res4.Should().Be(new DateTime(2024, 4, 11));
            res5.Should().Be(new DateTime(2024, 4, 12));
            res6.Should().Be(new DateTime(2024, 4, 13));
            res7.Should().Be(new DateTime(2024, 4, 14));
        }
    }
}
