using RetoScheduler;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RetoSchedulerTest
{
    public class UnitTestsInputData : IEnumerable<object[]>
    {
        public readonly List<object[]> data = new List<object[]>()
        {
            new object[]{
                new Configuration
                (new DateTime(2020,1,4), ConfigType.Once, true,new DateTime(2020,1,8,14,0,0),Occurs.Daily,0),
                new OutPut(new DateTime(2020,1,8,14,0,0),"Occurs once. Schedule will be used on 08/01/2020 at 14:00 starting on 01/01/2020")
            },
            new object[]
            {
                new Configuration(new DateTime(2020, 1, 4), ConfigType.Recurring, true , null, Occurs.Daily, 1),
                new OutPut(new DateTime(2020, 1, 5),"Occurs every day. Schedule will be used on 05/01/2020 starting on 01/01/2020")
            },
            new object[]
            {
                new Configuration
                (new DateTime (2024,4,4), ConfigType.Once, true , new DateTime(2024,05,10,16,0,0),Occurs.Daily,0),
                new OutPut (new DateTime(2024,5,10,16,0,0),"Occurs once. Schedule will be used on 10/05/2024 at 16:00 starting on 01/01/2020")
            },
            new object[]
            {
                new Configuration(new DateTime(2023,12,24), ConfigType.Recurring, true , null, Occurs.Daily, 5),
                new OutPut(new DateTime(2023,12,29),"Occurs every day. Schedule will be used on 29/12/2023 starting on 01/01/2020")
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
