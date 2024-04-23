using RetoScheduler.Configurations;
using RetoScheduler.Configurations.Limits;
using RetoScheduler.Enums;
using System.Collections;

namespace RetoSchedulerTest
{
    public class SchedulerLimitsConfiguration : IEnumerable<object[]>
    {
        public readonly List<object[]> data = new List<object[]>()
        {
            new object[]
            {
                 new Configuration
                (new DateTime(2020, 1, 4), ConfigType.Recurring, true, null, Occurs.Daily,null,null, new DailyConfiguration(
                    DailyConfigType.Recurring,TimeOnly.MinValue, 2, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(4, 0, 0), new TimeOnly(8, 0, 0))), new DateLimits(new DateTime(2020, 1, 2),new DateTime(2020,1,3)))
            },
            new object[]
            {
                 new Configuration
                (new DateTime(2020, 1, 4), ConfigType.Recurring, true, new DateTime(2020, 1, 8, 14, 0, 0), Occurs.Daily,null,null, new DailyConfiguration(
                    DailyConfigType.Recurring,TimeOnly.MinValue, 2, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(4, 0, 0), new TimeOnly(8, 0, 0))), new DateLimits(new DateTime(2020, 1, 2),new DateTime(2020,1,3)))
            },
        };

        public IEnumerator<object[]> GetEnumerator()
        {
            return data.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return data.GetEnumerator();
        }
    }
}

