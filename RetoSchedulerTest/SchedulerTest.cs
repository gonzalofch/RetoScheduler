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
            //output.Description.Should().Be(expectedOutut.Description);
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
                new DailyConfiguration(DailyConfigType.Recurring, TimeOnly.MinValue, 2, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(4, 0, 0), new TimeOnly(8, 0, 0))), null);
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
                (new DateTime(2020, 1, 1, 0, 0, 0), ConfigType.Recurring, true, null, Occurs.Weekly, new WeeklyConfiguration(2, new List<DayOfWeek>() { DayOfWeek.Monday, DayOfWeek.Thursday, DayOfWeek.Friday }), new DailyConfiguration(DailyConfigType.Recurring, TimeOnly.MinValue, 2, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(4, 0, 0), new TimeOnly(8, 0, 0))), new DateLimits(new DateTime(2020, 1, 6), new DateTime(2020, 1, 2)));


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
        public void Should_Throw_Exception_If_EndTime_is_Before_StartTime()
        {
            var scheduler = new Scheduler();
            var configuration = new Configuration
                (new DateTime(2020, 1, 1, 0, 0, 0), ConfigType.Recurring, true, null, Occurs.Weekly, 
                new WeeklyConfiguration(2, new List<DayOfWeek>() { DayOfWeek.Monday, DayOfWeek.Thursday, DayOfWeek.Friday }), 
                new DailyConfiguration(DailyConfigType.Recurring, TimeOnly.MinValue, 2, DailyFrecuency.Hours, 
                new TimeLimits(new TimeOnly(12, 0, 0), new TimeOnly(8, 0, 0))), new DateLimits(new DateTime(2020, 1, 6), new DateTime(2020, 1, 2)));

            FluentActions
                 .Invoking(() => scheduler.Execute(configuration))
                 .Should()
                 .Throw<SchedulerException>("The EndTime cannot be earlier than Start Time");
        }
        [Fact]
        public void Should_Throw_Exception_If_EndTime_is_Before_CurrentTime()
        {
            var scheduler = new Scheduler();
            var configuration = new Configuration
                (new DateTime(2020, 1, 1, 9, 0, 0), ConfigType.Recurring, true, null, Occurs.Daily,
                null,
                new DailyConfiguration(DailyConfigType.Recurring, TimeOnly.MinValue, 10, DailyFrecuency.Minutes,
                new TimeLimits(new TimeOnly(4, 0, 0), new TimeOnly(8, 0, 0))), new DateLimits(new DateTime(2019, 12, 20), new DateTime(2020, 1, 1)));

            FluentActions
                 .Invoking(() => scheduler.Execute(configuration))
                 .Should()
                 .Throw<SchedulerException>("The EndTime cannot be earlier than Current Time");
        }
        [Fact]

        public void Should_Throw_Exception_If_OnceAtTime_is_Before_CurrentTime()
        {
            var scheduler = new Scheduler();
            var configuration = new Configuration
                (new DateTime(2020, 1, 1, 9, 0, 0), ConfigType.Once, true, new DateTime(2020,1,1), Occurs.Daily,
                null,
                new DailyConfiguration(DailyConfigType.Once, new TimeOnly(1,0,0), null, null,
                null), new DateLimits(new DateTime(2019, 12, 20), new DateTime(2020, 1, 12)));

            FluentActions
                 .Invoking(() => scheduler.Execute(configuration))
                 .Should()
                 .Throw<SchedulerException>("The execution time cannot be earlier than the Current Time");
        }


        [Fact]
        public void Should_Validate_Next_Execution_Time()
        {
            var scheduler = new Scheduler();


            var configuration1 = new Configuration
                (new DateTime(2020, 1, 4,0,0,0), ConfigType.Recurring, true, null, Occurs.Daily, null,
                new DailyConfiguration(DailyConfigType.Recurring, TimeOnly.MinValue, 3, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(4, 0, 0), new TimeOnly(8, 0, 0))), new DateLimits(new DateTime(2020, 1, 1)));

            var res1 = scheduler.Execute(configuration1);

            var configuration2 = new Configuration
                (res1.NextExecutionTime, ConfigType.Recurring, true, null, Occurs.Daily, null,
                new DailyConfiguration(DailyConfigType.Recurring, TimeOnly.MinValue, 3, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(4, 0, 0), new TimeOnly(8, 0, 0))), new DateLimits(new DateTime(2020, 1, 1)));

            var res2 = scheduler.Execute(configuration2);

            res1.NextExecutionTime.Should().Be(new DateTime(2020, 1, 4, 4, 0, 0));
            res2.NextExecutionTime.Should().Be(new DateTime(2020, 1, 4, 7, 0, 0));
        }

        [Fact]
        public void Should_Validate_Execution_Time_During_Week()
        {
            var scheduler = new Scheduler();
            var weeklyConfig = new WeeklyConfiguration(0, new List<DayOfWeek>() { DayOfWeek.Monday, DayOfWeek.Thursday, DayOfWeek.Friday });

            var configuration1 = new Configuration
                (new DateTime(2020, 1, 1), ConfigType.Recurring, true, null, Occurs.Daily,
                weeklyConfig,
                new DailyConfiguration(DailyConfigType.Recurring, TimeOnly.MinValue, 2, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(4, 0, 0), new TimeOnly(8, 0, 0))), new DateLimits(new DateTime(2020, 1, 1)));

            var res1 = scheduler.Execute(configuration1);

            var configuration2 = new Configuration
                (res1.NextExecutionTime, ConfigType.Recurring, true, null, Occurs.Daily,
                weeklyConfig,
                new DailyConfiguration(DailyConfigType.Recurring, TimeOnly.MinValue, 2, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(4, 0, 0), new TimeOnly(8, 0, 0))), new DateLimits(new DateTime(2020, 1, 1)));

            var res2 = scheduler.Execute(configuration2);

            var configuration3 = new Configuration
                (res2.NextExecutionTime, ConfigType.Recurring, true, null, Occurs.Daily,
                weeklyConfig,
                new DailyConfiguration(DailyConfigType.Recurring, TimeOnly.MinValue, 2, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(4, 0, 0), new TimeOnly(8, 0, 0))), new DateLimits(new DateTime(2020, 1, 1)));
            var res3 = scheduler.Execute(configuration3);

            var configuration4 = new Configuration
                (res3.NextExecutionTime, ConfigType.Recurring, true, null, Occurs.Daily,
                weeklyConfig,
                new DailyConfiguration(DailyConfigType.Recurring, TimeOnly.MinValue, 2, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(4, 0, 0), new TimeOnly(8, 0, 0))), new DateLimits(new DateTime(2020, 1, 1)));
            var res4 = scheduler.Execute(configuration4);

            var configuration5 = new Configuration
                (res4.NextExecutionTime, ConfigType.Recurring, true, null, Occurs.Daily,
                weeklyConfig,
                new DailyConfiguration(DailyConfigType.Recurring, TimeOnly.MinValue, 2, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(4, 0, 0), new TimeOnly(8, 0, 0))), new DateLimits(new DateTime(2020, 1, 1)));
            var res5 = scheduler.Execute(configuration5);

            var configuration6 = new Configuration
                (res5.NextExecutionTime, ConfigType.Recurring, true, null, Occurs.Daily,
                weeklyConfig,
                new DailyConfiguration(DailyConfigType.Recurring, TimeOnly.MinValue, 2, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(4, 0, 0), new TimeOnly(8, 0, 0))), new DateLimits(new DateTime(2020, 1, 1)));
            var res6 = scheduler.Execute(configuration6);

            var configuration7 = new Configuration
                (res6.NextExecutionTime, ConfigType.Recurring, true, null, Occurs.Daily,
                weeklyConfig,
                new DailyConfiguration(DailyConfigType.Recurring, TimeOnly.MinValue, 2, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(4, 0, 0), new TimeOnly(8, 0, 0))), new DateLimits(new DateTime(2020, 1, 1)));
            var res7 = scheduler.Execute(configuration7);

            res1.NextExecutionTime.Should().Be(new DateTime(2020, 1, 2, 4, 0, 0));
            res2.NextExecutionTime.Should().Be(new DateTime(2020, 1, 2, 6, 0, 0));
            res3.NextExecutionTime.Should().Be(new DateTime(2020, 1, 2, 8, 0, 0));
            res4.NextExecutionTime.Should().Be(new DateTime(2020, 1, 3, 4, 0, 0));
            res5.NextExecutionTime.Should().Be(new DateTime(2020, 1, 3, 6, 0, 0));
            res6.NextExecutionTime.Should().Be(new DateTime(2020, 1, 3, 8, 0, 0));
            res7.NextExecutionTime.Should().Be(new DateTime(2020, 1, 6, 4, 0, 0));
        }

        [Fact]
        public void Should_Validate_Execution_Time_Space_Between_Weeks()
        {
            var scheduler = new Scheduler();
            var weeklyConfig = new WeeklyConfiguration(2, new List<DayOfWeek>() { DayOfWeek.Monday, DayOfWeek.Thursday, DayOfWeek.Friday });

            var configuration1 = new Configuration
                (new DateTime(2020, 1, 13), ConfigType.Recurring, true, null, Occurs.Daily,
                weeklyConfig,
                new DailyConfiguration(DailyConfigType.Recurring, TimeOnly.MinValue, 2, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(4, 0, 0), new TimeOnly(8, 0, 0))), new DateLimits(new DateTime(2020, 1, 1)));

            var res1 = scheduler.Execute(configuration1);

            var configuration2 = new Configuration
                (res1.NextExecutionTime, ConfigType.Recurring, true, null, Occurs.Daily,
                weeklyConfig,
                new DailyConfiguration(DailyConfigType.Recurring, TimeOnly.MinValue, 2, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(4, 0, 0), new TimeOnly(8, 0, 0))), new DateLimits(new DateTime(2020, 1, 1)));

            var res2 = scheduler.Execute(configuration2);

            var configuration3 = new Configuration
                (res2.NextExecutionTime, ConfigType.Recurring, true, null, Occurs.Daily,
                weeklyConfig,
                new DailyConfiguration(DailyConfigType.Recurring, TimeOnly.MinValue, 2, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(4, 0, 0), new TimeOnly(8, 0, 0))), new DateLimits(new DateTime(2020, 1, 1)));
            var res3 = scheduler.Execute(configuration3);

            var configuration4 = new Configuration
                (res3.NextExecutionTime, ConfigType.Recurring, true, null, Occurs.Daily,
                weeklyConfig,
                new DailyConfiguration(DailyConfigType.Recurring, TimeOnly.MinValue, 2, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(4, 0, 0), new TimeOnly(8, 0, 0))), new DateLimits(new DateTime(2020, 1, 1)));
            var res4 = scheduler.Execute(configuration4);

            var configuration5 = new Configuration
                (res4.NextExecutionTime, ConfigType.Recurring, true, null, Occurs.Daily,
                weeklyConfig,
                new DailyConfiguration(DailyConfigType.Recurring, TimeOnly.MinValue, 2, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(4, 0, 0), new TimeOnly(8, 0, 0))), new DateLimits(new DateTime(2020, 1, 1)));
            var res5 = scheduler.Execute(configuration5);

            var configuration6 = new Configuration
                (res5.NextExecutionTime, ConfigType.Recurring, true, null, Occurs.Daily,
                weeklyConfig,
                new DailyConfiguration(DailyConfigType.Recurring, TimeOnly.MinValue, 2, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(4, 0, 0), new TimeOnly(8, 0, 0))), new DateLimits(new DateTime(2020, 1, 1)));
            var res6 = scheduler.Execute(configuration6);

            var configuration7 = new Configuration
                (res6.NextExecutionTime, ConfigType.Recurring, true, null, Occurs.Daily,
                weeklyConfig,
                new DailyConfiguration(DailyConfigType.Recurring, TimeOnly.MinValue, 2, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(4, 0, 0), new TimeOnly(8, 0, 0))), new DateLimits(new DateTime(2020, 1, 1)));
            var res7 = scheduler.Execute(configuration7);


            var configuration8 = new Configuration
                (res7.NextExecutionTime, ConfigType.Recurring, true, null, Occurs.Daily,
                weeklyConfig,
                new DailyConfiguration(DailyConfigType.Recurring, TimeOnly.MinValue, 2, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(4, 0, 0), new TimeOnly(8, 0, 0))), new DateLimits(new DateTime(2020, 1, 1)));
            var res8 = scheduler.Execute(configuration8);

            var configuration9 = new Configuration
                (res8.NextExecutionTime, ConfigType.Recurring, true, null, Occurs.Daily,
                weeklyConfig,
                new DailyConfiguration(DailyConfigType.Recurring, TimeOnly.MinValue, 2, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(4, 0, 0), new TimeOnly(8, 0, 0))), new DateLimits(new DateTime(2020, 1, 1)));
            var res9 = scheduler.Execute(configuration9);

            var configuration10 = new Configuration
                (res9.NextExecutionTime, ConfigType.Recurring, true, null, Occurs.Daily,
                weeklyConfig,
                new DailyConfiguration(DailyConfigType.Recurring, TimeOnly.MinValue, 2, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(4, 0, 0), new TimeOnly(8, 0, 0))), new DateLimits(new DateTime(2020, 1, 1)));
            var res10 = scheduler.Execute(configuration10);

            var configuration11 = new Configuration
                (res10.NextExecutionTime, ConfigType.Recurring, true, null, Occurs.Daily,
                weeklyConfig,
                new DailyConfiguration(DailyConfigType.Recurring, TimeOnly.MinValue, 2, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(4, 0, 0), new TimeOnly(8, 0, 0))), new DateLimits(new DateTime(2020, 1, 1)));
            var res11 = scheduler.Execute(configuration11);
            res1.NextExecutionTime.Should().Be(new DateTime(2020, 1, 13, 4, 0, 0));
            res2.NextExecutionTime.Should().Be(new DateTime(2020, 1, 13, 6, 0, 0));
            res3.NextExecutionTime.Should().Be(new DateTime(2020, 1, 13, 8, 0, 0));
            res4.NextExecutionTime.Should().Be(new DateTime(2020, 1, 16, 4, 0, 0));
            res5.NextExecutionTime.Should().Be(new DateTime(2020, 1, 16, 6, 0, 0));
            res6.NextExecutionTime.Should().Be(new DateTime(2020, 1, 16, 8, 0, 0));
            res7.NextExecutionTime.Should().Be(new DateTime(2020, 1, 17, 4, 0, 0));
            res8.NextExecutionTime.Should().Be(new DateTime(2020, 1, 17, 6, 0, 0));
            res9.NextExecutionTime.Should().Be(new DateTime(2020, 1, 17, 8, 0, 0));
            res10.NextExecutionTime.Should().Be(new DateTime(2020, 2, 3, 4, 0, 0));
            res11.NextExecutionTime.Should().Be(new DateTime(2020, 2, 3, 6, 0, 0));
        }
    }
}