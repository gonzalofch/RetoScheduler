using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RetoScheduler
{
    public class Month
    {
        public int Year { get; set; }

        public int MonthIndex { get; set; }



        public Month(int year, int monthIndex)
        {
          
            Year = year;
            MonthIndex = monthIndex;
        }
        public IReadOnlyList<DateTime> GetMonthDays(DayOfWeek? day=null) {


            return Enumerable.Range(1, DateTime.DaysInMonth(Year, MonthIndex))
                        .Select(x =>new DateTime(Year,MonthIndex,x))
                        .Where(//aplicar el filtro)
                        .ToList();
        }

    }
}
