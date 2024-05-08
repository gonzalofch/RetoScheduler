using Microsoft.Extensions.Localization;
using RetoScheduler.Configurations;
using RetoScheduler.Enums;
using RetoScheduler.Exceptions;

namespace RetoScheduler.Runners
{
    public class InOnceRunner
    {
        public DateTime Run(Configuration config, bool executed)
        {
            if (config.ConfigDateTime.HasValue == false)
            {
                throw new SchedulerException("Scheduler:Errors:RequiredConfigDateTimeInOnce");
            }

            DateTime dateTime = config.ConfigDateTime.Value;


            return DailyRunner.Run(config.DailyConfiguration, dateTime, executed);
        }
    }
}
