using RetoScheduler.Configurations;
using RetoScheduler.Enums;
using RetoScheduler.Exceptions;
using System.Text;
using RetoScheduler.Extensions;
namespace RetoScheduler
{
    public static class DescriptionBuilder
    {

        private const string Space = " ";

        public static string CalculateDescription(DateTime dateTime, Configuration config)
        {
            StringBuilder description = new StringBuilder("Occurs ");

            if (config.Type == ConfigType.Once && config.ConfigDateTime.HasValue)
            {
                description.Append(GetOnceAtDescription(dateTime));
            }
            else
            {
                string configurationTypeDescription = config.MonthlyConfiguration != null
                    ? GetMonthlyDescription(config)
                    : GetWeeklyDescription(config);

                description.Append(configurationTypeDescription);
            }

            description.Append(GetDailyDescription(config));
            description.Append(GetDateLimitsDescription(config));

            return description.ToString();
        }

        private static string GetOnceAtDescription(DateTime dateTime)
        {
            return "once at " + dateTime.ToString("dd/MM/yyyy ");
        }

        private static string GetDateLimitsDescription(Configuration config)
        {
            string startDate = config.DateLimits.StartDate.Date.ToString("dd/MM/yyyy");
            string endDate = config.DateLimits.EndDate?.ToString("dd/MM/yyyy");
            return config.DateLimits.EndDate.HasValue
                ? "starting on " + startDate + " and finishing on " + endDate
                : "starting on " + startDate;
        }

        private static string GetMonthlyDescription(Configuration config)
        {
            string monthlyDescription = "the ";

            monthlyDescription += config.MonthlyConfiguration.Type == MonthlyConfigType.DayNumberOption
                ? GetMonthlyDayOfNumber(config)
                : GetMonthlyWeekdaysMessage(config);

            monthlyDescription += "of very ";
            monthlyDescription += GetMonthlyFrecuencyMessage(config);

            return monthlyDescription;
        }

        private static string GetMonthlyDayOfNumber(Configuration config)
        {
            int dayNumber = config.MonthlyConfiguration.DayNumber;
            return config.MonthlyConfiguration.DayNumber switch
            {
                1 or 21 or 31 => dayNumber + "st ",
                2 => dayNumber + "nd ",
                3 => dayNumber + "rd ",
                > 3 and < 32 => dayNumber + "th ",
                _ => throw new SchedulerException("Not supported action for monthly day number message"),
            };
        }

        private static string GetMonthlyWeekdaysMessage(Configuration config)
        {
            string ordinal = config.MonthlyConfiguration.OrdinalNumber.ToString().ToLower() + Space;
            string selectedWeekDay = config.MonthlyConfiguration.SelectedDay.ToString().ToLower() + Space;

            return ordinal + selectedWeekDay;
        }

        private static string GetMonthlyFrecuencyMessage(Configuration config)
        {
            return config.MonthlyConfiguration.Frecuency switch
            {
                0 => "months ",
                1 => config.MonthlyConfiguration.Frecuency + " month ",
                > 1 => config.MonthlyConfiguration.Frecuency + " months ",
                _ => throw new SchedulerException("Not supported action for monthly frecuency message"),
            };
        }

        private static string GetWeeklyDescription(Configuration config)
        {
            return config.WeeklyConfiguration != null
                ? "every " + GetWeeklyFrecuencyMessage(config)
                : "every " + "day ";
        }

        private static string GetWeeklyFrecuencyMessage(Configuration config)
        {
            string weeklyMessage = config.WeeklyConfiguration.FrecuencyInWeeks switch
            {
                0 => "week ",
                1 => config.WeeklyConfiguration.FrecuencyInWeeks + " week ",
                > 1 => config.WeeklyConfiguration.FrecuencyInWeeks + " weeks ",
                _ => throw new SchedulerException("Not supported action for weekly frecuency message"),
            };
            return weeklyMessage + DescriptionBuilder.GetListDayOfWeekInString(config.WeeklyConfiguration.SelectedDays);
        }
        private static string GetListDayOfWeekInString(List<DayOfWeek> selectedDays)
        {
            StringBuilder formattedList = new StringBuilder("on");

            foreach (var item in selectedDays)
            {
                string dayInLower = item.ToString().ToLower();
                if (item == selectedDays.Last() && selectedDays.Count() >= 2)
                {
                    formattedList.Append(" and ");
                    formattedList.Append(dayInLower);
                    formattedList.Append(Space);
                }
                else
                {
                    string separator = item == selectedDays.First()
                        ? Space
                        : ", ";

                    formattedList.Append(separator);
                    formattedList.Append(dayInLower);
                }
            }

            return formattedList.ToString();
        }

        private static string GetDailyDescription(Configuration config)
        {

            if (config.DailyConfiguration.Type == DailyConfigType.Once && config.DailyConfiguration.OnceAt != TimeOnly.MinValue)
            {
                string dailyExecutionTime = config.DailyConfiguration.OnceAt.ParseAmPm();
                return "one time at " + dailyExecutionTime + Space;
            }

            var limits = config.DailyConfiguration.TimeLimits;
            string dailyDescription = config.WeeklyConfiguration == null ? "and " : string.Empty;
            string timeStartLimit = limits.StartTime.ParseAmPm();
            string timeEndLimit = limits.EndTime.ParseAmPm();

            return dailyDescription + GetDailyFrecuencyMessage(config) + timeStartLimit + " and " + timeEndLimit + Space;
        }

        private static string GetDailyFrecuencyMessage(Configuration config)
        {
            string timeUnit = config.DailyConfiguration.DailyFrecuencyType switch
            {
                DailyFrecuency.Hours => "hours",
                DailyFrecuency.Minutes => "minutes",
                DailyFrecuency.Seconds => "seconds",
                _ => throw new SchedulerException("Not supported action for daily frecuency message"),
            };

            return "every " + config.DailyConfiguration.Frecuency + Space + timeUnit + " between ";
        }
    }
}
