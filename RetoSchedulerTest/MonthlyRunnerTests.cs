﻿

using FluentAssertions;
using RetoScheduler;
using RetoScheduler.Configurations;
using RetoScheduler.Enums;
using RetoScheduler.Exceptions;
using RetoScheduler.Extensions;
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
            outPut.Should().Be(new DateTime(2023, 2, 28));
        }

        [Fact]
        public void Should_Be_Next_Dates_For_Month_DayOptionNumber_Skipping_2_Months()
        {
            var outPut1 = MonthlyRunner.Run(
                 MonthlyConfiguration.DayOption(30, 2),
                 new DateTime(2023, 3, 31), true);

            outPut1.Should().Be(new DateTime(2023, 5, 30));
        }

        [Fact]
        public void Should_Be_Next_Date_For_Month_DayOptionNumber_Skipping_Months()
        {
            var outPut1 = MonthlyRunner.Run(
                 MonthlyConfiguration.DayOption(29, 1),
                 new DateTime(2023, 1, 17), false);
            outPut1.Should().Be(new DateTime(2023, 1, 29));
        }

        [Fact]
        public void Should_Be_Next_Date_For_Month_WeekOptionNumber_First_Thursday()
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
                output1.AddDays(1), true);

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
                new DateTime(2023, 1, 27), false);

            output.Should().Be(new DateTime(2023, 2, 23));
        }

        [Fact]
        public void Should_Be_Next_Date_For_Month_DayOptionNumber_Possible_Month()
        {
            var monthlyConfiguration = MonthlyConfiguration.DayOption(20, 2);
            var output = MonthlyRunner.Run(monthlyConfiguration, new DateTime(2024, 5, 21, 0, 0, 0)
                , true);

            output.Should().Be(new DateTime(2024, 7, 20, 0, 0, 0));
        }

        [Fact]
        public void Should_Get_Next_Possible_Month_For_Month_DayOption()
        {
            var monthlyConfiguration = MonthlyConfiguration.DayOption(20, 2);
            var output = MonthlyRunner.Run(monthlyConfiguration, new DateTime(2024, 5, 21, 10, 0, 0)
                , true);

            output.Date.Should().Be(new DateTime(2024, 7, 20));
        }

        [Fact]
        public void Should_Not_Get_Next_Possible_()
        {
            var monthlyConfiguration = MonthlyConfiguration.DayOption(20, 2);
            var output = MonthlyRunner.Run(monthlyConfiguration, new DateTime(2024, 5, 20, 10, 0, 0)
                , false);

            output.Date.Should().Be(new DateTime(2024, 5, 20));
        }

        [Fact]
        public void Should_Be_Next_Date_For_Month_WeekDayOption_Executed_First_Thursday()
        {
            var monthlyConfiguration = MonthlyConfiguration.WeekDayOption(Ordinal.First, KindOfDay.Thursday, 2);
            var output = MonthlyRunner.Run(monthlyConfiguration, new DateTime(2024, 5, 3, 0, 0, 0)
                , false);

            output.Should().Be(new DateTime(2024, 6, 6, 0, 0, 0));

        }

        [Fact]
        public void Should_Be_Next_Date_For_Month_WeekDayOption_Executed_First_Monday()
        {
            var monthlyConfiguration = MonthlyConfiguration.WeekDayOption(Ordinal.First, KindOfDay.Monday, 2);
            var output = MonthlyRunner.Run(monthlyConfiguration, new DateTime(2024, 5, 7, 0, 0, 0)
                , false);

            output.Should().Be(new DateTime(2024, 6, 3, 0, 0, 0));
        }
        [Fact]
        public void Should_Be_Next_Date_For_Month_WeekDayOption_Executed_First_Monday_Skipping_Months()
        {
            var monthlyConfiguration = MonthlyConfiguration.WeekDayOption(Ordinal.First, KindOfDay.Monday, 2);
            var output = MonthlyRunner.Run(monthlyConfiguration, new DateTime(2024, 5, 7, 0, 0, 0)
                , true);

            output.Should().Be(new DateTime(2024, 7, 1, 0, 0, 0));
        }

        [Fact]
        public void Should_Be_Next_Date_For_Month_WeekDayOption_Executed_First_WeekDay()
        {
            var monthlyConfiguration = MonthlyConfiguration.WeekDayOption(Ordinal.First, KindOfDay.WeekDay, 2);
            var output = MonthlyRunner.Run(monthlyConfiguration, new DateTime(2024, 5, 3, 0, 0, 0)
                , false);

            output.Should().Be(new DateTime(2024, 6, 3, 0, 0, 0));
        }

        [Fact]
        public void Should_Be_Next_Date_For_Month_WeekDayOption_Executed_First_WeekDay_Skipping_Months()
        {
            var monthlyConfiguration = MonthlyConfiguration.WeekDayOption(Ordinal.First, KindOfDay.WeekDay, 2);
            var output = MonthlyRunner.Run(monthlyConfiguration, new DateTime(2024, 5, 3, 0, 0, 0)
                , true);

            output.Should().Be(new DateTime(2024, 7, 1, 0, 0, 0));
        }

        [Fact]
        public void Should_Be_Next_Date_For_Month_WeekDayOption_Executed_Second_WeekDay_Skipping_Months()
        {
            var monthlyConfiguration = MonthlyConfiguration.WeekDayOption(Ordinal.Second, KindOfDay.WeekDay, 2);
            var output = MonthlyRunner.Run(monthlyConfiguration, new DateTime(2024, 5, 4, 0, 0, 0)
                , true);

            output.Should().Be(new DateTime(2024, 7, 2, 0, 0, 0));
        }

        [Fact]
        public void Should_Be_Next_Date_For_Month_WeekDayOption_Executed_Second_WeekDay_Skipping_Months_Many()
        {
            var monthlyConfiguration = MonthlyConfiguration.WeekDayOption(Ordinal.Second, KindOfDay.WeekDay, 1);
            var res1 = MonthlyRunner.Run(monthlyConfiguration, new DateTime(2024, 5, 3, 0, 0, 0)
                , true);

            res1.Should().Be(new DateTime(2024, 6, 4, 0, 0, 0));
            var res2 = MonthlyRunner.Run(monthlyConfiguration, res1.AddDays(1)
                , true);

            res2.Should().Be(new DateTime(2024, 7, 2, 0, 0, 0));
        }

        [Fact]
        public void Should_Skip_Invalid_Dates_February_Skipping_1_Month()
        {
            var monthlyConfiguration = MonthlyConfiguration.DayOption(30, 1);
            var date = new DateTime(2024, 1, 31, 4, 0, 0);
            var res1 = MonthlyRunner.Run(monthlyConfiguration, date, true);
            res1.Should().Be(new DateTime(2024, 2, 29, 0, 0, 0));
        }

        [Fact]
        public void Should_Skip_Invalid_Dates_February_Skipping_1_Month_Day29()
        {
            var monthlyConfiguration = MonthlyConfiguration.DayOption(30, 1);
            var date = new DateTime(2023, 1, 31, 4, 0, 0);
            var res1 = MonthlyRunner.Run(monthlyConfiguration, date, true);
            res1.Should().Be(new DateTime(2023, 2, 28, 0, 0, 0));
        }

        [Fact]
        public void Should_Skip_Invalid_Dates_February_Skipping_1_Month_Day31()
        {
            var monthlyConfiguration = MonthlyConfiguration.DayOption(31, 1);
            var date = new DateTime(2023, 2, 1, 4, 0, 0);
            var res1 = MonthlyRunner.Run(monthlyConfiguration, date, true);
            res1.Should().Be(new DateTime(2023, 2, 28, 0, 0, 0));
        }

        [Fact]
        public void Should_Skip_Invalid_Dates_February_Skipping_2_Months()
        {
            var monthlyConfiguration = MonthlyConfiguration.DayOption(31, 2);

            var res1 = MonthlyRunner.Run(monthlyConfiguration, new DateTime(2024, 1, 2, 4, 0, 0), true);
            res1.Should().Be(new DateTime(2024, 3, 31, 0, 0, 0));
        }

        [Fact]
        public void Should_Skip_Invalid_Dates_February_Skipping_1_Months()
        {
            var monthlyConfiguration = MonthlyConfiguration.DayOption(30, 1);

            var res1 = MonthlyRunner.Run(monthlyConfiguration, new DateTime(2024, 1, 31, 4, 0, 0), true);
            res1.Should().Be(new DateTime(2024, 2, 29, 0, 0, 0));
        }


        [Fact]
        public void Should_Be_Next_Execution_And_Next_Time_For_DayOptionNumber()
        {
            var outPut = MonthlyRunner.Run(MonthlyConfiguration.DayOption(5, 2), new DateTime(2024, 1, 1, 14, 0, 0), false);

            outPut.Should().Be(new DateTime(2024, 1, 5, 0, 0, 0));
        }

        [Fact]
        public void Should_Be_Next_Execution_And_Next_Time_For_DayOptionNumber_Executed()
        {
            var outPut = MonthlyRunner.Run(MonthlyConfiguration.DayOption(5, 2), new DateTime(2024, 1, 5, 14, 0, 0), true);

            outPut.Should().Be(new DateTime(2024, 1, 5, 14, 0, 0));
        }

        [Fact]
        public void Should_Be_Next_Execution_And_Next_Time_For_Same_DayOptionNumber()
        {
            var outPut = MonthlyRunner.Run(MonthlyConfiguration.DayOption(1, 2), new DateTime(2024, 1, 1, 14, 0, 0), false);

            outPut.Should().Be(new DateTime(2024, 1, 1, 14, 0, 0));
        }

        [Fact]
        public void Should_Be_Next_Executions_And_Next_Time_For_Same_DayOptionNumber_SkippingMonths()
        {
            var outPut = MonthlyRunner.Run(MonthlyConfiguration.DayOption(1, 3), new DateTime(2024, 1, 1, 14, 0, 0), true);

            outPut.Should().Be(new DateTime(2024, 1, 1, 14, 0, 0));

            var outPut2 = MonthlyRunner.Run(MonthlyConfiguration.DayOption(1, 3), outPut.AddHours(2), true);
            outPut2.Should().Be(new DateTime(2024, 1, 1, 16, 0, 0));
        }

        [Fact]
        public void Should_Be_Next_Executions_And_Next_Time_For_Same_DayOptionNumber31_SkippingMonths()
        {
            var outPut = MonthlyRunner.Run(MonthlyConfiguration.DayOption(31, 3), new DateTime(2024, 1, 31, 14, 0, 0), false);

            outPut.Should().Be(new DateTime(2024, 1, 31, 14, 0, 0));

            var outPut2 = MonthlyRunner.Run(MonthlyConfiguration.DayOption(31, 3), outPut.AddHours(12), true);
            outPut2.Should().Be(new DateTime(2024, 4, 30, 0, 0, 0));
        }

        [Fact]
        public void Should_Be_Next_Executions_And_Next_Time_For_Same_DayOptionNumber31_SkippingMonths_And_Skipping_Days_At_First_Execution()
        {
            var outPut = MonthlyRunner.Run(MonthlyConfiguration.DayOption(31, 3), new DateTime(2024, 1, 20, 14, 0, 0), false);

            outPut.Should().Be(new DateTime(2024, 1, 31, 0, 0, 0));

            var outPut2 = MonthlyRunner.Run(MonthlyConfiguration.DayOption(31, 3), outPut.AddDays(1), true);
            outPut2.Should().Be(new DateTime(2024, 4, 30, 0, 0, 0));
        }

        [Fact]
        public void Should_Not_Execute()
        {
            var monthlyConfiguration = MonthlyConfiguration.DayOption(30, 0);

            FluentActions
               .Invoking(() => MonthlyRunner.Run(monthlyConfiguration, new DateTime(2024, 1, 31, 4, 0, 0), true))
               .Should()
               .Throw<SchedulerException>();
        }

    }
}
