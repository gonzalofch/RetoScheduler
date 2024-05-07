using FluentAssertions;
using Microsoft.VisualStudio.TestPlatform.Utilities;
using RetoScheduler;
using RetoScheduler.Configurations;
using RetoScheduler.Configurations.Limits;
using RetoScheduler.Enums;
using RetoScheduler.Exceptions;
using RetoScheduler.Runners;
using System.Net.Http.Headers;

namespace RetoSchedulerTest
{
    public class DailyRunnerTest
    {
        [Fact]
        public void Should_Be_Next_Time_OnceType1()
        {
            DailyRunner.Run(
                DailyConfiguration.Once(new TimeOnly(8, 30, 10)),
                new DateTime(2024, 1, 1),
                false)
                .Should().Be(new DateTime(2024, 1, 1, 8, 30, 10));
        }

        [Fact]
        public void Should_Be_Next_Time_OnceType2()
        {
            DailyRunner.Run(
                DailyConfiguration.Once(new TimeOnly(12, 30, 10)),
                new DateTime(2024, 1, 1, 1, 1, 1),
                false)
                .Should().Be(new DateTime(2024, 1, 1, 12, 30, 10));
        }

        [Fact]
        public void Should_Be_Next_Time_OnceType3()
        {
            DailyRunner.Run(
                DailyConfiguration.Once(new TimeOnly(18, 30, 30)),
                new DateTime(2024, 1, 5, 16, 30, 30),
                false)
                .Should().Be(new DateTime(2024, 1, 5, 18, 30, 30));
        }

        [Fact]
        public void Should_Be_Next_Time_OnceType3_Executed()
        {
            DailyRunner.Run(
                DailyConfiguration.Once(new TimeOnly(18, 30, 30)),
                new DateTime(2024, 1, 5, 16, 30, 30),
                true)
                .Should().Be(new DateTime(2024, 1, 5, 18, 30, 30));
        }

        [Fact]
        public void Should_Be_Next_Time_RecurringType_Not_Executed1()
        {
            DailyRunner.Run(
                DailyConfiguration.Recurring(2, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(0, 0, 0), new TimeOnly(1, 0, 0))),
                new DateTime(2024, 1, 1, 0, 0, 0),
                false
                ).Should().Be(new DateTime(2024, 1, 1, 0, 0, 0));
        }

        [Fact]
        public void Should_Be_Next_Time_RecurringType_DateTime_Plus_Hours_Is_Before_StartTime_NotExecuted()
        {
            DailyRunner.Run(
                DailyConfiguration.Recurring(4, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(5, 0, 0), new TimeOnly(10, 0, 0))),
                new DateTime(2024, 1, 1, 0, 0, 0),
                true
                ).Should().Be(new DateTime(2024, 1, 1, 5, 0, 0));
        }

        [Fact]
        public void Should_Be_Next_Time_RecurringType_DateTime_Plus_Hours_Is_After_StartTime_Not_Executed1()
        {
            DailyRunner.Run(
                DailyConfiguration.Recurring(5, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(4, 0, 0), new TimeOnly(8, 0, 0))),
                new DateTime(2024, 1, 1, 0, 0, 0),
                false
                ).Should().Be(new DateTime(2024, 1, 1, 4, 0, 0));
        }

        [Fact]
        public void Should_Be_Next_Time_RecurringType_DateTime_Plus_Hours_Is_After_StartTime_Not_Executed2()
        {
            DailyRunner.Run(
                DailyConfiguration.Recurring(4, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(3, 0, 0), new TimeOnly(5, 0, 0))),
                new DateTime(2024, 1, 1, 0, 0, 0),
                false
                ).Should().Be(new DateTime(2024, 1, 1, 3, 0, 0));
        }

        [Fact]
        public void Should_Be_Next_Time_RecurringType_DateTime_Is_After_StartTime_Not_Executed1()
        {
            DailyRunner.Run(
                DailyConfiguration.Recurring(2, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(3, 0, 0), new TimeOnly(9, 0, 0))),
                new DateTime(2024, 1, 1, 5, 0, 0),
                false
                ).Should().Be(new DateTime(2024, 1, 1, 5, 0, 0));
        }

        [Fact]
        public void Should_Be_Next_Time_RecurringType_DateTime_Is_After_StartTime_Not_Executed2()
        {
            DailyRunner.Run(
                DailyConfiguration.Recurring(7, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(5, 0, 0), new TimeOnly(10, 0, 0))),
                new DateTime(2024, 1, 1),
                true
                ).Should().Be(new DateTime(2024, 1, 1, 7, 0, 0));
        }

        //falta probar executed

        [Fact]
        public void Should_Be_Next_Time_RecurringType_DateTime_Is_Before_StartTime_Executed1()
        {
            DailyRunner.Run(
                DailyConfiguration.Recurring(2, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(5, 0, 0), new TimeOnly(10, 0, 0))),
                new DateTime(2024, 1, 1),
                true
                ).Should().Be(new DateTime(2024, 1, 1, 5, 0, 0));
        }

        [Fact]
        public void Should_Be_Next_Time_RecurringType_DateTime_Is_Before_StartTime_Executed2()
        {
            DailyRunner.Run(
                DailyConfiguration.Recurring(2, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(5, 0, 0), new TimeOnly(10, 0, 0))),
                new DateTime(2024, 1, 1),
                true
                ).Should().Be(new DateTime(2024, 1, 1, 5, 0, 0));
        }

        [Fact]
        public void Should_Be_Next_Time_RecurringType_DateTime_Is_Before_StartTime_Executed3()
        {
            DailyRunner.Run(
                DailyConfiguration.Recurring(3, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(5, 0, 0), new TimeOnly(10, 0, 0))),
                new DateTime(2024, 1, 1, 2, 0, 0),
                true
                ).Should().Be(new DateTime(2024, 1, 1, 5, 0, 0));
        }

        [Fact]
        public void Should_Be_Next_Time_RecurringType_Executed1()
        {
            DailyRunner.Run(
                DailyConfiguration.Recurring(4, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(5, 0, 0), new TimeOnly(10, 0, 0))),
                new DateTime(2024, 1, 1, 2, 2, 2),
                true
                ).Should().Be(new DateTime(2024, 1, 1, 6, 2, 2));
        }

        [Fact]
        public void Should_Be_Next_Time_RecurringType_Executed2()
        {
            DailyRunner.Run(
                DailyConfiguration.Recurring(6, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(5, 0, 0), new TimeOnly(10, 0, 0))),
                new DateTime(2024, 1, 1),
                true
                ).Should().Be(new DateTime(2024, 1, 1, 6, 0, 0));
        }

        [Fact]
        public void Should_Be_Next_Time_RecurringType_Executed3()
        {
            DailyRunner.Run(
                DailyConfiguration.Recurring(6, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(5, 0, 0), new TimeOnly(10, 0, 0))),
                new DateTime(2024, 1, 1, 3, 0, 0),
                true
                ).Should().Be(new DateTime(2024, 1, 1, 9, 0, 0));
        }

        [Fact]
        public void Should_Be_Next_Time_RecurringType_Executed4()
        {
            DailyRunner.Run(
                DailyConfiguration.Recurring(2, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(5, 0, 0), new TimeOnly(10, 0, 0))),
                new DateTime(2024, 1, 1, 6, 0, 0),
                true
                ).Should().Be(new DateTime(2024, 1, 1, 8, 0, 0));
        }

        [Fact]
        public void Should_Be_Next_Time_RecurringType__Executed5()
        {
            DailyRunner.Run(
                DailyConfiguration.Recurring(7, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(5, 0, 0), new TimeOnly(10, 0, 0))),
                new DateTime(2024, 1, 1),
                true
                ).Should().Be(new DateTime(2024, 1, 1, 7, 0, 0));
        }
    }
}