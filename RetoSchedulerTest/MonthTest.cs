using FluentAssertions;
using RetoScheduler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RetoSchedulerTest
{
    public class MonthTest
    {
        [Fact]
        public void Should_Get_Days_In_Month()
        {
            Month february2024 = new Month(2024, 2);
            var days = february2024.GetMonthDays();
            days.Should().HaveCount(29);

        }

        [Fact]
        public void Should_Get_Days_In_Month_Selected()
        {
            Month february2024 = new Month(2024, 2);
            var days = february2024.GetMonthDays();
            days.Should().HaveCount(29);
        }

        [Fact]
        public void Should_Get_Days_In_Month_Selected_DayOfWeek()
        {
            Month february2024 = new Month(2024, 2);
            var days = february2024.GetMonthDays(DayOfWeek.Monday);
            days.Should().HaveCount(4);
        }
    }

}
