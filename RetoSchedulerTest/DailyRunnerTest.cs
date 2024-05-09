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
        public void Should_Be_Next_Time_OnceType()
        {
            var output = DailyRunner.Run(
                DailyConfiguration.Once(new TimeOnly(8, 30, 10)),
                new DateTime(2024, 1, 1),
                false);
            output.Should().Be(new DateTime(2024, 1, 1, 8, 30, 10));
        }

        [Fact]
        public void Should_Be_Next_Time_OnceType_CurrentTime_Is_Same_Time()
        {
            var output = DailyRunner.Run(
                DailyConfiguration.Once(new TimeOnly(8, 30, 10)),
                new DateTime(2024, 1, 1, 8, 30, 10),
                false);
            output.Should().Be(new DateTime(2024, 1, 1, 8, 30, 10));
        }

        [Fact]
        public void Should_Be_Next_Time_OnceType_CurrentTime_Is_After_ExecutionTime()
        {
            var output = DailyRunner.Run(
                DailyConfiguration.Once(new TimeOnly(8, 30, 10)),
                new DateTime(2024, 1, 1, 9, 0, 0),
                false);
            output.Should().Be(new DateTime(2024, 1, 2, 8, 30, 10));
        }

        [Fact]
        public void Should_Be_Next_Time_Recurring_Before_StartTime()
        {
            var output = DailyRunner.Run(
                    DailyConfiguration.Recurring(1, DailyFrecuency.Hours,
                    new TimeLimits(new TimeOnly(3, 0, 0), new TimeOnly(6, 0, 0))),
                    new DateTime(2024, 1, 1, 2, 0, 0), false);
            output.Should().Be(new DateTime(2024, 1, 1, 3, 0, 0));
        }

        [Fact]
        public void Should_Be_Next_Time_Recurring_Between_TimeLimits()
        {
            var output = DailyRunner.Run(
                    DailyConfiguration.Recurring(1, DailyFrecuency.Hours,
                    new TimeLimits(new TimeOnly(3, 0, 0), new TimeOnly(6, 0, 0))),
                    new DateTime(2024, 1, 1, 4, 0, 0), false);
            output.Should().Be(new DateTime(2024, 1, 1, 4, 0, 0));
        }

        [Fact]
        public void Should_Be_Next_Time_Recurring_Is_EndTime()
        {
            var output = DailyRunner.Run(
                    DailyConfiguration.Recurring(1, DailyFrecuency.Hours,
                    new TimeLimits(new TimeOnly(3, 0, 0), new TimeOnly(6, 0, 0))),
                    new DateTime(2024, 1, 1, 6, 0, 0), false);

            output.Should().Be(new DateTime(2024, 1, 1, 6, 0, 0));
        }

        [Fact]
        public void Should_Be_Next_Time_Recurring_After_EndTime()
        {
            var output = DailyRunner.Run(
                    DailyConfiguration.Recurring(1, DailyFrecuency.Hours,
                    new TimeLimits(new TimeOnly(3, 0, 0), new TimeOnly(6, 0, 0))),
                    new DateTime(2024, 1, 1, 8, 0, 0), false);

            output.Should().Be(new DateTime(2024, 1, 2, 3, 0, 0));
        }

        [Fact]
        public void Should_Be_Next_Time_Recurring_Adding_Hours()
        {
            var output = DailyRunner.Run(
                    DailyConfiguration.Recurring(1, DailyFrecuency.Hours,
                    new TimeLimits(new TimeOnly(3, 0, 0), new TimeOnly(6, 0, 0))),
                    new DateTime(2024, 1, 1, 5, 0, 0), true);

            output.Should().Be(new DateTime(2024, 1, 1, 6, 0, 0));
        }

        [Fact]
        public void Should_Be_Next_Time_Recurring_Adding_Minutes()
        {
            var output = DailyRunner.Run(
                    DailyConfiguration.Recurring(1, DailyFrecuency.Minutes,
                    new TimeLimits(new TimeOnly(3, 0, 0), new TimeOnly(6, 0, 0))),
                    new DateTime(2024, 1, 1, 5, 0, 0), true);

            output.Should().Be(new DateTime(2024, 1, 1, 5, 1, 0));
        }

        [Fact]
        public void Should_Be_Next_Time_Recurring_Adding_Seconds()
        {
            var output = DailyRunner.Run(
                    DailyConfiguration.Recurring(1, DailyFrecuency.Seconds,
                    new TimeLimits(new TimeOnly(3, 0, 0), new TimeOnly(6, 0, 0))),
                    new DateTime(2024, 1, 1, 5, 0, 0), true);

            output.Should().Be(new DateTime(2024, 1, 1, 5, 0, 1));
        }

        [Fact]
        public void Should_Be_Next_Times_Once_Many_Times()
        {
            var output1 = DailyRunner.Run(
               DailyConfiguration.Once(new TimeOnly(8, 30, 10)),
               new DateTime(2024, 1, 1, 8, 30, 10),
               true);

            output1.Should().Be(new DateTime(2024, 1, 2, 8, 30, 10));
        }

        [Fact]
        public void Should_Be_Next_Times_Recurring_Many_Times()
        {
            var output = DailyRunner.Run(
                    DailyConfiguration.Recurring(7, DailyFrecuency.Hours,
                    new TimeLimits(new TimeOnly(3, 0, 0), new TimeOnly(10, 0, 0))),
                    new DateTime(2024, 1, 1, 3, 30, 0), true);

            output.Should().Be(new DateTime(2024, 1, 2, 3, 0, 0));
        }


    }
}