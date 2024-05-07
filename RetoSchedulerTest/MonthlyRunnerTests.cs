

using FluentAssertions;
using RetoScheduler;
using RetoScheduler.Configurations;
using RetoScheduler.Enums;
using RetoScheduler.Runners;

namespace RetoSchedulerTest
{
    public class MonthlyRunnerTests
    {
        [Fact]
        public void Should_Be_Next_Date_For_Month_DayOptionNumber()
        {
            var outPut = MonthlyRunner.Run(MonthlyConfiguration.DayOption(5, 2), new DateTime(2024, 1, 1), false);

            outPut.Should().Be(new DateTime(2024, 1, 5));
        }

        [Fact]
        public void Should_Be_Next_Date_For_Month_DayOptionNumber_Skipping_2_Months()
        {
            var outPut = MonthlyRunner.Run(MonthlyConfiguration.DayOption(5, 2), new DateTime(2024, 1, 1), false);

            outPut.Should().Be(new DateTime(2024, 1, 5));
        }

        [Fact]
        public void Should_Be_Next_Date_For_Month_DayOptionNumber_Skipping_2_Months_Date_Is_After_Daynumber()
        {
            var outPut = MonthlyRunner.Run(MonthlyConfiguration.DayOption(5, 2), new DateTime(2024, 1, 6), false);

            outPut.Should().Be(new DateTime(2024, 2, 5));
        }

        [Fact]
        public void Should_Be_Next_Date_For_Month_DayOptionNumber_Skipping_Dates_That_Doesnt_Exist()
        {
            var outPut = MonthlyRunner.Run(MonthlyConfiguration.DayOption(30, 2), new DateTime(2023, 1, 31), false);
            outPut.Should().Be(new DateTime(2023, 3, 30));
        }

        [Fact]
        public void Should_Be_Next_Dates_For_Month_DayOptionNumber_Skipping_2_Months()
        {
            var outPut1 = MonthlyRunner.Run(
                 MonthlyConfiguration.DayOption(30, 2),
                 new DateTime(2023, 1, 31), false);
            outPut1.Should().Be(new DateTime(2023, 3, 30));

            var outPut2 = MonthlyRunner.Run(
                 MonthlyConfiguration.DayOption(30, 2),
                 outPut1, true);
            outPut2.Should().Be(new DateTime(2023, 3, 30));
        }

        [Fact]
        public void Should_Be_Next_Dates_For_Month_DayOptionNumber_Skipping_Months()
        {
            var outPut1 = MonthlyRunner.Run(
                 MonthlyConfiguration.DayOption(29, 1),
                 new DateTime(2023, 1, 17), false);
            outPut1.Should().Be(new DateTime(2023, 1, 29));
        }

        [Fact]
        public void Should_Be_Next_Dates_For_Month_WeekOptionNumber_First_Thursday()
        {
            var output = MonthlyRunner.Run(MonthlyConfiguration.WeekDayOption(Ordinal.First, KindOfDay.Thursday, 2),
                new DateTime(2023, 1, 1), false);

            output.Should().Be(new DateTime(2023, 1, 5));
        }

        [Fact]
        public void Should_Be_Next_Dates_For_Month_WeekOptionNumber_Second_Wednesday()
        {
            var output1 = MonthlyRunner.Run(MonthlyConfiguration.WeekDayOption(Ordinal.Second, KindOfDay.Wednesday, 2),
                new DateTime(2023, 1, 1), false);

            output1.Should().Be(new DateTime(2023, 1, 11));

            var output2 = MonthlyRunner.Run(MonthlyConfiguration.WeekDayOption(Ordinal.Second, KindOfDay.Wednesday, 2),
                output1, true);

            output2.Should().Be(new DateTime(2023, 3, 8));
        }

        [Fact]
        public void Should_Be_Next_Dates_For_Month_WeekOptionNumber_Last_Thursday()
        {
            var output = MonthlyRunner.Run(MonthlyConfiguration.WeekDayOption(Ordinal.Last, KindOfDay.Thursday, 2),
                new DateTime(2023, 1, 1), false);

            output.Should().Be(new DateTime(2023, 1, 26));
        }

        [Fact]
        public void Should_Be_Next_Dates_For_Month_WeekOptionNumber_Last_Thursday2()
        {
            var output = MonthlyRunner.Run(MonthlyConfiguration.WeekDayOption(Ordinal.Last, KindOfDay.Thursday, 2),
                new DateTime(2023, 1, 1), false);

            output.Should().Be(new DateTime(2023, 1, 26));
        }
    }
}
