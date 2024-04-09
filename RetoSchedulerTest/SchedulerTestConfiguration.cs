using RetoScheduler;
using RetoScheduler.Configurations;
using RetoScheduler.Enums;
using System.Collections;

namespace RetoSchedulerTest
{
    public class SchedulerTestConfiguration : IEnumerable<object[]>
    {
        public readonly List<object[]> data = new List<object[]>()
        {
            new object[]{
                new Configuration
                (new DateTime(2020,1,4), ConfigType.Once, true,new DateTime(2020,1,8,14,0,0),Occurs.Daily,0,new DateLimits(new DateTime(2020,1,1))),
                new OutPut(new DateTime(2020,1,8,14,0,0),"Occurs once. Schedule will be used on 08/01/2020 at 14:00 starting on 01/01/2020")
            },
            new object[]
            {
                new Configuration(new DateTime(2020, 1, 4), ConfigType.Recurring, true , null, Occurs.Daily, 1,new DateLimits(new DateTime(2020,1,1))),
                new OutPut(new DateTime(2020, 1, 4),"Occurs every day. Schedule will be used on 04/01/2020 starting on 01/01/2020")
            },
            new object[]
            {
                new Configuration
                (new DateTime (2024,4,4), ConfigType.Once, true , new DateTime(2024,05,10,16,0,0),Occurs.Daily,0,new DateLimits(new DateTime(2020,5,12))),
                new OutPut (new DateTime(2024,5,10,16,0,0),"Occurs once. Schedule will be used on 10/05/2024 at 16:00 starting on 12/05/2020")
            },
            new object[]
            {
                new Configuration(new DateTime(2023,12,24), ConfigType.Recurring, true , null, Occurs.Daily, 5,new DateLimits(new DateTime(2023,12,30))),
                new OutPut(new DateTime(2023,12,30),"Occurs every 5 days. Schedule will be used on 30/12/2023 starting on 30/12/2023")
            },
            new object[]
            {
                new Configuration
                (new DateTime (2024,4,4), ConfigType.Once, true , new DateTime(2024,05,10,16,0,0),Occurs.Daily,0,new DateLimits(new DateTime(2020,5,10),null)),
                new OutPut (new DateTime(2024,5,10,16,0,0),"Occurs once. Schedule will be used on 10/05/2024 at 16:00 starting on 10/05/2020")
            },
            new object[]
            {
                new Configuration(new DateTime(2023,12,24), ConfigType.Recurring, true , null, Occurs.Daily, 1,new DateLimits(new DateTime(2020,12,27))),
                new OutPut(new DateTime(2023,12,24),"Occurs every day. Schedule will be used on 24/12/2023 starting on 27/12/2020")
            },
            new object[]
            {

                new Configuration(
                    new DateTime(2020, 1, 4), ConfigType.Recurring, true, new DateTime(2020, 1, 8, 14, 0, 0), Occurs.Daily, 1,
                    new DateLimits(new DateTime(2020, 1, 7),new DateTime(2020,1,9))),
                new OutPut(new DateTime(2020,1,7),"Occurs every day. Schedule will be used on 07/01/2020 starting on 07/01/2020")
            },
            new object[]
            {
            
                new Configuration(
                    new DateTime(2020, 1, 4), ConfigType.Recurring, true, new DateTime(2020, 1, 8, 14, 0, 0), Occurs.Daily, 1,
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
