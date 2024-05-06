using Microsoft.Extensions.Localization;
using RetoScheduler.Configurations;
using RetoScheduler.Enums;
using RetoScheduler.Exceptions;
using RetoScheduler.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RetoScheduler.Runners
{
    public class InOnceRunner
    {
        private readonly IStringLocalizer L;
        public InOnceRunner(IStringLocalizer scSchedulerLocalizer ) { 
        L = scSchedulerLocalizer;
        }

        public DateTime Run(Configuration config)
        {
            if (config.ConfigDateTime.HasValue == false)
            {
                throw new SchedulerException(L["Scheduler:Errors:RequiredConfigDateTimeInOnce"]);
            }
            DateTime configDateTime = config.ConfigDateTime.Value;

            return config.DailyConfiguration.Type == DailyConfigType.Once
                ? configDateTime.Add(config.DailyConfiguration.OnceAt.ToTimeSpan())
                : configDateTime.Add(config.DailyConfiguration.TimeLimits.StartTime.ToTimeSpan());
        }
    }
}
