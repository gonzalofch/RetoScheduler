using FluentAssertions;
using RetoScheduler.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RetoSchedulerTest.Extensions
{
    public class TimeOnlyExtensionsTests
    {
        [Fact]
        public void Should_Be_Expected_String()
        {
            var res1 = new TimeOnly(5,0,0).ParseAmPm();
            var res2 = new TimeOnly(8, 5, 55).ParseAmPm();
            var res3 = new TimeOnly(10, 30, 43).ParseAmPm();
            var res4 = new TimeOnly(12, 0, 0).ParseAmPm();
            var res5 = new TimeOnly(12, 30, 40).ParseAmPm();
            var res6 = new TimeOnly(16, 40, 0).ParseAmPm();
            var res7 = new TimeOnly(23, 45, 10).ParseAmPm();
            var res8 = new TimeOnly(0, 0, 0).ParseAmPm();

            res1.Should().Be("5:00:00 AM");
            res2.Should().Be("8:05:55 AM");
            res3.Should().Be("10:30:43 AM");
            res4.Should().Be("12:00:00 PM");
            res5.Should().Be("12:30:40 PM");
            res6.Should().Be("4:40:00 PM");
            res7.Should().Be("11:45:10 PM");
            res8.Should().Be("12:00:00 AM");
        }
    }
}
