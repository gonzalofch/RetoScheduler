using BenchmarkDotNet.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RetoScheduler.Benchmarks
{
    public class GetListDayOfWeekInStringBenchmark
    {
        private List<DayOfWeek> _days;

        public GetListDayOfWeekInStringBenchmark()
        {
            _days = new List<DayOfWeek>() { DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Friday, DayOfWeek.Saturday, DayOfWeek.Sunday };
        }

        [Benchmark]
        public string GetListDayOfWeekInString()
        {
            string formattedList = "on";

            foreach (var item in _days)
            {
                string dayInLower = item.ToString().ToLower();
                if (item == _days.Last() && _days.Count() >= 2)
                {
                    formattedList += " and " + dayInLower + " ";
                }
                else
                {
                    formattedList += item == _days.First()
                        ? " "
                        : ", ";

                    formattedList += dayInLower;
                }
            }

            return formattedList;
        }

        [Benchmark]
        public string GetListDayOfWeekInStringWithStringBuilder()
        {
            StringBuilder formattedList = new StringBuilder("on");

            foreach (var item in _days)
            {
                string dayInLower = item.ToString().ToLower();
                if (item == _days.Last() && _days.Count() >= 2)
                {
                    formattedList.Append(" and ");
                    formattedList.Append(dayInLower);
                    formattedList.Append(" ");
                }
                else
                {
                    string separator = item == _days.First()
                        ? " "
                        : ", ";

                    formattedList.Append(separator);
                    formattedList.Append(dayInLower);
                }
            }

            return formattedList.ToString();
        }
    }
}
