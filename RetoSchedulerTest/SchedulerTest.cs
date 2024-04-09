using FluentAssertions;
using RetoScheduler;
using RetoScheduler.Configurations;
using RetoScheduler.Configurations.Limits;
using RetoScheduler.Enums;
using RetoScheduler.Exceptions;

namespace RetoSchedulerTest
{
    public class SchedulerTest
    {
        [Theory, ClassData(typeof(SchedulerTestConfiguration))]
        public void OutPut_Should_Be_Expected(Configuration configuration, OutPut expectedOutut)
        {
            var scheduler = new Scheduler();
            var output = scheduler.Execute(configuration);

            output.NextExecutionTime.Should().Be(expectedOutut.NextExecutionTime);
            output.Description.Should().Be(expectedOutut.Description);
        }

        [Fact]
        public void Next_Execution_Time_Should_Throw_Exception_And_Message()
        {
            var scheduler = new Scheduler();
            var configuration = new Configuration(new DateTime(2020, 1, 4), ConfigType.Once, true, null, Occurs.Daily, null, new DailyConfiguration(DailyConfigType.Once, new TimeOnly(16, 0, 0), null, null, null), new DateLimits(new DateTime(2020, 1, 1)));

            FluentActions
                .Invoking(() => scheduler.Execute(configuration))
                .Should()
                .Throw<SchedulerException>()
                .And.Message
                .Should().Be("Once Types requires an obligatory DateTime");
        }

        [Theory]
        [InlineData(ConfigType.Once)]
        [InlineData(ConfigType.Recurring)]
        public void Should_Throw_Exception_Disabled_Check_And_Message(ConfigType configType)
        {
            var scheduler = new Scheduler();
            var configuration = new Configuration(new DateTime(2020, 1, 4), configType, false, null, Occurs.Daily, null, new DailyConfiguration(DailyConfigType.Once, new TimeOnly(16, 0, 0), null, null, null), new DateLimits(new DateTime(2020, 1, 1)));

            FluentActions
               .Invoking(() => scheduler.Execute(configuration))
               .Should()
               .Throw<SchedulerException>()
               .And.Message
               .Should().Be("You need to check field to Run Program");
        }

        [Fact]
        public void Should_Throw_Exception_Configuration_Dont_Have_Limit()
        {
            var scheduler = new Scheduler();
            var configuration = new Configuration
                (new DateTime(2020, 1, 1, 0, 0, 0), ConfigType.Recurring, true, null, Occurs.Weekly, new WeeklyConfiguration(2, new List<DayOfWeek>() { DayOfWeek.Monday, DayOfWeek.Thursday, DayOfWeek.Friday }), 
                new DailyConfiguration(DailyConfigType.Recurring, null, 2, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(4, 0, 0), new TimeOnly(8, 0, 0))), null);
            FluentActions
               .Invoking(() => scheduler.Execute(configuration))
               .Should()
               .Throw<SchedulerException>("Limits Can`t be null");
        }

        [Fact]
        public void Should_Throw_Exception_EndDate_Is_Before_StartDate()
        {
            var scheduler = new Scheduler();
            var configuration = new Configuration
                (new DateTime(2020, 1, 1, 0, 0, 0), ConfigType.Recurring, true, null, Occurs.Weekly, new WeeklyConfiguration(2, new List<DayOfWeek>() { DayOfWeek.Monday, DayOfWeek.Thursday, DayOfWeek.Friday }), new DailyConfiguration(DailyConfigType.Recurring, null, 2, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(4, 0, 0), new TimeOnly(8, 0, 0))), new DateLimits(new DateTime(2020, 1, 6), new DateTime(2020, 1, 2)));
            

            FluentActions
                 .Invoking(() => scheduler.Execute(configuration))
                 .Should()
                 .Throw<SchedulerException>("The end date cannot be earlier than the initial date");
        }

        [Theory, ClassData(typeof(SchedulerLimitsConfiguration))]
        public void Should_Throw_Exception_If_Limits_Are_Out_Of_Range(Configuration configuration)
        {
            var scheduler = new Scheduler();
            FluentActions
                .Invoking(() => scheduler.Execute(configuration))
                .Should()
                .Throw<SchedulerException>("DateTime can't be out of start and end range");

        }

        [Fact]
        public void Should_Validate_Next_Execution_Time()
        {
            var scheduler = new Scheduler();

            
            var configuration1 = new Configuration
                (new DateTime(2020, 1, 4), ConfigType.Recurring, true, null, Occurs.Daily, null, new DailyConfiguration(DailyConfigType.Recurring, null, 2, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(4, 0, 0), new TimeOnly(8, 0, 0))), new DateLimits(new DateTime(2020, 1, 1)));

            var res1 = scheduler.Execute(configuration1);

            var configuration2 = new Configuration
                (res1.NextExecutionTime, ConfigType.Recurring, true, null, Occurs.Daily, null, new DailyConfiguration(DailyConfigType.Recurring, null, 2, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(4, 0, 0), new TimeOnly(8, 0, 0))), new DateLimits(new DateTime(2020, 1, 1)));

            var res2 = scheduler.Execute(configuration2);

            res1.NextExecutionTime.Should().Be(new DateTime(2020, 1, 4));
            res2.NextExecutionTime.Should().Be(new DateTime(2020, 1, 5));
        }
    }
}