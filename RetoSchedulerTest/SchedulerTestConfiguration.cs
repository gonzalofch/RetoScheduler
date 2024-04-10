using RetoScheduler;
using RetoScheduler.Configurations;
using RetoScheduler.Configurations.Limits;
using RetoScheduler.Enums;
using System;
using System.Collections;
using System.Collections.Generic;

namespace RetoSchedulerTest
{
    public class SchedulerTestConfiguration : IEnumerable<object[]>
    {
        public readonly List<object[]> data = new List<object[]>()
        {



            new object[]{
                new Configuration
                (new DateTime(2020,1,1,0,0,0), ConfigType.Recurring,true,null,Occurs.Weekly,new WeeklyConfiguration(2, new List<DayOfWeek>(){DayOfWeek.Monday,DayOfWeek.Thursday,DayOfWeek.Friday}),new DailyConfiguration(DailyConfigType.Recurring,TimeOnly.MinValue,2,DailyFrecuency.Hours,new TimeLimits(new TimeOnly(4,0,0),new TimeOnly(8,0,0))),new DateLimits(new DateTime(2020,1,1))),
                new OutPut(new DateTime(2020,1,2,4,0,0),"Occurs every 2 weeks on monday, thursday and friday every 2 hours between 4:00 AM and 8:00 AM starting on 01/01/2020")
            },


            new object[]{
                new Configuration
                (new DateTime(2020,1,4), ConfigType.Once, true,new DateTime(2020,1,8),Occurs.Daily,null,new DailyConfiguration(DailyConfigType.Once,new TimeOnly(14,0,0),null,null,null), new DateLimits(new DateTime(2020,1,1))),
                new OutPut(new DateTime(2020,1,8,14,0,0),"Occurs once. Schedule will be used on 08/01/2020 at 14:00 starting on 01/01/2020")
            },
            new object[]
            {
                new Configuration(new DateTime(2020, 1, 4), ConfigType.Recurring, true , null, Occurs.Daily,null,new DailyConfiguration(DailyConfigType.Recurring,TimeOnly.MinValue,3,DailyFrecuency.Hours,new TimeLimits(new TimeOnly(10,0,0),new TimeOnly(15,0,0))), new DateLimits(new DateTime(2020,1,1),null)),
                new OutPut(new DateTime(2020, 1, 4),"Occurs every day. Schedule will be used on 04/01/2020 starting on 01/01/2020")
            },
            new object[]
            {
                new Configuration
                (new DateTime (2024,4,4), ConfigType.Once, true , new DateTime(2024,05,10),Occurs.Daily,null,new DailyConfiguration(DailyConfigType.Once,new TimeOnly(16,0,0),null,null,null),new DateLimits(new DateTime(2020,5,12))),
                new OutPut (new DateTime(2024,5,10,16,0,0),"Occurs once. Schedule will be used on 10/05/2024 at 16:00 starting on 12/05/2020")
            },
            new object[]
            {
                new Configuration(new DateTime(2023,12,24), ConfigType.Recurring, true , null, Occurs.Daily,null,new DailyConfiguration(DailyConfigType.Once,new TimeOnly(16,0,0),null,null,null),new DateLimits(new DateTime(2023,12,30))),
                new OutPut(new DateTime(2023,12,30),"Occurs every 5 days. Schedule will be used on 30/12/2023 starting on 30/12/2023")
            },
            new object[]
            {
                new Configuration
                (new DateTime (2024,4,4), ConfigType.Once, true , new DateTime(2024,05,10,16,0,0),Occurs.Daily,null,new DailyConfiguration(DailyConfigType.Recurring,TimeOnly.MinValue,30,DailyFrecuency.Minutes,new TimeLimits(new TimeOnly(),new TimeOnly())),new DateLimits(new DateTime(2020,5,10),null)),
                new OutPut (new DateTime(2024,5,10,16,0,0),"Occurs once. Schedule will be used on 10/05/2024 at 16:00 starting on 10/05/2020")
            },
            new object[]
            {
                new Configuration(new DateTime(2023,12,24), ConfigType.Recurring, true , null, Occurs.Daily,null,new DailyConfiguration(DailyConfigType.Once,new TimeOnly(10,0,0),null,null,null),new DateLimits(new DateTime(2020,12,27))),
                new OutPut(new DateTime(2023,12,24),"Occurs every day. Schedule will be used on 24/12/2023 starting on 27/12/2020")
            },
            new object[]
            {

                new Configuration(
                new DateTime(2020, 1, 4), ConfigType.Recurring, true, new DateTime(2020, 1, 8), Occurs.Daily,null,new DailyConfiguration(DailyConfigType.Once,new TimeOnly(14, 0, 0),null,null,null),new DateLimits(new DateTime(2020, 1, 7),new DateTime(2020,1,9))),
                new OutPut(new DateTime(2020,1,7),"Occurs every day. Schedule will be used on 07/01/2020 starting on 07/01/2020")
            },
            new object[]
            {

                new Configuration(
                    new DateTime(2020, 1, 4), ConfigType.Recurring, true, new DateTime(2020, 1, 8), Occurs.Daily,null,new DailyConfiguration(DailyConfigType.Once,new TimeOnly(14,0,0),null,null,null),
                    new DateLimits(new DateTime(2020, 1, 9),new DateTime(2020,1,10))),
                    new OutPut(new DateTime(2020,1,9),"Occurs every day. Schedule will be used on 09/01/2020 starting on 09/01/2020")
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
