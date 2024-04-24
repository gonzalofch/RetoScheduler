using FluentAssertions;
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities.Serialization;
using RetoScheduler;
using RetoScheduler.Configurations;
using RetoScheduler.Configurations.Limits;
using RetoScheduler.Enums;
using RetoScheduler.Exceptions;
using RetoScheduler.Extensions;
using Xunit;


namespace RetoSchedulerTest
{
    public class SchedulerTest
    {

        [Theory]
        [ClassData(typeof(SchedulerTestConfiguration))]
        [ClassData(typeof(SchedulerTestMonthlyConfiguration))]
        public void OutPut_Should_Be_Expected_Configurations(Configuration configuration, OutPut expectedOutput)
        {
            var scheduler = new Scheduler();
            var output = scheduler.Execute(configuration);

            output.NextExecutionTime.Should().Be(expectedOutput.NextExecutionTime);
            output.Description.Should().Be(expectedOutput.Description);
        }

        [Fact]
        public void Next_Execution_Time_Should_Throw_Exception_And_Message()
        {
            var scheduler = new Scheduler();
            var configuration = new Configuration(new DateTime(2020, 1, 4), ConfigType.Once, true, null, Occurs.Daily,
                null, null, new DailyConfiguration(DailyConfigType.Once, new TimeOnly(16, 0, 0), null, null, null), new DateLimits(new DateTime(2020, 1, 1)));

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
            var configuration = new Configuration(new DateTime(2020, 1, 4), configType, false, null, Occurs.Daily, null, null,
                new DailyConfiguration(DailyConfigType.Once, new TimeOnly(16, 0, 0), null, null, null), new DateLimits(new DateTime(2020, 1, 1)));

            FluentActions
               .Invoking(() => scheduler.Execute(configuration))
               .Should()
               .Throw<SchedulerException>()
               .And.Message
               .Should().Be("You need to check field to run the Scheduler");
        }

        [Fact]
        public void Should_Throw_Exception_Configuration_Dont_Have_Limit()
        {
            var scheduler = new Scheduler();
            var configuration = new Configuration
                (new DateTime(2020, 1, 1, 0, 0, 0), ConfigType.Recurring, true, null, Occurs.Weekly, null,
                new WeeklyConfiguration(2, new List<DayOfWeek>() { DayOfWeek.Monday, DayOfWeek.Thursday, DayOfWeek.Friday }),
                new DailyConfiguration(DailyConfigType.Recurring, TimeOnly.MinValue, 2, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(4, 0, 0), new TimeOnly(8, 0, 0))), new DateLimits(new DateTime(2020, 1, 6), new DateTime(2020, 1, 2)));
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
                (new DateTime(2020, 1, 1, 0, 0, 0), ConfigType.Recurring, true, null, Occurs.Weekly, null, new WeeklyConfiguration(2, new List<DayOfWeek>() { DayOfWeek.Monday, DayOfWeek.Thursday, DayOfWeek.Friday }), new DailyConfiguration(DailyConfigType.Recurring, TimeOnly.MinValue, 2, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(4, 0, 0), new TimeOnly(8, 0, 0))), new DateLimits(new DateTime(2020, 1, 6), new DateTime(2020, 1, 2)));


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
                (new DateTime(2020, 1, 1, 0, 0, 0), ConfigType.Recurring, true, null, Occurs.Weekly, null,
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
                (new DateTime(2020, 1, 1, 9, 0, 0), ConfigType.Recurring, true, null, Occurs.Daily, null,
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
                (new DateTime(2020, 1, 1, 9, 0, 0), ConfigType.Once, true, new DateTime(2020, 1, 1), Occurs.Daily, null,
                null,
                new DailyConfiguration(DailyConfigType.Once, new TimeOnly(2, 0, 0), null, null,
                null), new DateLimits(new DateTime(2020, 1, 1), new DateTime(2020, 1, 1)));

            FluentActions
                 .Invoking(() => scheduler.Execute(configuration))
                 .Should()
                 .Throw<SchedulerException>("The execution time cannot be earlier than the Current Time");
        }

        [Fact]
        public void Should_Throw_Exception_If_Daily_Frecuency_Is_Zero_Or_Negative()
        {
            var scheduler = new Scheduler();
            var configuration = new Configuration
                (new DateTime(2020, 1, 1, 9, 0, 0), ConfigType.Recurring, true, new DateTime(2020, 1, 1), Occurs.Daily, null, null,
                new DailyConfiguration(DailyConfigType.Recurring, TimeOnly.MinValue, -10, DailyFrecuency.Minutes,
                new TimeLimits(new TimeOnly(4, 0, 0), new TimeOnly(10, 0, 0))),
                new DateLimits(new DateTime(2019, 12, 20), new DateTime(2020, 1, 12)));

            FluentActions
                 .Invoking(() => scheduler.Execute(configuration))
                 .Should()
                 .Throw<SchedulerException>("Don't should put negative numbers or zero in number field");
        }

        [Fact]
        public void Should_Throw_Exception_If_EndTime_Is_Before_StartTime()
        {
            var scheduler = new Scheduler();
            var configuration = new Configuration
                (new DateTime(2020, 1, 1, 9, 10, 40),
                ConfigType.Recurring,
                true,
                new DateTime(2020, 1, 1),
                Occurs.Daily, null,
                null,
                new DailyConfiguration(DailyConfigType.Recurring, TimeOnly.MinValue, 3, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(10, 0, 0), new TimeOnly(9, 20, 20))),
                new DateLimits(new DateTime(2020, 1, 1), new DateTime(2020, 1, 1)));

            FluentActions
                 .Invoking(() => scheduler.Execute(configuration))
                 .Should()
                 .Throw<SchedulerException>("The EndTime cannot be earlier than StartTime");
        }

        [Fact]
        public void Should_Validate_Next_Execution_Time()
        {
            var scheduler = new Scheduler();


            var configuration1 = new Configuration
                (new DateTime(2020, 1, 4, 0, 0, 0), ConfigType.Recurring, true, null, Occurs.Daily, null, null,
                new DailyConfiguration(DailyConfigType.Recurring, TimeOnly.MinValue, 3, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(4, 0, 0), new TimeOnly(8, 0, 0))), new DateLimits(new DateTime(2020, 1, 1)));

            var res1 = scheduler.Execute(configuration1);

            var configuration2 = new Configuration
                (res1.NextExecutionTime, ConfigType.Recurring, true, null, Occurs.Daily, null, null,
                new DailyConfiguration(DailyConfigType.Recurring, TimeOnly.MinValue, 3, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(4, 0, 0), new TimeOnly(8, 0, 0))), new DateLimits(new DateTime(2020, 1, 1)));

            var res2 = scheduler.Execute(configuration2);

            res1.NextExecutionTime.Should().Be(new DateTime(2020, 1, 4, 4, 0, 0));
            res1.Description.Should().Be("Occurs every day and every 3 hours between 4:00:00 AM and 8:00:00 AM starting on 01/01/2020");

            res2.NextExecutionTime.Should().Be(new DateTime(2020, 1, 4, 7, 0, 0));
            res2.Description.Should().Be("Occurs every day and every 3 hours between 4:00:00 AM and 8:00:00 AM starting on 01/01/2020");
        }

        [Fact]
        public void Should_Validate_Execution_Time_During_Week()
        {
            var scheduler = new Scheduler();
            var weeklyConfig = new WeeklyConfiguration(0, new List<DayOfWeek>() { DayOfWeek.Monday, DayOfWeek.Thursday, DayOfWeek.Friday });

            var configuration1 = new Configuration
                (new DateTime(2020, 1, 1), ConfigType.Recurring, true, null, Occurs.Daily, null,
                weeklyConfig,
                new DailyConfiguration(DailyConfigType.Recurring, TimeOnly.MinValue, 2, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(4, 0, 0), new TimeOnly(8, 0, 0))), new DateLimits(new DateTime(2020, 1, 1)));

            var res1 = scheduler.Execute(configuration1);

            var configuration2 = new Configuration
                (res1.NextExecutionTime, ConfigType.Recurring, true, null, Occurs.Daily, null,
                weeklyConfig,
                new DailyConfiguration(DailyConfigType.Recurring, TimeOnly.MinValue, 2, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(4, 0, 0), new TimeOnly(8, 0, 0))), new DateLimits(new DateTime(2020, 1, 1)));

            var res2 = scheduler.Execute(configuration2);

            var configuration3 = new Configuration
                (res2.NextExecutionTime, ConfigType.Recurring, true, null, Occurs.Daily, null,
                weeklyConfig,
                new DailyConfiguration(DailyConfigType.Recurring, TimeOnly.MinValue, 2, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(4, 0, 0), new TimeOnly(8, 0, 0))), new DateLimits(new DateTime(2020, 1, 1)));
            var res3 = scheduler.Execute(configuration3);

            var configuration4 = new Configuration
                (res3.NextExecutionTime, ConfigType.Recurring, true, null, Occurs.Daily, null,
                weeklyConfig,
                new DailyConfiguration(DailyConfigType.Recurring, TimeOnly.MinValue, 2, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(4, 0, 0), new TimeOnly(8, 0, 0))), new DateLimits(new DateTime(2020, 1, 1)));
            var res4 = scheduler.Execute(configuration4);

            var configuration5 = new Configuration
                (res4.NextExecutionTime, ConfigType.Recurring, true, null, Occurs.Daily, null,
                weeklyConfig,
                new DailyConfiguration(DailyConfigType.Recurring, TimeOnly.MinValue, 2, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(4, 0, 0), new TimeOnly(8, 0, 0))), new DateLimits(new DateTime(2020, 1, 1)));
            var res5 = scheduler.Execute(configuration5);

            var configuration6 = new Configuration
                (res5.NextExecutionTime, ConfigType.Recurring, true, null, Occurs.Daily, null,
                weeklyConfig,
                new DailyConfiguration(DailyConfigType.Recurring, TimeOnly.MinValue, 2, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(4, 0, 0), new TimeOnly(8, 0, 0))), new DateLimits(new DateTime(2020, 1, 1)));
            var res6 = scheduler.Execute(configuration6);

            var configuration7 = new Configuration
                (res6.NextExecutionTime, ConfigType.Recurring, true, null, Occurs.Daily, null,
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
            res1.Description.Should().Be("Occurs every week on monday, thursday and friday every 2 hours between 4:00:00 AM and 8:00:00 AM starting on 01/01/2020");
            res2.Description.Should().Be("Occurs every week on monday, thursday and friday every 2 hours between 4:00:00 AM and 8:00:00 AM starting on 01/01/2020");
            res3.Description.Should().Be("Occurs every week on monday, thursday and friday every 2 hours between 4:00:00 AM and 8:00:00 AM starting on 01/01/2020");
            res4.Description.Should().Be("Occurs every week on monday, thursday and friday every 2 hours between 4:00:00 AM and 8:00:00 AM starting on 01/01/2020");
            res5.Description.Should().Be("Occurs every week on monday, thursday and friday every 2 hours between 4:00:00 AM and 8:00:00 AM starting on 01/01/2020");
            res6.Description.Should().Be("Occurs every week on monday, thursday and friday every 2 hours between 4:00:00 AM and 8:00:00 AM starting on 01/01/2020");
            res7.Description.Should().Be("Occurs every week on monday, thursday and friday every 2 hours between 4:00:00 AM and 8:00:00 AM starting on 01/01/2020");
        }

        [Fact]
        public void Should_Validate_Execution_Time_Space_Between_Weeks_AddingHours()
        {
            var scheduler = new Scheduler();
            var weeklyConfig = new WeeklyConfiguration(2, new List<DayOfWeek>() { DayOfWeek.Monday, DayOfWeek.Thursday, DayOfWeek.Friday });

            var configuration1 = new Configuration
                (new DateTime(2020, 1, 13), ConfigType.Recurring, true, null, Occurs.Daily, null,
                weeklyConfig,
                new DailyConfiguration(DailyConfigType.Recurring, TimeOnly.MinValue, 2, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(4, 0, 0), new TimeOnly(8, 0, 0))), new DateLimits(new DateTime(2020, 1, 1)));

            var res1 = scheduler.Execute(configuration1);

            var configuration2 = new Configuration
                (res1.NextExecutionTime, ConfigType.Recurring, true, null, Occurs.Daily, null,
                weeklyConfig,
                new DailyConfiguration(DailyConfigType.Recurring, TimeOnly.MinValue, 2, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(4, 0, 0), new TimeOnly(8, 0, 0))), new DateLimits(new DateTime(2020, 1, 1)));

            var res2 = scheduler.Execute(configuration2);

            var configuration3 = new Configuration
                (res2.NextExecutionTime, ConfigType.Recurring, true, null, Occurs.Daily, null,
                weeklyConfig,
                new DailyConfiguration(DailyConfigType.Recurring, TimeOnly.MinValue, 2, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(4, 0, 0), new TimeOnly(8, 0, 0))), new DateLimits(new DateTime(2020, 1, 1)));
            var res3 = scheduler.Execute(configuration3);

            var configuration4 = new Configuration
                (res3.NextExecutionTime, ConfigType.Recurring, true, null, Occurs.Daily, null,
                weeklyConfig,
                new DailyConfiguration(DailyConfigType.Recurring, TimeOnly.MinValue, 2, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(4, 0, 0), new TimeOnly(8, 0, 0))), new DateLimits(new DateTime(2020, 1, 1)));
            var res4 = scheduler.Execute(configuration4);

            var configuration5 = new Configuration
                (res4.NextExecutionTime, ConfigType.Recurring, true, null, Occurs.Daily, null,
                weeklyConfig,
                new DailyConfiguration(DailyConfigType.Recurring, TimeOnly.MinValue, 2, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(4, 0, 0), new TimeOnly(8, 0, 0))), new DateLimits(new DateTime(2020, 1, 1)));
            var res5 = scheduler.Execute(configuration5);

            var configuration6 = new Configuration
                (res5.NextExecutionTime, ConfigType.Recurring, true, null, Occurs.Daily, null,
                weeklyConfig,
                new DailyConfiguration(DailyConfigType.Recurring, TimeOnly.MinValue, 2, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(4, 0, 0), new TimeOnly(8, 0, 0))), new DateLimits(new DateTime(2020, 1, 1)));
            var res6 = scheduler.Execute(configuration6);

            var configuration7 = new Configuration
                (res6.NextExecutionTime, ConfigType.Recurring, true, null, Occurs.Daily, null,
                weeklyConfig,
                new DailyConfiguration(DailyConfigType.Recurring, TimeOnly.MinValue, 2, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(4, 0, 0), new TimeOnly(8, 0, 0))), new DateLimits(new DateTime(2020, 1, 1)));
            var res7 = scheduler.Execute(configuration7);


            var configuration8 = new Configuration
                (res7.NextExecutionTime, ConfigType.Recurring, true, null, Occurs.Daily, null,
                weeklyConfig,
                new DailyConfiguration(DailyConfigType.Recurring, TimeOnly.MinValue, 2, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(4, 0, 0), new TimeOnly(8, 0, 0))), new DateLimits(new DateTime(2020, 1, 1)));
            var res8 = scheduler.Execute(configuration8);

            var configuration9 = new Configuration
                (res8.NextExecutionTime, ConfigType.Recurring, true, null, Occurs.Daily, null,
                weeklyConfig,
                new DailyConfiguration(DailyConfigType.Recurring, TimeOnly.MinValue, 2, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(4, 0, 0), new TimeOnly(8, 0, 0))), new DateLimits(new DateTime(2020, 1, 1)));
            var res9 = scheduler.Execute(configuration9);

            var configuration10 = new Configuration
                (res9.NextExecutionTime, ConfigType.Recurring, true, null, Occurs.Daily, null,
                weeklyConfig,
                new DailyConfiguration(DailyConfigType.Recurring, TimeOnly.MinValue, 2, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(4, 0, 0), new TimeOnly(8, 0, 0))), new DateLimits(new DateTime(2020, 1, 1)));
            var res10 = scheduler.Execute(configuration10);

            var configuration11 = new Configuration
                (res10.NextExecutionTime, ConfigType.Recurring, true, null, Occurs.Daily, null,
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
            res1.Description.Should().Be("Occurs every 2 weeks on monday, thursday and friday every 2 hours between 4:00:00 AM and 8:00:00 AM starting on 01/01/2020");
            res2.Description.Should().Be("Occurs every 2 weeks on monday, thursday and friday every 2 hours between 4:00:00 AM and 8:00:00 AM starting on 01/01/2020");
            res3.Description.Should().Be("Occurs every 2 weeks on monday, thursday and friday every 2 hours between 4:00:00 AM and 8:00:00 AM starting on 01/01/2020");
            res4.Description.Should().Be("Occurs every 2 weeks on monday, thursday and friday every 2 hours between 4:00:00 AM and 8:00:00 AM starting on 01/01/2020");
            res5.Description.Should().Be("Occurs every 2 weeks on monday, thursday and friday every 2 hours between 4:00:00 AM and 8:00:00 AM starting on 01/01/2020");
            res6.Description.Should().Be("Occurs every 2 weeks on monday, thursday and friday every 2 hours between 4:00:00 AM and 8:00:00 AM starting on 01/01/2020");
            res7.Description.Should().Be("Occurs every 2 weeks on monday, thursday and friday every 2 hours between 4:00:00 AM and 8:00:00 AM starting on 01/01/2020");
            res8.Description.Should().Be("Occurs every 2 weeks on monday, thursday and friday every 2 hours between 4:00:00 AM and 8:00:00 AM starting on 01/01/2020");
            res9.Description.Should().Be("Occurs every 2 weeks on monday, thursday and friday every 2 hours between 4:00:00 AM and 8:00:00 AM starting on 01/01/2020");
            res10.Description.Should().Be("Occurs every 2 weeks on monday, thursday and friday every 2 hours between 4:00:00 AM and 8:00:00 AM starting on 01/01/2020");
            res11.Description.Should().Be("Occurs every 2 weeks on monday, thursday and friday every 2 hours between 4:00:00 AM and 8:00:00 AM starting on 01/01/2020");
        }

        [Fact]
        public void Should_Validate_Execution_Time_Space_Between_Weeks_AddingMinutes()
        {
            var scheduler = new Scheduler();
            var weeklyConfig = new WeeklyConfiguration(2, new List<DayOfWeek>() { DayOfWeek.Monday, DayOfWeek.Thursday, DayOfWeek.Friday });
            var dailyFrecuencyType = DailyFrecuency.Minutes;
            var configuration1 = new Configuration
                (new DateTime(2020, 1, 13), ConfigType.Recurring, true, null, Occurs.Daily, null,
                weeklyConfig,
                new DailyConfiguration(DailyConfigType.Recurring, TimeOnly.MinValue, 2, dailyFrecuencyType, new TimeLimits(new TimeOnly(4, 0, 0), new TimeOnly(8, 0, 0))), new DateLimits(new DateTime(2020, 1, 1)));

            var res1 = scheduler.Execute(configuration1);

            var configuration2 = new Configuration
                (res1.NextExecutionTime, ConfigType.Recurring, true, null, Occurs.Daily, null,
                weeklyConfig,
                new DailyConfiguration(DailyConfigType.Recurring, TimeOnly.MinValue, 2, dailyFrecuencyType, new TimeLimits(new TimeOnly(4, 0, 0), new TimeOnly(8, 0, 0))), new DateLimits(new DateTime(2020, 1, 1)));

            var res2 = scheduler.Execute(configuration2);

            var configuration3 = new Configuration
                (res2.NextExecutionTime, ConfigType.Recurring, true, null, Occurs.Daily, null,
                weeklyConfig,
                new DailyConfiguration(DailyConfigType.Recurring, TimeOnly.MinValue, 2, dailyFrecuencyType, new TimeLimits(new TimeOnly(4, 0, 0), new TimeOnly(8, 0, 0))), new DateLimits(new DateTime(2020, 1, 1)));
            var res3 = scheduler.Execute(configuration3);

            var configuration4 = new Configuration
                (res3.NextExecutionTime, ConfigType.Recurring, true, null, Occurs.Daily, null,
                weeklyConfig,
                new DailyConfiguration(DailyConfigType.Recurring, TimeOnly.MinValue, 2, dailyFrecuencyType, new TimeLimits(new TimeOnly(4, 0, 0), new TimeOnly(8, 0, 0))), new DateLimits(new DateTime(2020, 1, 1)));
            var res4 = scheduler.Execute(configuration4);

            var configuration5 = new Configuration
                (res4.NextExecutionTime, ConfigType.Recurring, true, null, Occurs.Daily, null,
                weeklyConfig,
                new DailyConfiguration(DailyConfigType.Recurring, TimeOnly.MinValue, 2, dailyFrecuencyType, new TimeLimits(new TimeOnly(4, 0, 0), new TimeOnly(8, 0, 0))), new DateLimits(new DateTime(2020, 1, 1)));
            var res5 = scheduler.Execute(configuration5);

            var configuration6 = new Configuration
                (res5.NextExecutionTime, ConfigType.Recurring, true, null, Occurs.Daily, null,
                weeklyConfig,
                new DailyConfiguration(DailyConfigType.Recurring, TimeOnly.MinValue, 2, dailyFrecuencyType, new TimeLimits(new TimeOnly(4, 0, 0), new TimeOnly(8, 0, 0))), new DateLimits(new DateTime(2020, 1, 1)));
            var res6 = scheduler.Execute(configuration6);

            var configuration7 = new Configuration
                (res6.NextExecutionTime, ConfigType.Recurring, true, null, Occurs.Daily, null,
                weeklyConfig,
                new DailyConfiguration(DailyConfigType.Recurring, TimeOnly.MinValue, 2, dailyFrecuencyType, new TimeLimits(new TimeOnly(4, 0, 0), new TimeOnly(8, 0, 0))), new DateLimits(new DateTime(2020, 1, 1)));
            var res7 = scheduler.Execute(configuration7);


            var configuration8 = new Configuration
                (res7.NextExecutionTime, ConfigType.Recurring, true, null, Occurs.Daily, null,
                weeklyConfig,
                new DailyConfiguration(DailyConfigType.Recurring, TimeOnly.MinValue, 2, dailyFrecuencyType, new TimeLimits(new TimeOnly(4, 0, 0), new TimeOnly(8, 0, 0))), new DateLimits(new DateTime(2020, 1, 1)));
            var res8 = scheduler.Execute(configuration8);

            var configuration9 = new Configuration
                (res8.NextExecutionTime, ConfigType.Recurring, true, null, Occurs.Daily, null,
                weeklyConfig,
                new DailyConfiguration(DailyConfigType.Recurring, TimeOnly.MinValue, 2, dailyFrecuencyType, new TimeLimits(new TimeOnly(4, 0, 0), new TimeOnly(8, 0, 0))), new DateLimits(new DateTime(2020, 1, 1)));
            var res9 = scheduler.Execute(configuration9);

            var configuration10 = new Configuration
                (res9.NextExecutionTime, ConfigType.Recurring, true, null, Occurs.Daily, null,
                weeklyConfig,
                new DailyConfiguration(DailyConfigType.Recurring, TimeOnly.MinValue, 2, dailyFrecuencyType, new TimeLimits(new TimeOnly(4, 0, 0), new TimeOnly(8, 0, 0))), new DateLimits(new DateTime(2020, 1, 1)));
            var res10 = scheduler.Execute(configuration10);

            var configuration11 = new Configuration
                (res10.NextExecutionTime, ConfigType.Recurring, true, null, Occurs.Daily, null,
                weeklyConfig,
                new DailyConfiguration(DailyConfigType.Recurring, TimeOnly.MinValue, 2, dailyFrecuencyType, new TimeLimits(new TimeOnly(4, 0, 0), new TimeOnly(8, 0, 0))), new DateLimits(new DateTime(2020, 1, 1)));
            var res11 = scheduler.Execute(configuration11);
            res1.NextExecutionTime.Should().Be(new DateTime(2020, 1, 13, 4, 0, 0));
            res2.NextExecutionTime.Should().Be(new DateTime(2020, 1, 13, 4, 2, 0));
            res3.NextExecutionTime.Should().Be(new DateTime(2020, 1, 13, 4, 4, 0));
            res4.NextExecutionTime.Should().Be(new DateTime(2020, 1, 13, 4, 6, 0));
            res5.NextExecutionTime.Should().Be(new DateTime(2020, 1, 13, 4, 8, 0));
            res6.NextExecutionTime.Should().Be(new DateTime(2020, 1, 13, 4, 10, 0));
            res7.NextExecutionTime.Should().Be(new DateTime(2020, 1, 13, 4, 12, 0));
            res8.NextExecutionTime.Should().Be(new DateTime(2020, 1, 13, 4, 14, 0));
            res9.NextExecutionTime.Should().Be(new DateTime(2020, 1, 13, 4, 16, 0));
            res10.NextExecutionTime.Should().Be(new DateTime(2020, 1, 13, 4, 18, 0));
            res11.NextExecutionTime.Should().Be(new DateTime(2020, 1, 13, 4, 20, 0));
            res1.Description.Should().Be("Occurs every 2 weeks on monday, thursday and friday every 2 minutes between 4:00:00 AM and 8:00:00 AM starting on 01/01/2020");
            res2.Description.Should().Be("Occurs every 2 weeks on monday, thursday and friday every 2 minutes between 4:00:00 AM and 8:00:00 AM starting on 01/01/2020");
            res3.Description.Should().Be("Occurs every 2 weeks on monday, thursday and friday every 2 minutes between 4:00:00 AM and 8:00:00 AM starting on 01/01/2020");
            res4.Description.Should().Be("Occurs every 2 weeks on monday, thursday and friday every 2 minutes between 4:00:00 AM and 8:00:00 AM starting on 01/01/2020");
            res5.Description.Should().Be("Occurs every 2 weeks on monday, thursday and friday every 2 minutes between 4:00:00 AM and 8:00:00 AM starting on 01/01/2020");
            res6.Description.Should().Be("Occurs every 2 weeks on monday, thursday and friday every 2 minutes between 4:00:00 AM and 8:00:00 AM starting on 01/01/2020");
            res7.Description.Should().Be("Occurs every 2 weeks on monday, thursday and friday every 2 minutes between 4:00:00 AM and 8:00:00 AM starting on 01/01/2020");
            res8.Description.Should().Be("Occurs every 2 weeks on monday, thursday and friday every 2 minutes between 4:00:00 AM and 8:00:00 AM starting on 01/01/2020");
            res9.Description.Should().Be("Occurs every 2 weeks on monday, thursday and friday every 2 minutes between 4:00:00 AM and 8:00:00 AM starting on 01/01/2020");
            res10.Description.Should().Be("Occurs every 2 weeks on monday, thursday and friday every 2 minutes between 4:00:00 AM and 8:00:00 AM starting on 01/01/2020");
            res11.Description.Should().Be("Occurs every 2 weeks on monday, thursday and friday every 2 minutes between 4:00:00 AM and 8:00:00 AM starting on 01/01/2020");
        }

        [Fact]
        public void Should_Validate_Execution_Time_Space_Between_Weeks_AddingSeconds()
        {
            var scheduler = new Scheduler();
            var weeklyConfig = new WeeklyConfiguration(2, new List<DayOfWeek>() { DayOfWeek.Monday, DayOfWeek.Thursday, DayOfWeek.Friday });
            var dailyFrecuencyType = DailyFrecuency.Seconds;
            var configuration1 = new Configuration
                (new DateTime(2020, 1, 13), ConfigType.Recurring, true, null, Occurs.Daily, null,
                weeklyConfig,
                new DailyConfiguration(DailyConfigType.Recurring, TimeOnly.MinValue, 5, dailyFrecuencyType, new TimeLimits(new TimeOnly(4, 0, 50), new TimeOnly(8, 0, 0))), new DateLimits(new DateTime(2020, 1, 1)));

            var res1 = scheduler.Execute(configuration1);

            var configuration2 = new Configuration
                (res1.NextExecutionTime, ConfigType.Recurring, true, null, Occurs.Daily, null,
                weeklyConfig,
                new DailyConfiguration(DailyConfigType.Recurring, TimeOnly.MinValue, 5, dailyFrecuencyType, new TimeLimits(new TimeOnly(4, 0, 50), new TimeOnly(8, 0, 0))), new DateLimits(new DateTime(2020, 1, 1)));

            var res2 = scheduler.Execute(configuration2);

            var configuration3 = new Configuration
                (res2.NextExecutionTime, ConfigType.Recurring, true, null, Occurs.Daily, null,
                weeklyConfig,
                new DailyConfiguration(DailyConfigType.Recurring, TimeOnly.MinValue, 5, dailyFrecuencyType, new TimeLimits(new TimeOnly(4, 0, 50), new TimeOnly(8, 0, 0))), new DateLimits(new DateTime(2020, 1, 1)));
            var res3 = scheduler.Execute(configuration3);

            var configuration4 = new Configuration
                (res3.NextExecutionTime, ConfigType.Recurring, true, null, Occurs.Daily, null,
                weeklyConfig,
                new DailyConfiguration(DailyConfigType.Recurring, TimeOnly.MinValue, 5, dailyFrecuencyType, new TimeLimits(new TimeOnly(4, 0, 50), new TimeOnly(8, 0, 0))), new DateLimits(new DateTime(2020, 1, 1)));
            var res4 = scheduler.Execute(configuration4);

            var configuration5 = new Configuration
                (res4.NextExecutionTime, ConfigType.Recurring, true, null, Occurs.Daily, null,
                weeklyConfig,
                new DailyConfiguration(DailyConfigType.Recurring, TimeOnly.MinValue, 5, dailyFrecuencyType, new TimeLimits(new TimeOnly(4, 0, 50), new TimeOnly(8, 0, 0))), new DateLimits(new DateTime(2020, 1, 1)));
            var res5 = scheduler.Execute(configuration5);

            var configuration6 = new Configuration
                (res5.NextExecutionTime, ConfigType.Recurring, true, null, Occurs.Daily, null,
                weeklyConfig,
                new DailyConfiguration(DailyConfigType.Recurring, TimeOnly.MinValue, 5, dailyFrecuencyType, new TimeLimits(new TimeOnly(4, 0, 50), new TimeOnly(8, 0, 0))), new DateLimits(new DateTime(2020, 1, 1)));
            var res6 = scheduler.Execute(configuration6);

            var configuration7 = new Configuration
                (res6.NextExecutionTime, ConfigType.Recurring, true, null, Occurs.Daily, null,
                weeklyConfig,
                new DailyConfiguration(DailyConfigType.Recurring, TimeOnly.MinValue, 5, dailyFrecuencyType, new TimeLimits(new TimeOnly(4, 0, 50), new TimeOnly(8, 0, 0))), new DateLimits(new DateTime(2020, 1, 1)));
            var res7 = scheduler.Execute(configuration7);


            var configuration8 = new Configuration
                (res7.NextExecutionTime, ConfigType.Recurring, true, null, Occurs.Daily, null,
                weeklyConfig,
                new DailyConfiguration(DailyConfigType.Recurring, TimeOnly.MinValue, 5, dailyFrecuencyType, new TimeLimits(new TimeOnly(4, 0, 50), new TimeOnly(8, 0, 0))), new DateLimits(new DateTime(2020, 1, 1)));
            var res8 = scheduler.Execute(configuration8);

            var configuration9 = new Configuration
                (res8.NextExecutionTime, ConfigType.Recurring, true, null, Occurs.Daily, null,
                weeklyConfig,
                new DailyConfiguration(DailyConfigType.Recurring, TimeOnly.MinValue, 5, dailyFrecuencyType, new TimeLimits(new TimeOnly(4, 0, 50), new TimeOnly(8, 0, 0))), new DateLimits(new DateTime(2020, 1, 1)));
            var res9 = scheduler.Execute(configuration9);

            var configuration10 = new Configuration
                (res9.NextExecutionTime, ConfigType.Recurring, true, null, Occurs.Daily, null,
                weeklyConfig,
                new DailyConfiguration(DailyConfigType.Recurring, TimeOnly.MinValue, 5, dailyFrecuencyType, new TimeLimits(new TimeOnly(4, 0, 50), new TimeOnly(8, 0, 0))), new DateLimits(new DateTime(2020, 1, 1)));
            var res10 = scheduler.Execute(configuration10);

            var configuration11 = new Configuration
                (res10.NextExecutionTime, ConfigType.Recurring, true, null, Occurs.Daily, null,
                weeklyConfig,
                new DailyConfiguration(DailyConfigType.Recurring, TimeOnly.MinValue, 5, dailyFrecuencyType, new TimeLimits(new TimeOnly(4, 0, 50), new TimeOnly(8, 0, 0))), new DateLimits(new DateTime(2020, 1, 1)));
            var res11 = scheduler.Execute(configuration11);

            res1.NextExecutionTime.Should().Be(new DateTime(2020, 1, 13, 4, 0, 50));
            res2.NextExecutionTime.Should().Be(new DateTime(2020, 1, 13, 4, 0, 55));
            res3.NextExecutionTime.Should().Be(new DateTime(2020, 1, 13, 4, 1, 00));
            res4.NextExecutionTime.Should().Be(new DateTime(2020, 1, 13, 4, 1, 5));
            res5.NextExecutionTime.Should().Be(new DateTime(2020, 1, 13, 4, 1, 10));
            res6.NextExecutionTime.Should().Be(new DateTime(2020, 1, 13, 4, 1, 15));
            res7.NextExecutionTime.Should().Be(new DateTime(2020, 1, 13, 4, 1, 20));
            res8.NextExecutionTime.Should().Be(new DateTime(2020, 1, 13, 4, 1, 25));
            res9.NextExecutionTime.Should().Be(new DateTime(2020, 1, 13, 4, 1, 30));
            res10.NextExecutionTime.Should().Be(new DateTime(2020, 1, 13, 4, 1, 35));
            res11.NextExecutionTime.Should().Be(new DateTime(2020, 1, 13, 4, 1, 40));
            res1.Description.Should().Be("Occurs every 2 weeks on monday, thursday and friday every 5 seconds between 4:00:50 AM and 8:00:00 AM starting on 01/01/2020");
            res2.Description.Should().Be("Occurs every 2 weeks on monday, thursday and friday every 5 seconds between 4:00:50 AM and 8:00:00 AM starting on 01/01/2020");
            res3.Description.Should().Be("Occurs every 2 weeks on monday, thursday and friday every 5 seconds between 4:00:50 AM and 8:00:00 AM starting on 01/01/2020");
            res4.Description.Should().Be("Occurs every 2 weeks on monday, thursday and friday every 5 seconds between 4:00:50 AM and 8:00:00 AM starting on 01/01/2020");
            res5.Description.Should().Be("Occurs every 2 weeks on monday, thursday and friday every 5 seconds between 4:00:50 AM and 8:00:00 AM starting on 01/01/2020");
            res6.Description.Should().Be("Occurs every 2 weeks on monday, thursday and friday every 5 seconds between 4:00:50 AM and 8:00:00 AM starting on 01/01/2020");
            res7.Description.Should().Be("Occurs every 2 weeks on monday, thursday and friday every 5 seconds between 4:00:50 AM and 8:00:00 AM starting on 01/01/2020");
            res8.Description.Should().Be("Occurs every 2 weeks on monday, thursday and friday every 5 seconds between 4:00:50 AM and 8:00:00 AM starting on 01/01/2020");
            res9.Description.Should().Be("Occurs every 2 weeks on monday, thursday and friday every 5 seconds between 4:00:50 AM and 8:00:00 AM starting on 01/01/2020");
            res10.Description.Should().Be("Occurs every 2 weeks on monday, thursday and friday every 5 seconds between 4:00:50 AM and 8:00:00 AM starting on 01/01/2020");
            res11.Description.Should().Be("Occurs every 2 weeks on monday, thursday and friday every 5 seconds between 4:00:50 AM and 8:00:00 AM starting on 01/01/2020");
        }

        [Fact]
        public void Should_Validate_Execution_Time_BetweenWeeks_With_DailyFrecuency_OnceAt()
        {
            var scheduler = new Scheduler();
            var weeklyConfig = new WeeklyConfiguration(2, new List<DayOfWeek>() { DayOfWeek.Monday, DayOfWeek.Thursday, DayOfWeek.Friday });
            var configuration1 = new Configuration
                (new DateTime(2020, 1, 13), ConfigType.Recurring, true, null, Occurs.Weekly, null,
                weeklyConfig,
                new DailyConfiguration(DailyConfigType.Once, new TimeOnly(5, 0, 0), null, null, null), new DateLimits(new DateTime(2020, 1, 1)));
            var res1 = scheduler.Execute(configuration1);

            var configuration2 = new Configuration
               (res1.NextExecutionTime, ConfigType.Recurring, true, null, Occurs.Weekly, null,
               weeklyConfig,
               new DailyConfiguration(DailyConfigType.Once, new TimeOnly(5, 0, 0), null, null, null), new DateLimits(new DateTime(2020, 1, 1)));
            var res2 = scheduler.Execute(configuration2);

            var configuration3 = new Configuration
               (res2.NextExecutionTime, ConfigType.Recurring, true, null, Occurs.Weekly, null,
               weeklyConfig,
               new DailyConfiguration(DailyConfigType.Once, new TimeOnly(5, 0, 0), null, null, null), new DateLimits(new DateTime(2020, 1, 1)));
            var res3 = scheduler.Execute(configuration3);

            var configuration4 = new Configuration
               (res3.NextExecutionTime, ConfigType.Recurring, true, null, Occurs.Weekly, null,
               weeklyConfig,
               new DailyConfiguration(DailyConfigType.Once, new TimeOnly(5, 0, 0), null, null, null), new DateLimits(new DateTime(2020, 1, 1)));
            var res4 = scheduler.Execute(configuration4);

            var configuration5 = new Configuration
               (res4.NextExecutionTime, ConfigType.Recurring, true, null, Occurs.Weekly, null,
               weeklyConfig,
               new DailyConfiguration(DailyConfigType.Once, new TimeOnly(5, 0, 0), null, null, null), new DateLimits(new DateTime(2020, 1, 1)));
            var res5 = scheduler.Execute(configuration5);

            res1.NextExecutionTime.Should().Be(new DateTime(2020, 1, 13, 5, 0, 0));
            res2.NextExecutionTime.Should().Be(new DateTime(2020, 1, 16, 5, 0, 0));
            res3.NextExecutionTime.Should().Be(new DateTime(2020, 1, 17, 5, 0, 0));
            res4.NextExecutionTime.Should().Be(new DateTime(2020, 2, 3, 5, 0, 0));
            res5.NextExecutionTime.Should().Be(new DateTime(2020, 2, 6, 5, 0, 0));
            res1.Description.Should().Be("Occurs every 2 weeks on monday, thursday and friday one time at 5:00:00 AM starting on 01/01/2020");
            res2.Description.Should().Be("Occurs every 2 weeks on monday, thursday and friday one time at 5:00:00 AM starting on 01/01/2020");
            res3.Description.Should().Be("Occurs every 2 weeks on monday, thursday and friday one time at 5:00:00 AM starting on 01/01/2020");
            res4.Description.Should().Be("Occurs every 2 weeks on monday, thursday and friday one time at 5:00:00 AM starting on 01/01/2020");
            res5.Description.Should().Be("Occurs every 2 weeks on monday, thursday and friday one time at 5:00:00 AM starting on 01/01/2020");
        }

        [Fact]
        public void Should_Be_Next_Execution_Date_DayNumber()
        {
            var scheduler = new Scheduler();
            var config = new Configuration(new DateTime(2020, 4, 1, 4, 18, 0), ConfigType.Recurring, true, null,
                Occurs.Monthly, new MonthlyConfiguration(MonthlyConfigType.DayNumberOption, 8, Ordinal.First, KindOfDay.Monday, 0), null,
                new DailyConfiguration(DailyConfigType.Recurring, TimeOnly.MinValue, 1, DailyFrecuency.Hours,
                new TimeLimits(new TimeOnly(3, 0, 0), new TimeOnly(6, 0, 0))), new DateLimits(new DateTime(2020, 1, 1)));
            var res1 = scheduler.Execute(config);
            res1.NextExecutionTime.Should().Be(new DateTime(2020, 4, 8, 4, 18, 0));
            res1.Description.Should().Be("Occurs the 8th of very months and every 1 hours between 3:00:00 AM and 6:00:00 AM starting on 01/01/2020");

        }

        [Fact]
        public void Should_Be_Next_Execution_Date_DayNumber_SchedulerExample_Skipping_Months()
        {
            var scheduler = new Scheduler();
            var config1 = new Configuration(
                new DateTime(2020, 4, 1, 4, 18, 0),
                ConfigType.Recurring, true,
                null,
                Occurs.Monthly,
                    new MonthlyConfiguration(MonthlyConfigType.DayNumberOption, 8, Ordinal.First, KindOfDay.Monday, 3),
                    null,
                    new DailyConfiguration(DailyConfigType.Recurring, TimeOnly.MinValue, 6,
                    DailyFrecuency.Hours, new TimeLimits(new TimeOnly(3, 0, 0), new TimeOnly(6, 0, 0))),
                new DateLimits(new DateTime(2020, 1, 1)));

            var res1 = scheduler.Execute(config1);
            res1.NextExecutionTime.Should().Be(new DateTime(2020, 4, 8, 4, 18, 0));
            res1.Description.Should().Be("Occurs the 8th of very 3 months and every 6 hours between 3:00:00 AM and 6:00:00 AM starting on 01/01/2020");
            var config2 = new Configuration(res1.NextExecutionTime, ConfigType.Recurring, true, null,
                Occurs.Monthly, new MonthlyConfiguration(MonthlyConfigType.DayNumberOption, 8, Ordinal.First, KindOfDay.Monday, 3),
                null,
                new DailyConfiguration(DailyConfigType.Recurring, TimeOnly.MinValue, 6, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(3, 0, 0), new TimeOnly(6, 0, 0))),
                new DateLimits(new DateTime(2020, 1, 1)));
            var res2 = scheduler.Execute(config2);
            res2.NextExecutionTime.Should().Be(new DateTime(2020, 8, 8, 3, 0, 0));
            res2.Description.Should().Be("Occurs the 8th of very 3 months and every 6 hours between 3:00:00 AM and 6:00:00 AM starting on 01/01/2020");

            var config3 = new Configuration(res2.NextExecutionTime, ConfigType.Recurring, true
                , null, Occurs.Monthly, new MonthlyConfiguration(MonthlyConfigType.DayNumberOption, 8, Ordinal.First, KindOfDay.Monday, 3),
                null,
                new DailyConfiguration(DailyConfigType.Recurring, TimeOnly.MinValue, 6, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(3, 0, 0), new TimeOnly(6, 0, 0))),
                new DateLimits(new DateTime(2020, 1, 1)));
            var res3 = scheduler.Execute(config3);
            res3.NextExecutionTime.Should().Be(new DateTime(2020, 12, 8, 3, 0, 0));
            res3.Description.Should().Be("Occurs the 8th of very 3 months and every 6 hours between 3:00:00 AM and 6:00:00 AM starting on 01/01/2020");

            var config4 = new Configuration(res3.NextExecutionTime, ConfigType.Recurring, true, null, Occurs.Monthly, new MonthlyConfiguration(MonthlyConfigType.DayNumberOption, 8, Ordinal.First, KindOfDay.Monday, 3), null,
                new DailyConfiguration(DailyConfigType.Recurring, TimeOnly.MinValue, 6, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(3, 0, 0), new TimeOnly(6, 0, 0))), new DateLimits(new DateTime(2020, 1, 1)));
            var res4 = scheduler.Execute(config4);
            res4.NextExecutionTime.Should().Be(new DateTime(2021, 4, 8, 3, 0, 0));
            res4.Description.Should().Be("Occurs the 8th of very 3 months and every 6 hours between 3:00:00 AM and 6:00:00 AM starting on 01/01/2020");


            var scheduler2 = new Scheduler();

            var exampleConfig1 = new Configuration(new DateTime(2020, 1, 4, 0, 0, 0), ConfigType.Recurring, true, null, Occurs.Monthly,
                new MonthlyConfiguration(MonthlyConfigType.DayNumberOption, 8, Ordinal.First, KindOfDay.Monday, 0),
                null,
                new DailyConfiguration(DailyConfigType.Recurring, TimeOnly.MinValue, 6, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(3, 0, 0), new TimeOnly(6, 0, 0))),
                new DateLimits(new DateTime(2020, 1, 1)));
            var example1 = scheduler2.Execute(exampleConfig1);
            example1.NextExecutionTime.Should().Be(new DateTime(2020, 1, 8, 3, 0, 0));
            example1.Description.Should().Be("Occurs the 8th of very months and every 6 hours between 3:00:00 AM and 6:00:00 AM starting on 01/01/2020");

            var exampleConfig2 = new Configuration(example1.NextExecutionTime, ConfigType.Recurring, true, null, Occurs.Monthly,
                new MonthlyConfiguration(MonthlyConfigType.DayNumberOption, 8, Ordinal.First, KindOfDay.Monday, 0),
                null,
                new DailyConfiguration(DailyConfigType.Recurring, TimeOnly.MinValue, 6, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(3, 0, 0), new TimeOnly(6, 0, 0))),
                new DateLimits(new DateTime(2020, 1, 1)));
            var example2 = scheduler2.Execute(exampleConfig2);
            example2.NextExecutionTime.Should().Be(new DateTime(2020, 2, 8, 3, 0, 0));
            example2.Description.Should().Be("Occurs the 8th of very months and every 6 hours between 3:00:00 AM and 6:00:00 AM starting on 01/01/2020");

            var exampleConfig3 = new Configuration(example2.NextExecutionTime, ConfigType.Recurring, true, null, Occurs.Monthly,
                new MonthlyConfiguration(MonthlyConfigType.DayNumberOption, 8, Ordinal.First, KindOfDay.Monday, 0),
                null,
                new DailyConfiguration(DailyConfigType.Recurring, TimeOnly.MinValue, 6, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(3, 0, 0), new TimeOnly(6, 0, 0))),
                new DateLimits(new DateTime(2020, 1, 1)));
            var example3 = scheduler2.Execute(exampleConfig3);
            example3.NextExecutionTime.Should().Be(new DateTime(2020, 3, 8, 3, 0, 0));
            example3.Description.Should().Be("Occurs the 8th of very months and every 6 hours between 3:00:00 AM and 6:00:00 AM starting on 01/01/2020");

            var exampleConfig4 = new Configuration(example3.NextExecutionTime, ConfigType.Recurring, true, null, Occurs.Monthly,
                new MonthlyConfiguration(MonthlyConfigType.DayNumberOption, 8, Ordinal.First, KindOfDay.Monday, 0),
                null,
                new DailyConfiguration(DailyConfigType.Recurring, TimeOnly.MinValue, 6, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(3, 0, 0), new TimeOnly(6, 0, 0))),
                new DateLimits(new DateTime(2020, 1, 1)));
            var example4 = scheduler2.Execute(exampleConfig4);
            example4.NextExecutionTime.Should().Be(new DateTime(2020, 4, 8, 3, 0, 0));
            example4.Description.Should().Be("Occurs the 8th of very months and every 6 hours between 3:00:00 AM and 6:00:00 AM starting on 01/01/2020");
        }

        [Fact]
        public void Shoud_Be_Next_Execution_Month_Skipping_Months_And_Dates_That_Doesnt_Exist()
        {
            var scheduler = new Scheduler();
            var config1 = new Configuration(new DateTime(2023, 12, 31, 4, 18, 0), ConfigType.Recurring, true, null, Occurs.Monthly,
                new MonthlyConfiguration(MonthlyConfigType.DayNumberOption, 30, Ordinal.First, KindOfDay.Monday, 0), null,
                new DailyConfiguration(DailyConfigType.Recurring, TimeOnly.MinValue, 6, DailyFrecuency.Hours,
                new TimeLimits(new TimeOnly(4, 0, 0), new TimeOnly(6, 0, 0))), new DateLimits(new DateTime(2020, 1, 1)));
            var res1 = scheduler.Execute(config1);
            res1.NextExecutionTime.Should().Be(new DateTime(2024, 1, 30, 4, 0, 0));
            res1.Description.Should().Be("Occurs the 30th of very months and every 6 hours between 4:00:00 AM and 6:00:00 AM starting on 01/01/2020");

            var config2 = new Configuration(res1.NextExecutionTime, ConfigType.Recurring, true, null, Occurs.Monthly,
                new MonthlyConfiguration(MonthlyConfigType.DayNumberOption, 30, Ordinal.First, KindOfDay.Monday, 0), null,
                new DailyConfiguration(DailyConfigType.Recurring, TimeOnly.MinValue, 6, DailyFrecuency.Hours,
                new TimeLimits(new TimeOnly(4, 0, 0), new TimeOnly(6, 0, 0))), new DateLimits(new DateTime(2020, 1, 1)));
            var res2 = scheduler.Execute(config2);
            res2.NextExecutionTime.Should().Be(new DateTime(2024, 3, 30, 4, 0, 0));
            res2.Description.Should().Be("Occurs the 30th of very months and every 6 hours between 4:00:00 AM and 6:00:00 AM starting on 01/01/2020");

            var config3 = new Configuration(res2.NextExecutionTime, ConfigType.Recurring, true, null, Occurs.Monthly,
                new MonthlyConfiguration(MonthlyConfigType.DayNumberOption, 30, Ordinal.First, KindOfDay.Monday, 0), null,
                new DailyConfiguration(DailyConfigType.Recurring, TimeOnly.MinValue, 6, DailyFrecuency.Hours,
                new TimeLimits(new TimeOnly(4, 0, 0), new TimeOnly(6, 0, 0))), new DateLimits(new DateTime(2020, 1, 1)));
            var res3 = scheduler.Execute(config3);
            res3.NextExecutionTime.Should().Be(new DateTime(2024, 4, 30, 4, 0, 0));
            res3.Description.Should().Be("Occurs the 30th of very months and every 6 hours between 4:00:00 AM and 6:00:00 AM starting on 01/01/2020");

            var config4 = new Configuration(res3.NextExecutionTime, ConfigType.Recurring, true, null, Occurs.Monthly,
                new MonthlyConfiguration(MonthlyConfigType.DayNumberOption, 30, Ordinal.First, KindOfDay.Monday, 0), null,
                new DailyConfiguration(DailyConfigType.Recurring, TimeOnly.MinValue, 6, DailyFrecuency.Hours,
                new TimeLimits(new TimeOnly(4, 0, 0), new TimeOnly(6, 0, 0))), new DateLimits(new DateTime(2020, 1, 1)));
            var res4 = scheduler.Execute(config4);
            res4.NextExecutionTime.Should().Be(new DateTime(2024, 5, 30, 4, 0, 0));
            res4.Description.Should().Be("Occurs the 30th of very months and every 6 hours between 4:00:00 AM and 6:00:00 AM starting on 01/01/2020");



            var schedulerExample = new Scheduler();
            var exampleConfig1 = new Configuration(new DateTime(2023, 12, 31, 4, 18, 0), ConfigType.Recurring, true, null, Occurs.Monthly,
                new MonthlyConfiguration(MonthlyConfigType.DayNumberOption, 31, Ordinal.First, KindOfDay.Monday, 0),
                null,
                new DailyConfiguration(DailyConfigType.Recurring, TimeOnly.MinValue, 6, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(3, 0, 0), new TimeOnly(6, 0, 0))),
                new DateLimits(new DateTime(2020, 1, 1)));
            var exampleRes1 = schedulerExample.Execute(exampleConfig1);
            exampleRes1.NextExecutionTime.Should().Be(new DateTime(2023, 12, 31, 4, 18, 0));
            exampleRes1.Description.Should().Be("Occurs the 31st of very months and every 6 hours between 3:00:00 AM and 6:00:00 AM starting on 01/01/2020");

            var exampleConfig2 = new Configuration(exampleRes1.NextExecutionTime, ConfigType.Recurring, true, null, Occurs.Monthly,
                new MonthlyConfiguration(MonthlyConfigType.DayNumberOption, 31, Ordinal.First, KindOfDay.Monday, 0),
                null,
                new DailyConfiguration(DailyConfigType.Recurring, TimeOnly.MinValue, 6, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(3, 0, 0), new TimeOnly(6, 0, 0))),
                new DateLimits(new DateTime(2020, 1, 1)));
            var exampleRes2 = schedulerExample.Execute(exampleConfig2);
            exampleRes2.NextExecutionTime.Should().Be(new DateTime(2024, 1, 31, 3, 0, 0));
            exampleRes2.Description.Should().Be("Occurs the 31st of very months and every 6 hours between 3:00:00 AM and 6:00:00 AM starting on 01/01/2020");

            var exampleConfig3 = new Configuration(exampleRes2.NextExecutionTime, ConfigType.Recurring, true, null, Occurs.Monthly,
                new MonthlyConfiguration(MonthlyConfigType.DayNumberOption, 31, Ordinal.First, KindOfDay.Monday, 0),
                null,
                new DailyConfiguration(DailyConfigType.Recurring, TimeOnly.MinValue, 6, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(3, 0, 0), new TimeOnly(6, 0, 0))),
                new DateLimits(new DateTime(2020, 1, 1)));
            var exampleRes3 = schedulerExample.Execute(exampleConfig3);
            exampleRes3.NextExecutionTime.Should().Be(new DateTime(2024, 3, 31, 3, 0, 0));
            exampleRes3.Description.Should().Be("Occurs the 31st of very months and every 6 hours between 3:00:00 AM and 6:00:00 AM starting on 01/01/2020");

            var exampleConfig4 = new Configuration(exampleRes3.NextExecutionTime, ConfigType.Recurring, true, null, Occurs.Monthly,
                new MonthlyConfiguration(MonthlyConfigType.DayNumberOption, 31, Ordinal.First, KindOfDay.Monday, 0),
                null,
                new DailyConfiguration(DailyConfigType.Recurring, TimeOnly.MinValue, 6, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(3, 0, 0), new TimeOnly(6, 0, 0))),
                new DateLimits(new DateTime(2020, 1, 1)));
            var exampleRes4 = schedulerExample.Execute(exampleConfig4);
            exampleRes4.NextExecutionTime.Should().Be(new DateTime(2024, 5, 31, 3, 0, 0));
            exampleRes4.Description.Should().Be("Occurs the 31st of very months and every 6 hours between 3:00:00 AM and 6:00:00 AM starting on 01/01/2020");
        }

        [Fact]
        public void Should_Be_Next_Execution_Date_Ordinals_Thursday()
        {
            var schedulerExample = new Scheduler();
            var schedulerExampleConfig1 = new Configuration(new DateTime(2020, 1, 1), ConfigType.Recurring, true, null, Occurs.Monthly,
                new MonthlyConfiguration(MonthlyConfigType.WeekDayOption, 1, Ordinal.First, KindOfDay.Thursday, 3),
                null,
                new DailyConfiguration(DailyConfigType.Recurring, TimeOnly.MinValue, 1, DailyFrecuency.Hours,
                new TimeLimits(new TimeOnly(3, 0, 0), new TimeOnly(6, 0, 0))), new DateLimits(new DateTime(2020, 1, 1)));
            var exampleResult1 = schedulerExample.Execute(schedulerExampleConfig1);
            exampleResult1.NextExecutionTime.Should().Be(new DateTime(2020, 1, 2, 3, 0, 0));
            exampleResult1.Description.Should().Be("Occurs the first thursday of very 3 months and every 1 hours between 3:00:00 AM and 6:00:00 AM starting on 01/01/2020");

            var schedulerExample2 = new Scheduler();
            var schedulerExampleConfig2 = new Configuration(new DateTime(2020, 1, 1), ConfigType.Recurring, true, null, Occurs.Monthly,
                new MonthlyConfiguration(MonthlyConfigType.WeekDayOption, 1, Ordinal.Second, KindOfDay.Thursday, 3),
                null,
                new DailyConfiguration(DailyConfigType.Recurring, TimeOnly.MinValue, 1, DailyFrecuency.Hours,
                new TimeLimits(new TimeOnly(3, 0, 0), new TimeOnly(6, 0, 0))),
                new DateLimits(new DateTime(2020, 1, 1)));
            var exampleResult2 = schedulerExample2.Execute(schedulerExampleConfig2);
            exampleResult2.NextExecutionTime.Should().Be(new DateTime(2020, 1, 9, 3, 0, 0));
            exampleResult2.Description.Should().Be("Occurs the second thursday of very 3 months and every 1 hours between 3:00:00 AM and 6:00:00 AM starting on 01/01/2020");

            var schedulerExample3 = new Scheduler();
            var schedulerExampleConfig3 = new Configuration(new DateTime(2020, 1, 1), ConfigType.Recurring, true, null, Occurs.Monthly,
                new MonthlyConfiguration(MonthlyConfigType.WeekDayOption, 1, Ordinal.Third, KindOfDay.Thursday, 3),
                null,
                new DailyConfiguration(DailyConfigType.Recurring, TimeOnly.MinValue, 1, DailyFrecuency.Hours,
                new TimeLimits(new TimeOnly(3, 0, 0), new TimeOnly(6, 0, 0))), new DateLimits(new DateTime(2020, 1, 1)));
            var exampleResult3 = schedulerExample3.Execute(schedulerExampleConfig3);
            exampleResult3.NextExecutionTime.Should().Be(new DateTime(2020, 1, 16, 3, 0, 0));
            exampleResult3.Description.Should().Be("Occurs the third thursday of very 3 months and every 1 hours between 3:00:00 AM and 6:00:00 AM starting on 01/01/2020");

            var schedulerExample4 = new Scheduler();
            var schedulerExampleConfig4 = new Configuration(new DateTime(2020, 1, 1), ConfigType.Recurring, true, null, Occurs.Monthly,
                new MonthlyConfiguration(MonthlyConfigType.WeekDayOption, 1, Ordinal.Fourth, KindOfDay.Thursday, 3), null,
                new DailyConfiguration(DailyConfigType.Recurring, TimeOnly.MinValue, 1, DailyFrecuency.Hours,
                new TimeLimits(new TimeOnly(3, 0, 0), new TimeOnly(6, 0, 0))),
                new DateLimits(new DateTime(2020, 1, 1)));
            var exampleResult4 = schedulerExample4.Execute(schedulerExampleConfig4);
            exampleResult4.NextExecutionTime.Should().Be(new DateTime(2020, 1, 23, 3, 0, 0));
            exampleResult4.Description.Should().Be("Occurs the fourth thursday of very 3 months and every 1 hours between 3:00:00 AM and 6:00:00 AM starting on 01/01/2020");

            var schedulerExampleLast = new Scheduler();
            var schedulerExampleConfigLast = new Configuration(new DateTime(2020, 1, 1), ConfigType.Recurring, true, null, Occurs.Monthly,
                new MonthlyConfiguration(MonthlyConfigType.WeekDayOption, 1, Ordinal.Last, KindOfDay.Thursday, 3),
                null,
                new DailyConfiguration(DailyConfigType.Recurring, TimeOnly.MinValue, 1, DailyFrecuency.Hours,
                new TimeLimits(new TimeOnly(3, 0, 0), new TimeOnly(6, 0, 0))), new DateLimits(new DateTime(2020, 1, 1)));
            var exampleResultLast = schedulerExampleLast.Execute(schedulerExampleConfigLast);
            exampleResultLast.NextExecutionTime.Should().Be(new DateTime(2020, 1, 30, 3, 0, 0));
            exampleResultLast.Description.Should().Be("Occurs the last thursday of very 3 months and every 1 hours between 3:00:00 AM and 6:00:00 AM starting on 01/01/2020");
        }

        [Fact]
        public void Should_Be_Next_Execution_Date_Ordinals_Weekdays()
        {
            var schedulerExample5 = new Scheduler();
            var weekdaysConfiguration1 = new Configuration(new DateTime(2020, 1, 1), ConfigType.Recurring, true, null, Occurs.Monthly,
                new MonthlyConfiguration(MonthlyConfigType.WeekDayOption, 1, Ordinal.First, KindOfDay.WeekDay, 3),
                null,
                new DailyConfiguration(DailyConfigType.Recurring, TimeOnly.MinValue, 1, DailyFrecuency.Hours,
                new TimeLimits(new TimeOnly(3, 0, 0), new TimeOnly(6, 0, 0))),
                new DateLimits(new DateTime(2020, 1, 1)));
            var weekdaysResult1 = schedulerExample5.Execute(weekdaysConfiguration1);
            weekdaysResult1.NextExecutionTime.Should().Be(new DateTime(2020, 1, 1, 3, 0, 0));
            weekdaysResult1.Description.Should().Be("Occurs the first weekday of very 3 months and every 1 hours between 3:00:00 AM and 6:00:00 AM starting on 01/01/2020");

            var schedulerExample6 = new Scheduler();
            var weekdaysConfiguration2 = new Configuration(new DateTime(2020, 1, 1), ConfigType.Recurring, true, null, Occurs.Monthly,
                new MonthlyConfiguration(MonthlyConfigType.WeekDayOption, 1, Ordinal.Second, KindOfDay.WeekDay, 3),
                null,
                new DailyConfiguration(DailyConfigType.Recurring, TimeOnly.MinValue, 1, DailyFrecuency.Hours,
                new TimeLimits(new TimeOnly(3, 0, 0), new TimeOnly(6, 0, 0))),
                new DateLimits(new DateTime(2020, 1, 1)));
            var weekdaysResult2 = schedulerExample6.Execute(weekdaysConfiguration2);
            weekdaysResult2.NextExecutionTime.Should().Be(new DateTime(2020, 1, 2, 3, 0, 0));
            weekdaysResult2.Description.Should().Be("Occurs the second weekday of very 3 months and every 1 hours between 3:00:00 AM and 6:00:00 AM starting on 01/01/2020");

            var schedulerExample7 = new Scheduler();
            var weekdaysConfiguration3 = new Configuration(new DateTime(2020, 1, 1), ConfigType.Recurring, true, null, Occurs.Monthly,
                new MonthlyConfiguration(MonthlyConfigType.WeekDayOption, 1, Ordinal.Third, KindOfDay.WeekDay, 3),
                null,
                new DailyConfiguration(DailyConfigType.Recurring, TimeOnly.MinValue, 1, DailyFrecuency.Hours,
                new TimeLimits(new TimeOnly(3, 0, 0), new TimeOnly(6, 0, 0))),
                new DateLimits(new DateTime(2020, 1, 1)));
            var weekdaysResult3 = schedulerExample7.Execute(weekdaysConfiguration3);
            weekdaysResult3.NextExecutionTime.Should().Be(new DateTime(2020, 1, 3, 3, 0, 0));
            weekdaysResult3.Description.Should().Be("Occurs the third weekday of very 3 months and every 1 hours between 3:00:00 AM and 6:00:00 AM starting on 01/01/2020");

            var schedulerExample8 = new Scheduler();
            var weekdaysConfiguration4 = new Configuration(new DateTime(2020, 1, 1), ConfigType.Recurring, true, null, Occurs.Monthly,
                new MonthlyConfiguration(MonthlyConfigType.WeekDayOption, 1, Ordinal.Fourth, KindOfDay.WeekDay, 3),
                null,
                new DailyConfiguration(DailyConfigType.Recurring, TimeOnly.MinValue, 1, DailyFrecuency.Hours,
                new TimeLimits(new TimeOnly(3, 0, 0), new TimeOnly(6, 0, 0))),
                new DateLimits(new DateTime(2020, 1, 1)));
            var weekdaysResult4 = schedulerExample8.Execute(weekdaysConfiguration4);
            weekdaysResult4.NextExecutionTime.Should().Be(new DateTime(2020, 1, 6, 3, 0, 0));
            weekdaysResult4.Description.Should().Be("Occurs the fourth weekday of very 3 months and every 1 hours between 3:00:00 AM and 6:00:00 AM starting on 01/01/2020");
        }

        [Fact]
        public void Should_Be_Next_Execution_Date_Ordinals_WeekEndDays()
        {
            var schedulerWeekendExample5 = new Scheduler();
            var weekEndDaysConfiguration1 = new Configuration(new DateTime(2020, 1, 1), ConfigType.Recurring, true, null, Occurs.Monthly,
                new MonthlyConfiguration(MonthlyConfigType.WeekDayOption, 1, Ordinal.First, KindOfDay.WeekEndDay, 3),
                null,
                new DailyConfiguration(DailyConfigType.Recurring, TimeOnly.MinValue, 1, DailyFrecuency.Hours,
                new TimeLimits(new TimeOnly(3, 0, 0), new TimeOnly(6, 0, 0))),
                new DateLimits(new DateTime(2020, 1, 1)));
            var weekEndDaysResult1 = schedulerWeekendExample5.Execute(weekEndDaysConfiguration1);
            weekEndDaysResult1.NextExecutionTime.Should().Be(new DateTime(2020, 1, 4, 3, 0, 0));
            weekEndDaysResult1.Description.Should().Be("Occurs the first weekendday of very 3 months and every 1 hours between 3:00:00 AM and 6:00:00 AM starting on 01/01/2020");

            var schedulerWeekendExample6 = new Scheduler();
            var weekEndDaysConfiguration2 = new Configuration(new DateTime(2020, 1, 1), ConfigType.Recurring, true, null, Occurs.Monthly,
                new MonthlyConfiguration(MonthlyConfigType.WeekDayOption, 1, Ordinal.Second, KindOfDay.WeekEndDay, 3),
                null,
                new DailyConfiguration(DailyConfigType.Recurring, TimeOnly.MinValue, 1, DailyFrecuency.Hours,
                new TimeLimits(new TimeOnly(3, 0, 0), new TimeOnly(6, 0, 0))),
                new DateLimits(new DateTime(2020, 1, 1)));
            var weekEndDaysResult2 = schedulerWeekendExample6.Execute(weekEndDaysConfiguration2);
            weekEndDaysResult2.NextExecutionTime.Should().Be(new DateTime(2020, 1, 5, 3, 0, 0));
            weekEndDaysResult2.Description.Should().Be("Occurs the second weekendday of very 3 months and every 1 hours between 3:00:00 AM and 6:00:00 AM starting on 01/01/2020");

            var schedulerWeekendExample7 = new Scheduler();
            var weekEndDaysConfiguration3 = new Configuration(new DateTime(2020, 1, 1), ConfigType.Recurring, true, null, Occurs.Monthly,
                new MonthlyConfiguration(MonthlyConfigType.WeekDayOption, 1, Ordinal.Third, KindOfDay.WeekEndDay, 3),
                null,
                new DailyConfiguration(DailyConfigType.Recurring, TimeOnly.MinValue, 1, DailyFrecuency.Hours,
                new TimeLimits(new TimeOnly(3, 0, 0), new TimeOnly(6, 0, 0))),
                new DateLimits(new DateTime(2020, 1, 1)));
            var weekEndDaysResult3 = schedulerWeekendExample7.Execute(weekEndDaysConfiguration3);
            weekEndDaysResult3.NextExecutionTime.Should().Be(new DateTime(2020, 1, 11, 3, 0, 0));
            weekEndDaysResult3.Description.Should().Be("Occurs the third weekendday of very 3 months and every 1 hours between 3:00:00 AM and 6:00:00 AM starting on 01/01/2020");

            var schedulerWeekendExample8 = new Scheduler();
            var weekEndDaysConfiguration4 = new Configuration(new DateTime(2020, 1, 1), ConfigType.Recurring, true, null, Occurs.Monthly,
                new MonthlyConfiguration(MonthlyConfigType.WeekDayOption, 1, Ordinal.Fourth, KindOfDay.WeekEndDay, 3),
                null,
                new DailyConfiguration(DailyConfigType.Recurring, TimeOnly.MinValue, 1, DailyFrecuency.Hours,
                new TimeLimits(new TimeOnly(3, 0, 0), new TimeOnly(6, 0, 0))),
                new DateLimits(new DateTime(2020, 1, 1)));
            var weekEndDaysResult4 = schedulerWeekendExample8.Execute(weekEndDaysConfiguration4);
            weekEndDaysResult4.NextExecutionTime.Should().Be(new DateTime(2020, 1, 12, 3, 0, 0));
            weekEndDaysResult4.Description.Should().Be("Occurs the fourth weekendday of very 3 months and every 1 hours between 3:00:00 AM and 6:00:00 AM starting on 01/01/2020");
        }

        [Fact]
        public void Should_Be_Next_Execution_Date_Ordinal_WeekDay_Skipping_Months_Scheduler_Example()
        {
            var schedulerExample1 = new Scheduler();
            var schedulerExampleConfig1 = new Configuration(new DateTime(2020, 1, 1), ConfigType.Recurring, true, null, Occurs.Monthly,
                new MonthlyConfiguration(MonthlyConfigType.WeekDayOption, 1, Ordinal.First, KindOfDay.Thursday, 3), null,
                new DailyConfiguration(DailyConfigType.Recurring, TimeOnly.MinValue, 6, DailyFrecuency.Hours,
                new TimeLimits(new TimeOnly(3, 0, 0), new TimeOnly(6, 0, 0))), new DateLimits(new DateTime(2020, 1, 1)));
            var schedulerExampleResult1 = schedulerExample1.Execute(schedulerExampleConfig1);
            schedulerExampleResult1.NextExecutionTime.Should().Be(new DateTime(2020, 1, 2, 3, 0, 0));
            schedulerExampleResult1.Description.Should().Be("Occurs the first thursday of very 3 months and every 6 hours between 3:00:00 AM and 6:00:00 AM starting on 01/01/2020");

            var schedulerExampleConfig2 = new Configuration(schedulerExampleResult1.NextExecutionTime, ConfigType.Recurring, true, null, Occurs.Monthly,
                new MonthlyConfiguration(MonthlyConfigType.WeekDayOption, 1, Ordinal.First, KindOfDay.Thursday, 3), null,
                new DailyConfiguration(DailyConfigType.Recurring, TimeOnly.MinValue, 6, DailyFrecuency.Hours,
                new TimeLimits(new TimeOnly(3, 0, 0), new TimeOnly(6, 0, 0))), new DateLimits(new DateTime(2020, 1, 1)));
            var schedulerExampleResult2 = schedulerExample1.Execute(schedulerExampleConfig2);
            schedulerExampleResult2.NextExecutionTime.Should().Be(new DateTime(2020, 4, 2, 3, 0, 0));
            schedulerExampleResult2.Description.Should().Be("Occurs the first thursday of very 3 months and every 6 hours between 3:00:00 AM and 6:00:00 AM starting on 01/01/2020");

            var schedulerExampleConfig3 = new Configuration(schedulerExampleResult2.NextExecutionTime, ConfigType.Recurring, true, null, Occurs.Monthly,
                new MonthlyConfiguration(MonthlyConfigType.WeekDayOption, 1, Ordinal.First, KindOfDay.Thursday, 3), null,
                new DailyConfiguration(DailyConfigType.Recurring, TimeOnly.MinValue, 6, DailyFrecuency.Hours,
                new TimeLimits(new TimeOnly(3, 0, 0), new TimeOnly(6, 0, 0))), new DateLimits(new DateTime(2020, 1, 1)));
            var schedulerExampleResult3 = schedulerExample1.Execute(schedulerExampleConfig3);
            schedulerExampleResult3.NextExecutionTime.Should().Be(new DateTime(2020, 7, 2, 3, 0, 0));
            schedulerExampleResult3.Description.Should().Be("Occurs the first thursday of very 3 months and every 6 hours between 3:00:00 AM and 6:00:00 AM starting on 01/01/2020");
        }

        [Fact]
        public void Should_Be_Next_Execution_Date_Ordinal_WeekDay_Skipping_Months_Thursday()
        {
            //example1
            var thursdayExample1 = new Scheduler();
            var thursdayExampleConfig1 = new Configuration(new DateTime(2024, 1, 1), ConfigType.Recurring, true, null, Occurs.Monthly,
                new MonthlyConfiguration(MonthlyConfigType.WeekDayOption, 1, Ordinal.First, KindOfDay.Thursday, 3), null,
                new DailyConfiguration(DailyConfigType.Recurring, TimeOnly.MinValue, 6, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(3, 30, 0), new TimeOnly(6, 0, 0))), new DateLimits(new DateTime(2020, 1, 1)));
            var exampleResult1 = thursdayExample1.Execute(thursdayExampleConfig1);
            exampleResult1.NextExecutionTime.Should().Be(new DateTime(2024, 1, 4, 3, 30, 0));
            exampleResult1.Description.Should().Be("Occurs the first thursday of very 3 months and every 6 hours between 3:30:00 AM and 6:00:00 AM starting on 01/01/2020");

            var thursdayExampleConfig2 = new Configuration(exampleResult1.NextExecutionTime, ConfigType.Recurring, true, null, Occurs.Monthly,
                new MonthlyConfiguration(MonthlyConfigType.WeekDayOption, 1, Ordinal.First, KindOfDay.Thursday, 3), null,
                new DailyConfiguration(DailyConfigType.Recurring, TimeOnly.MinValue, 6, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(3, 30, 0), new TimeOnly(6, 0, 0))), new DateLimits(new DateTime(2020, 1, 1)));
            var exampleResult2 = thursdayExample1.Execute(thursdayExampleConfig2);
            exampleResult2.NextExecutionTime.Should().Be(new DateTime(2024, 4, 4, 3, 30, 0));
            exampleResult2.Description.Should().Be("Occurs the first thursday of very 3 months and every 6 hours between 3:30:00 AM and 6:00:00 AM starting on 01/01/2020");

            var thursdayExampleConfig3 = new Configuration(exampleResult2.NextExecutionTime, ConfigType.Recurring, true, null, Occurs.Monthly,
                new MonthlyConfiguration(MonthlyConfigType.WeekDayOption, 1, Ordinal.First, KindOfDay.Thursday, 3), null,
                new DailyConfiguration(DailyConfigType.Recurring, TimeOnly.MinValue, 6, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(3, 30, 0), new TimeOnly(6, 0, 0))), new DateLimits(new DateTime(2020, 1, 1)));
            var exampleResult3 = thursdayExample1.Execute(thursdayExampleConfig3);
            exampleResult3.NextExecutionTime.Should().Be(new DateTime(2024, 7, 4, 3, 30, 0));
            exampleResult3.Description.Should().Be("Occurs the first thursday of very 3 months and every 6 hours between 3:30:00 AM and 6:00:00 AM starting on 01/01/2020");

            //example2
            var thursdayExample2 = new Scheduler();
            var thursdayExample2Config1 = new Configuration(new DateTime(2024, 1, 1), ConfigType.Recurring, true, null, Occurs.Monthly,
                new MonthlyConfiguration(MonthlyConfigType.WeekDayOption, 1, Ordinal.Second, KindOfDay.Thursday, 2), null,
                new DailyConfiguration(DailyConfigType.Recurring, TimeOnly.MinValue, 6, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(3, 30, 0), new TimeOnly(6, 0, 0))), new DateLimits(new DateTime(2020, 1, 1)));
            var thursdayExample2Result1 = thursdayExample2.Execute(thursdayExample2Config1);
            thursdayExample2Result1.NextExecutionTime.Should().Be(new DateTime(2024, 1, 11, 3, 30, 0));
            thursdayExample2Result1.Description.Should().Be("Occurs the second thursday of very 2 months and every 6 hours between 3:30:00 AM and 6:00:00 AM starting on 01/01/2020");

            var thursdayExample2Config2 = new Configuration(thursdayExample2Result1.NextExecutionTime, ConfigType.Recurring, true, null, Occurs.Monthly,
                new MonthlyConfiguration(MonthlyConfigType.WeekDayOption, 1, Ordinal.Second, KindOfDay.Thursday, 2), null,
                new DailyConfiguration(DailyConfigType.Recurring, TimeOnly.MinValue, 6, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(3, 30, 0), new TimeOnly(6, 0, 0))), new DateLimits(new DateTime(2020, 1, 1)));
            var thursdayExample2Result2 = thursdayExample2.Execute(thursdayExample2Config2);
            thursdayExample2Result2.NextExecutionTime.Should().Be(new DateTime(2024, 3, 14, 3, 30, 0));
            thursdayExample2Result2.Description.Should().Be("Occurs the second thursday of very 2 months and every 6 hours between 3:30:00 AM and 6:00:00 AM starting on 01/01/2020");

            var thursdayExample2Config3 = new Configuration(thursdayExample2Result2.NextExecutionTime, ConfigType.Recurring, true, null, Occurs.Monthly,
                new MonthlyConfiguration(MonthlyConfigType.WeekDayOption, 1, Ordinal.Second, KindOfDay.Thursday, 2), null,
                new DailyConfiguration(DailyConfigType.Recurring, TimeOnly.MinValue, 6, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(3, 30, 0), new TimeOnly(6, 0, 0))), new DateLimits(new DateTime(2020, 1, 1)));
            var thursdayExample2Result3 = thursdayExample2.Execute(thursdayExample2Config3);
            thursdayExample2Result3.NextExecutionTime.Should().Be(new DateTime(2024, 5, 9, 3, 30, 0));
            thursdayExample2Result3.Description.Should().Be("Occurs the second thursday of very 2 months and every 6 hours between 3:30:00 AM and 6:00:00 AM starting on 01/01/2020");
        }

        [Fact]
        public void Should_Be_Next_Execution_Date_Ordinal_WeekDay_Skipping_Months_Weekdays()
        {
            var WeekDayExample1 = new Scheduler();
            var WeekDayExampleConfig1 = new Configuration(new DateTime(2024, 4, 5), ConfigType.Recurring, true, null, Occurs.Monthly,
                new MonthlyConfiguration(MonthlyConfigType.WeekDayOption, 1, Ordinal.First, KindOfDay.WeekDay, 2), null,
                new DailyConfiguration(DailyConfigType.Recurring, TimeOnly.MinValue, 6, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(5, 0, 0), new TimeOnly(6, 0, 0))),
                new DateLimits(new DateTime(2020, 1, 1)));
            var weekDayExampleResult1 = WeekDayExample1.Execute(WeekDayExampleConfig1);
            weekDayExampleResult1.NextExecutionTime.Should().Be(new DateTime(2024, 4, 5, 5, 0, 0));
            weekDayExampleResult1.Description.Should().Be("Occurs the first weekday of very 2 months and every 6 hours between 5:00:00 AM and 6:00:00 AM starting on 01/01/2020");

            var WeekDayExampleConfig2 = new Configuration(weekDayExampleResult1.NextExecutionTime, ConfigType.Recurring, true, null, Occurs.Monthly,
                new MonthlyConfiguration(MonthlyConfigType.WeekDayOption, 1, Ordinal.First, KindOfDay.WeekDay, 2), null,
                new DailyConfiguration(DailyConfigType.Recurring, TimeOnly.MinValue, 6, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(5, 0, 0), new TimeOnly(6, 0, 0))),
                new DateLimits(new DateTime(2020, 1, 1)));
            var weekDayExampleResult2 = WeekDayExample1.Execute(WeekDayExampleConfig2);
            weekDayExampleResult2.NextExecutionTime.Should().Be(new DateTime(2024, 6, 3, 5, 0, 0));
            weekDayExampleResult2.Description.Should().Be("Occurs the first weekday of very 2 months and every 6 hours between 5:00:00 AM and 6:00:00 AM starting on 01/01/2020");

            var WeekDayExampleConfig3 = new Configuration(weekDayExampleResult2.NextExecutionTime, ConfigType.Recurring, true, null, Occurs.Monthly,
                new MonthlyConfiguration(MonthlyConfigType.WeekDayOption, 1, Ordinal.First, KindOfDay.WeekDay, 2), null,
                new DailyConfiguration(DailyConfigType.Recurring, TimeOnly.MinValue, 6, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(5, 0, 0), new TimeOnly(6, 0, 0))),
                new DateLimits(new DateTime(2020, 1, 1)));
            var weekDayExampleResult3 = WeekDayExample1.Execute(WeekDayExampleConfig3);
            weekDayExampleResult3.NextExecutionTime.Should().Be(new DateTime(2024, 8, 1, 5, 0, 0));
            weekDayExampleResult3.Description.Should().Be("Occurs the first weekday of very 2 months and every 6 hours between 5:00:00 AM and 6:00:00 AM starting on 01/01/2020");

        }

        [Fact]
        public void Should_Be_Next_Execution_Date_Ordinal_WeekDay_Skipping_Months_WeekEndDays()
        {
            var weekEndDayExample1 = new Scheduler();
            var weekEndDayExampleConfig1 = new Configuration(new DateTime(2024, 4, 5), ConfigType.Recurring, true, null, Occurs.Monthly,
                new MonthlyConfiguration(MonthlyConfigType.WeekDayOption, 1, Ordinal.First, KindOfDay.WeekEndDay, 2), null,
                new DailyConfiguration(DailyConfigType.Recurring, TimeOnly.MinValue, 6, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(3, 0, 0), new TimeOnly(6, 0, 0))),
                new DateLimits(new DateTime(2020, 1, 1)));
            var weekEndDayExampleResult1 = weekEndDayExample1.Execute(weekEndDayExampleConfig1);
            weekEndDayExampleResult1.NextExecutionTime.Should().Be(new DateTime(2024, 4, 6, 3, 0, 0));
            weekEndDayExampleResult1.Description.Should().Be("Occurs the first weekendday of very 2 months and every 6 hours between 3:00:00 AM and 6:00:00 AM starting on 01/01/2020");

            var weekEndDayExampleConfig2 = new Configuration(weekEndDayExampleResult1.NextExecutionTime, ConfigType.Recurring, true, null, Occurs.Monthly,
                new MonthlyConfiguration(MonthlyConfigType.WeekDayOption, 1, Ordinal.First, KindOfDay.WeekEndDay, 2), null,
                new DailyConfiguration(DailyConfigType.Recurring, TimeOnly.MinValue, 6, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(3, 0, 0), new TimeOnly(6, 0, 0))),
                new DateLimits(new DateTime(2020, 1, 1)));
            var weekEndDayExampleResult2 = weekEndDayExample1.Execute(weekEndDayExampleConfig2);
            weekEndDayExampleResult2.NextExecutionTime.Should().Be(new DateTime(2024, 6, 1, 3, 0, 0));
            weekEndDayExampleResult2.Description.Should().Be("Occurs the first weekendday of very 2 months and every 6 hours between 3:00:00 AM and 6:00:00 AM starting on 01/01/2020");

            var weekEndDayExampleConfig3 = new Configuration(weekEndDayExampleResult2.NextExecutionTime, ConfigType.Recurring, true, null, Occurs.Monthly,
                new MonthlyConfiguration(MonthlyConfigType.WeekDayOption, 1, Ordinal.First, KindOfDay.WeekEndDay, 2), null,
                new DailyConfiguration(DailyConfigType.Recurring, TimeOnly.MinValue, 6, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(3, 0, 0), new TimeOnly(6, 0, 0))),
                new DateLimits(new DateTime(2020, 1, 1)));
            var weekEndDayExampleResult3 = weekEndDayExample1.Execute(weekEndDayExampleConfig3);
            weekEndDayExampleResult3.NextExecutionTime.Should().Be(new DateTime(2024, 8, 3, 3, 0, 0));
            weekEndDayExampleResult3.Description.Should().Be("Occurs the first weekendday of very 2 months and every 6 hours between 3:00:00 AM and 6:00:00 AM starting on 01/01/2020");


            var weekEndDayExample2 = new Scheduler();
            var weekEndDayExampleSecondConfig1 = new Configuration(new DateTime(2024, 4, 5), ConfigType.Recurring, true, null, Occurs.Monthly,
                new MonthlyConfiguration(MonthlyConfigType.WeekDayOption, 1, Ordinal.Second, KindOfDay.WeekEndDay, 2), null,
                new DailyConfiguration(DailyConfigType.Recurring, TimeOnly.MinValue, 6, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(3, 0, 0), new TimeOnly(6, 0, 0))),
                new DateLimits(new DateTime(2020, 1, 1)));
            var weekEndDayExampleSecondResult1 = weekEndDayExample2.Execute(weekEndDayExampleSecondConfig1);
            weekEndDayExampleSecondResult1.NextExecutionTime.Should().Be(new DateTime(2024, 4, 7, 3, 0, 0));
            weekEndDayExampleSecondResult1.Description.Should().Be("Occurs the second weekendday of very 2 months and every 6 hours between 3:00:00 AM and 6:00:00 AM starting on 01/01/2020");

            var weekEndDayExampleSecondConfig2 = new Configuration(weekEndDayExampleSecondResult1.NextExecutionTime, ConfigType.Recurring, true, null, Occurs.Monthly,
                new MonthlyConfiguration(MonthlyConfigType.WeekDayOption, 1, Ordinal.Second, KindOfDay.WeekEndDay, 2), null,
                new DailyConfiguration(DailyConfigType.Recurring, TimeOnly.MinValue, 6, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(3, 0, 0), new TimeOnly(6, 0, 0))),
                new DateLimits(new DateTime(2020, 1, 1)));
            var weekEndDayExampleSecondResult2 = weekEndDayExample2.Execute(weekEndDayExampleSecondConfig2);
            weekEndDayExampleSecondResult2.NextExecutionTime.Should().Be(new DateTime(2024, 6, 2, 3, 0, 0));
            weekEndDayExampleSecondResult2.Description.Should().Be("Occurs the second weekendday of very 2 months and every 6 hours between 3:00:00 AM and 6:00:00 AM starting on 01/01/2020");

            var weekEndDayExampleSecondConfig3 = new Configuration(weekEndDayExampleSecondResult2.NextExecutionTime, ConfigType.Recurring, true, null, Occurs.Monthly,
                new MonthlyConfiguration(MonthlyConfigType.WeekDayOption, 1, Ordinal.Second, KindOfDay.WeekEndDay, 2), null,
                new DailyConfiguration(DailyConfigType.Recurring, TimeOnly.MinValue, 6, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(3, 0, 0), new TimeOnly(6, 0, 0))),
                new DateLimits(new DateTime(2020, 1, 1)));
            var weekEndDayExampleSecondResult3 = weekEndDayExample2.Execute(weekEndDayExampleSecondConfig3);
            weekEndDayExampleSecondResult3.NextExecutionTime.Should().Be(new DateTime(2024, 8, 4, 3, 0, 0));
            weekEndDayExampleSecondResult3.Description.Should().Be("Occurs the second weekendday of very 2 months and every 6 hours between 3:00:00 AM and 6:00:00 AM starting on 01/01/2020");

            var schedulerExample = new Scheduler();
            var schedulerExampleConfig1 = new Configuration(new DateTime(2024, 4, 1), ConfigType.Recurring, true, null, Occurs.Monthly,
                new MonthlyConfiguration(MonthlyConfigType.WeekDayOption, 1, Ordinal.First, KindOfDay.WeekEndDay, 3), null,
                new DailyConfiguration(DailyConfigType.Recurring, TimeOnly.MinValue, 6, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(3, 0, 0), new TimeOnly(6, 0, 0))),
                new DateLimits(new DateTime(2020, 1, 1)));
            var exampleResult1 = schedulerExample.Execute(schedulerExampleConfig1);
            exampleResult1.NextExecutionTime.Should().Be(new DateTime(2024, 4, 6, 3, 0, 0));
            exampleResult1.Description.Should().Be("Occurs the first weekendday of very 3 months and every 6 hours between 3:00:00 AM and 6:00:00 AM starting on 01/01/2020");

            var schedulerExampleConfig2 = new Configuration(exampleResult1.NextExecutionTime, ConfigType.Recurring, true, null, Occurs.Monthly,
                new MonthlyConfiguration(MonthlyConfigType.WeekDayOption, 1, Ordinal.First, KindOfDay.WeekEndDay, 3), null,
                new DailyConfiguration(DailyConfigType.Recurring, TimeOnly.MinValue, 6, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(3, 0, 0), new TimeOnly(6, 0, 0))),
                new DateLimits(new DateTime(2020, 1, 1)));
            var exampleResult2 = schedulerExample.Execute(schedulerExampleConfig2);
            exampleResult2.NextExecutionTime.Should().Be(new DateTime(2024, 7, 6, 3, 0, 0));
            exampleResult2.Description.Should().Be("Occurs the first weekendday of very 3 months and every 6 hours between 3:00:00 AM and 6:00:00 AM starting on 01/01/2020");

            var schedulerExampleConfig3 = new Configuration(exampleResult2.NextExecutionTime, ConfigType.Recurring, true, null, Occurs.Monthly,
                new MonthlyConfiguration(MonthlyConfigType.WeekDayOption, 1, Ordinal.First, KindOfDay.WeekEndDay, 3), null,
                new DailyConfiguration(DailyConfigType.Recurring, TimeOnly.MinValue, 6, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(3, 0, 0), new TimeOnly(6, 0, 0))),
                new DateLimits(new DateTime(2020, 1, 1)));
            var exampleResult3 = schedulerExample.Execute(schedulerExampleConfig3);
            exampleResult3.NextExecutionTime.Should().Be(new DateTime(2024, 10, 5, 3, 0, 0));
            exampleResult3.Description.Should().Be("Occurs the first weekendday of very 3 months and every 6 hours between 3:00:00 AM and 6:00:00 AM starting on 01/01/2020");

            var schedulerExampleConfig4 = new Configuration(exampleResult3.NextExecutionTime, ConfigType.Recurring, true, null, Occurs.Monthly,
               new MonthlyConfiguration(MonthlyConfigType.WeekDayOption, 1, Ordinal.First, KindOfDay.WeekEndDay, 3), null,
               new DailyConfiguration(DailyConfigType.Recurring, TimeOnly.MinValue, 6, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(3, 0, 0), new TimeOnly(6, 0, 0))),
               new DateLimits(new DateTime(2020, 1, 1)));
            var exampleResult4 = schedulerExample.Execute(schedulerExampleConfig4);
            exampleResult4.NextExecutionTime.Should().Be(new DateTime(2025, 1, 4, 3, 0, 0));
            exampleResult4.Description.Should().Be("Occurs the first weekendday of very 3 months and every 6 hours between 3:00:00 AM and 6:00:00 AM starting on 01/01/2020");

            var schedulerExampleConfig5 = new Configuration(exampleResult4.NextExecutionTime, ConfigType.Recurring, true, null, Occurs.Monthly,
               new MonthlyConfiguration(MonthlyConfigType.WeekDayOption, 1, Ordinal.Last, KindOfDay.WeekEndDay, 3), null,
               new DailyConfiguration(DailyConfigType.Recurring, TimeOnly.MinValue, 6, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(3, 0, 0), new TimeOnly(6, 0, 0))),
               new DateLimits(new DateTime(2020, 1, 1)));
            var exampleResult5 = schedulerExample.Execute(schedulerExampleConfig5);
            exampleResult5.NextExecutionTime.Should().Be(new DateTime(2025, 4, 27, 3, 0, 0));
            exampleResult5.Description.Should().Be("Occurs the last weekendday of very 3 months and every 6 hours between 3:00:00 AM and 6:00:00 AM starting on 01/01/2020");
        }

        [Fact]
        public void Should_Be_Next_Execution_Date_Ordinal_WeekDay_Skipping_Months_Days()
        {
            var dayExample2 = new Scheduler();
            var dayExampleSecondConfig1 = new Configuration(new DateTime(2024, 4, 5), ConfigType.Recurring, true, null, Occurs.Monthly,
                new MonthlyConfiguration(MonthlyConfigType.WeekDayOption, 1, Ordinal.Second, KindOfDay.Day, 2), null,
                new DailyConfiguration(DailyConfigType.Recurring, TimeOnly.MinValue, 6, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(3, 0, 0), new TimeOnly(6, 0, 0))),
                new DateLimits(new DateTime(2020, 1, 1)));
            var dayExampleSecondResult1 = dayExample2.Execute(dayExampleSecondConfig1);
            dayExampleSecondResult1.NextExecutionTime.Should().Be(new DateTime(2024, 4, 6, 3, 0, 0));
            dayExampleSecondResult1.Description.Should().Be("Occurs the second day of very 2 months and every 6 hours between 3:00:00 AM and 6:00:00 AM starting on 01/01/2020");

            var dayExampleSecondConfig2 = new Configuration(dayExampleSecondResult1.NextExecutionTime, ConfigType.Recurring, true, null, Occurs.Monthly,
                new MonthlyConfiguration(MonthlyConfigType.WeekDayOption, 1, Ordinal.Second, KindOfDay.Day, 2), null,
                new DailyConfiguration(DailyConfigType.Recurring, TimeOnly.MinValue, 6, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(3, 0, 0), new TimeOnly(6, 0, 0))),
                new DateLimits(new DateTime(2020, 1, 1)));
            var dayExampleSecondResult2 = dayExample2.Execute(dayExampleSecondConfig2);
            dayExampleSecondResult2.NextExecutionTime.Should().Be(new DateTime(2024, 6, 2, 3, 0, 0));
            dayExampleSecondResult2.Description.Should().Be("Occurs the second day of very 2 months and every 6 hours between 3:00:00 AM and 6:00:00 AM starting on 01/01/2020");

            var dayExampleSecondConfig3 = new Configuration(dayExampleSecondResult2.NextExecutionTime, ConfigType.Recurring, true, null, Occurs.Monthly,
                new MonthlyConfiguration(MonthlyConfigType.WeekDayOption, 1, Ordinal.Second, KindOfDay.Day, 2), null,
                new DailyConfiguration(DailyConfigType.Recurring, TimeOnly.MinValue, 6, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(3, 0, 0), new TimeOnly(6, 0, 0))),
                new DateLimits(new DateTime(2020, 1, 1)));
            var dayExampleSecondResult3 = dayExample2.Execute(dayExampleSecondConfig3);
            dayExampleSecondResult3.NextExecutionTime.Should().Be(new DateTime(2024, 8, 2, 3, 0, 0));
            dayExampleSecondResult3.Description.Should().Be("Occurs the second day of very 2 months and every 6 hours between 3:00:00 AM and 6:00:00 AM starting on 01/01/2020");
        }

        [Fact]
        public void Should_Be_Next_Execution_Date_DayNumber_Skipping_Months_With_DailyConfiguration_Once_FebruaryCase()
        {
            var schedulerExample2 = new Scheduler();
            var schedulerExampleSecondConfig1 = new Configuration(new DateTime(2024, 1, 1, 0, 0, 0), ConfigType.Recurring, true, null, Occurs.Monthly,
                new MonthlyConfiguration(MonthlyConfigType.DayNumberOption, 30, Ordinal.First, KindOfDay.Thursday, 1), null,
                new DailyConfiguration(DailyConfigType.Once, new TimeOnly(8, 0, 0), null, null, null), new DateLimits(new DateTime(2024, 1, 1)));
            var schedulerExampleSecondResult1 = schedulerExample2.Execute(schedulerExampleSecondConfig1);
            schedulerExampleSecondResult1.NextExecutionTime.Should().Be(new DateTime(2024, 1, 30, 8, 0, 0));
            schedulerExampleSecondResult1.Description.Should().Be("Occurs the 30th of very 1 month one time at 8:00:00 AM starting on 01/01/2024");

            var schedulerExampleSecondConfig2 = new Configuration(schedulerExampleSecondResult1.NextExecutionTime, ConfigType.Recurring, true, null, Occurs.Monthly,
                new MonthlyConfiguration(MonthlyConfigType.DayNumberOption, 30, Ordinal.First, KindOfDay.Thursday, 1), null,
                new DailyConfiguration(DailyConfigType.Once, new TimeOnly(8, 0, 0), null, null, null), new DateLimits(new DateTime(2024, 1, 1)));
            var schedulerExampleSecondResult2 = schedulerExample2.Execute(schedulerExampleSecondConfig2);
            schedulerExampleSecondResult2.NextExecutionTime.Should().Be(new DateTime(2024, 3, 30, 8, 0, 0));
            schedulerExampleSecondResult2.Description.Should().Be("Occurs the 30th of very 1 month one time at 8:00:00 AM starting on 01/01/2024");
        }

        [Fact]
        public void Should_Be_Next_Execution_Date_DayNumber_Skipping_Months_With_DailyConfiguration_Once_Skip_First_Month()
        {
            var scheduler = new Scheduler();
            var schedulerSecondConfig1 = new Configuration(new DateTime(2024, 4, 1, 0, 0, 0), ConfigType.Recurring, true, null, Occurs.Monthly,
                new MonthlyConfiguration(MonthlyConfigType.DayNumberOption, 31, Ordinal.First, KindOfDay.Thursday, 3), null,
                new DailyConfiguration(DailyConfigType.Once, new TimeOnly(12, 0, 0), null, null, null), new DateLimits(new DateTime(2024, 1, 1)));
            var schedulerSecondResult1 = scheduler.Execute(schedulerSecondConfig1);
            schedulerSecondResult1.NextExecutionTime.Should().Be(new DateTime(2024, 5, 31, 12, 0, 0));
            schedulerSecondResult1.Description.Should().Be("Occurs the 31st of very 3 months one time at 12:00:00 PM starting on 01/01/2024");

            var schedulerSecondConfig2 = new Configuration(schedulerSecondResult1.NextExecutionTime, ConfigType.Recurring, true, null, Occurs.Monthly,
                new MonthlyConfiguration(MonthlyConfigType.DayNumberOption, 31, Ordinal.First, KindOfDay.Thursday, 3), null,
                new DailyConfiguration(DailyConfigType.Once, new TimeOnly(12, 0, 0), null, null, null), new DateLimits(new DateTime(2024, 1, 1)));
            var schedulerSecondResult2 = scheduler.Execute(schedulerSecondConfig2);
            schedulerSecondResult2.NextExecutionTime.Should().Be(new DateTime(2024, 8, 31, 12, 0, 0));
            schedulerSecondResult2.Description.Should().Be("Occurs the 31st of very 3 months one time at 12:00:00 PM starting on 01/01/2024");
        }

        //queda probar un caso con hora menor al onceAt en ese dia
        [Fact]
        public void Should_Be_Next_Execution_Date_DayNumber_Skipping_Months_With_DailyConfiguration_Once_RegularCases()
        {
            var scheduler = new Scheduler();
            var schedulerSecondConfig1 = new Configuration(new DateTime(2024, 1, 1, 0, 0, 0), ConfigType.Recurring, true, null, Occurs.Monthly,
                new MonthlyConfiguration(MonthlyConfigType.DayNumberOption, 20, Ordinal.First, KindOfDay.Thursday, 2), null,
                new DailyConfiguration(DailyConfigType.Once, new TimeOnly(12, 0, 0), null, null, null), new DateLimits(new DateTime(2024, 1, 1)));
            var schedulerSecondResult1 = scheduler.Execute(schedulerSecondConfig1);
            schedulerSecondResult1.NextExecutionTime.Should().Be(new DateTime(2024, 1, 20, 12, 0, 0));
            schedulerSecondResult1.Description.Should().Be("Occurs the 20th of very 2 months one time at 12:00:00 PM starting on 01/01/2024");

            var schedulerSecondConfig2 = new Configuration(schedulerSecondResult1.NextExecutionTime, ConfigType.Recurring, true, null, Occurs.Monthly,
                new MonthlyConfiguration(MonthlyConfigType.DayNumberOption, 20, Ordinal.First, KindOfDay.Thursday, 2), null,
                new DailyConfiguration(DailyConfigType.Once, new TimeOnly(12, 0, 0), null, null, null), new DateLimits(new DateTime(2024, 1, 1)));
            var schedulerSecondResult2 = scheduler.Execute(schedulerSecondConfig2);
            schedulerSecondResult2.NextExecutionTime.Should().Be(new DateTime(2024, 3, 20, 12, 0, 0));
            schedulerSecondResult2.Description.Should().Be("Occurs the 20th of very 2 months one time at 12:00:00 PM starting on 01/01/2024");

            var schedulerSecondConfig3 = new Configuration(schedulerSecondResult2.NextExecutionTime, ConfigType.Recurring, true, null, Occurs.Monthly,
                new MonthlyConfiguration(MonthlyConfigType.DayNumberOption, 20, Ordinal.First, KindOfDay.Thursday, 2), null,
                new DailyConfiguration(DailyConfigType.Once, new TimeOnly(12, 0, 0), null, null, null), new DateLimits(new DateTime(2024, 1, 1)));
            var schedulerSecondResult3 = scheduler.Execute(schedulerSecondConfig3);
            schedulerSecondResult3.NextExecutionTime.Should().Be(new DateTime(2024, 5, 20, 12, 0, 0));
            schedulerSecondResult3.Description.Should().Be("Occurs the 20th of very 2 months one time at 12:00:00 PM starting on 01/01/2024");

            var schedulerSecondConfig4 = new Configuration(schedulerSecondResult3.NextExecutionTime, ConfigType.Recurring, true, null, Occurs.Monthly,
                new MonthlyConfiguration(MonthlyConfigType.DayNumberOption, 20, Ordinal.First, KindOfDay.Thursday, 2), null,
                new DailyConfiguration(DailyConfigType.Once, new TimeOnly(12, 0, 0), null, null, null), new DateLimits(new DateTime(2024, 1, 1)));
            var schedulerSecondResult4 = scheduler.Execute(schedulerSecondConfig4);
            schedulerSecondResult4.NextExecutionTime.Should().Be(new DateTime(2024, 7, 20, 12, 0, 0));
            schedulerSecondResult4.Description.Should().Be("Occurs the 20th of very 2 months one time at 12:00:00 PM starting on 01/01/2024");

            var schedulerSecondConfig5 = new Configuration(schedulerSecondResult4.NextExecutionTime, ConfigType.Recurring, true, null, Occurs.Monthly,
                new MonthlyConfiguration(MonthlyConfigType.DayNumberOption, 20, Ordinal.First, KindOfDay.Thursday, 2), null,
                new DailyConfiguration(DailyConfigType.Once, new TimeOnly(12, 0, 0), null, null, null), new DateLimits(new DateTime(2024, 1, 1)));
            var schedulerSecondResult5 = scheduler.Execute(schedulerSecondConfig5);
            schedulerSecondResult5.NextExecutionTime.Should().Be(new DateTime(2024, 9, 20, 12, 0, 0));
            schedulerSecondResult5.Description.Should().Be("Occurs the 20th of very 2 months one time at 12:00:00 PM starting on 01/01/2024");
        }
        [Fact]
        public void Should_Be_Next_Execution_DayNumberOption_Number_Ordinals_Cases()
        {
            var scheduler = new Scheduler();
            var schedulerConfig1 = new Configuration(new DateTime(2024, 4, 1, 0, 0, 0), ConfigType.Recurring, true, null, Occurs.Monthly,
                new MonthlyConfiguration(MonthlyConfigType.DayNumberOption, 1, Ordinal.First, KindOfDay.Thursday, 3), null,
                new DailyConfiguration(DailyConfigType.Once, new TimeOnly(12, 0, 0), null, null, null), new DateLimits(new DateTime(2024, 1, 1)));
            var schedulerResult1 = scheduler.Execute(schedulerConfig1);
            schedulerResult1.NextExecutionTime.Should().Be(new DateTime(2024, 4, 1, 12, 0, 0));
            schedulerResult1.Description.Should().Be("Occurs the 1st of very 3 months one time at 12:00:00 PM starting on 01/01/2024");

            var scheduler2 = new Scheduler();
            var schedulerConfig2 = new Configuration(new DateTime(2024, 4, 1, 0, 0, 0), ConfigType.Recurring, true, null, Occurs.Monthly,
                new MonthlyConfiguration(MonthlyConfigType.DayNumberOption, 21, Ordinal.First, KindOfDay.Thursday, 3), null,
                new DailyConfiguration(DailyConfigType.Once, new TimeOnly(12, 0, 0), null, null, null), new DateLimits(new DateTime(2024, 1, 1)));
            var schedulerResult2 = scheduler2.Execute(schedulerConfig2);
            schedulerResult2.NextExecutionTime.Should().Be(new DateTime(2024, 4, 21, 12, 0, 0));
            schedulerResult2.Description.Should().Be("Occurs the 21st of very 3 months one time at 12:00:00 PM starting on 01/01/2024");

            var scheduler3 = new Scheduler();
            var schedulerConfig3 = new Configuration(new DateTime(2024, 4, 1, 0, 0, 0), ConfigType.Recurring, true, null, Occurs.Monthly,
                new MonthlyConfiguration(MonthlyConfigType.DayNumberOption, 31, Ordinal.First, KindOfDay.Thursday, 3), null,
                new DailyConfiguration(DailyConfigType.Once, new TimeOnly(12, 0, 0), null, null, null), new DateLimits(new DateTime(2024, 1, 1)));
            var schedulerResult3 = scheduler3.Execute(schedulerConfig3);
            schedulerResult3.NextExecutionTime.Should().Be(new DateTime(2024, 5, 31, 12, 0, 0));
            schedulerResult3.Description.Should().Be("Occurs the 31st of very 3 months one time at 12:00:00 PM starting on 01/01/2024");
        }

        [Fact]
        public void Should_Be_Next_Execution_Date_DayNumber_Skipping_Months_With_DailyConfiguration_Once()
        {
            var schedulerExample2 = new Scheduler();
            var schedulerExampleSecondConfig1 = new Configuration(new DateTime(2020, 1, 1, 0, 0, 0), ConfigType.Recurring, true, null, Occurs.Monthly,
                new MonthlyConfiguration(MonthlyConfigType.DayNumberOption, 10, Ordinal.First, KindOfDay.Thursday, 3), null,
                new DailyConfiguration(DailyConfigType.Once, new TimeOnly(8, 0, 0), null, null, null), new DateLimits(new DateTime(2020, 1, 1)));
            var schedulerExampleSecondResult1 = schedulerExample2.Execute(schedulerExampleSecondConfig1);
            schedulerExampleSecondResult1.NextExecutionTime.Should().Be(new DateTime(2020, 1, 10, 8, 0, 0));
            schedulerExampleSecondResult1.Description.Should().Be("Occurs the 10th of very 3 months one time at 8:00:00 AM starting on 01/01/2020");

            var schedulerExampleSecondConfig2 = new Configuration(schedulerExampleSecondResult1.NextExecutionTime, ConfigType.Recurring, true, null, Occurs.Monthly,
            new MonthlyConfiguration(MonthlyConfigType.DayNumberOption, 10, Ordinal.First, KindOfDay.Thursday, 3), null,
            new DailyConfiguration(DailyConfigType.Once, new TimeOnly(8, 0, 0), null, null, null), new DateLimits(new DateTime(2020, 1, 1)));
            var schedulerExampleSecondResult2 = schedulerExample2.Execute(schedulerExampleSecondConfig2);
            schedulerExampleSecondResult2.NextExecutionTime.Should().Be(new DateTime(2020, 4, 10, 8, 0, 0));
            schedulerExampleSecondResult2.Description.Should().Be("Occurs the 10th of very 3 months one time at 8:00:00 AM starting on 01/01/2020");

            var schedulerExampleSecondConfig3 = new Configuration(schedulerExampleSecondResult2.NextExecutionTime, ConfigType.Recurring, true, null, Occurs.Monthly,
            new MonthlyConfiguration(MonthlyConfigType.DayNumberOption, 10, Ordinal.First, KindOfDay.Thursday, 3), null,
            new DailyConfiguration(DailyConfigType.Once, new TimeOnly(8, 0, 0), null, null, null), new DateLimits(new DateTime(2020, 1, 1)));
            var schedulerExampleSecondResult3 = schedulerExample2.Execute(schedulerExampleSecondConfig3);
            schedulerExampleSecondResult3.NextExecutionTime.Should().Be(new DateTime(2020, 7, 10, 8, 0, 0));
            schedulerExampleSecondResult3.Description.Should().Be("Occurs the 10th of very 3 months one time at 8:00:00 AM starting on 01/01/2020");

            var schedulerExampleSecondConfig4 = new Configuration(schedulerExampleSecondResult3.NextExecutionTime, ConfigType.Recurring, true, null, Occurs.Monthly,
            new MonthlyConfiguration(MonthlyConfigType.DayNumberOption, 10, Ordinal.First, KindOfDay.Thursday, 3), null,
            new DailyConfiguration(DailyConfigType.Once, new TimeOnly(8, 0, 0), null, null, null), new DateLimits(new DateTime(2020, 1, 1)));
            var schedulerExampleSecondResult4 = schedulerExample2.Execute(schedulerExampleSecondConfig4);
            schedulerExampleSecondResult4.NextExecutionTime.Should().Be(new DateTime(2020, 10, 10, 8, 0, 0));
            schedulerExampleSecondResult4.Description.Should().Be("Occurs the 10th of very 3 months one time at 8:00:00 AM starting on 01/01/2020");

            var schedulerExampleSecondConfig5 = new Configuration(schedulerExampleSecondResult4.NextExecutionTime, ConfigType.Recurring, true, null, Occurs.Monthly,
            new MonthlyConfiguration(MonthlyConfigType.DayNumberOption, 10, Ordinal.First, KindOfDay.Thursday, 3), null,
            new DailyConfiguration(DailyConfigType.Once, new TimeOnly(8, 0, 0), null, null, null), new DateLimits(new DateTime(2020, 1, 1)));
            var schedulerExampleSecondResult5 = schedulerExample2.Execute(schedulerExampleSecondConfig5);
            schedulerExampleSecondResult5.NextExecutionTime.Should().Be(new DateTime(2021, 1, 10, 8, 0, 0));
            schedulerExampleSecondResult5.Description.Should().Be("Occurs the 10th of very 3 months one time at 8:00:00 AM starting on 01/01/2020");

            var schedulerExampleSecondConfig6 = new Configuration(schedulerExampleSecondResult5.NextExecutionTime, ConfigType.Recurring, true, null, Occurs.Monthly,
            new MonthlyConfiguration(MonthlyConfigType.DayNumberOption, 10, Ordinal.First, KindOfDay.Thursday, 3), null,
            new DailyConfiguration(DailyConfigType.Once, new TimeOnly(8, 0, 0), null, null, null), new DateLimits(new DateTime(2020, 1, 1)));
            var schedulerExampleSecondResult6 = schedulerExample2.Execute(schedulerExampleSecondConfig6);
            schedulerExampleSecondResult6.NextExecutionTime.Should().Be(new DateTime(2021, 4, 10, 8, 0, 0));
            schedulerExampleSecondResult6.Description.Should().Be("Occurs the 10th of very 3 months one time at 8:00:00 AM starting on 01/01/2020");

            var schedulerExampleSecondConfig7 = new Configuration(schedulerExampleSecondResult6.NextExecutionTime, ConfigType.Recurring, true, null, Occurs.Monthly,
            new MonthlyConfiguration(MonthlyConfigType.DayNumberOption, 10, Ordinal.First, KindOfDay.Thursday, 3), null,
            new DailyConfiguration(DailyConfigType.Once, new TimeOnly(8, 0, 0), null, null, null), new DateLimits(new DateTime(2020, 1, 1)));
            var schedulerExampleSecondResult7 = schedulerExample2.Execute(schedulerExampleSecondConfig7);
            schedulerExampleSecondResult7.NextExecutionTime.Should().Be(new DateTime(2021, 7, 10, 8, 0, 0));
            schedulerExampleSecondResult7.Description.Should().Be("Occurs the 10th of very 3 months one time at 8:00:00 AM starting on 01/01/2020");

            var schedulerExampleSecondConfig8 = new Configuration(schedulerExampleSecondResult7.NextExecutionTime, ConfigType.Recurring, true, null, Occurs.Monthly,
            new MonthlyConfiguration(MonthlyConfigType.DayNumberOption, 10, Ordinal.First, KindOfDay.Thursday, 3), null,
            new DailyConfiguration(DailyConfigType.Once, new TimeOnly(8, 0, 0), null, null, null), new DateLimits(new DateTime(2020, 1, 1)));
            var schedulerExampleSecondResult8 = schedulerExample2.Execute(schedulerExampleSecondConfig8);
            schedulerExampleSecondResult8.NextExecutionTime.Should().Be(new DateTime(2021, 10, 10, 8, 0, 0));
            schedulerExampleSecondResult8.Description.Should().Be("Occurs the 10th of very 3 months one time at 8:00:00 AM starting on 01/01/2020");

            var schedulerExampleSecondConfig9 = new Configuration(schedulerExampleSecondResult8.NextExecutionTime, ConfigType.Recurring, true, null, Occurs.Monthly,
            new MonthlyConfiguration(MonthlyConfigType.DayNumberOption, 10, Ordinal.First, KindOfDay.Thursday, 3), null,
            new DailyConfiguration(DailyConfigType.Once, new TimeOnly(8, 0, 0), null, null, null), new DateLimits(new DateTime(2020, 1, 1)));
            var schedulerExampleSecondResult9 = schedulerExample2.Execute(schedulerExampleSecondConfig9);
            schedulerExampleSecondResult9.NextExecutionTime.Should().Be(new DateTime(2022, 1, 10, 8, 0, 0));
            schedulerExampleSecondResult9.Description.Should().Be("Occurs the 10th of very 3 months one time at 8:00:00 AM starting on 01/01/2020");
        }

        [Fact]
        public void Should_Be_Next_Execution_Date_DayNumber_Skipping_Months_With_DailyConfiguration_Recurring_Scheduler_Example()
        {
            var schedulerExample2 = new Scheduler();
            var schedulerExampleSecondConfig1 = new Configuration(new DateTime(2020, 1, 1, 0, 0, 0), ConfigType.Recurring, true, null, Occurs.Monthly,
                new MonthlyConfiguration(MonthlyConfigType.DayNumberOption, 10, Ordinal.First, KindOfDay.Thursday, 3), null,
                new DailyConfiguration(DailyConfigType.Recurring, TimeOnly.MinValue, 1, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(3, 0, 0), new TimeOnly(6, 0, 0))), new DateLimits(new DateTime(2020, 1, 1)));
            var schedulerExampleSecondResult1 = schedulerExample2.Execute(schedulerExampleSecondConfig1);
            schedulerExampleSecondResult1.NextExecutionTime.Should().Be(new DateTime(2020, 1, 10, 3, 0, 0));
            schedulerExampleSecondResult1.Description.Should().Be("Occurs the 10th of very 3 months and every 1 hours between 3:00:00 AM and 6:00:00 AM starting on 01/01/2020");

            var schedulerExampleSecondConfig2 = new Configuration(schedulerExampleSecondResult1.NextExecutionTime, ConfigType.Recurring, true, null, Occurs.Monthly,
            new MonthlyConfiguration(MonthlyConfigType.DayNumberOption, 10, Ordinal.First, KindOfDay.Thursday, 3), null,
            new DailyConfiguration(DailyConfigType.Recurring, TimeOnly.MinValue, 1, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(3, 0, 0), new TimeOnly(6, 0, 0))), new DateLimits(new DateTime(2020, 1, 1)));
            var schedulerExampleSecondResult2 = schedulerExample2.Execute(schedulerExampleSecondConfig2);
            schedulerExampleSecondResult2.NextExecutionTime.Should().Be(new DateTime(2020, 1, 10, 4, 0, 0));
            schedulerExampleSecondResult2.Description.Should().Be("Occurs the 10th of very 3 months and every 1 hours between 3:00:00 AM and 6:00:00 AM starting on 01/01/2020");

            var schedulerExampleSecondConfig3 = new Configuration(schedulerExampleSecondResult2.NextExecutionTime, ConfigType.Recurring, true, null, Occurs.Monthly,
            new MonthlyConfiguration(MonthlyConfigType.DayNumberOption, 10, Ordinal.First, KindOfDay.Thursday, 3), null,
            new DailyConfiguration(DailyConfigType.Recurring, TimeOnly.MinValue, 1, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(3, 0, 0), new TimeOnly(6, 0, 0))), new DateLimits(new DateTime(2020, 1, 1)));
            var schedulerExampleSecondResult3 = schedulerExample2.Execute(schedulerExampleSecondConfig3);
            schedulerExampleSecondResult3.NextExecutionTime.Should().Be(new DateTime(2020, 1, 10, 5, 0, 0));
            schedulerExampleSecondResult3.Description.Should().Be("Occurs the 10th of very 3 months and every 1 hours between 3:00:00 AM and 6:00:00 AM starting on 01/01/2020");

            var schedulerExampleSecondConfig4 = new Configuration(schedulerExampleSecondResult3.NextExecutionTime, ConfigType.Recurring, true, null, Occurs.Monthly,
            new MonthlyConfiguration(MonthlyConfigType.DayNumberOption, 10, Ordinal.First, KindOfDay.Thursday, 3), null,
            new DailyConfiguration(DailyConfigType.Recurring, TimeOnly.MinValue, 1, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(3, 0, 0), new TimeOnly(6, 0, 0))), new DateLimits(new DateTime(2020, 1, 1)));
            var schedulerExampleSecondResult4 = schedulerExample2.Execute(schedulerExampleSecondConfig4);
            schedulerExampleSecondResult4.NextExecutionTime.Should().Be(new DateTime(2020, 1, 10, 6, 0, 0));
            schedulerExampleSecondResult4.Description.Should().Be("Occurs the 10th of very 3 months and every 1 hours between 3:00:00 AM and 6:00:00 AM starting on 01/01/2020");

            var schedulerExampleSecondConfig5 = new Configuration(schedulerExampleSecondResult4.NextExecutionTime, ConfigType.Recurring, true, null, Occurs.Monthly,
            new MonthlyConfiguration(MonthlyConfigType.DayNumberOption, 10, Ordinal.First, KindOfDay.Thursday, 3), null,
            new DailyConfiguration(DailyConfigType.Recurring, TimeOnly.MinValue, 1, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(3, 0, 0), new TimeOnly(6, 0, 0))), new DateLimits(new DateTime(2020, 1, 1)));
            var schedulerExampleSecondResult5 = schedulerExample2.Execute(schedulerExampleSecondConfig5);
            schedulerExampleSecondResult5.NextExecutionTime.Should().Be(new DateTime(2020, 5, 10, 3, 0, 0));
            schedulerExampleSecondResult5.Description.Should().Be("Occurs the 10th of very 3 months and every 1 hours between 3:00:00 AM and 6:00:00 AM starting on 01/01/2020");

            var schedulerExampleSecondConfig6 = new Configuration(schedulerExampleSecondResult5.NextExecutionTime, ConfigType.Recurring, true, null, Occurs.Monthly,
            new MonthlyConfiguration(MonthlyConfigType.DayNumberOption, 10, Ordinal.First, KindOfDay.Thursday, 3), null,
            new DailyConfiguration(DailyConfigType.Recurring, TimeOnly.MinValue, 1, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(3, 0, 0), new TimeOnly(6, 0, 0))), new DateLimits(new DateTime(2020, 1, 1)));
            var schedulerExampleSecondResult6 = schedulerExample2.Execute(schedulerExampleSecondConfig6);
            schedulerExampleSecondResult6.NextExecutionTime.Should().Be(new DateTime(2020, 5, 10, 4, 0, 0));
            schedulerExampleSecondResult6.Description.Should().Be("Occurs the 10th of very 3 months and every 1 hours between 3:00:00 AM and 6:00:00 AM starting on 01/01/2020");

            var schedulerExampleSecondConfig7 = new Configuration(schedulerExampleSecondResult6.NextExecutionTime, ConfigType.Recurring, true, null, Occurs.Monthly,
            new MonthlyConfiguration(MonthlyConfigType.DayNumberOption, 10, Ordinal.First, KindOfDay.Thursday, 3), null,
            new DailyConfiguration(DailyConfigType.Recurring, TimeOnly.MinValue, 1, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(3, 0, 0), new TimeOnly(6, 0, 0))), new DateLimits(new DateTime(2020, 1, 1)));
            var schedulerExampleSecondResult7 = schedulerExample2.Execute(schedulerExampleSecondConfig7);
            schedulerExampleSecondResult7.NextExecutionTime.Should().Be(new DateTime(2020, 5, 10, 5, 0, 0));
            schedulerExampleSecondResult7.Description.Should().Be("Occurs the 10th of very 3 months and every 1 hours between 3:00:00 AM and 6:00:00 AM starting on 01/01/2020");

            var schedulerExampleSecondConfig8 = new Configuration(schedulerExampleSecondResult7.NextExecutionTime, ConfigType.Recurring, true, null, Occurs.Monthly,
            new MonthlyConfiguration(MonthlyConfigType.DayNumberOption, 10, Ordinal.First, KindOfDay.Thursday, 3), null,
            new DailyConfiguration(DailyConfigType.Recurring, TimeOnly.MinValue, 1, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(3, 0, 0), new TimeOnly(6, 0, 0))), new DateLimits(new DateTime(2020, 1, 1)));
            var schedulerExampleSecondResult8 = schedulerExample2.Execute(schedulerExampleSecondConfig8);
            schedulerExampleSecondResult8.NextExecutionTime.Should().Be(new DateTime(2020, 5, 10, 6, 0, 0));
            schedulerExampleSecondResult8.Description.Should().Be("Occurs the 10th of very 3 months and every 1 hours between 3:00:00 AM and 6:00:00 AM starting on 01/01/2020");

            var schedulerExampleSecondConfig9 = new Configuration(schedulerExampleSecondResult8.NextExecutionTime, ConfigType.Recurring, true, null, Occurs.Monthly,
            new MonthlyConfiguration(MonthlyConfigType.DayNumberOption, 10, Ordinal.First, KindOfDay.Thursday, 3), null,
            new DailyConfiguration(DailyConfigType.Recurring, TimeOnly.MinValue, 1, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(3, 0, 0), new TimeOnly(6, 0, 0))), new DateLimits(new DateTime(2020, 1, 1)));
            var schedulerExampleSecondResult9 = schedulerExample2.Execute(schedulerExampleSecondConfig9);
            schedulerExampleSecondResult9.NextExecutionTime.Should().Be(new DateTime(2020, 9, 10, 3, 0, 0));
            schedulerExampleSecondResult9.Description.Should().Be("Occurs the 10th of very 3 months and every 1 hours between 3:00:00 AM and 6:00:00 AM starting on 01/01/2020");
        }

        [Fact]
        public void Should_Be_Next_Execution_Ordinal_DayOfWeek_Skipping_Months_With_DailyConfiguration_Recurring_Second_Scheduler_Example()
        {
            var schedulerExample2 = new Scheduler();
            var schedulerExampleSecondConfig1 = new Configuration(new DateTime(2020, 1, 1, 0, 0, 0), ConfigType.Recurring, true, null, Occurs.Monthly,
                new MonthlyConfiguration(MonthlyConfigType.WeekDayOption, 10, Ordinal.Second, KindOfDay.WeekEndDay, 1), null,
                new DailyConfiguration(DailyConfigType.Recurring, TimeOnly.MinValue, 1, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(3, 0, 0), new TimeOnly(6, 0, 0))), new DateLimits(new DateTime(2020, 1, 1)));
            var schedulerExampleSecondResult1 = schedulerExample2.Execute(schedulerExampleSecondConfig1);
            schedulerExampleSecondResult1.NextExecutionTime.Should().Be(new DateTime(2020, 1, 5, 3, 0, 0));
            schedulerExampleSecondResult1.Description.Should().Be("Occurs the second weekendday of very 1 month and every 1 hours between 3:00:00 AM and 6:00:00 AM starting on 01/01/2020");

            var schedulerExampleSecondConfig2 = new Configuration(schedulerExampleSecondResult1.NextExecutionTime, ConfigType.Recurring, true, null, Occurs.Monthly,
            new MonthlyConfiguration(MonthlyConfigType.WeekDayOption, 10, Ordinal.Second, KindOfDay.WeekEndDay, 1), null,
            new DailyConfiguration(DailyConfigType.Recurring, TimeOnly.MinValue, 1, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(3, 0, 0), new TimeOnly(6, 0, 0))), new DateLimits(new DateTime(2020, 1, 1)));
            var schedulerExampleSecondResult2 = schedulerExample2.Execute(schedulerExampleSecondConfig2);
            schedulerExampleSecondResult2.NextExecutionTime.Should().Be(new DateTime(2020, 1, 5, 4, 0, 0));
            schedulerExampleSecondResult2.Description.Should().Be("Occurs the second weekendday of very 1 month and every 1 hours between 3:00:00 AM and 6:00:00 AM starting on 01/01/2020");

            var schedulerExampleSecondConfig3 = new Configuration(schedulerExampleSecondResult2.NextExecutionTime, ConfigType.Recurring, true, null, Occurs.Monthly,
                new MonthlyConfiguration(MonthlyConfigType.WeekDayOption, 10, Ordinal.Second, KindOfDay.WeekEndDay, 1), null,
                new DailyConfiguration(DailyConfigType.Recurring, TimeOnly.MinValue, 1, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(3, 0, 0), new TimeOnly(6, 0, 0))), new DateLimits(new DateTime(2020, 1, 1)));
            var schedulerExampleSecondResult3 = schedulerExample2.Execute(schedulerExampleSecondConfig3);
            schedulerExampleSecondResult3.NextExecutionTime.Should().Be(new DateTime(2020, 1, 5, 5, 0, 0));
            schedulerExampleSecondResult3.Description.Should().Be("Occurs the second weekendday of very 1 month and every 1 hours between 3:00:00 AM and 6:00:00 AM starting on 01/01/2020");

            var schedulerExampleSecondConfig4 = new Configuration(schedulerExampleSecondResult3.NextExecutionTime, ConfigType.Recurring, true, null, Occurs.Monthly,
                new MonthlyConfiguration(MonthlyConfigType.WeekDayOption, 10, Ordinal.Second, KindOfDay.WeekEndDay, 1), null,
                new DailyConfiguration(DailyConfigType.Recurring, TimeOnly.MinValue, 1, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(3, 0, 0), new TimeOnly(6, 0, 0))), new DateLimits(new DateTime(2020, 1, 1)));
            var schedulerExampleSecondResult4 = schedulerExample2.Execute(schedulerExampleSecondConfig4);
            schedulerExampleSecondResult4.NextExecutionTime.Should().Be(new DateTime(2020, 1, 5, 6, 0, 0));
            schedulerExampleSecondResult4.Description.Should().Be("Occurs the second weekendday of very 1 month and every 1 hours between 3:00:00 AM and 6:00:00 AM starting on 01/01/2020");

            var schedulerExampleSecondConfig5 = new Configuration(schedulerExampleSecondResult4.NextExecutionTime, ConfigType.Recurring, true, null, Occurs.Monthly,
                new MonthlyConfiguration(MonthlyConfigType.WeekDayOption, 10, Ordinal.Second, KindOfDay.WeekEndDay, 1), null,
                new DailyConfiguration(DailyConfigType.Recurring, TimeOnly.MinValue, 1, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(3, 0, 0), new TimeOnly(6, 0, 0))), new DateLimits(new DateTime(2020, 1, 1)));
            var schedulerExampleSecondResult5 = schedulerExample2.Execute(schedulerExampleSecondConfig5);
            schedulerExampleSecondResult5.NextExecutionTime.Should().Be(new DateTime(2020, 2, 2, 3, 0, 0));
            schedulerExampleSecondResult5.Description.Should().Be("Occurs the second weekendday of very 1 month and every 1 hours between 3:00:00 AM and 6:00:00 AM starting on 01/01/2020");

            var schedulerExampleSecondConfig6 = new Configuration(schedulerExampleSecondResult5.NextExecutionTime, ConfigType.Recurring, true, null, Occurs.Monthly,
                new MonthlyConfiguration(MonthlyConfigType.WeekDayOption, 10, Ordinal.Second, KindOfDay.WeekEndDay, 1), null,
                new DailyConfiguration(DailyConfigType.Recurring, TimeOnly.MinValue, 1, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(3, 0, 0), new TimeOnly(6, 0, 0))), new DateLimits(new DateTime(2020, 1, 1)));
            var schedulerExampleSecondResult6 = schedulerExample2.Execute(schedulerExampleSecondConfig6);
            schedulerExampleSecondResult6.NextExecutionTime.Should().Be(new DateTime(2020, 2, 2, 4, 0, 0));
            schedulerExampleSecondResult6.Description.Should().Be("Occurs the second weekendday of very 1 month and every 1 hours between 3:00:00 AM and 6:00:00 AM starting on 01/01/2020");

            var schedulerExampleSecondConfig7 = new Configuration(schedulerExampleSecondResult6.NextExecutionTime, ConfigType.Recurring, true, null, Occurs.Monthly,
                new MonthlyConfiguration(MonthlyConfigType.WeekDayOption, 10, Ordinal.Second, KindOfDay.WeekEndDay, 1), null,
                new DailyConfiguration(DailyConfigType.Recurring, TimeOnly.MinValue, 1, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(3, 0, 0), new TimeOnly(6, 0, 0))), new DateLimits(new DateTime(2020, 1, 1)));
            var schedulerExampleSecondResult7 = schedulerExample2.Execute(schedulerExampleSecondConfig7);
            schedulerExampleSecondResult7.NextExecutionTime.Should().Be(new DateTime(2020, 2, 2, 5, 0, 0));
            schedulerExampleSecondResult7.Description.Should().Be("Occurs the second weekendday of very 1 month and every 1 hours between 3:00:00 AM and 6:00:00 AM starting on 01/01/2020");

            var schedulerExampleSecondConfig8 = new Configuration(schedulerExampleSecondResult7.NextExecutionTime, ConfigType.Recurring, true, null, Occurs.Monthly,
                new MonthlyConfiguration(MonthlyConfigType.WeekDayOption, 10, Ordinal.Second, KindOfDay.WeekEndDay, 1), null,
                new DailyConfiguration(DailyConfigType.Recurring, TimeOnly.MinValue, 1, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(3, 0, 0), new TimeOnly(6, 0, 0))), new DateLimits(new DateTime(2020, 1, 1)));
            var schedulerExampleSecondResult8 = schedulerExample2.Execute(schedulerExampleSecondConfig8);
            schedulerExampleSecondResult8.NextExecutionTime.Should().Be(new DateTime(2020, 2, 2, 6, 0, 0));
            schedulerExampleSecondResult8.Description.Should().Be("Occurs the second weekendday of very 1 month and every 1 hours between 3:00:00 AM and 6:00:00 AM starting on 01/01/2020");

            var schedulerExampleSecondConfig9 = new Configuration(schedulerExampleSecondResult8.NextExecutionTime, ConfigType.Recurring, true, null, Occurs.Monthly,
                new MonthlyConfiguration(MonthlyConfigType.WeekDayOption, 10, Ordinal.Second, KindOfDay.WeekEndDay, 1), null,
                new DailyConfiguration(DailyConfigType.Recurring, TimeOnly.MinValue, 1, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(3, 0, 0), new TimeOnly(6, 0, 0))), new DateLimits(new DateTime(2020, 1, 1)));
            var schedulerExampleSecondResult9 = schedulerExample2.Execute(schedulerExampleSecondConfig9);
            schedulerExampleSecondResult9.NextExecutionTime.Should().Be(new DateTime(2020, 3, 7, 3, 0, 0));
            schedulerExampleSecondResult9.Description.Should().Be("Occurs the second weekendday of very 1 month and every 1 hours between 3:00:00 AM and 6:00:00 AM starting on 01/01/2020");
        }

        [Fact]
        public void Should_Be_Next_Execution_Date_Ordinal_WeekDay_Skipping_Months_Days_With_DailyConfiguration_Recurring_Scheduler_Example()
        {
            var schedulerExample2 = new Scheduler();
            var schedulerExampleSecondConfig1 = new Configuration(new DateTime(2020, 1, 1, 0, 0, 0), ConfigType.Recurring, true, null, Occurs.Monthly,
                new MonthlyConfiguration(MonthlyConfigType.WeekDayOption, 1, Ordinal.First, KindOfDay.Thursday, 3), null,
                new DailyConfiguration(DailyConfigType.Recurring, TimeOnly.MinValue, 1, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(3, 0, 0), new TimeOnly(6, 0, 0))), new DateLimits(new DateTime(2020, 1, 1)));
            var schedulerExampleSecondResult1 = schedulerExample2.Execute(schedulerExampleSecondConfig1);
            schedulerExampleSecondResult1.NextExecutionTime.Should().Be(new DateTime(2020, 1, 2, 3, 0, 0));
            schedulerExampleSecondResult1.Description.Should().Be("Occurs the first thursday of very 3 months and every 1 hours between 3:00:00 AM and 6:00:00 AM starting on 01/01/2020");

            var schedulerExampleSecondConfig2 = new Configuration(schedulerExampleSecondResult1.NextExecutionTime, ConfigType.Recurring, true, null, Occurs.Monthly,
                new MonthlyConfiguration(MonthlyConfigType.WeekDayOption, 1, Ordinal.First, KindOfDay.Thursday, 3), null,
                new DailyConfiguration(DailyConfigType.Recurring, TimeOnly.MinValue, 1, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(3, 0, 0), new TimeOnly(6, 0, 0))), new DateLimits(new DateTime(2020, 1, 1)));
            var schedulerExampleSecondResult2 = schedulerExample2.Execute(schedulerExampleSecondConfig2);
            schedulerExampleSecondResult2.NextExecutionTime.Should().Be(new DateTime(2020, 1, 2, 4, 0, 0));
            schedulerExampleSecondResult2.Description.Should().Be("Occurs the first thursday of very 3 months and every 1 hours between 3:00:00 AM and 6:00:00 AM starting on 01/01/2020");

            var schedulerExampleSecondConfig3 = new Configuration(schedulerExampleSecondResult2.NextExecutionTime, ConfigType.Recurring, true, null, Occurs.Monthly,
                new MonthlyConfiguration(MonthlyConfigType.WeekDayOption, 1, Ordinal.First, KindOfDay.Thursday, 3), null,
                new DailyConfiguration(DailyConfigType.Recurring, TimeOnly.MinValue, 1, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(3, 0, 0), new TimeOnly(6, 0, 0))), new DateLimits(new DateTime(2020, 1, 1)));
            var schedulerExampleSecondResult3 = schedulerExample2.Execute(schedulerExampleSecondConfig3);
            schedulerExampleSecondResult3.NextExecutionTime.Should().Be(new DateTime(2020, 1, 2, 5, 0, 0));
            schedulerExampleSecondResult3.Description.Should().Be("Occurs the first thursday of very 3 months and every 1 hours between 3:00:00 AM and 6:00:00 AM starting on 01/01/2020");

            var schedulerExampleSecondConfig4 = new Configuration(schedulerExampleSecondResult3.NextExecutionTime, ConfigType.Recurring, true, null, Occurs.Monthly,
                new MonthlyConfiguration(MonthlyConfigType.WeekDayOption, 1, Ordinal.First, KindOfDay.Thursday, 3), null,
                new DailyConfiguration(DailyConfigType.Recurring, TimeOnly.MinValue, 1, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(3, 0, 0), new TimeOnly(6, 0, 0))), new DateLimits(new DateTime(2020, 1, 1)));
            var schedulerExampleSecondResult4 = schedulerExample2.Execute(schedulerExampleSecondConfig4);
            schedulerExampleSecondResult4.NextExecutionTime.Should().Be(new DateTime(2020, 1, 2, 6, 0, 0));
            schedulerExampleSecondResult4.Description.Should().Be("Occurs the first thursday of very 3 months and every 1 hours between 3:00:00 AM and 6:00:00 AM starting on 01/01/2020");

            var schedulerExampleSecondConfig5 = new Configuration(schedulerExampleSecondResult4.NextExecutionTime, ConfigType.Recurring, true, null, Occurs.Monthly,
                new MonthlyConfiguration(MonthlyConfigType.WeekDayOption, 1, Ordinal.First, KindOfDay.Thursday, 3), null,
                new DailyConfiguration(DailyConfigType.Recurring, TimeOnly.MinValue, 1, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(3, 0, 0), new TimeOnly(6, 0, 0))), new DateLimits(new DateTime(2020, 1, 1)));
            var schedulerExampleSecondResult5 = schedulerExample2.Execute(schedulerExampleSecondConfig5);
            schedulerExampleSecondResult5.NextExecutionTime.Should().Be(new DateTime(2020, 4, 2, 3, 0, 0));
            schedulerExampleSecondResult5.Description.Should().Be("Occurs the first thursday of very 3 months and every 1 hours between 3:00:00 AM and 6:00:00 AM starting on 01/01/2020");

            var schedulerExampleSecondConfig6 = new Configuration(schedulerExampleSecondResult5.NextExecutionTime, ConfigType.Recurring, true, null, Occurs.Monthly,
                new MonthlyConfiguration(MonthlyConfigType.WeekDayOption, 1, Ordinal.First, KindOfDay.Thursday, 3), null,
                new DailyConfiguration(DailyConfigType.Recurring, TimeOnly.MinValue, 1, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(3, 0, 0), new TimeOnly(6, 0, 0))), new DateLimits(new DateTime(2020, 1, 1)));
            var schedulerExampleSecondResult6 = schedulerExample2.Execute(schedulerExampleSecondConfig6);
            schedulerExampleSecondResult6.NextExecutionTime.Should().Be(new DateTime(2020, 4, 2, 4, 0, 0));
            schedulerExampleSecondResult6.Description.Should().Be("Occurs the first thursday of very 3 months and every 1 hours between 3:00:00 AM and 6:00:00 AM starting on 01/01/2020");

            var schedulerExampleSecondConfig7 = new Configuration(schedulerExampleSecondResult6.NextExecutionTime, ConfigType.Recurring, true, null, Occurs.Monthly,
                new MonthlyConfiguration(MonthlyConfigType.WeekDayOption, 1, Ordinal.First, KindOfDay.Thursday, 3), null,
                new DailyConfiguration(DailyConfigType.Recurring, TimeOnly.MinValue, 1, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(3, 0, 0), new TimeOnly(6, 0, 0))), new DateLimits(new DateTime(2020, 1, 1)));
            var schedulerExampleSecondResult7 = schedulerExample2.Execute(schedulerExampleSecondConfig7);
            schedulerExampleSecondResult7.NextExecutionTime.Should().Be(new DateTime(2020, 4, 2, 5, 0, 0));
            schedulerExampleSecondResult7.Description.Should().Be("Occurs the first thursday of very 3 months and every 1 hours between 3:00:00 AM and 6:00:00 AM starting on 01/01/2020");

            var schedulerExampleSecondConfig8 = new Configuration(schedulerExampleSecondResult7.NextExecutionTime, ConfigType.Recurring, true, null, Occurs.Monthly,
                new MonthlyConfiguration(MonthlyConfigType.WeekDayOption, 1, Ordinal.First, KindOfDay.Thursday, 3), null,
                new DailyConfiguration(DailyConfigType.Recurring, TimeOnly.MinValue, 1, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(3, 0, 0), new TimeOnly(6, 0, 0))), new DateLimits(new DateTime(2020, 1, 1)));
            var schedulerExampleSecondResult8 = schedulerExample2.Execute(schedulerExampleSecondConfig8);
            schedulerExampleSecondResult8.NextExecutionTime.Should().Be(new DateTime(2020, 4, 2, 6, 0, 0));
            schedulerExampleSecondResult8.Description.Should().Be("Occurs the first thursday of very 3 months and every 1 hours between 3:00:00 AM and 6:00:00 AM starting on 01/01/2020");

            var schedulerExampleSecondConfig9 = new Configuration(schedulerExampleSecondResult8.NextExecutionTime, ConfigType.Recurring, true, null, Occurs.Monthly,
                new MonthlyConfiguration(MonthlyConfigType.WeekDayOption, 1, Ordinal.First, KindOfDay.Thursday, 3), null,
                new DailyConfiguration(DailyConfigType.Recurring, TimeOnly.MinValue, 1, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(3, 0, 0), new TimeOnly(6, 0, 0))), new DateLimits(new DateTime(2020, 1, 1)));
            var schedulerExampleSecondResult9 = schedulerExample2.Execute(schedulerExampleSecondConfig9);
            schedulerExampleSecondResult9.NextExecutionTime.Should().Be(new DateTime(2020, 7, 2, 3, 0, 0));
            schedulerExampleSecondResult9.Description.Should().Be("Occurs the first thursday of very 3 months and every 1 hours between 3:00:00 AM and 6:00:00 AM starting on 01/01/2020");
        }

        [Fact]
        public void Should_Be_Next_Execution_Date_Ordinal_WeekDay_Skipping_Months_Days_With_DailyConfiguration_Recurring_Hours_Thursday()
        {
            var lastThursdayExample2 = new Scheduler();
            var lastThursdayExampleSecondConfig1 = new Configuration(new DateTime(2024, 1, 1, 0, 0, 0), ConfigType.Recurring, true, null, Occurs.Monthly,
                new MonthlyConfiguration(MonthlyConfigType.WeekDayOption, 1, Ordinal.Last, KindOfDay.Thursday, 1), null,
                new DailyConfiguration(DailyConfigType.Recurring, TimeOnly.MinValue, 2, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(14, 0, 0), new TimeOnly(20, 0, 0))), new DateLimits(new DateTime(2020, 1, 1)));
            var lastThursdayExampleSecondResult1 = lastThursdayExample2.Execute(lastThursdayExampleSecondConfig1);
            lastThursdayExampleSecondResult1.NextExecutionTime.Should().Be(new DateTime(2024, 1, 25, 14, 0, 0));
            lastThursdayExampleSecondResult1.Description.Should().Be("Occurs the last thursday of very 1 month and every 2 hours between 2:00:00 PM and 8:00:00 PM starting on 01/01/2020");

            var lastThursdayExampleSecondConfig2 = new Configuration(lastThursdayExampleSecondResult1.NextExecutionTime, ConfigType.Recurring, true, null, Occurs.Monthly,
                new MonthlyConfiguration(MonthlyConfigType.WeekDayOption, 1, Ordinal.Last, KindOfDay.Thursday, 1), null,
                new DailyConfiguration(DailyConfigType.Recurring, TimeOnly.MinValue, 2, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(14, 0, 0), new TimeOnly(20, 0, 0))), new DateLimits(new DateTime(2020, 1, 1)));
            var lastThursdayExampleSecondResult2 = lastThursdayExample2.Execute(lastThursdayExampleSecondConfig2);
            lastThursdayExampleSecondResult2.NextExecutionTime.Should().Be(new DateTime(2024, 1, 25, 16, 0, 0));
            lastThursdayExampleSecondResult2.Description.Should().Be("Occurs the last thursday of very 1 month and every 2 hours between 2:00:00 PM and 8:00:00 PM starting on 01/01/2020");

            var lastThursdayExampleSecondConfig3 = new Configuration(lastThursdayExampleSecondResult2.NextExecutionTime, ConfigType.Recurring, true, null, Occurs.Monthly,
                new MonthlyConfiguration(MonthlyConfigType.WeekDayOption, 1, Ordinal.Last, KindOfDay.Thursday, 1), null,
                new DailyConfiguration(DailyConfigType.Recurring, TimeOnly.MinValue, 2, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(14, 0, 0), new TimeOnly(20, 0, 0))), new DateLimits(new DateTime(2020, 1, 1)));
            var lastThursdayExampleSecondResult3 = lastThursdayExample2.Execute(lastThursdayExampleSecondConfig3);
            lastThursdayExampleSecondResult3.NextExecutionTime.Should().Be(new DateTime(2024, 1, 25, 18, 0, 0));
            lastThursdayExampleSecondResult3.Description.Should().Be("Occurs the last thursday of very 1 month and every 2 hours between 2:00:00 PM and 8:00:00 PM starting on 01/01/2020");

            var lastThursdayExampleSecondConfig4 = new Configuration(lastThursdayExampleSecondResult3.NextExecutionTime, ConfigType.Recurring, true, null, Occurs.Monthly,
                new MonthlyConfiguration(MonthlyConfigType.WeekDayOption, 1, Ordinal.Last, KindOfDay.Thursday, 1), null,
                new DailyConfiguration(DailyConfigType.Recurring, TimeOnly.MinValue, 2, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(14, 0, 0), new TimeOnly(20, 0, 0))), new DateLimits(new DateTime(2020, 1, 1)));
            var lastThursdayExampleSecondResult4 = lastThursdayExample2.Execute(lastThursdayExampleSecondConfig4);
            lastThursdayExampleSecondResult4.NextExecutionTime.Should().Be(new DateTime(2024, 1, 25, 20, 0, 0));
            lastThursdayExampleSecondResult4.Description.Should().Be("Occurs the last thursday of very 1 month and every 2 hours between 2:00:00 PM and 8:00:00 PM starting on 01/01/2020");

            var lastThursdayExampleSecondConfig5 = new Configuration(lastThursdayExampleSecondResult4.NextExecutionTime, ConfigType.Recurring, true, null, Occurs.Monthly,
                new MonthlyConfiguration(MonthlyConfigType.WeekDayOption, 1, Ordinal.Last, KindOfDay.Thursday, 1), null,
                new DailyConfiguration(DailyConfigType.Recurring, TimeOnly.MinValue, 2, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(14, 0, 0), new TimeOnly(20, 0, 0))), new DateLimits(new DateTime(2020, 1, 1)));
            var lastThursdayExampleSecondResult5 = lastThursdayExample2.Execute(lastThursdayExampleSecondConfig5);
            lastThursdayExampleSecondResult5.NextExecutionTime.Should().Be(new DateTime(2024, 2, 29, 14, 0, 0));
            lastThursdayExampleSecondResult5.Description.Should().Be("Occurs the last thursday of very 1 month and every 2 hours between 2:00:00 PM and 8:00:00 PM starting on 01/01/2020");

            var lastThursdayExampleSecondConfig6 = new Configuration(lastThursdayExampleSecondResult5.NextExecutionTime, ConfigType.Recurring, true, null, Occurs.Monthly,
                new MonthlyConfiguration(MonthlyConfigType.WeekDayOption, 1, Ordinal.Last, KindOfDay.Thursday, 1), null,
                new DailyConfiguration(DailyConfigType.Recurring, TimeOnly.MinValue, 2, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(14, 0, 0), new TimeOnly(20, 0, 0))), new DateLimits(new DateTime(2020, 1, 1)));
            var lastThursdayExampleSecondResult6 = lastThursdayExample2.Execute(lastThursdayExampleSecondConfig6);
            lastThursdayExampleSecondResult6.NextExecutionTime.Should().Be(new DateTime(2024, 2, 29, 16, 0, 0));
            lastThursdayExampleSecondResult6.Description.Should().Be("Occurs the last thursday of very 1 month and every 2 hours between 2:00:00 PM and 8:00:00 PM starting on 01/01/2020");

            var lastThursdayExampleSecondConfig7 = new Configuration(lastThursdayExampleSecondResult6.NextExecutionTime, ConfigType.Recurring, true, null, Occurs.Monthly,
                new MonthlyConfiguration(MonthlyConfigType.WeekDayOption, 1, Ordinal.Last, KindOfDay.Thursday, 1), null,
                new DailyConfiguration(DailyConfigType.Recurring, TimeOnly.MinValue, 2, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(14, 0, 0), new TimeOnly(20, 0, 0))), new DateLimits(new DateTime(2020, 1, 1)));
            var lastThursdayExampleSecondResult7 = lastThursdayExample2.Execute(lastThursdayExampleSecondConfig7);
            lastThursdayExampleSecondResult7.NextExecutionTime.Should().Be(new DateTime(2024, 2, 29, 18, 0, 0));
            lastThursdayExampleSecondResult7.Description.Should().Be("Occurs the last thursday of very 1 month and every 2 hours between 2:00:00 PM and 8:00:00 PM starting on 01/01/2020");

            var lastThursdayExampleSecondConfig8 = new Configuration(lastThursdayExampleSecondResult7.NextExecutionTime, ConfigType.Recurring, true, null, Occurs.Monthly,
                new MonthlyConfiguration(MonthlyConfigType.WeekDayOption, 1, Ordinal.Last, KindOfDay.Thursday, 1), null,
                new DailyConfiguration(DailyConfigType.Recurring, TimeOnly.MinValue, 2, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(14, 0, 0), new TimeOnly(20, 0, 0))), new DateLimits(new DateTime(2020, 1, 1)));
            var lastThursdayExampleSecondResult8 = lastThursdayExample2.Execute(lastThursdayExampleSecondConfig8);
            lastThursdayExampleSecondResult8.NextExecutionTime.Should().Be(new DateTime(2024, 2, 29, 20, 0, 0));
            lastThursdayExampleSecondResult8.Description.Should().Be("Occurs the last thursday of very 1 month and every 2 hours between 2:00:00 PM and 8:00:00 PM starting on 01/01/2020");

            var lastThursdayExampleSecondConfig9 = new Configuration(lastThursdayExampleSecondResult8.NextExecutionTime, ConfigType.Recurring, true, null, Occurs.Monthly,
                new MonthlyConfiguration(MonthlyConfigType.WeekDayOption, 1, Ordinal.Last, KindOfDay.Thursday, 1), null,
                new DailyConfiguration(DailyConfigType.Recurring, TimeOnly.MinValue, 2, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(14, 0, 0), new TimeOnly(20, 0, 0))), new DateLimits(new DateTime(2020, 1, 1)));
            var lastThursdayExampleSecondResult9 = lastThursdayExample2.Execute(lastThursdayExampleSecondConfig9);
            lastThursdayExampleSecondResult9.NextExecutionTime.Should().Be(new DateTime(2024, 3, 28, 14, 0, 0));
            lastThursdayExampleSecondResult9.Description.Should().Be("Occurs the last thursday of very 1 month and every 2 hours between 2:00:00 PM and 8:00:00 PM starting on 01/01/2020");
        }

        [Fact]
        public void Should_Skip_Months_Because_DayNumber_Is_Greater_Than_DaysInMonth()
        {
            var scheduler1 = new Scheduler();
            var schedulerSecondConfig1 = new Configuration(new DateTime(2024, 1, 31, 22, 0, 0), ConfigType.Recurring, true, null, Occurs.Monthly,
                new MonthlyConfiguration(MonthlyConfigType.DayNumberOption, 31, Ordinal.First, KindOfDay.Thursday, 1), null,
                new DailyConfiguration(DailyConfigType.Recurring, TimeOnly.MinValue, 3, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(14, 0, 0), new TimeOnly(20, 0, 0))), new DateLimits(new DateTime(2020, 1, 1)));
            var schedulerSecondResult1 = scheduler1.Execute(schedulerSecondConfig1);
            schedulerSecondResult1.NextExecutionTime.Should().Be(new DateTime(2024, 3, 31, 14, 0, 0));
            schedulerSecondResult1.Description.Should().Be("Occurs the 31st of very 1 month and every 3 hours between 2:00:00 PM and 8:00:00 PM starting on 01/01/2020");

            var scheduler2 = new Scheduler();
            var schedulerSecondConfig2 = new Configuration(new DateTime(2024, 1, 30, 7, 0, 0), ConfigType.Recurring, true, null, Occurs.Monthly,
                new MonthlyConfiguration(MonthlyConfigType.DayNumberOption, 30, Ordinal.First, KindOfDay.Thursday, 1), null,
                new DailyConfiguration(DailyConfigType.Recurring, TimeOnly.MinValue, 8, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(5, 0, 0), new TimeOnly(6, 0, 0))), new DateLimits(new DateTime(2020, 1, 1)));
            var schedulerSecondResult2 = scheduler2.Execute(schedulerSecondConfig2);
            schedulerSecondResult2.NextExecutionTime.Should().Be(new DateTime(2024, 3, 30, 5, 0, 0));
            schedulerSecondResult2.Description.Should().Be("Occurs the 30th of very 1 month and every 8 hours between 5:00:00 AM and 6:00:00 AM starting on 01/01/2020");
        }

        [Fact]
        public void Should_Be_Next_Execution_Date_Ordinal_WeekDay_Skipping_Months_Days_With_DailyConfiguration_Recurring_Hours_Third_Weekday()
        {

            var lastThursdayExample2 = new Scheduler();
            var lastThursdayExampleSecondConfig1 = new Configuration(new DateTime(2024, 1, 3, 15, 50, 0), ConfigType.Recurring, true, null, Occurs.Monthly,
                new MonthlyConfiguration(MonthlyConfigType.WeekDayOption, 1, Ordinal.Third, KindOfDay.WeekDay, 1), null,
                new DailyConfiguration(DailyConfigType.Recurring, TimeOnly.MinValue, 2, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(14, 0, 0), new TimeOnly(20, 0, 0))),
                new DateLimits(new DateTime(2024, 1, 1)));
            var lastThursdayExampleSecondResult1 = lastThursdayExample2.Execute(lastThursdayExampleSecondConfig1);
            lastThursdayExampleSecondResult1.NextExecutionTime.Should().Be(new DateTime(2024, 1, 8, 15, 50, 0));
            lastThursdayExampleSecondResult1.Description.Should().Be("Occurs the third weekday of very 1 month and every 2 hours between 2:00:00 PM and 8:00:00 PM starting on 01/01/2024");

            var lastThursdayExampleSecondConfig2 = new Configuration(lastThursdayExampleSecondResult1.NextExecutionTime, ConfigType.Recurring, true, null, Occurs.Monthly,
                new MonthlyConfiguration(MonthlyConfigType.WeekDayOption, 1, Ordinal.Third, KindOfDay.WeekDay, 1), null,
                new DailyConfiguration(DailyConfigType.Recurring, TimeOnly.MinValue, 2, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(14, 0, 0), new TimeOnly(20, 0, 0))),
                new DateLimits(new DateTime(2024, 1, 1)));
            var lastThursdayExampleSecondResult2 = lastThursdayExample2.Execute(lastThursdayExampleSecondConfig2);
            lastThursdayExampleSecondResult2.NextExecutionTime.Should().Be(new DateTime(2024, 1, 8, 17, 50, 0));
            lastThursdayExampleSecondResult2.Description.Should().Be("Occurs the third weekday of very 1 month and every 2 hours between 2:00:00 PM and 8:00:00 PM starting on 01/01/2024");

            var lastThursdayExampleSecondConfig3 = new Configuration(lastThursdayExampleSecondResult2.NextExecutionTime, ConfigType.Recurring, true, null, Occurs.Monthly,
                new MonthlyConfiguration(MonthlyConfigType.WeekDayOption, 1, Ordinal.Third, KindOfDay.WeekDay, 1), null,
                new DailyConfiguration(DailyConfigType.Recurring, TimeOnly.MinValue, 2, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(14, 0, 0), new TimeOnly(20, 0, 0))),
                new DateLimits(new DateTime(2024, 1, 1)));
            var lastThursdayExampleSecondResult3 = lastThursdayExample2.Execute(lastThursdayExampleSecondConfig3);
            lastThursdayExampleSecondResult3.NextExecutionTime.Should().Be(new DateTime(2024, 1, 8, 19, 50, 0));
            lastThursdayExampleSecondResult3.Description.Should().Be("Occurs the third weekday of very 1 month and every 2 hours between 2:00:00 PM and 8:00:00 PM starting on 01/01/2024");

            var lastThursdayExampleSecondConfig4 = new Configuration(lastThursdayExampleSecondResult3.NextExecutionTime, ConfigType.Recurring, true, null, Occurs.Monthly,
                new MonthlyConfiguration(MonthlyConfigType.WeekDayOption, 1, Ordinal.Third, KindOfDay.WeekDay, 1), null,
                new DailyConfiguration(DailyConfigType.Recurring, TimeOnly.MinValue, 2, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(14, 0, 0), new TimeOnly(20, 0, 0))),
                new DateLimits(new DateTime(2024, 1, 1)));
            var lastThursdayExampleSecondResult4 = lastThursdayExample2.Execute(lastThursdayExampleSecondConfig4);
            lastThursdayExampleSecondResult4.NextExecutionTime.Should().Be(new DateTime(2024, 2, 5, 14, 0, 0));
            lastThursdayExampleSecondResult4.Description.Should().Be("Occurs the third weekday of very 1 month and every 2 hours between 2:00:00 PM and 8:00:00 PM starting on 01/01/2024");

            var lastThursdayExampleSecondConfig5 = new Configuration(lastThursdayExampleSecondResult4.NextExecutionTime, ConfigType.Recurring, true, null, Occurs.Monthly,
                new MonthlyConfiguration(MonthlyConfigType.WeekDayOption, 1, Ordinal.Third, KindOfDay.WeekDay, 1), null,
                new DailyConfiguration(DailyConfigType.Recurring, TimeOnly.MinValue, 2, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(14, 0, 0), new TimeOnly(20, 0, 0))),
                new DateLimits(new DateTime(2024, 1, 1)));
            var lastThursdayExampleSecondResult5 = lastThursdayExample2.Execute(lastThursdayExampleSecondConfig5);
            lastThursdayExampleSecondResult5.NextExecutionTime.Should().Be(new DateTime(2024, 2, 5, 16, 0, 0));
            lastThursdayExampleSecondResult5.Description.Should().Be("Occurs the third weekday of very 1 month and every 2 hours between 2:00:00 PM and 8:00:00 PM starting on 01/01/2024");

            var lastThursdayExampleSecondConfig6 = new Configuration(lastThursdayExampleSecondResult5.NextExecutionTime, ConfigType.Recurring, true, null, Occurs.Monthly,
                new MonthlyConfiguration(MonthlyConfigType.WeekDayOption, 1, Ordinal.Third, KindOfDay.WeekDay, 1), null,
                new DailyConfiguration(DailyConfigType.Recurring, TimeOnly.MinValue, 2, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(14, 0, 0), new TimeOnly(20, 0, 0))),
                new DateLimits(new DateTime(2024, 1, 1)));
            var lastThursdayExampleSecondResult6 = lastThursdayExample2.Execute(lastThursdayExampleSecondConfig6);
            lastThursdayExampleSecondResult6.NextExecutionTime.Should().Be(new DateTime(2024, 2, 5, 18, 0, 0));
            lastThursdayExampleSecondResult6.Description.Should().Be("Occurs the third weekday of very 1 month and every 2 hours between 2:00:00 PM and 8:00:00 PM starting on 01/01/2024");

            var lastThursdayExampleSecondConfig7 = new Configuration(lastThursdayExampleSecondResult6.NextExecutionTime, ConfigType.Recurring, true, null, Occurs.Monthly,
                new MonthlyConfiguration(MonthlyConfigType.WeekDayOption, 1, Ordinal.Third, KindOfDay.WeekDay, 1), null,
                new DailyConfiguration(DailyConfigType.Recurring, TimeOnly.MinValue, 2, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(14, 0, 0), new TimeOnly(20, 0, 0))),
                new DateLimits(new DateTime(2024, 1, 1)));
            var lastThursdayExampleSecondResult7 = lastThursdayExample2.Execute(lastThursdayExampleSecondConfig7);
            lastThursdayExampleSecondResult7.NextExecutionTime.Should().Be(new DateTime(2024, 2, 5, 20, 0, 0));
            lastThursdayExampleSecondResult7.Description.Should().Be("Occurs the third weekday of very 1 month and every 2 hours between 2:00:00 PM and 8:00:00 PM starting on 01/01/2024");

            var lastThursdayExampleSecondConfig8 = new Configuration(lastThursdayExampleSecondResult7.NextExecutionTime, ConfigType.Recurring, true, null, Occurs.Monthly,
                new MonthlyConfiguration(MonthlyConfigType.WeekDayOption, 1, Ordinal.Third, KindOfDay.WeekDay, 1), null,
                new DailyConfiguration(DailyConfigType.Recurring, TimeOnly.MinValue, 2, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(14, 0, 0), new TimeOnly(20, 0, 0))),
                new DateLimits(new DateTime(2024, 1, 1)));
            var lastThursdayExampleSecondResult8 = lastThursdayExample2.Execute(lastThursdayExampleSecondConfig8);
            lastThursdayExampleSecondResult8.NextExecutionTime.Should().Be(new DateTime(2024, 3, 5, 14, 0, 0));
            lastThursdayExampleSecondResult8.Description.Should().Be("Occurs the third weekday of very 1 month and every 2 hours between 2:00:00 PM and 8:00:00 PM starting on 01/01/2024");

            var lastThursdayExampleSecondConfig9 = new Configuration(lastThursdayExampleSecondResult8.NextExecutionTime, ConfigType.Recurring, true, null, Occurs.Monthly,
                new MonthlyConfiguration(MonthlyConfigType.WeekDayOption, 1, Ordinal.Third, KindOfDay.WeekDay, 1), null,
                new DailyConfiguration(DailyConfigType.Recurring, TimeOnly.MinValue, 2, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(14, 0, 0), new TimeOnly(20, 0, 0))),
                new DateLimits(new DateTime(2024, 1, 1)));
            var lastThursdayExampleSecondResult9 = lastThursdayExample2.Execute(lastThursdayExampleSecondConfig9);
            lastThursdayExampleSecondResult9.NextExecutionTime.Should().Be(new DateTime(2024, 3, 5, 16, 0, 0));
            lastThursdayExampleSecondResult9.Description.Should().Be("Occurs the third weekday of very 1 month and every 2 hours between 2:00:00 PM and 8:00:00 PM starting on 01/01/2024");
            ;
        }
        [Fact]
        public void Should_Be_Next_Execution_Date_Ordinal_WeekDay_Skipping_Months_Days_With_DailyConfiguration_Recurring_Minutes_WeekEndDay()
        {
            var lastWeekEndDayExample2 = new Scheduler();
            var lastWeekEndDayExampleSecondConfig1 = new Configuration(new DateTime(2024, 1, 1, 0, 0, 0), ConfigType.Recurring, true, null, Occurs.Monthly,
                new MonthlyConfiguration(MonthlyConfigType.WeekDayOption, 1, Ordinal.Last, KindOfDay.WeekEndDay, 1), null,
                new DailyConfiguration(DailyConfigType.Recurring, TimeOnly.MinValue, 40, DailyFrecuency.Minutes, new TimeLimits(new TimeOnly(14, 0, 0), new TimeOnly(16, 0, 0))), new DateLimits(new DateTime(2020, 1, 1)));
            var lastWeekEndDayExampleSecondResult1 = lastWeekEndDayExample2.Execute(lastWeekEndDayExampleSecondConfig1);
            lastWeekEndDayExampleSecondResult1.NextExecutionTime.Should().Be(new DateTime(2024, 1, 28, 14, 0, 0));
            lastWeekEndDayExampleSecondResult1.Description.Should().Be("Occurs the last weekendday of very 1 month and every 40 minutes between 2:00:00 PM and 4:00:00 PM starting on 01/01/2020");

            var lastWeekEndDayExampleSecondConfig2 = new Configuration(lastWeekEndDayExampleSecondResult1.NextExecutionTime, ConfigType.Recurring, true, null, Occurs.Monthly,
                new MonthlyConfiguration(MonthlyConfigType.WeekDayOption, 1, Ordinal.Last, KindOfDay.WeekEndDay, 1), null,
                new DailyConfiguration(DailyConfigType.Recurring, TimeOnly.MinValue, 40, DailyFrecuency.Minutes, new TimeLimits(new TimeOnly(14, 0, 0), new TimeOnly(16, 0, 0))), new DateLimits(new DateTime(2020, 1, 1)));
            var lastWeekEndDayExampleSecondResult2 = lastWeekEndDayExample2.Execute(lastWeekEndDayExampleSecondConfig2);
            lastWeekEndDayExampleSecondResult2.NextExecutionTime.Should().Be(new DateTime(2024, 1, 28, 14, 40, 0));
            lastWeekEndDayExampleSecondResult2.Description.Should().Be("Occurs the last weekendday of very 1 month and every 40 minutes between 2:00:00 PM and 4:00:00 PM starting on 01/01/2020");

            var lastWeekEndDayExampleSecondConfig3 = new Configuration(lastWeekEndDayExampleSecondResult2.NextExecutionTime, ConfigType.Recurring, true, null, Occurs.Monthly,
                new MonthlyConfiguration(MonthlyConfigType.WeekDayOption, 1, Ordinal.Last, KindOfDay.WeekEndDay, 1), null,
                new DailyConfiguration(DailyConfigType.Recurring, TimeOnly.MinValue, 40, DailyFrecuency.Minutes, new TimeLimits(new TimeOnly(14, 0, 0), new TimeOnly(16, 0, 0))), new DateLimits(new DateTime(2020, 1, 1)));
            var lastWeekEndDayExampleSecondResult3 = lastWeekEndDayExample2.Execute(lastWeekEndDayExampleSecondConfig3);
            lastWeekEndDayExampleSecondResult3.NextExecutionTime.Should().Be(new DateTime(2024, 1, 28, 15, 20, 0));
            lastWeekEndDayExampleSecondResult3.Description.Should().Be("Occurs the last weekendday of very 1 month and every 40 minutes between 2:00:00 PM and 4:00:00 PM starting on 01/01/2020");

            var lastWeekEndDayExampleSecondConfig4 = new Configuration(lastWeekEndDayExampleSecondResult3.NextExecutionTime, ConfigType.Recurring, true, null, Occurs.Monthly,
                new MonthlyConfiguration(MonthlyConfigType.WeekDayOption, 1, Ordinal.Last, KindOfDay.WeekEndDay, 1), null,
                new DailyConfiguration(DailyConfigType.Recurring, TimeOnly.MinValue, 40, DailyFrecuency.Minutes, new TimeLimits(new TimeOnly(14, 0, 0), new TimeOnly(16, 0, 0))), new DateLimits(new DateTime(2020, 1, 1)));
            var lastWeekEndDayExampleSecondResult4 = lastWeekEndDayExample2.Execute(lastWeekEndDayExampleSecondConfig4);
            lastWeekEndDayExampleSecondResult4.NextExecutionTime.Should().Be(new DateTime(2024, 1, 28, 16, 0, 0));
            lastWeekEndDayExampleSecondResult4.Description.Should().Be("Occurs the last weekendday of very 1 month and every 40 minutes between 2:00:00 PM and 4:00:00 PM starting on 01/01/2020");

            var lastWeekEndDayExampleSecondConfig5 = new Configuration(lastWeekEndDayExampleSecondResult4.NextExecutionTime, ConfigType.Recurring, true, null, Occurs.Monthly,
                new MonthlyConfiguration(MonthlyConfigType.WeekDayOption, 1, Ordinal.Last, KindOfDay.WeekEndDay, 1), null,
                new DailyConfiguration(DailyConfigType.Recurring, TimeOnly.MinValue, 40, DailyFrecuency.Minutes, new TimeLimits(new TimeOnly(14, 0, 0), new TimeOnly(16, 0, 0))), new DateLimits(new DateTime(2020, 1, 1)));
            var lastWeekEndDayExampleSecondResult5 = lastWeekEndDayExample2.Execute(lastWeekEndDayExampleSecondConfig5);
            lastWeekEndDayExampleSecondResult5.NextExecutionTime.Should().Be(new DateTime(2024, 2, 25, 14, 0, 0));
            lastWeekEndDayExampleSecondResult5.Description.Should().Be("Occurs the last weekendday of very 1 month and every 40 minutes between 2:00:00 PM and 4:00:00 PM starting on 01/01/2020");

            var lastWeekEndDayExampleSecondConfig6 = new Configuration(lastWeekEndDayExampleSecondResult5.NextExecutionTime, ConfigType.Recurring, true, null, Occurs.Monthly,
                new MonthlyConfiguration(MonthlyConfigType.WeekDayOption, 1, Ordinal.Last, KindOfDay.WeekEndDay, 1), null,
                new DailyConfiguration(DailyConfigType.Recurring, TimeOnly.MinValue, 40, DailyFrecuency.Minutes, new TimeLimits(new TimeOnly(14, 0, 0), new TimeOnly(16, 0, 0))), new DateLimits(new DateTime(2020, 1, 1)));
            var lastWeekEndDayExampleSecondResult6 = lastWeekEndDayExample2.Execute(lastWeekEndDayExampleSecondConfig6);
            lastWeekEndDayExampleSecondResult6.NextExecutionTime.Should().Be(new DateTime(2024, 2, 25, 14, 40, 0));
            lastWeekEndDayExampleSecondResult6.Description.Should().Be("Occurs the last weekendday of very 1 month and every 40 minutes between 2:00:00 PM and 4:00:00 PM starting on 01/01/2020");

            var lastWeekEndDayExampleSecondConfig7 = new Configuration(lastWeekEndDayExampleSecondResult6.NextExecutionTime, ConfigType.Recurring, true, null, Occurs.Monthly,
                new MonthlyConfiguration(MonthlyConfigType.WeekDayOption, 1, Ordinal.Last, KindOfDay.WeekEndDay, 1), null,
                new DailyConfiguration(DailyConfigType.Recurring, TimeOnly.MinValue, 40, DailyFrecuency.Minutes, new TimeLimits(new TimeOnly(14, 0, 0), new TimeOnly(16, 0, 0))), new DateLimits(new DateTime(2020, 1, 1)));
            var lastWeekEndDayExampleSecondResult7 = lastWeekEndDayExample2.Execute(lastWeekEndDayExampleSecondConfig7);
            lastWeekEndDayExampleSecondResult7.NextExecutionTime.Should().Be(new DateTime(2024, 2, 25, 15, 20, 0));
            lastWeekEndDayExampleSecondResult7.Description.Should().Be("Occurs the last weekendday of very 1 month and every 40 minutes between 2:00:00 PM and 4:00:00 PM starting on 01/01/2020");

            var lastWeekEndDayExampleSecondConfig8 = new Configuration(lastWeekEndDayExampleSecondResult7.NextExecutionTime, ConfigType.Recurring, true, null, Occurs.Monthly,
                new MonthlyConfiguration(MonthlyConfigType.WeekDayOption, 1, Ordinal.Last, KindOfDay.WeekEndDay, 1), null,
                new DailyConfiguration(DailyConfigType.Recurring, TimeOnly.MinValue, 40, DailyFrecuency.Minutes, new TimeLimits(new TimeOnly(14, 0, 0), new TimeOnly(16, 0, 0))), new DateLimits(new DateTime(2020, 1, 1)));
            var lastWeekEndDayExampleSecondResult8 = lastWeekEndDayExample2.Execute(lastWeekEndDayExampleSecondConfig8);
            lastWeekEndDayExampleSecondResult8.NextExecutionTime.Should().Be(new DateTime(2024, 2, 25, 16, 0, 0));
            lastWeekEndDayExampleSecondResult8.Description.Should().Be("Occurs the last weekendday of very 1 month and every 40 minutes between 2:00:00 PM and 4:00:00 PM starting on 01/01/2020");

            var lastWeekEndDayExampleSecondConfig9 = new Configuration(lastWeekEndDayExampleSecondResult8.NextExecutionTime, ConfigType.Recurring, true, null, Occurs.Monthly,
                new MonthlyConfiguration(MonthlyConfigType.WeekDayOption, 1, Ordinal.Last, KindOfDay.WeekEndDay, 1), null,
                new DailyConfiguration(DailyConfigType.Recurring, TimeOnly.MinValue, 40, DailyFrecuency.Minutes, new TimeLimits(new TimeOnly(14, 0, 0), new TimeOnly(16, 0, 0))), new DateLimits(new DateTime(2020, 1, 1)));
            var lastWeekEndDayExampleSecondResult9 = lastWeekEndDayExample2.Execute(lastWeekEndDayExampleSecondConfig9);
            lastWeekEndDayExampleSecondResult9.NextExecutionTime.Should().Be(new DateTime(2024, 3, 31, 14, 0, 0));
            lastWeekEndDayExampleSecondResult9.Description.Should().Be("Occurs the last weekendday of very 1 month and every 40 minutes between 2:00:00 PM and 4:00:00 PM starting on 01/01/2020");
        }

        [Fact]
        public void Should_Be_Next_Execution_Date_Ordinal_WeekDay_Skipping_Months_Days_With_DailyConfiguration_Recurring_Seconds_WeekDay()
        {
            var lastWeekEndDayExample2 = new Scheduler();
            var lastWeekEndDayExampleSecondConfig0 = new Configuration(new DateTime(2024, 1, 1, 0, 0, 0), ConfigType.Recurring, true, null, Occurs.Monthly,
               new MonthlyConfiguration(MonthlyConfigType.WeekDayOption, 1, Ordinal.Last, KindOfDay.WeekDay, 1), null,
               new DailyConfiguration(DailyConfigType.Recurring, TimeOnly.MinValue, 13, DailyFrecuency.Seconds, new TimeLimits(new TimeOnly(14, 0, 0), new TimeOnly(14, 2, 0))), new DateLimits(new DateTime(2020, 1, 1)));
            var lastWeekEndDayExampleSecondResult0 = lastWeekEndDayExample2.Execute(lastWeekEndDayExampleSecondConfig0);
            lastWeekEndDayExampleSecondResult0.NextExecutionTime.Should().Be(new DateTime(2024, 1, 31, 14, 0, 0));
            lastWeekEndDayExampleSecondResult0.Description.Should().Be("Occurs the last weekday of very 1 month and every 13 seconds between 2:00:00 PM and 2:02:00 PM starting on 01/01/2020");

            var lastWeekEndDayExampleSecondConfig1 = new Configuration(lastWeekEndDayExampleSecondResult0.NextExecutionTime, ConfigType.Recurring, true, null, Occurs.Monthly,
                new MonthlyConfiguration(MonthlyConfigType.WeekDayOption, 1, Ordinal.Last, KindOfDay.WeekDay, 1), null,
                new DailyConfiguration(DailyConfigType.Recurring, TimeOnly.MinValue, 13, DailyFrecuency.Seconds, new TimeLimits(new TimeOnly(14, 0, 0), new TimeOnly(14, 2, 0))), new DateLimits(new DateTime(2020, 1, 1)));
            var lastWeekEndDayExampleSecondResult1 = lastWeekEndDayExample2.Execute(lastWeekEndDayExampleSecondConfig1);
            lastWeekEndDayExampleSecondResult1.NextExecutionTime.Should().Be(new DateTime(2024, 1, 31, 14, 0, 13));
            lastWeekEndDayExampleSecondResult1.Description.Should().Be("Occurs the last weekday of very 1 month and every 13 seconds between 2:00:00 PM and 2:02:00 PM starting on 01/01/2020");

            var lastWeekEndDayExampleSecondConfig2 = new Configuration(lastWeekEndDayExampleSecondResult1.NextExecutionTime, ConfigType.Recurring, true, null, Occurs.Monthly,
                new MonthlyConfiguration(MonthlyConfigType.WeekDayOption, 1, Ordinal.Last, KindOfDay.WeekDay, 1), null,
                new DailyConfiguration(DailyConfigType.Recurring, TimeOnly.MinValue, 13, DailyFrecuency.Seconds, new TimeLimits(new TimeOnly(14, 0, 0), new TimeOnly(14, 2, 0))), new DateLimits(new DateTime(2020, 1, 1)));
            var lastWeekEndDayExampleSecondResult2 = lastWeekEndDayExample2.Execute(lastWeekEndDayExampleSecondConfig2);
            lastWeekEndDayExampleSecondResult2.NextExecutionTime.Should().Be(new DateTime(2024, 1, 31, 14, 0, 26));
            lastWeekEndDayExampleSecondResult2.Description.Should().Be("Occurs the last weekday of very 1 month and every 13 seconds between 2:00:00 PM and 2:02:00 PM starting on 01/01/2020");

            var lastWeekEndDayExampleSecondConfig3 = new Configuration(lastWeekEndDayExampleSecondResult2.NextExecutionTime, ConfigType.Recurring, true, null, Occurs.Monthly,
                new MonthlyConfiguration(MonthlyConfigType.WeekDayOption, 1, Ordinal.Last, KindOfDay.WeekDay, 1), null,
                new DailyConfiguration(DailyConfigType.Recurring, TimeOnly.MinValue, 13, DailyFrecuency.Seconds, new TimeLimits(new TimeOnly(14, 0, 0), new TimeOnly(14, 2, 0))), new DateLimits(new DateTime(2020, 1, 1)));
            var lastWeekEndDayExampleSecondResult3 = lastWeekEndDayExample2.Execute(lastWeekEndDayExampleSecondConfig3);
            lastWeekEndDayExampleSecondResult3.NextExecutionTime.Should().Be(new DateTime(2024, 1, 31, 14, 0, 39));
            lastWeekEndDayExampleSecondResult3.Description.Should().Be("Occurs the last weekday of very 1 month and every 13 seconds between 2:00:00 PM and 2:02:00 PM starting on 01/01/2020");

            var lastWeekEndDayExampleSecondConfig4 = new Configuration(lastWeekEndDayExampleSecondResult3.NextExecutionTime, ConfigType.Recurring, true, null, Occurs.Monthly,
                new MonthlyConfiguration(MonthlyConfigType.WeekDayOption, 1, Ordinal.Last, KindOfDay.WeekDay, 1), null,
                new DailyConfiguration(DailyConfigType.Recurring, TimeOnly.MinValue, 13, DailyFrecuency.Seconds, new TimeLimits(new TimeOnly(14, 0, 0), new TimeOnly(14, 2, 0))), new DateLimits(new DateTime(2020, 1, 1)));
            var lastWeekEndDayExampleSecondResult4 = lastWeekEndDayExample2.Execute(lastWeekEndDayExampleSecondConfig4);
            lastWeekEndDayExampleSecondResult4.NextExecutionTime.Should().Be(new DateTime(2024, 1, 31, 14, 0, 52));
            lastWeekEndDayExampleSecondResult4.Description.Should().Be("Occurs the last weekday of very 1 month and every 13 seconds between 2:00:00 PM and 2:02:00 PM starting on 01/01/2020");

            var lastWeekEndDayExampleSecondConfig5 = new Configuration(lastWeekEndDayExampleSecondResult4.NextExecutionTime, ConfigType.Recurring, true, null, Occurs.Monthly,
                new MonthlyConfiguration(MonthlyConfigType.WeekDayOption, 1, Ordinal.Last, KindOfDay.WeekDay, 1), null,
                new DailyConfiguration(DailyConfigType.Recurring, TimeOnly.MinValue, 13, DailyFrecuency.Seconds, new TimeLimits(new TimeOnly(14, 0, 0), new TimeOnly(14, 2, 0))), new DateLimits(new DateTime(2020, 1, 1)));
            var lastWeekEndDayExampleSecondResult5 = lastWeekEndDayExample2.Execute(lastWeekEndDayExampleSecondConfig5);
            lastWeekEndDayExampleSecondResult5.NextExecutionTime.Should().Be(new DateTime(2024, 1, 31, 14, 1, 5));
            lastWeekEndDayExampleSecondResult5.Description.Should().Be("Occurs the last weekday of very 1 month and every 13 seconds between 2:00:00 PM and 2:02:00 PM starting on 01/01/2020");

            var lastWeekEndDayExampleSecondConfig6 = new Configuration(lastWeekEndDayExampleSecondResult5.NextExecutionTime, ConfigType.Recurring, true, null, Occurs.Monthly,
                new MonthlyConfiguration(MonthlyConfigType.WeekDayOption, 1, Ordinal.Last, KindOfDay.WeekDay, 1), null,
                new DailyConfiguration(DailyConfigType.Recurring, TimeOnly.MinValue, 13, DailyFrecuency.Seconds, new TimeLimits(new TimeOnly(14, 0, 0), new TimeOnly(14, 2, 0))), new DateLimits(new DateTime(2020, 1, 1)));
            var lastWeekEndDayExampleSecondResult6 = lastWeekEndDayExample2.Execute(lastWeekEndDayExampleSecondConfig6);
            lastWeekEndDayExampleSecondResult6.NextExecutionTime.Should().Be(new DateTime(2024, 1, 31, 14, 1, 18));
            lastWeekEndDayExampleSecondResult6.Description.Should().Be("Occurs the last weekday of very 1 month and every 13 seconds between 2:00:00 PM and 2:02:00 PM starting on 01/01/2020");

            var lastWeekEndDayExampleSecondConfig7 = new Configuration(lastWeekEndDayExampleSecondResult6.NextExecutionTime, ConfigType.Recurring, true, null, Occurs.Monthly,
                new MonthlyConfiguration(MonthlyConfigType.WeekDayOption, 1, Ordinal.Last, KindOfDay.WeekDay, 1), null,
                new DailyConfiguration(DailyConfigType.Recurring, TimeOnly.MinValue, 13, DailyFrecuency.Seconds, new TimeLimits(new TimeOnly(14, 0, 0), new TimeOnly(14, 2, 0))), new DateLimits(new DateTime(2020, 1, 1)));
            var lastWeekEndDayExampleSecondResult7 = lastWeekEndDayExample2.Execute(lastWeekEndDayExampleSecondConfig7);
            lastWeekEndDayExampleSecondResult7.NextExecutionTime.Should().Be(new DateTime(2024, 1, 31, 14, 1, 31));
            lastWeekEndDayExampleSecondResult7.Description.Should().Be("Occurs the last weekday of very 1 month and every 13 seconds between 2:00:00 PM and 2:02:00 PM starting on 01/01/2020");

            var lastWeekEndDayExampleSecondConfig8 = new Configuration(lastWeekEndDayExampleSecondResult7.NextExecutionTime, ConfigType.Recurring, true, null, Occurs.Monthly,
                new MonthlyConfiguration(MonthlyConfigType.WeekDayOption, 1, Ordinal.Last, KindOfDay.WeekDay, 1), null,
                new DailyConfiguration(DailyConfigType.Recurring, TimeOnly.MinValue, 13, DailyFrecuency.Seconds, new TimeLimits(new TimeOnly(14, 0, 0), new TimeOnly(14, 2, 0))), new DateLimits(new DateTime(2020, 1, 1)));
            var lastWeekEndDayExampleSecondResult8 = lastWeekEndDayExample2.Execute(lastWeekEndDayExampleSecondConfig8);
            lastWeekEndDayExampleSecondResult8.NextExecutionTime.Should().Be(new DateTime(2024, 1, 31, 14, 1, 44));
            lastWeekEndDayExampleSecondResult8.Description.Should().Be("Occurs the last weekday of very 1 month and every 13 seconds between 2:00:00 PM and 2:02:00 PM starting on 01/01/2020");

            var lastWeekEndDayExampleSecondConfig9 = new Configuration(lastWeekEndDayExampleSecondResult8.NextExecutionTime, ConfigType.Recurring, true, null, Occurs.Monthly,
                new MonthlyConfiguration(MonthlyConfigType.WeekDayOption, 1, Ordinal.Last, KindOfDay.WeekDay, 1), null,
                new DailyConfiguration(DailyConfigType.Recurring, TimeOnly.MinValue, 13, DailyFrecuency.Seconds, new TimeLimits(new TimeOnly(14, 0, 0), new TimeOnly(14, 2, 0))), new DateLimits(new DateTime(2020, 1, 1)));
            var lastWeekEndDayExampleSecondResult9 = lastWeekEndDayExample2.Execute(lastWeekEndDayExampleSecondConfig9);
            lastWeekEndDayExampleSecondResult9.NextExecutionTime.Should().Be(new DateTime(2024, 1, 31, 14, 1, 57));
            lastWeekEndDayExampleSecondResult9.Description.Should().Be("Occurs the last weekday of very 1 month and every 13 seconds between 2:00:00 PM and 2:02:00 PM starting on 01/01/2020");

            var lastWeekEndDayExampleSecondConfig10 = new Configuration(lastWeekEndDayExampleSecondResult9.NextExecutionTime, ConfigType.Recurring, true, null, Occurs.Monthly,
               new MonthlyConfiguration(MonthlyConfigType.WeekDayOption, 1, Ordinal.Last, KindOfDay.WeekDay, 1), null,
               new DailyConfiguration(DailyConfigType.Recurring, TimeOnly.MinValue, 13, DailyFrecuency.Seconds, new TimeLimits(new TimeOnly(14, 0, 0), new TimeOnly(14, 2, 0))), new DateLimits(new DateTime(2020, 1, 1)));
            var lastWeekEndDayExampleSecondResult10 = lastWeekEndDayExample2.Execute(lastWeekEndDayExampleSecondConfig10);
            lastWeekEndDayExampleSecondResult10.NextExecutionTime.Should().Be(new DateTime(2024, 2, 29, 14, 0, 0));
            lastWeekEndDayExampleSecondResult10.Description.Should().Be("Occurs the last weekday of very 1 month and every 13 seconds between 2:00:00 PM and 2:02:00 PM starting on 01/01/2020");
        }
    }
}