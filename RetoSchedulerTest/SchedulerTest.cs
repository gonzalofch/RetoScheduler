using FluentAssertions;
using RetoScheduler;
using RetoScheduler.Configurations;
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
            var configuration = new Configuration(new DateTime(2020, 1, 4), ConfigType.Once, true, null, Occurs.Daily, 0, new DateLimits(new DateTime(2020, 1, 1)));

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
            var configuration = new Configuration(new DateTime(2020, 1, 5), configType, false, null, Occurs.Daily, 0, new DateLimits(new DateTime(2020, 1, 1)));
            FluentActions
               .Invoking(() => scheduler.Execute(configuration))
               .Should()
               .Throw<SchedulerException>()
               .And.Message
               .Should().Be("You need to check field to Run Program");
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(0)]
        public void Should_Throw_Exception_Negative_Number_Of_Days_In_Recurring_Type(int frecuencyInDays)
        {
            var scheduler = new Scheduler();
            var configuration = new Configuration(new DateTime(2020, 1, 5), ConfigType.Recurring, true, null, Occurs.Daily, frecuencyInDays, new DateLimits(new DateTime(2020, 1, 1)));
            FluentActions
               .Invoking(() => scheduler.Execute(configuration))
               .Should()
               .Throw<SchedulerException>()
               .And.Message
               .Should()
               .Be("Don't should put negative numbers or zero in number field");
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(0)]
        public void Should_Not_Throw_Exception_Negative_Number_Of_Days_In_Once_Type(int frecuencyInDays)
        {
            var scheduler = new Scheduler();
            var configuration = new Configuration(new DateTime(2020, 1, 5), ConfigType.Once, true, DateTime.Now, Occurs.Daily, frecuencyInDays, new DateLimits(new DateTime(2020, 1, 1)));
            FluentActions
               .Invoking(() => scheduler.Execute(configuration))
               .Should()
               .NotThrow<SchedulerException>();
        }

        [Fact]
        public void Should_Throw_Exception_Configuration_Dont_Have_Limit()
        {
            var scheduler = new Scheduler();
            var configuration = new Configuration(new DateTime(2020, 1, 5), ConfigType.Once, true, DateTime.Now, Occurs.Daily, 1, null);
            FluentActions
               .Invoking(() => scheduler.Execute(configuration))
               .Should()
               .Throw<SchedulerException>("Limits Can`t be null");
        }

        [Fact]
        public void Should_Throw_Exception_EndDate_Is_Before_StartDate()
        {
            var scheduler = new Scheduler();
            var configuration = new Configuration(new DateTime(2020, 1, 5), ConfigType.Once, true, DateTime.Now, Occurs.Daily, 1, new DateLimits(new DateTime(2020, 1, 6), new DateTime(2020, 1, 2)));

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

            var configuration1 = new Configuration(new DateTime(2020, 1, 4), ConfigType.Recurring, true, DateTime.Now, Occurs.Daily, 1, new DateLimits(new DateTime(2020, 1, 1)));
            var res1 = scheduler.Execute(configuration1);
            var configuration2 = new Configuration(res1.NextExecutionTime, ConfigType.Recurring, true, DateTime.Now, Occurs.Daily, 1, new DateLimits(new DateTime(2020, 1, 1)));
            var res2 = scheduler.Execute(configuration2);

            res1.NextExecutionTime.Should().Be(new DateTime(2020, 1, 4));
            res2.NextExecutionTime.Should().Be(new DateTime(2020, 1, 5));
        }
    }
}