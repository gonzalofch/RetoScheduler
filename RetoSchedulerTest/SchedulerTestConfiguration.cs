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
                (new DateTime(2020,1,4), ConfigType.Once, true,new DateTime(2020,1,8,14,0,0),Occurs.Daily,0,new Limits(new DateTime(2020,1,1))),
                new OutPut(new DateTime(2020,1,8,14,0,0),"Occurs once. Schedule will be used on 08/01/2020 at 14:00 starting on 01/01/2020")
            },
            new object[]
            {
                new Configuration(new DateTime(2020, 1, 4), ConfigType.Recurring, true , null, Occurs.Daily, 1,new Limits(new DateTime(2020,1,1))),
                new OutPut(new DateTime(2020, 1, 5),"Occurs every day. Schedule will be used on 05/01/2020 starting on 01/01/2020")
            },
            new object[]
            {
                new Configuration
                (new DateTime (2024,4,4), ConfigType.Once, true , new DateTime(2024,05,10,16,0,0),Occurs.Daily,0,new Limits(new DateTime(2020,5,12))),
                new OutPut (new DateTime(2024,5,10,16,0,0),"Occurs once. Schedule will be used on 10/05/2024 at 16:00 starting on 12/05/2020")
            },
            new object[]
            {
                new Configuration(new DateTime(2023,12,24), ConfigType.Recurring, true , null, Occurs.Daily, 5,new Limits(new DateTime(2020,12,30))),
                new OutPut(new DateTime(2023,12,29),"Occurs every day. Schedule will be used on 29/12/2023 starting on 30/12/2020")
            },
            new object[]
            {
                new Configuration
                (new DateTime (2024,4,4), ConfigType.Once, true , new DateTime(2024,05,10,16,0,0),Occurs.Daily,0,new Limits(new DateTime(2020,5,10),null)),
                new OutPut (new DateTime(2024,5,10,16,0,0),"Occurs once. Schedule will be used on 10/05/2024 at 16:00 starting on 10/05/2020")
            },
            new object[]
            {
                new Configuration(new DateTime(2023,12,24), ConfigType.Recurring, true , null, Occurs.Daily, 5,new Limits(new DateTime(2020,12,27))),
                new OutPut(new DateTime(2023,12,29),"Occurs every day. Schedule will be used on 29/12/2023 starting on 27/12/2020")
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
