using RetoScheduler.Enums;
using RetoScheduler.Exceptions;
using RetoScheduler.Extensions;
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
        public IReadOnlyList<DateTime> GetMonthDays(DayOfWeek? day = null,int? currentDay =null)
        {
            return Enumerable.Range(1, DateTime.DaysInMonth(Year, MonthIndex))
                        .Select(x => new DateTime(Year, MonthIndex, x))
                        .WhereIf(day != null, x => x.DayOfWeek == day)
                        .WhereIf(currentDay!=null, x=> x.Day>currentDay)
                        .ToList();

        }

        public DateTime OrdinalFromMonth(IReadOnlyList<DateTime> list, Ordinal ordinal)
        {
            var monthDays = list;
            bool greaterThanIndex1 = (Ordinal.First == ordinal || Ordinal.Last == ordinal) && monthDays.Count < 1;
            bool greaterThanIndex2 = Ordinal.Second == ordinal && monthDays.Count < 2;
            bool greaterThanIndex3 = Ordinal.Third == ordinal && monthDays.Count < 3;
            bool greaterThanIndex4 = Ordinal.Fourth == ordinal && monthDays.Count < 4;
            if (greaterThanIndex1 || greaterThanIndex2 || greaterThanIndex3 || greaterThanIndex4)
            {
                throw new SchedulerException("The index is greater than the number of days");
            }

            var dateTime = ordinal switch
            {
                Ordinal.First => list[0],
                Ordinal.Second => list[1],
                Ordinal.Third => list[2],
                Ordinal.Fourth => list[3],
                Ordinal.Last => list.Last(),
                _ => DateTime.MinValue,
            };
            //saber si realmente existe un valor del index para ese elemento, sino devolver un error
            return dateTime;
        }

    }
}
