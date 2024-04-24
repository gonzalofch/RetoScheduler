using RetoScheduler.Enums;
using RetoScheduler.Exceptions;

namespace RetoScheduler.Extensions
{
    public static class DateTimeExtensions
    {
        public static DateTime AddWeeks(this DateTime dateTime, int numberOfWeeks)
        {
            return dateTime.AddDays(numberOfWeeks * 7);
        }

        public static DateTime NextDayOfWeek(this DateTime dateTime, DayOfWeek nextDayOfWeek)
        {
            for (int addedDays = 0; dateTime.DayOfWeek != nextDayOfWeek; dateTime = dateTime.AddDays(1), addedDays++)
            {

            }
            return dateTime;
        }

        public static DateTime JumpToDayNumber(this DateTime dateTime, int dayNumber)
        {

            try
            {
                return new DateTime(dateTime.Year, dateTime.Month, dayNumber, dateTime.Hour, dateTime.Minute, dateTime.Second);
            }
            catch (Exception)
            {
                throw new SchedulerException("Jump to a day number that doesn't exists in the month");
            }
        }
    }
}
