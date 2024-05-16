using FluentAssertions;
using Microsoft.VisualStudio.TestPlatform.Utilities;
using RetoScheduler;
using RetoScheduler.Configurations;
using RetoScheduler.Configurations.Limits;
using RetoScheduler.Enums;
using RetoScheduler.Exceptions;


namespace RetoSchedulerTest
{
    public class SchedulerTest
    {
        [Theory]
        [ClassData(typeof(SchedulerTestConfiguration))]
        [ClassData(typeof(SchedulerTestMonthlyConfiguration))]
        public void Outputs_Should_Be_Expected(Configuration configuration, OutPut expectedOutput)
        {
            var scheduler = new Scheduler();
            var output = scheduler.Execute(configuration);

            output.NextExecutionTime.Should().Be(expectedOutput.NextExecutionTime);
            output.Description.Should().Be(expectedOutput.Description);
        }

        [Theory]
        [ClassData(typeof(SchedulerTestSpanishTexts))]
        [UseCulture("es-ES")]
        public void OutPuts_Should_Be_Expected_Spanish(Configuration configuration, OutPut expectedOutput)
        {
            var scheduler = new Scheduler();
            var output = scheduler.Execute(configuration);

            output.NextExecutionTime.Should().Be(expectedOutput.NextExecutionTime);
            output.Description.Should().Be(expectedOutput.Description);
        }

        [Fact]
        public void Should_Not_Execute_If_ConfigDateTime_For_OnceType_Is_Null()
        {
            var scheduler = new Scheduler();
            var configuration = new Configuration(new DateTime(2020, 1, 4), ConfigType.Once, true, null, Occurs.Daily, null, null, DailyConfiguration.Once(new TimeOnly(16, 0, 0)), new DateLimits(new DateTime(2020, 1, 1)));

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
        public void Should_Not_Execute_If_Check_Is_Disabled(ConfigType configType)
        {
            var scheduler = new Scheduler();
            var configuration = new Configuration(new DateTime(2020, 1, 4), configType, false, null, Occurs.Daily, null, null,
                DailyConfiguration.Once(new TimeOnly(16, 0, 0), null), new DateLimits(new DateTime(2020, 1, 1)));

            FluentActions
               .Invoking(() => scheduler.Execute(configuration))
               .Should()
               .Throw<SchedulerException>()
               .And.Message
               .Should().Be("You need to check field to run the Scheduler");
        }

        [Fact]
        public void Should_Not_Execute_If_Configuration_Dont_Has_DateLimits()
        {
            var scheduler = new Scheduler();
            var configuration = new Configuration(new DateTime(2020, 1, 1, 0, 0, 0), ConfigType.Recurring, true, null, Occurs.Weekly, null, new WeeklyConfiguration(2, new List<DayOfWeek>() { DayOfWeek.Monday, DayOfWeek.Thursday, DayOfWeek.Friday }),
                DailyConfiguration.Recurring(2, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(4, 0, 0), new TimeOnly(8, 0, 0))), null);

            FluentActions
               .Invoking(() => scheduler.Execute(configuration))
               .Should()
               .Throw<SchedulerException>()
               .And.Message
               .Should().Be("Limits Can`t be null");
        }

        [Fact]
        public void Should_Not_Execute_If_EndDate_Is_Before_StartDate()
        {
            var scheduler = new Scheduler();
            var configuration = new Configuration(new DateTime(2020, 1, 1, 0, 0, 0), ConfigType.Recurring, true, null, Occurs.Weekly, null, new WeeklyConfiguration(2, new List<DayOfWeek>() { DayOfWeek.Monday, DayOfWeek.Thursday, DayOfWeek.Friday }),
                DailyConfiguration.Recurring(2, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(4, 0, 0), new TimeOnly(8, 0, 0))), new DateLimits(new DateTime(2020, 1, 6), new DateTime(2020, 1, 2)));

            FluentActions
                 .Invoking(() => scheduler.Execute(configuration))
                 .Should()
                 .Throw<SchedulerException>()
                 .And.Message
                 .Should().Be("The end date cannot be earlier than the initial date");
        }

        [Fact]
        public void Should_Not_Execute_If_EndDate_Is_Before_StartDate_Spanish()
        {
            var scheduler = new Scheduler();
            var configuration = new Configuration(new DateTime(2020, 1, 1, 0, 0, 0), ConfigType.Recurring, true, null, Occurs.Weekly, null, new WeeklyConfiguration(2, new List<DayOfWeek>() { DayOfWeek.Monday, DayOfWeek.Thursday, DayOfWeek.Friday }),
                DailyConfiguration.Recurring(2, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(4, 0, 0), new TimeOnly(8, 0, 0))), new DateLimits(new DateTime(2020, 1, 6), new DateTime(2020, 1, 2)), Cultures.es_ES);

            FluentActions
                 .Invoking(() => scheduler.Execute(configuration))
                 .Should()
                 .Throw<SchedulerException>()
                 .And.Message
                 .Should().Be("La fecha de fin no puede ser anterior a la fecha de inicio");
        }

        [Theory, ClassData(typeof(SchedulerLimitsConfiguration))]
        public void Should_Not_Execute_If_DateLimits_Are_Out_Of_Range(Configuration configuration)
        {
            var scheduler = new Scheduler();
            FluentActions
                .Invoking(() => scheduler.Execute(configuration))
                .Should()
                .Throw<SchedulerException>()
                .And.Message
                .Should().Be("DateTime can't be out of start and end range field");
        }

        [Fact]
        public void Should_Not_Execute_If_EndTime_Is_Before_StartTime()
        {
            var scheduler = new Scheduler();
            var configuration = new Configuration(new DateTime(2020, 1, 1, 0, 0, 0), ConfigType.Recurring, true, null, Occurs.Weekly, null, new WeeklyConfiguration(2, new List<DayOfWeek>() { DayOfWeek.Monday, DayOfWeek.Thursday, DayOfWeek.Friday }),
                DailyConfiguration.Recurring(2, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(12, 0, 0), new TimeOnly(8, 0, 0))), new DateLimits(new DateTime(2020, 1, 6), new DateTime(2020, 1, 2)));

            FluentActions
                 .Invoking(() => scheduler.Execute(configuration))
                 .Should()
                 .Throw<SchedulerException>()
               .And.Message
               .Should().Be("The EndTime cannot be earlier than StartTime");
        }

        [Fact]
        public void Should_Not_Execute_If_EndTime_Is_Before_StartTime_Spanish()
        {
            var scheduler = new Scheduler();
            var configuration = new Configuration(new DateTime(2020, 1, 1, 0, 0, 0), ConfigType.Recurring, true, null, Occurs.Weekly, null, new WeeklyConfiguration(2, new List<DayOfWeek>() { DayOfWeek.Monday, DayOfWeek.Thursday, DayOfWeek.Friday }),
                 DailyConfiguration.Recurring(2, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(12, 0, 0), new TimeOnly(8, 0, 0))), new DateLimits(new DateTime(2020, 1, 6), new DateTime(2020, 1, 2)), Cultures.es_ES);

            FluentActions
                 .Invoking(() => scheduler.Execute(configuration))
                 .Should()
                 .Throw<SchedulerException>()
               .And.Message
               .Should().Be("La hora de fin no puede ser anterior a la hora de inicio");
        }

        [Fact]
        public void Should_Not_Execute_If_OnceAtTime_Is_Before_CurrentTime_For_OnceType()
        {
            var scheduler = new Scheduler();
            var configuration = new Configuration(new DateTime(2020, 1, 1, 9, 0, 0), ConfigType.Once, true, new DateTime(2020, 1, 1), Occurs.Daily, null, null,
                DailyConfiguration.Once(new TimeOnly(2, 0, 0), null), new DateLimits(new DateTime(2020, 1, 1), new DateTime(2020, 1, 1)));

            FluentActions
                 .Invoking(() => scheduler.Execute(configuration))
                 .Should()
                 .Throw<SchedulerException>()
                .And.Message
                 .Should().Be("The execution time cannot be earlier than the Current Time");
        }

        [Fact]
        public void Should_Not_Execute_If_Daily_Frecuency_Is_Zero_Or_Negative()
        {
            var scheduler = new Scheduler();
            var configuration = new Configuration(new DateTime(2020, 1, 1, 9, 0, 0), ConfigType.Recurring, true, new DateTime(2020, 1, 1), Occurs.Daily, null, null,
                DailyConfiguration.Recurring(-10, DailyFrecuency.Minutes, new TimeLimits(new TimeOnly(4, 0, 0), new TimeOnly(10, 0, 0))), new DateLimits(new DateTime(2019, 12, 20), new DateTime(2020, 1, 12)));

            FluentActions
                 .Invoking(() => scheduler.Execute(configuration))
                 .Should()
                 .Throw<SchedulerException>()
                 .And.Message
                 .Should().Be("Don't should put negative numbers or zero in number field");
        }

        [Fact]
        public void Should_Validate_Next_Execution_Time()
        {
            var scheduler = new Scheduler();
            var configuration = new Configuration(new DateTime(2020, 1, 4, 0, 0, 0), ConfigType.Recurring, true, null, Occurs.Daily, null, null,
                DailyConfiguration.Recurring(3, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(4, 0, 0), new TimeOnly(8, 0, 0))), new DateLimits(new DateTime(2020, 1, 1)));

            var outputList = scheduler.ExecuteMany(configuration, 2);
            outputList[0].NextExecutionTime.Should().Be(new DateTime(2020, 1, 4, 4, 0, 0));
            outputList[0].Description.Should().Be("Occurs every day and every 3 hours between 04:00:00 and 08:00:00 starting on 1/1/2020");
            outputList[1].NextExecutionTime.Should().Be(new DateTime(2020, 1, 4, 7, 0, 0));
            outputList[1].Description.Should().Be("Occurs every day and every 3 hours between 04:00:00 and 08:00:00 starting on 1/1/2020");
        }

        [Fact]
        public void Should_Validate_Execution_Time_During_Week()
        {
            var scheduler = new Scheduler();
            var configuration = new Configuration(new DateTime(2020, 1, 1), ConfigType.Recurring, true, null, Occurs.Daily, null, new WeeklyConfiguration(0, new List<DayOfWeek>() { DayOfWeek.Monday, DayOfWeek.Thursday, DayOfWeek.Friday }),
                DailyConfiguration.Recurring(2, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(4, 0, 0), new TimeOnly(8, 0, 0))), new DateLimits(new DateTime(2020, 1, 1)));

            var outputList = scheduler.ExecuteMany(configuration, 7);
            outputList[0].NextExecutionTime.Should().Be(new DateTime(2020, 1, 2, 4, 0, 0));
            outputList[1].NextExecutionTime.Should().Be(new DateTime(2020, 1, 2, 6, 0, 0));
            outputList[2].NextExecutionTime.Should().Be(new DateTime(2020, 1, 2, 8, 0, 0));
            outputList[3].NextExecutionTime.Should().Be(new DateTime(2020, 1, 3, 4, 0, 0));
            outputList[4].NextExecutionTime.Should().Be(new DateTime(2020, 1, 3, 6, 0, 0));
            outputList[5].NextExecutionTime.Should().Be(new DateTime(2020, 1, 3, 8, 0, 0));
            outputList[6].NextExecutionTime.Should().Be(new DateTime(2020, 1, 6, 4, 0, 0));
            outputList[0].Description.Should().Be("Occurs every week on monday, thursday and friday every 2 hours between 04:00:00 and 08:00:00 starting on 1/1/2020");
            outputList[1].Description.Should().Be("Occurs every week on monday, thursday and friday every 2 hours between 04:00:00 and 08:00:00 starting on 1/1/2020");
            outputList[2].Description.Should().Be("Occurs every week on monday, thursday and friday every 2 hours between 04:00:00 and 08:00:00 starting on 1/1/2020");
            outputList[3].Description.Should().Be("Occurs every week on monday, thursday and friday every 2 hours between 04:00:00 and 08:00:00 starting on 1/1/2020");
            outputList[4].Description.Should().Be("Occurs every week on monday, thursday and friday every 2 hours between 04:00:00 and 08:00:00 starting on 1/1/2020");
            outputList[5].Description.Should().Be("Occurs every week on monday, thursday and friday every 2 hours between 04:00:00 and 08:00:00 starting on 1/1/2020");
            outputList[6].Description.Should().Be("Occurs every week on monday, thursday and friday every 2 hours between 04:00:00 and 08:00:00 starting on 1/1/2020");
        }

        [Fact]
        public void Should_Validate_Execution_Time_Space_Between_Weeks_And_AddingHours()
        {
            var scheduler = new Scheduler();
            var configuration = new Configuration(new DateTime(2020, 1, 13), ConfigType.Recurring, true, null, Occurs.Daily, null, new WeeklyConfiguration(2, new List<DayOfWeek>() { DayOfWeek.Monday, DayOfWeek.Thursday, DayOfWeek.Friday }),
                DailyConfiguration.Recurring(2, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(4, 0, 0), new TimeOnly(8, 0, 0))), new DateLimits(new DateTime(2020, 1, 1)));

            var outputList = scheduler.ExecuteMany(configuration, 11);
            outputList[0].NextExecutionTime.Should().Be(new DateTime(2020, 1, 13, 4, 0, 0));
            outputList[1].NextExecutionTime.Should().Be(new DateTime(2020, 1, 13, 6, 0, 0));
            outputList[2].NextExecutionTime.Should().Be(new DateTime(2020, 1, 13, 8, 0, 0));
            outputList[3].NextExecutionTime.Should().Be(new DateTime(2020, 1, 16, 4, 0, 0));
            outputList[4].NextExecutionTime.Should().Be(new DateTime(2020, 1, 16, 6, 0, 0));
            outputList[5].NextExecutionTime.Should().Be(new DateTime(2020, 1, 16, 8, 0, 0));
            outputList[6].NextExecutionTime.Should().Be(new DateTime(2020, 1, 17, 4, 0, 0));
            outputList[7].NextExecutionTime.Should().Be(new DateTime(2020, 1, 17, 6, 0, 0));
            outputList[8].NextExecutionTime.Should().Be(new DateTime(2020, 1, 17, 8, 0, 0));
            outputList[9].NextExecutionTime.Should().Be(new DateTime(2020, 2, 3, 4, 0, 0));
            outputList[10].NextExecutionTime.Should().Be(new DateTime(2020, 2, 3, 6, 0, 0));
            outputList[0].Description.Should().Be("Occurs every 2 weeks on monday, thursday and friday every 2 hours between 04:00:00 and 08:00:00 starting on 1/1/2020");
            outputList[1].Description.Should().Be("Occurs every 2 weeks on monday, thursday and friday every 2 hours between 04:00:00 and 08:00:00 starting on 1/1/2020");
            outputList[2].Description.Should().Be("Occurs every 2 weeks on monday, thursday and friday every 2 hours between 04:00:00 and 08:00:00 starting on 1/1/2020");
            outputList[3].Description.Should().Be("Occurs every 2 weeks on monday, thursday and friday every 2 hours between 04:00:00 and 08:00:00 starting on 1/1/2020");
            outputList[4].Description.Should().Be("Occurs every 2 weeks on monday, thursday and friday every 2 hours between 04:00:00 and 08:00:00 starting on 1/1/2020");
            outputList[5].Description.Should().Be("Occurs every 2 weeks on monday, thursday and friday every 2 hours between 04:00:00 and 08:00:00 starting on 1/1/2020");
            outputList[6].Description.Should().Be("Occurs every 2 weeks on monday, thursday and friday every 2 hours between 04:00:00 and 08:00:00 starting on 1/1/2020");
            outputList[7].Description.Should().Be("Occurs every 2 weeks on monday, thursday and friday every 2 hours between 04:00:00 and 08:00:00 starting on 1/1/2020");
            outputList[8].Description.Should().Be("Occurs every 2 weeks on monday, thursday and friday every 2 hours between 04:00:00 and 08:00:00 starting on 1/1/2020");
            outputList[9].Description.Should().Be("Occurs every 2 weeks on monday, thursday and friday every 2 hours between 04:00:00 and 08:00:00 starting on 1/1/2020");
            outputList[10].Description.Should().Be("Occurs every 2 weeks on monday, thursday and friday every 2 hours between 04:00:00 and 08:00:00 starting on 1/1/2020");
        }

        [Fact]
        public void Should_Validate_Execution_Time_Space_Between_Weeks_And_AddingMinutes()
        {
            var scheduler = new Scheduler();
            var configuration = new Configuration(new DateTime(2020, 1, 13), ConfigType.Recurring, true, null, Occurs.Daily, null, new WeeklyConfiguration(2, new List<DayOfWeek>() { DayOfWeek.Monday, DayOfWeek.Thursday, DayOfWeek.Friday }),
                DailyConfiguration.Recurring(2, DailyFrecuency.Minutes, new TimeLimits(new TimeOnly(4, 0, 0), new TimeOnly(8, 0, 0))), new DateLimits(new DateTime(2020, 1, 1)));

            var outputList = scheduler.ExecuteMany(configuration, 11);
            outputList[0].NextExecutionTime.Should().Be(new DateTime(2020, 1, 13, 4, 0, 0));
            outputList[1].NextExecutionTime.Should().Be(new DateTime(2020, 1, 13, 4, 2, 0));
            outputList[2].NextExecutionTime.Should().Be(new DateTime(2020, 1, 13, 4, 4, 0));
            outputList[3].NextExecutionTime.Should().Be(new DateTime(2020, 1, 13, 4, 6, 0));
            outputList[4].NextExecutionTime.Should().Be(new DateTime(2020, 1, 13, 4, 8, 0));
            outputList[5].NextExecutionTime.Should().Be(new DateTime(2020, 1, 13, 4, 10, 0));
            outputList[6].NextExecutionTime.Should().Be(new DateTime(2020, 1, 13, 4, 12, 0));
            outputList[7].NextExecutionTime.Should().Be(new DateTime(2020, 1, 13, 4, 14, 0));
            outputList[8].NextExecutionTime.Should().Be(new DateTime(2020, 1, 13, 4, 16, 0));
            outputList[9].NextExecutionTime.Should().Be(new DateTime(2020, 1, 13, 4, 18, 0));
            outputList[10].NextExecutionTime.Should().Be(new DateTime(2020, 1, 13, 4, 20, 0));
            outputList[0].Description.Should().Be("Occurs every 2 weeks on monday, thursday and friday every 2 minutes between 04:00:00 and 08:00:00 starting on 1/1/2020");
            outputList[1].Description.Should().Be("Occurs every 2 weeks on monday, thursday and friday every 2 minutes between 04:00:00 and 08:00:00 starting on 1/1/2020");
            outputList[2].Description.Should().Be("Occurs every 2 weeks on monday, thursday and friday every 2 minutes between 04:00:00 and 08:00:00 starting on 1/1/2020");
            outputList[3].Description.Should().Be("Occurs every 2 weeks on monday, thursday and friday every 2 minutes between 04:00:00 and 08:00:00 starting on 1/1/2020");
            outputList[4].Description.Should().Be("Occurs every 2 weeks on monday, thursday and friday every 2 minutes between 04:00:00 and 08:00:00 starting on 1/1/2020");
            outputList[5].Description.Should().Be("Occurs every 2 weeks on monday, thursday and friday every 2 minutes between 04:00:00 and 08:00:00 starting on 1/1/2020");
            outputList[6].Description.Should().Be("Occurs every 2 weeks on monday, thursday and friday every 2 minutes between 04:00:00 and 08:00:00 starting on 1/1/2020");
            outputList[7].Description.Should().Be("Occurs every 2 weeks on monday, thursday and friday every 2 minutes between 04:00:00 and 08:00:00 starting on 1/1/2020");
            outputList[8].Description.Should().Be("Occurs every 2 weeks on monday, thursday and friday every 2 minutes between 04:00:00 and 08:00:00 starting on 1/1/2020");
            outputList[9].Description.Should().Be("Occurs every 2 weeks on monday, thursday and friday every 2 minutes between 04:00:00 and 08:00:00 starting on 1/1/2020");
            outputList[10].Description.Should().Be("Occurs every 2 weeks on monday, thursday and friday every 2 minutes between 04:00:00 and 08:00:00 starting on 1/1/2020");
        }

        [Fact]
        public void Should_Validate_Execution_Time_Space_Between_Weeks_And_AddingSeconds()
        {
            var scheduler = new Scheduler();
            var configuration = new Configuration(new DateTime(2020, 1, 13), ConfigType.Recurring, true, null, Occurs.Daily, null, new WeeklyConfiguration(2, new List<DayOfWeek>() { DayOfWeek.Monday, DayOfWeek.Thursday, DayOfWeek.Friday }),
                DailyConfiguration.Recurring(5, DailyFrecuency.Seconds, new TimeLimits(new TimeOnly(4, 0, 50), new TimeOnly(8, 0, 0))), new DateLimits(new DateTime(2020, 1, 1)));

            var outputList = scheduler.ExecuteMany(configuration, 11);
            outputList[0].NextExecutionTime.Should().Be(new DateTime(2020, 1, 13, 4, 0, 50));
            outputList[1].NextExecutionTime.Should().Be(new DateTime(2020, 1, 13, 4, 0, 55));
            outputList[2].NextExecutionTime.Should().Be(new DateTime(2020, 1, 13, 4, 1, 00));
            outputList[3].NextExecutionTime.Should().Be(new DateTime(2020, 1, 13, 4, 1, 5));
            outputList[4].NextExecutionTime.Should().Be(new DateTime(2020, 1, 13, 4, 1, 10));
            outputList[5].NextExecutionTime.Should().Be(new DateTime(2020, 1, 13, 4, 1, 15));
            outputList[6].NextExecutionTime.Should().Be(new DateTime(2020, 1, 13, 4, 1, 20));
            outputList[7].NextExecutionTime.Should().Be(new DateTime(2020, 1, 13, 4, 1, 25));
            outputList[8].NextExecutionTime.Should().Be(new DateTime(2020, 1, 13, 4, 1, 30));
            outputList[9].NextExecutionTime.Should().Be(new DateTime(2020, 1, 13, 4, 1, 35));
            outputList[10].NextExecutionTime.Should().Be(new DateTime(2020, 1, 13, 4, 1, 40));
            outputList[0].Description.Should().Be("Occurs every 2 weeks on monday, thursday and friday every 5 seconds between 04:00:50 and 08:00:00 starting on 1/1/2020");
            outputList[1].Description.Should().Be("Occurs every 2 weeks on monday, thursday and friday every 5 seconds between 04:00:50 and 08:00:00 starting on 1/1/2020");
            outputList[2].Description.Should().Be("Occurs every 2 weeks on monday, thursday and friday every 5 seconds between 04:00:50 and 08:00:00 starting on 1/1/2020");
            outputList[3].Description.Should().Be("Occurs every 2 weeks on monday, thursday and friday every 5 seconds between 04:00:50 and 08:00:00 starting on 1/1/2020");
            outputList[4].Description.Should().Be("Occurs every 2 weeks on monday, thursday and friday every 5 seconds between 04:00:50 and 08:00:00 starting on 1/1/2020");
            outputList[5].Description.Should().Be("Occurs every 2 weeks on monday, thursday and friday every 5 seconds between 04:00:50 and 08:00:00 starting on 1/1/2020");
            outputList[6].Description.Should().Be("Occurs every 2 weeks on monday, thursday and friday every 5 seconds between 04:00:50 and 08:00:00 starting on 1/1/2020");
            outputList[7].Description.Should().Be("Occurs every 2 weeks on monday, thursday and friday every 5 seconds between 04:00:50 and 08:00:00 starting on 1/1/2020");
            outputList[8].Description.Should().Be("Occurs every 2 weeks on monday, thursday and friday every 5 seconds between 04:00:50 and 08:00:00 starting on 1/1/2020");
            outputList[9].Description.Should().Be("Occurs every 2 weeks on monday, thursday and friday every 5 seconds between 04:00:50 and 08:00:00 starting on 1/1/2020");
            outputList[10].Description.Should().Be("Occurs every 2 weeks on monday, thursday and friday every 5 seconds between 04:00:50 and 08:00:00 starting on 1/1/2020");
        }

        [Fact]
        public void Should_Validate_Execution_Time_BetweenWeeks_With_DailyFrecuency_OnceAt()
        {
            var scheduler = new Scheduler();
            var configuration = new Configuration(new DateTime(2020, 1, 13), ConfigType.Recurring, true, null, Occurs.Weekly, null, new WeeklyConfiguration(2, new List<DayOfWeek>() { DayOfWeek.Monday, DayOfWeek.Thursday, DayOfWeek.Friday }),
                DailyConfiguration.Once(new TimeOnly(5, 0, 0), null), new DateLimits(new DateTime(2020, 1, 1)));

            var outputList = scheduler.ExecuteMany(configuration, 5);
            outputList[0].NextExecutionTime.Should().Be(new DateTime(2020, 1, 13, 5, 0, 0));
            outputList[1].NextExecutionTime.Should().Be(new DateTime(2020, 1, 16, 5, 0, 0));
            outputList[2].NextExecutionTime.Should().Be(new DateTime(2020, 1, 17, 5, 0, 0));
            outputList[3].NextExecutionTime.Should().Be(new DateTime(2020, 2, 3, 5, 0, 0));
            outputList[4].NextExecutionTime.Should().Be(new DateTime(2020, 2, 6, 5, 0, 0));
            outputList[0].Description.Should().Be("Occurs every 2 weeks on monday, thursday and friday one time at 05:00:00 starting on 1/1/2020");
            outputList[1].Description.Should().Be("Occurs every 2 weeks on monday, thursday and friday one time at 05:00:00 starting on 1/1/2020");
            outputList[2].Description.Should().Be("Occurs every 2 weeks on monday, thursday and friday one time at 05:00:00 starting on 1/1/2020");
            outputList[3].Description.Should().Be("Occurs every 2 weeks on monday, thursday and friday one time at 05:00:00 starting on 1/1/2020");
            outputList[4].Description.Should().Be("Occurs every 2 weeks on monday, thursday and friday one time at 05:00:00 starting on 1/1/2020");
        }

        [Fact]
        public void Should_Be_Next_Execution_Date_For_Month_DayOption()
        {
            var scheduler = new Scheduler();
            var configuration = new Configuration(new DateTime(2020, 4, 1, 4, 18, 0), ConfigType.Recurring, true, null, Occurs.Monthly, MonthlyConfiguration.DayOption(8, 1), null,
                DailyConfiguration.Recurring(1, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(3, 0, 0), new TimeOnly(6, 0, 0))), new DateLimits(new DateTime(2020, 1, 1)));

            var output = scheduler.Execute(configuration);
            output.NextExecutionTime.Should().Be(new DateTime(2020, 4, 8, 3, 0, 0));
            output.Description.Should().Be("Occurs the 8th of very 1 month and every 1 hours between 03:00:00 and 06:00:00 starting on 1/1/2020");
        }

        [Fact]
        public void Should_Be_Next_Executions_Date_For_Month_DayOption_Skipping_Months_SchedulerExample()
        {
            var scheduler = new Scheduler();
            var configuration = new Configuration(new DateTime(2020, 4, 1, 4, 18, 0), ConfigType.Recurring, true, null, Occurs.Monthly, MonthlyConfiguration.DayOption(8, 3), null,
                    DailyConfiguration.Recurring(6, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(3, 0, 0), new TimeOnly(6, 0, 0))), new DateLimits(new DateTime(2020, 1, 1)));

            var outputList = scheduler.ExecuteMany(configuration, 4);
            outputList[0].NextExecutionTime.Should().Be(new DateTime(2020, 4, 8, 3, 0, 0));
            outputList[0].Description.Should().Be("Occurs the 8th of very 3 months and every 6 hours between 03:00:00 and 06:00:00 starting on 1/1/2020");
            outputList[1].NextExecutionTime.Should().Be(new DateTime(2020, 7, 8, 3, 0, 0));
            outputList[1].Description.Should().Be("Occurs the 8th of very 3 months and every 6 hours between 03:00:00 and 06:00:00 starting on 1/1/2020");
            //outputList[2].NextExecutionTime.Should().Be(new DateTime(2020, 10, 8, 3, 0, 0));
            //outputList[2].Description.Should().Be("Occurs the 8th of very 3 months and every 6 hours between 03:00:00 and 06:00:00 starting on 1/1/2020");
            //outputList[3].NextExecutionTime.Should().Be(new DateTime(2021, 1, 8, 3, 0, 0));
            //outputList[3].Description.Should().Be("Occurs the 8th of very 3 months and every 6 hours between 03:00:00 and 06:00:00 starting on 1/1/2020");
        }

        [Fact]
        public void Should_Be_Next_Executions_Date_For_Month_DayOption_Skipping_0_Months()
        {
            var scheduler = new Scheduler();
            var configuration = new Configuration(new DateTime(2020, 1, 4, 0, 0, 0), ConfigType.Recurring, true, null, Occurs.Monthly, MonthlyConfiguration.DayOption(8, 1), null,
                DailyConfiguration.Recurring(6, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(3, 0, 0), new TimeOnly(6, 0, 0))), new DateLimits(new DateTime(2020, 1, 1)));

            var outputList = scheduler.ExecuteMany(configuration, 4);
            outputList[0].NextExecutionTime.Should().Be(new DateTime(2020, 1, 8, 3, 0, 0));
            outputList[0].Description.Should().Be("Occurs the 8th of very 1 month and every 6 hours between 03:00:00 and 06:00:00 starting on 1/1/2020");
            outputList[1].NextExecutionTime.Should().Be(new DateTime(2020, 2, 8, 3, 0, 0));
            outputList[1].Description.Should().Be("Occurs the 8th of very 1 month and every 6 hours between 03:00:00 and 06:00:00 starting on 1/1/2020");
            outputList[2].NextExecutionTime.Should().Be(new DateTime(2020, 3, 8, 3, 0, 0));
            outputList[2].Description.Should().Be("Occurs the 8th of very 1 month and every 6 hours between 03:00:00 and 06:00:00 starting on 1/1/2020");
            outputList[3].NextExecutionTime.Should().Be(new DateTime(2020, 4, 8, 3, 0, 0));
            outputList[3].Description.Should().Be("Occurs the 8th of very 1 month and every 6 hours between 03:00:00 and 06:00:00 starting on 1/1/2020");
        }

        [Fact]
        public void Shoud_Be_Next_Executions_For_Month_DayOption30th_Skipping_Months_And_Dates_That_Doesnt_Exist()
        {
            var scheduler = new Scheduler();
            var configuration = new Configuration(new DateTime(2023, 12, 31, 4, 18, 0), ConfigType.Recurring, true, null, Occurs.Monthly, MonthlyConfiguration.DayOption(30, 1), null,
                DailyConfiguration.Recurring(6, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(4, 0, 0), new TimeOnly(6, 0, 0))), new DateLimits(new DateTime(2020, 1, 1)));

            var outputList = scheduler.ExecuteMany(configuration, 4);
            outputList[0].NextExecutionTime.Should().Be(new DateTime(2024, 1, 30, 4, 0, 0));
            outputList[0].Description.Should().Be("Occurs the 30th of very 1 month and every 6 hours between 04:00:00 and 06:00:00 starting on 1/1/2020");
            outputList[1].NextExecutionTime.Should().Be(new DateTime(2024, 2, 29, 4, 0, 0));
            outputList[1].Description.Should().Be("Occurs the 30th of very 1 month and every 6 hours between 04:00:00 and 06:00:00 starting on 1/1/2020");
            outputList[2].NextExecutionTime.Should().Be(new DateTime(2024, 3, 30, 4, 0, 0));
            outputList[2].Description.Should().Be("Occurs the 30th of very 1 month and every 6 hours between 04:00:00 and 06:00:00 starting on 1/1/2020");
            outputList[3].NextExecutionTime.Should().Be(new DateTime(2024, 4, 30, 4, 0, 0));
            outputList[3].Description.Should().Be("Occurs the 30th of very 1 month and every 6 hours between 04:00:00 and 06:00:00 starting on 1/1/2020");
        }

        [Fact]
        public void Shoud_Be_Next_Executions_For_Month_DayOption31st_Skipping_Months_And_Dates_That_Doesnt_Exist()
        {
            var scheduler = new Scheduler();
            var configuration = new Configuration(new DateTime(2023, 12, 31, 4, 18, 0), ConfigType.Recurring, true, null, Occurs.Monthly, MonthlyConfiguration.DayOption(31, 1), null,
                DailyConfiguration.Recurring(6, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(3, 0, 0), new TimeOnly(6, 0, 0))), new DateLimits(new DateTime(2020, 1, 1)));

            var outputList = scheduler.ExecuteMany(configuration, 4);
            outputList[0].NextExecutionTime.Should().Be(new DateTime(2023, 12, 31, 4, 18, 0));
            outputList[0].Description.Should().Be("Occurs the 31st of very 1 month and every 6 hours between 03:00:00 and 06:00:00 starting on 1/1/2020");
            outputList[1].NextExecutionTime.Should().Be(new DateTime(2024, 1, 31, 3, 0, 0));
            outputList[1].Description.Should().Be("Occurs the 31st of very 1 month and every 6 hours between 03:00:00 and 06:00:00 starting on 1/1/2020");
            outputList[2].NextExecutionTime.Should().Be(new DateTime(2024, 2, 29, 3, 0, 0));
            outputList[2].Description.Should().Be("Occurs the 31st of very 1 month and every 6 hours between 03:00:00 and 06:00:00 starting on 1/1/2020");
            outputList[3].NextExecutionTime.Should().Be(new DateTime(2024, 3, 31, 3, 0, 0));
            outputList[3].Description.Should().Be("Occurs the 31st of very 1 month and every 6 hours between 03:00:00 and 06:00:00 starting on 1/1/2020");
        }

        [Fact]
        public void Should_Be_Next_Execution_For_Month_WeekDayOption_Last_Thursday()
        {
            var scheduler = new Scheduler();
            var configuration = new Configuration(new DateTime(2020, 1, 1), ConfigType.Recurring, true, null, Occurs.Monthly, MonthlyConfiguration.WeekDayOption(Ordinal.Last, KindOfDay.Thursday, 3), null,
                DailyConfiguration.Recurring(1, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(3, 0, 0), new TimeOnly(6, 0, 0))), new DateLimits(new DateTime(2020, 1, 1)));

            var output = scheduler.Execute(configuration);
            output.NextExecutionTime.Should().Be(new DateTime(2020, 1, 30, 3, 0, 0));
            output.Description.Should().Be("Occurs the last thursday of very 3 months and every 1 hours between 03:00:00 and 06:00:00 starting on 1/1/2020");
        }

        [Fact]
        public void Should_Be_Next_Execution_For_Month_WeekDayOption_Fourth_Thursday()
        {
            var scheduler = new Scheduler();
            var configuration = new Configuration(new DateTime(2020, 1, 1), ConfigType.Recurring, true, null, Occurs.Monthly, MonthlyConfiguration.WeekDayOption(Ordinal.Fourth, KindOfDay.Thursday, 3), null,
                DailyConfiguration.Recurring(1, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(3, 0, 0), new TimeOnly(6, 0, 0))), new DateLimits(new DateTime(2020, 1, 1)));

            var output = scheduler.Execute(configuration);
            output.NextExecutionTime.Should().Be(new DateTime(2020, 1, 23, 3, 0, 0));
            output.Description.Should().Be("Occurs the fourth thursday of very 3 months and every 1 hours between 03:00:00 and 06:00:00 starting on 1/1/2020");
        }

        [Fact]
        public void Should_Be_Next_Execution_For_Month_WeekDayOption_Third_Thursday()
        {
            var scheduler = new Scheduler();
            var configuration = new Configuration(new DateTime(2020, 1, 1), ConfigType.Recurring, true, null, Occurs.Monthly, MonthlyConfiguration.WeekDayOption(Ordinal.Third, KindOfDay.Thursday, 3), null,
                DailyConfiguration.Recurring(1, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(3, 0, 0), new TimeOnly(6, 0, 0))), new DateLimits(new DateTime(2020, 1, 1)));

            var output = scheduler.Execute(configuration);
            output.NextExecutionTime.Should().Be(new DateTime(2020, 1, 16, 3, 0, 0));
            output.Description.Should().Be("Occurs the third thursday of very 3 months and every 1 hours between 03:00:00 and 06:00:00 starting on 1/1/2020");
        }

        [Fact]
        public void Should_Be_Next_Execution_For_Month_WeekDayOption_Second_Thursday()
        {
            var scheduler = new Scheduler();
            var configuration = new Configuration(new DateTime(2020, 1, 1), ConfigType.Recurring, true, null, Occurs.Monthly, MonthlyConfiguration.WeekDayOption(Ordinal.Second, KindOfDay.Thursday, 3), null,
                DailyConfiguration.Recurring(1, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(3, 0, 0), new TimeOnly(6, 0, 0))), new DateLimits(new DateTime(2020, 1, 1)));

            var output = scheduler.Execute(configuration);
            output.NextExecutionTime.Should().Be(new DateTime(2020, 1, 9, 3, 0, 0));
            output.Description.Should().Be("Occurs the second thursday of very 3 months and every 1 hours between 03:00:00 and 06:00:00 starting on 1/1/2020");
        }

        [Fact]
        public void Should_Be_Next_Execution_For_Month_WeekDayOption_First_Thursday()
        {
            var scheduler = new Scheduler();
            var configuration = new Configuration(new DateTime(2020, 1, 1), ConfigType.Recurring, true, null, Occurs.Monthly, MonthlyConfiguration.WeekDayOption(Ordinal.First, KindOfDay.Thursday, 3), null,
                DailyConfiguration.Recurring(1, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(3, 0, 0), new TimeOnly(6, 0, 0))), new DateLimits(new DateTime(2020, 1, 1)));

            var output = scheduler.Execute(configuration);
            output.NextExecutionTime.Should().Be(new DateTime(2020, 1, 2, 3, 0, 0));
            output.Description.Should().Be("Occurs the first thursday of very 3 months and every 1 hours between 03:00:00 and 06:00:00 starting on 1/1/2020");
        }

        [Fact]
        public void Should_Be_Next_Execution_For_Month_WeekDayOption_First_Weekday()
        {
            var scheduler = new Scheduler();
            var configuration = new Configuration(new DateTime(2020, 1, 1), ConfigType.Recurring, true, null, Occurs.Monthly, MonthlyConfiguration.WeekDayOption(Ordinal.First, KindOfDay.WeekDay, 3), null,
                DailyConfiguration.Recurring(1, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(3, 0, 0), new TimeOnly(6, 0, 0))), new DateLimits(new DateTime(2020, 1, 1)));

            var output = scheduler.Execute(configuration);
            output.NextExecutionTime.Should().Be(new DateTime(2020, 1, 1, 3, 0, 0));
            output.Description.Should().Be("Occurs the first weekday of very 3 months and every 1 hours between 03:00:00 and 06:00:00 starting on 1/1/2020");
        }

        [Fact]
        public void Should_Be_Next_Execution_For_Month_WeekDayOption_Second_Weekday()
        {
            var scheduler = new Scheduler();
            var configuration = new Configuration(new DateTime(2020, 1, 1), ConfigType.Recurring, true, null, Occurs.Monthly, MonthlyConfiguration.WeekDayOption(Ordinal.Second, KindOfDay.WeekDay, 3), null,
                DailyConfiguration.Recurring(1, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(3, 0, 0), new TimeOnly(6, 0, 0))), new DateLimits(new DateTime(2020, 1, 1)));

            var output = scheduler.Execute(configuration);
            output.NextExecutionTime.Should().Be(new DateTime(2020, 1, 2, 3, 0, 0));
            output.Description.Should().Be("Occurs the second weekday of very 3 months and every 1 hours between 03:00:00 and 06:00:00 starting on 1/1/2020");
        }

        [Fact]
        public void Should_Be_Next_Execution_For_Month_WeekDayOption_Third_Weekdays()
        {
            var scheduler = new Scheduler();
            var configuration = new Configuration(new DateTime(2020, 1, 1), ConfigType.Recurring, true, null, Occurs.Monthly, MonthlyConfiguration.WeekDayOption(Ordinal.Third, KindOfDay.WeekDay, 3), null,
                DailyConfiguration.Recurring(1, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(3, 0, 0), new TimeOnly(6, 0, 0))), new DateLimits(new DateTime(2020, 1, 1)));

            var output = scheduler.Execute(configuration);
            output.NextExecutionTime.Should().Be(new DateTime(2020, 1, 3, 3, 0, 0));
            output.Description.Should().Be("Occurs the third weekday of very 3 months and every 1 hours between 03:00:00 and 06:00:00 starting on 1/1/2020");
        }

        [Fact]
        public void Should_Be_Next_Execution_For_Month_WeekDayOption_Fourth_Weekdays()
        {
            var scheduler = new Scheduler();
            var configuration = new Configuration(new DateTime(2020, 1, 1), ConfigType.Recurring, true, null, Occurs.Monthly, MonthlyConfiguration.WeekDayOption(Ordinal.Fourth, KindOfDay.WeekDay, 3), null,
                DailyConfiguration.Recurring(1, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(3, 0, 0), new TimeOnly(6, 0, 0))), new DateLimits(new DateTime(2020, 1, 1)));

            var output = scheduler.Execute(configuration);
            output.NextExecutionTime.Should().Be(new DateTime(2020, 1, 6, 3, 0, 0));
            output.Description.Should().Be("Occurs the fourth weekday of very 3 months and every 1 hours between 03:00:00 and 06:00:00 starting on 1/1/2020");
        }

        [Fact]
        public void Should_Be_Next_Execution_For_Month_WeekDayOption_First_WeekEndDay()
        {
            var scheduler = new Scheduler();
            var configuration = new Configuration(new DateTime(2020, 1, 1), ConfigType.Recurring, true, null, Occurs.Monthly, MonthlyConfiguration.WeekDayOption(Ordinal.First, KindOfDay.WeekEndDay, 3), null,
                DailyConfiguration.Recurring(1, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(3, 0, 0), new TimeOnly(6, 0, 0))), new DateLimits(new DateTime(2020, 1, 1)));

            var output = scheduler.Execute(configuration);
            output.NextExecutionTime.Should().Be(new DateTime(2020, 1, 4, 3, 0, 0));
            output.Description.Should().Be("Occurs the first weekendday of very 3 months and every 1 hours between 03:00:00 and 06:00:00 starting on 1/1/2020");
        }

        [Fact]
        public void Should_Be_Next_Execution_For_Month_WeekDayOption_Second_WeekEndDay()
        {
            var scheduler = new Scheduler();
            var configuration = new Configuration(new DateTime(2020, 1, 1), ConfigType.Recurring, true, null, Occurs.Monthly, MonthlyConfiguration.WeekDayOption(Ordinal.Second, KindOfDay.WeekEndDay, 3), null,
                DailyConfiguration.Recurring(1, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(3, 0, 0), new TimeOnly(6, 0, 0))), new DateLimits(new DateTime(2020, 1, 1)));

            var output = scheduler.Execute(configuration);
            output.NextExecutionTime.Should().Be(new DateTime(2020, 1, 5, 3, 0, 0));
            output.Description.Should().Be("Occurs the second weekendday of very 3 months and every 1 hours between 03:00:00 and 06:00:00 starting on 1/1/2020");
        }

        [Fact]
        public void Should_Be_Next_Execution_For_Month_WeekDayOption_Third_WeekEndDay()
        {
            var scheduler = new Scheduler();
            var configuration = new Configuration(new DateTime(2020, 1, 1), ConfigType.Recurring, true, null, Occurs.Monthly, MonthlyConfiguration.WeekDayOption(Ordinal.Third, KindOfDay.WeekEndDay, 3), null,
                DailyConfiguration.Recurring(1, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(3, 0, 0), new TimeOnly(6, 0, 0))), new DateLimits(new DateTime(2020, 1, 1)));

            var output = scheduler.Execute(configuration);
            output.NextExecutionTime.Should().Be(new DateTime(2020, 1, 11, 3, 0, 0));
            output.Description.Should().Be("Occurs the third weekendday of very 3 months and every 1 hours between 03:00:00 and 06:00:00 starting on 1/1/2020");
        }

        [Fact]
        public void Should_Be_Next_Execution_For_Month_WeekDayOption_Fourth_WeekEndDay()
        {
            var scheduler = new Scheduler();
            var configuration = new Configuration(new DateTime(2020, 1, 1), ConfigType.Recurring, true, null, Occurs.Monthly, MonthlyConfiguration.WeekDayOption(Ordinal.Fourth, KindOfDay.WeekEndDay, 3), null,
                DailyConfiguration.Recurring(1, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(3, 0, 0), new TimeOnly(6, 0, 0))), new DateLimits(new DateTime(2020, 1, 1)));

            var output = scheduler.Execute(configuration);
            output.NextExecutionTime.Should().Be(new DateTime(2020, 1, 12, 3, 0, 0));
            output.Description.Should().Be("Occurs the fourth weekendday of very 3 months and every 1 hours between 03:00:00 and 06:00:00 starting on 1/1/2020");
        }

        [Fact]
        public void Should_Be_Next_Executions_For_Month_WeekDayOption_First_Thursday_Skipping_3_Months_Scheduler_Example()
        {
            var scheduler = new Scheduler();
            var configuration = new Configuration(new DateTime(2020, 1, 1), ConfigType.Recurring, true, null, Occurs.Monthly, MonthlyConfiguration.WeekDayOption(Ordinal.First, KindOfDay.Thursday, 3), null,
                DailyConfiguration.Recurring(6, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(3, 0, 0), new TimeOnly(6, 0, 0))), new DateLimits(new DateTime(2020, 1, 1)));

            var outputList = scheduler.ExecuteMany(configuration, 3);
            outputList[0].NextExecutionTime.Should().Be(new DateTime(2020, 1, 2, 3, 0, 0));
            outputList[0].Description.Should().Be("Occurs the first thursday of very 3 months and every 6 hours between 03:00:00 and 06:00:00 starting on 1/1/2020");
            outputList[1].NextExecutionTime.Should().Be(new DateTime(2020, 4, 2, 3, 0, 0));
            outputList[1].Description.Should().Be("Occurs the first thursday of very 3 months and every 6 hours between 03:00:00 and 06:00:00 starting on 1/1/2020");
            outputList[2].NextExecutionTime.Should().Be(new DateTime(2020, 7, 2, 3, 0, 0));
            outputList[2].Description.Should().Be("Occurs the first thursday of very 3 months and every 6 hours between 03:00:00 and 06:00:00 starting on 1/1/2020");
        }

        [Fact]
        public void Should_Be_Next_Executions_For_Month_WeekDayOption_First_Thursday_Skipping_3_Months()
        {
            var scheduler = new Scheduler();
            var configuration = new Configuration(new DateTime(2024, 1, 1), ConfigType.Recurring, true, null, Occurs.Monthly, MonthlyConfiguration.WeekDayOption(Ordinal.First, KindOfDay.Thursday, 3), null,
                DailyConfiguration.Recurring(6, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(3, 30, 0), new TimeOnly(6, 0, 0))), new DateLimits(new DateTime(2020, 1, 1)));

            var outputList = scheduler.ExecuteMany(configuration, 3);
            outputList[0].NextExecutionTime.Should().Be(new DateTime(2024, 1, 4, 3, 30, 0));
            outputList[0].Description.Should().Be("Occurs the first thursday of very 3 months and every 6 hours between 03:30:00 and 06:00:00 starting on 1/1/2020");
            outputList[1].NextExecutionTime.Should().Be(new DateTime(2024, 4, 4, 3, 30, 0));
            outputList[1].Description.Should().Be("Occurs the first thursday of very 3 months and every 6 hours between 03:30:00 and 06:00:00 starting on 1/1/2020");
            outputList[2].NextExecutionTime.Should().Be(new DateTime(2024, 7, 4, 3, 30, 0));
            outputList[2].Description.Should().Be("Occurs the first thursday of very 3 months and every 6 hours between 03:30:00 and 06:00:00 starting on 1/1/2020");
        }

        [Fact]
        public void Should_Be_Next_Executions_For_Month_WeekDayOption_Second_Thursday_Skipping_2_Months()
        {
            var scheduler = new Scheduler();
            var configuration = new Configuration(new DateTime(2024, 1, 1), ConfigType.Recurring, true, null, Occurs.Monthly, MonthlyConfiguration.WeekDayOption(Ordinal.Second, KindOfDay.Thursday, 2), null,
                DailyConfiguration.Recurring(6, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(3, 30, 0), new TimeOnly(6, 0, 0))), new DateLimits(new DateTime(2020, 1, 1)));

            var outputList = scheduler.ExecuteMany(configuration, 3);
            outputList[0].NextExecutionTime.Should().Be(new DateTime(2024, 1, 11, 3, 30, 0));
            outputList[0].Description.Should().Be("Occurs the second thursday of very 2 months and every 6 hours between 03:30:00 and 06:00:00 starting on 1/1/2020");
            outputList[1].NextExecutionTime.Should().Be(new DateTime(2024, 3, 14, 3, 30, 0));
            outputList[1].Description.Should().Be("Occurs the second thursday of very 2 months and every 6 hours between 03:30:00 and 06:00:00 starting on 1/1/2020");
            outputList[2].NextExecutionTime.Should().Be(new DateTime(2024, 5, 9, 3, 30, 0));
            outputList[2].Description.Should().Be("Occurs the second thursday of very 2 months and every 6 hours between 03:30:00 and 06:00:00 starting on 1/1/2020");
        }

        [Fact]
        public void Should_Be_Next_Executions_For_Month_WeekDayOption_First_Weekday_Skipping_2_Months()
        {
            var scheduler = new Scheduler();
            var configuration = new Configuration(new DateTime(2024, 4, 5), ConfigType.Recurring, true, null, Occurs.Monthly, MonthlyConfiguration.WeekDayOption(Ordinal.First, KindOfDay.WeekDay, 2), null,
                DailyConfiguration.Recurring(6, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(5, 0, 0), new TimeOnly(6, 0, 0))), new DateLimits(new DateTime(2020, 1, 1)));

            var outputList = scheduler.ExecuteMany(configuration, 3);
            outputList[0].NextExecutionTime.Should().Be(new DateTime(2024, 5, 1, 5, 0, 0));
            outputList[0].Description.Should().Be("Occurs the first weekday of very 2 months and every 6 hours between 05:00:00 and 06:00:00 starting on 1/1/2020");
            outputList[1].NextExecutionTime.Should().Be(new DateTime(2024, 7, 1, 5, 0, 0));
            outputList[1].Description.Should().Be("Occurs the first weekday of very 2 months and every 6 hours between 05:00:00 and 06:00:00 starting on 1/1/2020");
            outputList[2].NextExecutionTime.Should().Be(new DateTime(2024, 9, 2, 5, 0, 0));
            outputList[2].Description.Should().Be("Occurs the first weekday of very 2 months and every 6 hours between 05:00:00 and 06:00:00 starting on 1/1/2020");
        }

        [Fact]
        public void Should_Be_Next_Executions_For_Month_WeekDayOption_First_WeekEndDay_Skipping_2_Months()
        {
            var scheduler = new Scheduler();
            var configuration = new Configuration(new DateTime(2024, 4, 5), ConfigType.Recurring, true, null, Occurs.Monthly, MonthlyConfiguration.WeekDayOption(Ordinal.First, KindOfDay.WeekEndDay, 2), null,
                DailyConfiguration.Recurring(6, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(3, 0, 0), new TimeOnly(6, 0, 0))), new DateLimits(new DateTime(2020, 1, 1)));

            var outputList = scheduler.ExecuteMany(configuration, 3);
            outputList[0].NextExecutionTime.Should().Be(new DateTime(2024, 4, 6, 3, 0, 0));
            outputList[0].Description.Should().Be("Occurs the first weekendday of very 2 months and every 6 hours between 03:00:00 and 06:00:00 starting on 1/1/2020");
            outputList[1].NextExecutionTime.Should().Be(new DateTime(2024, 6, 1, 3, 0, 0));
            outputList[1].Description.Should().Be("Occurs the first weekendday of very 2 months and every 6 hours between 03:00:00 and 06:00:00 starting on 1/1/2020");
            outputList[2].NextExecutionTime.Should().Be(new DateTime(2024, 8, 3, 3, 0, 0));
            outputList[2].Description.Should().Be("Occurs the first weekendday of very 2 months and every 6 hours between 03:00:00 and 06:00:00 starting on 1/1/2020");
        }

        [Fact]
        public void Should_Be_Next_Executions_For_Month_WeekDayOption_First_WeekEndDay_Skipping_3_Months()
        {
            var scheduler = new Scheduler();
            var configuration = new Configuration(new DateTime(2024, 4, 1), ConfigType.Recurring, true, null, Occurs.Monthly, MonthlyConfiguration.WeekDayOption(Ordinal.First, KindOfDay.WeekEndDay, 3), null,
                DailyConfiguration.Recurring(6, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(3, 0, 0), new TimeOnly(6, 0, 0))), new DateLimits(new DateTime(2020, 1, 1)));

            var outputList = scheduler.ExecuteMany(configuration, 4);
            outputList[0].NextExecutionTime.Should().Be(new DateTime(2024, 4, 6, 3, 0, 0));
            outputList[0].Description.Should().Be("Occurs the first weekendday of very 3 months and every 6 hours between 03:00:00 and 06:00:00 starting on 1/1/2020");
            outputList[1].NextExecutionTime.Should().Be(new DateTime(2024, 7, 6, 3, 0, 0));
            outputList[1].Description.Should().Be("Occurs the first weekendday of very 3 months and every 6 hours between 03:00:00 and 06:00:00 starting on 1/1/2020");
            outputList[2].NextExecutionTime.Should().Be(new DateTime(2024, 10, 5, 3, 0, 0));
            outputList[2].Description.Should().Be("Occurs the first weekendday of very 3 months and every 6 hours between 03:00:00 and 06:00:00 starting on 1/1/2020");
            outputList[3].NextExecutionTime.Should().Be(new DateTime(2025, 1, 4, 3, 0, 0));
            outputList[3].Description.Should().Be("Occurs the first weekendday of very 3 months and every 6 hours between 03:00:00 and 06:00:00 starting on 1/1/2020");
        }

        [Fact]
        public void Should_Be_Next_Executions_For_Month_WeekDayOption_Second_WeekEndDay_Skipping_2_Months()
        {
            var scheduler = new Scheduler();
            var configuration = new Configuration(new DateTime(2024, 4, 5), ConfigType.Recurring, true, null, Occurs.Monthly, MonthlyConfiguration.WeekDayOption(Ordinal.Second, KindOfDay.WeekEndDay, 2), null,
                DailyConfiguration.Recurring(6, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(3, 0, 0), new TimeOnly(6, 0, 0))), new DateLimits(new DateTime(2020, 1, 1)));

            var outputList = scheduler.ExecuteMany(configuration, 3);
            outputList[0].NextExecutionTime.Should().Be(new DateTime(2024, 4, 7, 3, 0, 0));
            outputList[0].Description.Should().Be("Occurs the second weekendday of very 2 months and every 6 hours between 03:00:00 and 06:00:00 starting on 1/1/2020");
            outputList[1].NextExecutionTime.Should().Be(new DateTime(2024, 6, 2, 3, 0, 0));
            outputList[1].Description.Should().Be("Occurs the second weekendday of very 2 months and every 6 hours between 03:00:00 and 06:00:00 starting on 1/1/2020");
            outputList[2].NextExecutionTime.Should().Be(new DateTime(2024, 8, 4, 3, 0, 0));
            outputList[2].Description.Should().Be("Occurs the second weekendday of very 2 months and every 6 hours between 03:00:00 and 06:00:00 starting on 1/1/2020");
        }

        [Fact]
        public void Should_Be_Next_Executions_For_Month_WeekDayOption_Second_Day_Skipping_2_Months()
        {
            var scheduler = new Scheduler();
            var configuration = new Configuration(new DateTime(2024, 4, 5), ConfigType.Recurring, true, null, Occurs.Monthly, MonthlyConfiguration.WeekDayOption(Ordinal.Second, KindOfDay.Day, 2), null,
                DailyConfiguration.Recurring(6, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(3, 0, 0), new TimeOnly(6, 0, 0))), new DateLimits(new DateTime(2020, 1, 1)));

            var outputList = scheduler.ExecuteMany(configuration, 3);
            outputList[0].NextExecutionTime.Should().Be(new DateTime(2024, 5, 2, 3, 0, 0));
            outputList[0].Description.Should().Be("Occurs the second day of very 2 months and every 6 hours between 03:00:00 and 06:00:00 starting on 1/1/2020");
            outputList[1].NextExecutionTime.Should().Be(new DateTime(2024, 7, 2, 3, 0, 0));
            outputList[1].Description.Should().Be("Occurs the second day of very 2 months and every 6 hours between 03:00:00 and 06:00:00 starting on 1/1/2020");
            outputList[2].NextExecutionTime.Should().Be(new DateTime(2024, 9, 2, 3, 0, 0));
            outputList[2].Description.Should().Be("Occurs the second day of very 2 months and every 6 hours between 03:00:00 and 06:00:00 starting on 1/1/2020");
        }

        [Fact]
        public void Should_Be_Next_Executions_For_Month_DayOption_Skipping_1_Months_With_DailyConfiguration_OnceType_FebruaryCase()
        {
            var scheduler = new Scheduler();
            var configuration = new Configuration(new DateTime(2024, 1, 1, 0, 0, 0), ConfigType.Recurring, true, null, Occurs.Monthly, MonthlyConfiguration.DayOption(30, 1), null,
                DailyConfiguration.Once(new TimeOnly(8, 0, 0), null), new DateLimits(new DateTime(2024, 1, 1)));

            var outputList = scheduler.ExecuteMany(configuration, 2);
            outputList[0].NextExecutionTime.Should().Be(new DateTime(2024, 1, 30, 8, 0, 0));
            outputList[0].Description.Should().Be("Occurs the 30th of very 1 month one time at 08:00:00 starting on 1/1/2024");
            outputList[1].NextExecutionTime.Should().Be(new DateTime(2024, 2, 29, 8, 0, 0));
            outputList[1].Description.Should().Be("Occurs the 30th of very 1 month one time at 08:00:00 starting on 1/1/2024");
        }

        [Fact]
        public void Should_Be_Next_Executions_For_Month_DayOption_Skipping_3_Months_With_DailyConfiguration_OnceType_And_Skip_First_Month()
        {
            var scheduler = new Scheduler();
            var configuration = new Configuration(new DateTime(2024, 4, 1, 0, 0, 0), ConfigType.Recurring, true, null, Occurs.Monthly, MonthlyConfiguration.DayOption(31, 3), null,
                DailyConfiguration.Once(new TimeOnly(12, 0, 0), null), new DateLimits(new DateTime(2024, 1, 1)));

            var outputList = scheduler.ExecuteMany(configuration, 2);
            outputList[0].NextExecutionTime.Should().Be(new DateTime(2024, 4, 30, 12, 0, 0));
            outputList[0].Description.Should().Be("Occurs the 31st of very 3 months one time at 12:00:00 starting on 1/1/2024");
            outputList[1].NextExecutionTime.Should().Be(new DateTime(2024, 7, 31, 12, 0, 0));
            outputList[1].Description.Should().Be("Occurs the 31st of very 3 months one time at 12:00:00 starting on 1/1/2024");
        }

        [Fact]
        public void Should_Be_Next_Executions_For_Month_DayOption_Skipping_2_Months_With_DailyConfiguration_OnceType_RegularCases()
        {
            var scheduler = new Scheduler();
            var configuration = new Configuration(new DateTime(2024, 1, 1, 0, 0, 0), ConfigType.Recurring, true, null, Occurs.Monthly, MonthlyConfiguration.DayOption(20, 2), null,
                DailyConfiguration.Once(new TimeOnly(12, 0, 0), null), new DateLimits(new DateTime(2024, 1, 1)));

            var outputList = scheduler.ExecuteMany(configuration, 5);
            outputList[0].NextExecutionTime.Should().Be(new DateTime(2024, 1, 20, 12, 0, 0));
            outputList[0].Description.Should().Be("Occurs the 20th of very 2 months one time at 12:00:00 starting on 1/1/2024");
            outputList[1].NextExecutionTime.Should().Be(new DateTime(2024, 3, 20, 12, 0, 0));
            outputList[1].Description.Should().Be("Occurs the 20th of very 2 months one time at 12:00:00 starting on 1/1/2024");
            outputList[2].NextExecutionTime.Should().Be(new DateTime(2024, 5, 20, 12, 0, 0));
            outputList[2].Description.Should().Be("Occurs the 20th of very 2 months one time at 12:00:00 starting on 1/1/2024");
            outputList[3].NextExecutionTime.Should().Be(new DateTime(2024, 7, 20, 12, 0, 0));
            outputList[3].Description.Should().Be("Occurs the 20th of very 2 months one time at 12:00:00 starting on 1/1/2024");
            outputList[4].NextExecutionTime.Should().Be(new DateTime(2024, 9, 20, 12, 0, 0));
            outputList[4].Description.Should().Be("Occurs the 20th of very 2 months one time at 12:00:00 starting on 1/1/2024");
        }

        [Fact]
        public void Should_Be_Next_Execution_For_Month_DayOption1st_Skipping_3_Months_StOrdinalCase()
        {
            var scheduler = new Scheduler();
            var configuration = new Configuration(new DateTime(2024, 4, 1, 0, 0, 0), ConfigType.Recurring, true, null, Occurs.Monthly, MonthlyConfiguration.DayOption(1, 3), null,
                DailyConfiguration.Once(new TimeOnly(12, 0, 0), null), new DateLimits(new DateTime(2024, 1, 1)));

            var outPut = scheduler.Execute(configuration);
            outPut.NextExecutionTime.Should().Be(new DateTime(2024, 4, 1, 12, 0, 0));
            outPut.Description.Should().Be("Occurs the 1st of very 3 months one time at 12:00:00 starting on 1/1/2024");
        }

        [Fact]
        public void Should_Be_Next_Execution_For_Month_DayOption21st_Skipping_3_Months_StOrdinalCase()
        {
            var scheduler = new Scheduler();
            var configuration = new Configuration(new DateTime(2024, 4, 1, 0, 0, 0), ConfigType.Recurring, true, null, Occurs.Monthly, MonthlyConfiguration.DayOption(21, 3), null,
                DailyConfiguration.Once(new TimeOnly(12, 0, 0), null), new DateLimits(new DateTime(2024, 1, 1)));

            var output = scheduler.Execute(configuration);
            output.NextExecutionTime.Should().Be(new DateTime(2024, 4, 21, 12, 0, 0));
            output.Description.Should().Be("Occurs the 21st of very 3 months one time at 12:00:00 starting on 1/1/2024");
        }

        [Fact]
        public void Should_Be_Next_Execution_For_Month_DayOption31st_Skipping_3_Months_StOrdinalCase()
        {
            var scheduler = new Scheduler();
            var configuration = new Configuration(new DateTime(2024, 4, 1, 0, 0, 0), ConfigType.Recurring, true, null, Occurs.Monthly, MonthlyConfiguration.DayOption(31, 3), null,
                DailyConfiguration.Once(new TimeOnly(12, 0, 0), null), new DateLimits(new DateTime(2024, 1, 1)));

            var output = scheduler.Execute(configuration);
            output.NextExecutionTime.Should().Be(new DateTime(2024, 4, 30, 12, 0, 0));
            output.Description.Should().Be("Occurs the 31st of very 3 months one time at 12:00:00 starting on 1/1/2024");
        }

        [Fact]
        public void Should_Be_Next_Executions_For_Month_DayOption10_Skipping_3_Months_With_DailyConfiguration_OnceType()
        {
            var scheduler = new Scheduler();
            var configuration = new Configuration(new DateTime(2020, 1, 1, 0, 0, 0), ConfigType.Recurring, true, null, Occurs.Monthly, MonthlyConfiguration.DayOption(10, 3), null,
                DailyConfiguration.Once(new TimeOnly(8, 0, 0), null), new DateLimits(new DateTime(2020, 1, 1)));

            var outputList = scheduler.ExecuteMany(configuration, 9);
            outputList[0].NextExecutionTime.Should().Be(new DateTime(2020, 1, 10, 8, 0, 0));
            outputList[0].Description.Should().Be("Occurs the 10th of very 3 months one time at 08:00:00 starting on 1/1/2020");
            outputList[1].NextExecutionTime.Should().Be(new DateTime(2020, 4, 10, 8, 0, 0));
            outputList[1].Description.Should().Be("Occurs the 10th of very 3 months one time at 08:00:00 starting on 1/1/2020");
            outputList[2].NextExecutionTime.Should().Be(new DateTime(2020, 7, 10, 8, 0, 0));
            outputList[2].Description.Should().Be("Occurs the 10th of very 3 months one time at 08:00:00 starting on 1/1/2020");
            outputList[3].NextExecutionTime.Should().Be(new DateTime(2020, 10, 10, 8, 0, 0));
            outputList[3].Description.Should().Be("Occurs the 10th of very 3 months one time at 08:00:00 starting on 1/1/2020");
            outputList[4].NextExecutionTime.Should().Be(new DateTime(2021, 1, 10, 8, 0, 0));
            outputList[4].Description.Should().Be("Occurs the 10th of very 3 months one time at 08:00:00 starting on 1/1/2020");
            outputList[5].NextExecutionTime.Should().Be(new DateTime(2021, 4, 10, 8, 0, 0));
            outputList[5].Description.Should().Be("Occurs the 10th of very 3 months one time at 08:00:00 starting on 1/1/2020");
            outputList[6].NextExecutionTime.Should().Be(new DateTime(2021, 7, 10, 8, 0, 0));
            outputList[6].Description.Should().Be("Occurs the 10th of very 3 months one time at 08:00:00 starting on 1/1/2020");
            outputList[7].NextExecutionTime.Should().Be(new DateTime(2021, 10, 10, 8, 0, 0));
            outputList[7].Description.Should().Be("Occurs the 10th of very 3 months one time at 08:00:00 starting on 1/1/2020");
            outputList[8].NextExecutionTime.Should().Be(new DateTime(2022, 1, 10, 8, 0, 0));
            outputList[8].Description.Should().Be("Occurs the 10th of very 3 months one time at 08:00:00 starting on 1/1/2020");
        }

        [Fact]
        public void Should_Be_Next_Executions_For_Month_DayOption_Skipping_3_Months_With_DailyConfiguration_RecurringType_Scheduler_Example()
        {
            var scheduler = new Scheduler();
            var configuration = new Configuration(new DateTime(2020, 1, 1, 0, 0, 0), ConfigType.Recurring, true, null, Occurs.Monthly, MonthlyConfiguration.DayOption(10, 3), null,
                DailyConfiguration.Recurring(1, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(3, 0, 0), new TimeOnly(6, 0, 0))), new DateLimits(new DateTime(2020, 1, 1)));

            var outputList = scheduler.ExecuteMany(configuration, 9);
            outputList[0].NextExecutionTime.Should().Be(new DateTime(2020, 1, 10, 3, 0, 0));
            outputList[0].Description.Should().Be("Occurs the 10th of very 3 months and every 1 hours between 03:00:00 and 06:00:00 starting on 1/1/2020");
            outputList[1].NextExecutionTime.Should().Be(new DateTime(2020, 1, 10, 4, 0, 0));
            outputList[1].Description.Should().Be("Occurs the 10th of very 3 months and every 1 hours between 03:00:00 and 06:00:00 starting on 1/1/2020");
            outputList[2].NextExecutionTime.Should().Be(new DateTime(2020, 1, 10, 5, 0, 0));
            outputList[2].Description.Should().Be("Occurs the 10th of very 3 months and every 1 hours between 03:00:00 and 06:00:00 starting on 1/1/2020");
            outputList[3].NextExecutionTime.Should().Be(new DateTime(2020, 1, 10, 6, 0, 0));
            outputList[3].Description.Should().Be("Occurs the 10th of very 3 months and every 1 hours between 03:00:00 and 06:00:00 starting on 1/1/2020");
            outputList[4].NextExecutionTime.Should().Be(new DateTime(2020, 4, 10, 3, 0, 0));
            outputList[4].Description.Should().Be("Occurs the 10th of very 3 months and every 1 hours between 03:00:00 and 06:00:00 starting on 1/1/2020");
            outputList[5].NextExecutionTime.Should().Be(new DateTime(2020, 4, 10, 4, 0, 0));
            outputList[5].Description.Should().Be("Occurs the 10th of very 3 months and every 1 hours between 03:00:00 and 06:00:00 starting on 1/1/2020");
            outputList[6].NextExecutionTime.Should().Be(new DateTime(2020, 4, 10, 5, 0, 0));
            outputList[6].Description.Should().Be("Occurs the 10th of very 3 months and every 1 hours between 03:00:00 and 06:00:00 starting on 1/1/2020");
            outputList[7].NextExecutionTime.Should().Be(new DateTime(2020, 4, 10, 6, 0, 0));
            outputList[7].Description.Should().Be("Occurs the 10th of very 3 months and every 1 hours between 03:00:00 and 06:00:00 starting on 1/1/2020");
            outputList[8].NextExecutionTime.Should().Be(new DateTime(2020, 7, 10, 3, 0, 0));
            outputList[8].Description.Should().Be("Occurs the 10th of very 3 months and every 1 hours between 03:00:00 and 06:00:00 starting on 1/1/2020");
        }

        [Fact]
        public void Should_Be_Next_Executions_For_Month_WeekDayOption_Second_WeekEndDay_Skipping_1_Months_With_DailyConfiguration_RecurringType()
        {
            var scheduler = new Scheduler();
            var configuration = new Configuration(new DateTime(2020, 1, 1, 0, 0, 0), ConfigType.Recurring, true, null, Occurs.Monthly, MonthlyConfiguration.WeekDayOption(Ordinal.Second, KindOfDay.WeekEndDay, 1), null,
                DailyConfiguration.Recurring(1, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(3, 0, 0), new TimeOnly(6, 0, 0))), new DateLimits(new DateTime(2020, 1, 1)));

            var outputList = scheduler.ExecuteMany(configuration, 9);
            outputList[0].NextExecutionTime.Should().Be(new DateTime(2020, 1, 5, 3, 0, 0));
            outputList[0].Description.Should().Be("Occurs the second weekendday of very 1 month and every 1 hours between 03:00:00 and 06:00:00 starting on 1/1/2020");
            outputList[1].NextExecutionTime.Should().Be(new DateTime(2020, 1, 5, 4, 0, 0));
            outputList[1].Description.Should().Be("Occurs the second weekendday of very 1 month and every 1 hours between 03:00:00 and 06:00:00 starting on 1/1/2020");
            outputList[2].NextExecutionTime.Should().Be(new DateTime(2020, 1, 5, 5, 0, 0));
            outputList[2].Description.Should().Be("Occurs the second weekendday of very 1 month and every 1 hours between 03:00:00 and 06:00:00 starting on 1/1/2020");
            outputList[3].NextExecutionTime.Should().Be(new DateTime(2020, 1, 5, 6, 0, 0));
            outputList[3].Description.Should().Be("Occurs the second weekendday of very 1 month and every 1 hours between 03:00:00 and 06:00:00 starting on 1/1/2020");
            outputList[4].NextExecutionTime.Should().Be(new DateTime(2020, 2, 2, 3, 0, 0));
            outputList[4].Description.Should().Be("Occurs the second weekendday of very 1 month and every 1 hours between 03:00:00 and 06:00:00 starting on 1/1/2020");
            outputList[5].NextExecutionTime.Should().Be(new DateTime(2020, 2, 2, 4, 0, 0));
            outputList[5].Description.Should().Be("Occurs the second weekendday of very 1 month and every 1 hours between 03:00:00 and 06:00:00 starting on 1/1/2020");
            outputList[6].NextExecutionTime.Should().Be(new DateTime(2020, 2, 2, 5, 0, 0));
            outputList[6].Description.Should().Be("Occurs the second weekendday of very 1 month and every 1 hours between 03:00:00 and 06:00:00 starting on 1/1/2020");
            outputList[7].NextExecutionTime.Should().Be(new DateTime(2020, 2, 2, 6, 0, 0));
            outputList[7].Description.Should().Be("Occurs the second weekendday of very 1 month and every 1 hours between 03:00:00 and 06:00:00 starting on 1/1/2020");
            outputList[8].NextExecutionTime.Should().Be(new DateTime(2020, 3, 7, 3, 0, 0));
            outputList[8].Description.Should().Be("Occurs the second weekendday of very 1 month and every 1 hours between 03:00:00 and 06:00:00 starting on 1/1/2020");
        }

        [Fact]
        public void Should_Be_Next_Executions_For_Month_WeekDayOption_First_Thursday_Skipping_3_Months_With_DailyConfiguration_RecurringType_Scheduler_Example()
        {
            var scheduler = new Scheduler();
            var configuration = new Configuration(new DateTime(2020, 1, 1, 0, 0, 0), ConfigType.Recurring, true, null, Occurs.Monthly, MonthlyConfiguration.WeekDayOption(Ordinal.First, KindOfDay.Thursday, 3), null,
                DailyConfiguration.Recurring(1, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(3, 0, 0), new TimeOnly(6, 0, 0))), new DateLimits(new DateTime(2020, 1, 1)));

            var outputList = scheduler.ExecuteMany(configuration, 9);
            outputList[0].NextExecutionTime.Should().Be(new DateTime(2020, 1, 2, 3, 0, 0));
            outputList[0].Description.Should().Be("Occurs the first thursday of very 3 months and every 1 hours between 03:00:00 and 06:00:00 starting on 1/1/2020");
            outputList[1].NextExecutionTime.Should().Be(new DateTime(2020, 1, 2, 4, 0, 0));
            outputList[1].Description.Should().Be("Occurs the first thursday of very 3 months and every 1 hours between 03:00:00 and 06:00:00 starting on 1/1/2020");
            outputList[2].NextExecutionTime.Should().Be(new DateTime(2020, 1, 2, 5, 0, 0));
            outputList[2].Description.Should().Be("Occurs the first thursday of very 3 months and every 1 hours between 03:00:00 and 06:00:00 starting on 1/1/2020");
            outputList[3].NextExecutionTime.Should().Be(new DateTime(2020, 1, 2, 6, 0, 0));
            outputList[3].Description.Should().Be("Occurs the first thursday of very 3 months and every 1 hours between 03:00:00 and 06:00:00 starting on 1/1/2020");
            outputList[4].NextExecutionTime.Should().Be(new DateTime(2020, 4, 2, 3, 0, 0));
            outputList[4].Description.Should().Be("Occurs the first thursday of very 3 months and every 1 hours between 03:00:00 and 06:00:00 starting on 1/1/2020");
            outputList[5].NextExecutionTime.Should().Be(new DateTime(2020, 4, 2, 4, 0, 0));
            outputList[5].Description.Should().Be("Occurs the first thursday of very 3 months and every 1 hours between 03:00:00 and 06:00:00 starting on 1/1/2020");
            outputList[6].NextExecutionTime.Should().Be(new DateTime(2020, 4, 2, 5, 0, 0));
            outputList[6].Description.Should().Be("Occurs the first thursday of very 3 months and every 1 hours between 03:00:00 and 06:00:00 starting on 1/1/2020");
            outputList[7].NextExecutionTime.Should().Be(new DateTime(2020, 4, 2, 6, 0, 0));
            outputList[7].Description.Should().Be("Occurs the first thursday of very 3 months and every 1 hours between 03:00:00 and 06:00:00 starting on 1/1/2020");
            outputList[8].NextExecutionTime.Should().Be(new DateTime(2020, 7, 2, 3, 0, 0));
            outputList[8].Description.Should().Be("Occurs the first thursday of very 3 months and every 1 hours between 03:00:00 and 06:00:00 starting on 1/1/2020");
        }

        [Fact]
        public void Should_Be_Next_Executions_For_Month_WeekDayOption_Last_Thursday_Skipping_1_Months_With_DailyConfiguration_RecurringType_And_Adding_Hours()
        {
            var scheduler = new Scheduler();
            var configuration = new Configuration(new DateTime(2024, 1, 1, 0, 0, 0), ConfigType.Recurring, true, null, Occurs.Monthly, MonthlyConfiguration.WeekDayOption(Ordinal.Last, KindOfDay.Thursday, 1), null,
                DailyConfiguration.Recurring(2, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(14, 0, 0), new TimeOnly(20, 0, 0))), new DateLimits(new DateTime(2020, 1, 1)));

            var outputList = scheduler.ExecuteMany(configuration, 9);
            outputList[0].NextExecutionTime.Should().Be(new DateTime(2024, 1, 25, 14, 0, 0));
            outputList[0].Description.Should().Be("Occurs the last thursday of very 1 month and every 2 hours between 14:00:00 and 20:00:00 starting on 1/1/2020");
            outputList[1].NextExecutionTime.Should().Be(new DateTime(2024, 1, 25, 16, 0, 0));
            outputList[1].Description.Should().Be("Occurs the last thursday of very 1 month and every 2 hours between 14:00:00 and 20:00:00 starting on 1/1/2020");
            outputList[2].NextExecutionTime.Should().Be(new DateTime(2024, 1, 25, 18, 0, 0));
            outputList[2].Description.Should().Be("Occurs the last thursday of very 1 month and every 2 hours between 14:00:00 and 20:00:00 starting on 1/1/2020");
            outputList[3].NextExecutionTime.Should().Be(new DateTime(2024, 1, 25, 20, 0, 0));
            outputList[3].Description.Should().Be("Occurs the last thursday of very 1 month and every 2 hours between 14:00:00 and 20:00:00 starting on 1/1/2020");
            outputList[4].NextExecutionTime.Should().Be(new DateTime(2024, 2, 29, 14, 0, 0));
            outputList[4].Description.Should().Be("Occurs the last thursday of very 1 month and every 2 hours between 14:00:00 and 20:00:00 starting on 1/1/2020");
            outputList[5].NextExecutionTime.Should().Be(new DateTime(2024, 2, 29, 16, 0, 0));
            outputList[5].Description.Should().Be("Occurs the last thursday of very 1 month and every 2 hours between 14:00:00 and 20:00:00 starting on 1/1/2020");
            outputList[6].NextExecutionTime.Should().Be(new DateTime(2024, 2, 29, 18, 0, 0));
            outputList[6].Description.Should().Be("Occurs the last thursday of very 1 month and every 2 hours between 14:00:00 and 20:00:00 starting on 1/1/2020");
            outputList[7].NextExecutionTime.Should().Be(new DateTime(2024, 2, 29, 20, 0, 0));
            outputList[7].Description.Should().Be("Occurs the last thursday of very 1 month and every 2 hours between 14:00:00 and 20:00:00 starting on 1/1/2020");
            outputList[8].NextExecutionTime.Should().Be(new DateTime(2024, 3, 28, 14, 0, 0));
            outputList[8].Description.Should().Be("Occurs the last thursday of very 1 month and every 2 hours between 14:00:00 and 20:00:00 starting on 1/1/2020");
        }

        [Fact]
        public void Should_Skip_Months_Because_DayNumber31_Is_Greater_Than_DaysInMonth()
        {
            var scheduler = new Scheduler();
            var configuration = new Configuration(new DateTime(2024, 1, 31, 22, 0, 0), ConfigType.Recurring, true, null, Occurs.Monthly, MonthlyConfiguration.DayOption(31, 1), null,
                DailyConfiguration.Recurring(3, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(14, 0, 0), new TimeOnly(20, 0, 0))), new DateLimits(new DateTime(2020, 1, 1)));

            var output = scheduler.Execute(configuration);
            output.NextExecutionTime.Should().Be(new DateTime(2024, 2, 29, 14, 0, 0));
            output.Description.Should().Be("Occurs the 31st of very 1 month and every 3 hours between 14:00:00 and 20:00:00 starting on 1/1/2020");
        }

        [Fact]
        public void Should_Skip_First_30th_Because_EndTime_Is_Before_CurrentTime_Skipping()
        {
            var scheduler = new Scheduler();
            var configuration = new Configuration(new DateTime(2024, 1, 30, 7, 0, 0), ConfigType.Recurring, true, null, Occurs.Monthly, MonthlyConfiguration.DayOption(30, 1), null,
                DailyConfiguration.Recurring(8, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(5, 0, 0), new TimeOnly(6, 0, 0))), new DateLimits(new DateTime(2020, 1, 1)));

            var output = scheduler.Execute(configuration);
            output.NextExecutionTime.Should().Be(new DateTime(2024, 2, 29, 5, 0, 0));
            output.Description.Should().Be("Occurs the 30th of very 1 month and every 8 hours between 05:00:00 and 06:00:00 starting on 1/1/2020");
        }

        [Fact]
        public void Should_Be_Next_ExecutionTime_And_In_The_Same_Day_For_Month_WeekDayOption_First_WeekDay()
        {
            var scheduler = new Scheduler();
            var configuration = new Configuration(new DateTime(2024, 1, 3, 15, 50, 0), ConfigType.Recurring, true, null, Occurs.Monthly, MonthlyConfiguration.WeekDayOption(Ordinal.First, KindOfDay.WeekDay, 1), null,
                DailyConfiguration.Recurring(2, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(14, 0, 0), new TimeOnly(20, 0, 0))), new DateLimits(new DateTime(2024, 1, 1)));

            var output = scheduler.Execute(configuration);
            output.NextExecutionTime.Should().Be(new DateTime(2024, 2, 1, 14, 0, 0));
            output.Description.Should().Be("Occurs the first weekday of very 1 month and every 2 hours between 14:00:00 and 20:00:00 starting on 1/1/2024");
        }

        [Fact]
        public void Should_Be_Next_Executions_For_Month_WeekDayOption_Third_WeekDay_Skipping_1_Months_With_DailyConfiguration_RecurringType_And_Adding_Hours()
        {
            var scheduler = new Scheduler();
            var configuration = new Configuration(new DateTime(2024, 1, 3, 1, 50, 0), ConfigType.Recurring, true, null, Occurs.Monthly, MonthlyConfiguration.WeekDayOption(Ordinal.Third, KindOfDay.WeekDay, 1), null,
                DailyConfiguration.Recurring(2, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(14, 0, 0), new TimeOnly(20, 0, 0))), new DateLimits(new DateTime(2024, 1, 1)));

            var outputList = scheduler.ExecuteMany(configuration, 9);
            outputList[0].NextExecutionTime.Should().Be(new DateTime(2024, 1, 3, 14, 0, 0));
            outputList[0].Description.Should().Be("Occurs the third weekday of very 1 month and every 2 hours between 14:00:00 and 20:00:00 starting on 1/1/2024");
            outputList[1].NextExecutionTime.Should().Be(new DateTime(2024, 1, 3, 16, 0, 0));
            outputList[1].Description.Should().Be("Occurs the third weekday of very 1 month and every 2 hours between 14:00:00 and 20:00:00 starting on 1/1/2024");
            outputList[2].NextExecutionTime.Should().Be(new DateTime(2024, 1, 3, 18, 0, 0));
            outputList[2].Description.Should().Be("Occurs the third weekday of very 1 month and every 2 hours between 14:00:00 and 20:00:00 starting on 1/1/2024");
            outputList[3].NextExecutionTime.Should().Be(new DateTime(2024, 1, 3, 20, 0, 0));
            outputList[3].Description.Should().Be("Occurs the third weekday of very 1 month and every 2 hours between 14:00:00 and 20:00:00 starting on 1/1/2024");
            outputList[4].NextExecutionTime.Should().Be(new DateTime(2024, 2, 5, 14, 0, 0));
            outputList[4].Description.Should().Be("Occurs the third weekday of very 1 month and every 2 hours between 14:00:00 and 20:00:00 starting on 1/1/2024");
            outputList[5].NextExecutionTime.Should().Be(new DateTime(2024, 2, 5, 16, 0, 0));
            outputList[5].Description.Should().Be("Occurs the third weekday of very 1 month and every 2 hours between 14:00:00 and 20:00:00 starting on 1/1/2024");
            outputList[6].NextExecutionTime.Should().Be(new DateTime(2024, 2, 5, 18, 0, 0));
            outputList[6].Description.Should().Be("Occurs the third weekday of very 1 month and every 2 hours between 14:00:00 and 20:00:00 starting on 1/1/2024");
            outputList[7].NextExecutionTime.Should().Be(new DateTime(2024, 2, 5, 20, 0, 0));
            outputList[7].Description.Should().Be("Occurs the third weekday of very 1 month and every 2 hours between 14:00:00 and 20:00:00 starting on 1/1/2024");
            outputList[8].NextExecutionTime.Should().Be(new DateTime(2024, 3, 5, 14, 0, 0));
            outputList[8].Description.Should().Be("Occurs the third weekday of very 1 month and every 2 hours between 14:00:00 and 20:00:00 starting on 1/1/2024");
        }

        [Fact]
        public void Should_Be_Next_Executions_For_Month_WeekDayOption_Last_WeekEndDay_Skipping_1_Months_With_DailyConfiguration_RecurringType_And_Adding_Minutes_()
        {
            var scheduler = new Scheduler();
            var configuration = new Configuration(new DateTime(2024, 1, 1, 0, 0, 0), ConfigType.Recurring, true, null, Occurs.Monthly, MonthlyConfiguration.WeekDayOption(Ordinal.Last, KindOfDay.WeekEndDay, 1), null,
                DailyConfiguration.Recurring(40, DailyFrecuency.Minutes, new TimeLimits(new TimeOnly(14, 0, 0), new TimeOnly(16, 0, 0))), new DateLimits(new DateTime(2020, 1, 1)));

            var outputList = scheduler.ExecuteMany(configuration, 9);
            outputList[0].NextExecutionTime.Should().Be(new DateTime(2024, 1, 28, 14, 0, 0));
            outputList[0].Description.Should().Be("Occurs the last weekendday of very 1 month and every 40 minutes between 14:00:00 and 16:00:00 starting on 1/1/2020");
            outputList[1].NextExecutionTime.Should().Be(new DateTime(2024, 1, 28, 14, 40, 0));
            outputList[1].Description.Should().Be("Occurs the last weekendday of very 1 month and every 40 minutes between 14:00:00 and 16:00:00 starting on 1/1/2020");
            outputList[2].NextExecutionTime.Should().Be(new DateTime(2024, 1, 28, 15, 20, 0));
            outputList[2].Description.Should().Be("Occurs the last weekendday of very 1 month and every 40 minutes between 14:00:00 and 16:00:00 starting on 1/1/2020");
            outputList[3].NextExecutionTime.Should().Be(new DateTime(2024, 1, 28, 16, 0, 0));
            outputList[3].Description.Should().Be("Occurs the last weekendday of very 1 month and every 40 minutes between 14:00:00 and 16:00:00 starting on 1/1/2020");
            outputList[4].NextExecutionTime.Should().Be(new DateTime(2024, 2, 25, 14, 0, 0));
            outputList[4].Description.Should().Be("Occurs the last weekendday of very 1 month and every 40 minutes between 14:00:00 and 16:00:00 starting on 1/1/2020");
            outputList[5].NextExecutionTime.Should().Be(new DateTime(2024, 2, 25, 14, 40, 0));
            outputList[5].Description.Should().Be("Occurs the last weekendday of very 1 month and every 40 minutes between 14:00:00 and 16:00:00 starting on 1/1/2020");
            outputList[6].NextExecutionTime.Should().Be(new DateTime(2024, 2, 25, 15, 20, 0));
            outputList[6].Description.Should().Be("Occurs the last weekendday of very 1 month and every 40 minutes between 14:00:00 and 16:00:00 starting on 1/1/2020");
            outputList[7].NextExecutionTime.Should().Be(new DateTime(2024, 2, 25, 16, 0, 0));
            outputList[7].Description.Should().Be("Occurs the last weekendday of very 1 month and every 40 minutes between 14:00:00 and 16:00:00 starting on 1/1/2020");
            outputList[8].NextExecutionTime.Should().Be(new DateTime(2024, 3, 31, 14, 0, 0));
            outputList[8].Description.Should().Be("Occurs the last weekendday of very 1 month and every 40 minutes between 14:00:00 and 16:00:00 starting on 1/1/2020");
        }

        [Fact]
        public void Should_End_Execution_If_NextExecution_Is_Greater_Than_End_Date()
        {
            var scheduler = new Scheduler();
            var configuration = new Configuration(new DateTime(2024, 1, 1), ConfigType.Recurring, true, null, Occurs.Monthly, MonthlyConfiguration.WeekDayOption(Ordinal.Last, KindOfDay.WeekDay, 1), null,
               DailyConfiguration.Recurring(13, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(10, 0, 0), new TimeOnly(14, 2, 0))), new DateLimits(new DateTime(2020, 1, 1), new DateTime(2024, 5, 30)));

            var outputList = scheduler.ExecuteMany(configuration, 2);
            outputList[0].NextExecutionTime.Should().Be(new DateTime(2024, 1, 31, 10, 0, 0));
            outputList[1].NextExecutionTime.Should().Be(new DateTime(2024, 2, 29, 10, 0, 0));
        }

        [Fact]
        public void Should_Be_Next_Executions_For_Month_WeekDayOption_Last_WeekDay_Skipping_1_Months_With_DailyConfiguration_RecurringType_And_Adding_Seconds()
        {
            var scheduler = new Scheduler();
            var configuration = new Configuration(new DateTime(2024, 1, 1, 0, 0, 0), ConfigType.Recurring, true, null, Occurs.Monthly, MonthlyConfiguration.WeekDayOption(Ordinal.Last, KindOfDay.WeekDay, 1), null,
               DailyConfiguration.Recurring(13, DailyFrecuency.Seconds, new TimeLimits(new TimeOnly(14, 0, 0), new TimeOnly(14, 2, 0))), new DateLimits(new DateTime(2020, 1, 1)));

            var outputList = scheduler.ExecuteMany(configuration, 11);
            outputList[0].NextExecutionTime.Should().Be(new DateTime(2024, 1, 31, 14, 0, 0));
            outputList[0].Description.Should().Be("Occurs the last weekday of very 1 month and every 13 seconds between 14:00:00 and 14:02:00 starting on 1/1/2020");
            outputList[1].NextExecutionTime.Should().Be(new DateTime(2024, 1, 31, 14, 0, 13));
            outputList[1].Description.Should().Be("Occurs the last weekday of very 1 month and every 13 seconds between 14:00:00 and 14:02:00 starting on 1/1/2020");
            outputList[2].NextExecutionTime.Should().Be(new DateTime(2024, 1, 31, 14, 0, 26));
            outputList[2].Description.Should().Be("Occurs the last weekday of very 1 month and every 13 seconds between 14:00:00 and 14:02:00 starting on 1/1/2020");
            outputList[3].NextExecutionTime.Should().Be(new DateTime(2024, 1, 31, 14, 0, 39));
            outputList[3].Description.Should().Be("Occurs the last weekday of very 1 month and every 13 seconds between 14:00:00 and 14:02:00 starting on 1/1/2020");
            outputList[4].NextExecutionTime.Should().Be(new DateTime(2024, 1, 31, 14, 0, 52));
            outputList[4].Description.Should().Be("Occurs the last weekday of very 1 month and every 13 seconds between 14:00:00 and 14:02:00 starting on 1/1/2020");
            outputList[5].NextExecutionTime.Should().Be(new DateTime(2024, 1, 31, 14, 1, 5));
            outputList[5].Description.Should().Be("Occurs the last weekday of very 1 month and every 13 seconds between 14:00:00 and 14:02:00 starting on 1/1/2020");
            outputList[6].NextExecutionTime.Should().Be(new DateTime(2024, 1, 31, 14, 1, 18));
            outputList[6].Description.Should().Be("Occurs the last weekday of very 1 month and every 13 seconds between 14:00:00 and 14:02:00 starting on 1/1/2020");
            outputList[7].NextExecutionTime.Should().Be(new DateTime(2024, 1, 31, 14, 1, 31));
            outputList[7].Description.Should().Be("Occurs the last weekday of very 1 month and every 13 seconds between 14:00:00 and 14:02:00 starting on 1/1/2020");
            outputList[8].NextExecutionTime.Should().Be(new DateTime(2024, 1, 31, 14, 1, 44));
            outputList[8].Description.Should().Be("Occurs the last weekday of very 1 month and every 13 seconds between 14:00:00 and 14:02:00 starting on 1/1/2020");
            outputList[9].NextExecutionTime.Should().Be(new DateTime(2024, 1, 31, 14, 1, 57));
            outputList[9].Description.Should().Be("Occurs the last weekday of very 1 month and every 13 seconds between 14:00:00 and 14:02:00 starting on 1/1/2020");
            outputList[10].NextExecutionTime.Should().Be(new DateTime(2024, 2, 29, 14, 0, 0));
            outputList[10].Description.Should().Be("Occurs the last weekday of very 1 month and every 13 seconds between 14:00:00 and 14:02:00 starting on 1/1/2020");
        }
    }
}