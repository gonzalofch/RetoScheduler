using FluentAssertions;
using RetoScheduler.Localization;

namespace RetoSchedulerTest
{
    public class SchedulerLocalizerTests
    {
        [Fact, UseCulture("en-US")]
        public void Should_Translate_English()
        {
            SchedulerLocalizer localizer = new SchedulerLocalizer();
            var expected1 = localizer["Scheduler:Errors:NotEnabled"];
            expected1.Value.Should().Be("You need to check field to run the Scheduler");
        }

        [Fact, UseCulture("es-ES")]
        public void Should_Translate_Spanish()
        {
            SchedulerLocalizer localizer = new SchedulerLocalizer();
            var expected1 = localizer["Scheduler:Errors:NotEnabled"];
            expected1.Value.Should().Be("Necesitas marcar el campo para ejecutar el Programador");
        }
    }
}
