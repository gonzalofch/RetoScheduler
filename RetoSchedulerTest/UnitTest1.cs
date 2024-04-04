using FluentAssertions;
using Microsoft.VisualStudio.TestPlatform.Utilities;
using RetoScheduler;
using System.Runtime.InteropServices;

namespace RetoSchedulerTest
{
    public class UnitTest1
    {
        [Theory, ClassData(typeof(UnitTestsInputData))]
        public void
            _Execution_Time_Should_Be_Once_And_Daily(Configuration configuration, OutPut outPut)
        {

            var Scheduler = new Scheduler();
            var res = Scheduler.Init(configuration);

            res.NextExecutionTime.Should().Be(outPut.NextExecutionTime);
            res.Description.Should().Be(outPut.Description);
        }


        [Fact]
        public void Next_Execution_Time_Should_Throw_Exception_And_Message()
        {

            var scheduler = new Scheduler();
            var configuration = new Configuration(new DateTime(2020, 1, 4), ConfigType.Once, true, null, Occurs.Daily, 0);

            FluentActions
                .Invoking(() => scheduler.Init(configuration))
                .Should()
                .Throw<Exception>()
                .And.Message
                .Should().Be("Once Types requires an obligatory DateTime");
        }
        [Theory]
        [InlineData(ConfigType.Once)]
        [InlineData(ConfigType.Recurring)]

        public void Should_Throw_Exception_Disabled_Check_And_Message(ConfigType configType)
        {
            var scheduler = new Scheduler();
            var configuration = new Configuration(new DateTime(2020, 1, 5), configType, false, null, Occurs.Daily, 0);
            FluentActions
               .Invoking(() => scheduler.Init(configuration))
               .Should()
               .Throw<Exception>()
               .And.Message
               .Should().Be("You need to check field to Run Program");
        }
        [Theory]
        [InlineData(-1)]
        [InlineData(0)]
        public void Should_Throw_Exception_Negative_Number_Of_Days_In_Recurring_Type(int frecuencyInDays)
        {
            var scheduler = new Scheduler();
            var configuration = new Configuration(new DateTime(2020, 1, 5), ConfigType.Recurring, true, null, Occurs.Daily, frecuencyInDays);
            FluentActions
               .Invoking(() => scheduler.Init(configuration))
               .Should()
               .Throw<Exception>()
               .And.Message
               .Should().Be("Don't should put negative numbers or zero in number field");
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(0)]
        public void Should_Not_Throw_Exception_Negative_Number_Of_Days_In_Once_Type(int frecuencyInDays)
        {
            var scheduler = new Scheduler();
            var configuration = new Configuration(new DateTime(2020, 1, 5), ConfigType.Once, true, DateTime.Now, Occurs.Daily, frecuencyInDays);
            FluentActions
               .Invoking(() => scheduler.Init(configuration))
               .Should().NotThrow<Exception>();
        }
    }
}