using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RetoScheduler
{
    public static class DescriptionBuilder
    {
        public static string GetListDayOfWeekInString(List<DayOfWeek> selectedDays)
        {
            StringBuilder formattedList = new StringBuilder("on");

            foreach (var item in selectedDays)
            {
                string dayInLower = item.ToString().ToLower();
                if (item == selectedDays.Last() && selectedDays.Count() >= 2)
                {
                    formattedList.Append(" and ");
                    formattedList.Append(dayInLower);
                    formattedList.Append(" ");
                }
                else
                {
                    string separator = item == selectedDays.First()
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
