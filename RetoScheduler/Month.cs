using RetoScheduler.Enums;
using RetoScheduler.Exceptions;
using RetoScheduler.Extensions;

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
        public IReadOnlyList<DateTime> GetMonthDays(DayOfWeek? day = null,int? currentDay =null)
        {
            return Enumerable.Range(1, DateTime.DaysInMonth(Year, MonthIndex))
                        .Select(x => new DateTime(Year, MonthIndex, x))
                        .WhereIf(day != null, x => x.DayOfWeek == day)
                        .WhereIf(currentDay!=null, x=> x.Day>currentDay)
                        .ToList();
        }
    }
}
