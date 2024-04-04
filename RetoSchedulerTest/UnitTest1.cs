using FluentAssertions;
using RetoScheduler;

namespace RetoSchedulerTest
{
    public class UnitTest1
    {
        [Theory,ClassData(typeof(UnitTestsInputData))]
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
     
    }
}