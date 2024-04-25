using RetoScheduler.Configurations;
using RetoScheduler.Enums;
using RetoScheduler.Exceptions;
using System.Text;
using RetoScheduler.Extensions;
using RetoScheduler.Localization;
using Microsoft.Extensions.Localization;
using System.Globalization;
using System.Security.Cryptography;
namespace RetoScheduler
{
    public class DescriptionBuilder
    {

        private const string Space = " ";

        private SchedulerLocalizer localizer = new SchedulerLocalizer();

        public string CalculateDescription(DateTime dateTime, Configuration config)
        {
            StringBuilder description = new StringBuilder(localizer["Scheduler:String:OccursWithSpace"]);

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

        private string GetOnceAtDescription(DateTime dateTime)
        {
            return localizer["Scheduler:String:OnceAtWithSpace"] + dateTime.ToShortDateString() + Space;
        }

        private string GetDateLimitsDescription(Configuration config)
        {
            string startDate = config.DateLimits.StartDate.Date.ToShortDateString();
            string endDate = config.DateLimits.EndDate?.ToShortDateString();
            return config.DateLimits.EndDate.HasValue
                ? localizer["Scheduler:String:StartingOnWithSpace"] + startDate + localizer["Scheduler:String:AndFinishingOnWithSpaces"] + endDate
                : localizer["Scheduler:String:StartingOnWithSpace"] + startDate;
        }

        private string GetMonthlyDescription(Configuration config)
        {
            string monthlyDescription = localizer["Scheduler:String:TheWithSpace"];

            monthlyDescription += config.MonthlyConfiguration.Type == MonthlyConfigType.DayNumberOption
                ? GetMonthlyDayOfNumber(config)
                : GetMonthlyWeekdaysMessage(config);

            monthlyDescription += localizer["Scheduler:String:OfVeryWithSpace"];
            monthlyDescription += GetMonthlyFrecuencyMessage(config);

            return monthlyDescription;
        }

        private string GetMonthlyDayOfNumber(Configuration config)
        {
            int dayNumber = config.MonthlyConfiguration.DayNumber;
            return config.MonthlyConfiguration.DayNumber switch
            {
                1 or 21 or 31 => dayNumber + localizer["Scheduler:String:OrdinalStWithSpace"],
                2 => dayNumber + localizer["Scheduler:String:OrdinalNdWithSpace"],
                3 => dayNumber + localizer["Scheduler:String:OrdinalRdWithSpace"],
                > 3 and < 32 => dayNumber + localizer["Scheduler:String:OrdinalThWithSpace"],
                _ => throw new SchedulerException(localizer["DescriptionBuilder:Errors:NotSupportedMonthlyDayNumber"]),
            };
        }

        private string GetMonthlyWeekdaysMessage(Configuration config)
        {
            string ordinal = (int)config.MonthlyConfiguration.OrdinalNumber switch
            {
                1 => localizer["Scheduler:String:OrdinalFirst"] + Space,
                2 => localizer["Scheduler:String:OrdinalSecond"] + Space,
                3 => localizer["Scheduler:String:OrdinalThird"] + Space,
                4 => localizer["Scheduler:String:OrdinalFourth"] + Space,
                5 => localizer["Scheduler:String:OrdinalLast"] + Space,
                _ => throw new SchedulerException(localizer["DescriptionBuilder:Errors:NotSupportedMonthlyDayNumber"]),
            };
            string selectedWeekDay = (int)config.MonthlyConfiguration.SelectedDay switch
            {
                0 => localizer["Scheduler:DayOfWeek:Sunday"] + Space,
                1 => localizer["Scheduler:DayOfWeek:Monday"] + Space,
                2 => localizer["Scheduler:DayOfWeek:Tuesday"] + Space,
                3 => localizer["Scheduler:DayOfWeek:Wednesday"] + Space,
                4 => localizer["Scheduler:DayOfWeek:Thursday"] + Space,
                5 => localizer["Scheduler:DayOfWeek:Friday"] + Space,
                6 => localizer["Scheduler:DayOfWeek:Saturday"] + Space,
                7 => localizer["Scheduler:KindOfDay:Day"] + Space,
                8 => localizer["Scheduler:KindOfDay:WeekDay"] + Space,
                9 => localizer["Scheduler:KindOfDay:WeekEndDay"] + Space,
                _ => throw new SchedulerException(localizer["DescriptionBuilder:Errors:NotSupportedWeeklyFrequency"]),
            };

            return ordinal + selectedWeekDay;
        }

        private string GetMonthlyFrecuencyMessage(Configuration config)
        {
            return config.MonthlyConfiguration.Frecuency switch
            {
                0 => localizer["Scheduler:String:MonthsWithSpace"],
                1 => config.MonthlyConfiguration.Frecuency + localizer["Scheduler:String:MonthWithSpaces"],
                > 1 => config.MonthlyConfiguration.Frecuency + localizer["Scheduler:String:MonthsWithSpaces"],
                _ => throw new SchedulerException(localizer["DescriptionBuilder:Errors:NotSupportedMonthlyFrequency"]),
            };
        }

        private string GetWeeklyDescription(Configuration config)
        {
            return config.WeeklyConfiguration != null
                ? localizer["Scheduler:String:EveryWithSpace"] + GetWeeklyFrecuencyMessage(config)
                : localizer["Scheduler:String:EveryWithSpace"] + localizer["Scheduler:String:Day"];
        }

        private string GetWeeklyFrecuencyMessage(Configuration config)
        {
            string weeklyMessage = config.WeeklyConfiguration.FrecuencyInWeeks switch
            {
                0 => localizer["Scheduler:String:WeekWithSpace"],
                1 => config.WeeklyConfiguration.FrecuencyInWeeks + localizer["Scheduler:String:WeekWithSpaces"],
                > 1 => config.WeeklyConfiguration.FrecuencyInWeeks + localizer["Scheduler:String:WeeksWithSpaces"],
                _ => throw new SchedulerException(localizer["DescriptionBuilder:Errors:NotSupportedWeeklyFrequency"]),
            };
            return weeklyMessage + GetListDayOfWeekInString(config.WeeklyConfiguration.SelectedDays);
        }

        private string GetListDayOfWeekInString(List<DayOfWeek> selectedDays)
        {
            StringBuilder formattedList = new StringBuilder(localizer["Scheduler:String:On"]);

            foreach (var item in selectedDays)
            {
                string dayOfWeek = (int)item switch
                {
                    1 => localizer["Scheduler:DayOfWeek:Monday"],
                    2 => localizer["Scheduler:DayOfWeek:Tuesday"],
                    3 => localizer["Scheduler:DayOfWeek:Wednesday"],
                    4 => localizer["Scheduler:DayOfWeek:Thursday"],
                    5 => localizer["Scheduler:DayOfWeek:Friday"],
                    6 => localizer["Scheduler:DayOfWeek:Saturday"],
                    0 => localizer["Scheduler:DayOfWeek:Sunday"],
                    _ => throw new SchedulerException(localizer["DescriptionBuilder:Errors:NotSupportedWeeklyFrequency"]),
                };
                if (item == selectedDays.Last() && selectedDays.Count() >= 2)
                {
                    formattedList.Append(localizer["Scheduler:String:AndWithSpaces"]);
                    formattedList.Append(dayOfWeek);
                    formattedList.Append(Space);
                }
                else
                {
                    string separator = item == selectedDays.First()
                        ? Space
                        : ", ";

                    formattedList.Append(separator);
                    formattedList.Append(dayOfWeek);
                }
            }

            return formattedList.ToString();
        }

        private string GetDailyDescription(Configuration config)
        {
            //HH:mm:ss
            if (config.DailyConfiguration.Type == DailyConfigType.Once && config.DailyConfiguration.OnceAt != TimeOnly.MinValue)
            {
                string dailyExecutionTime = config.DailyConfiguration.OnceAt.ToString("HH:mm:ss", CultureInfo.CurrentCulture);
                return localizer["Scheduler:String:OneTimeAtWithSpace"] + dailyExecutionTime + Space;
            }

            var limits = config.DailyConfiguration.TimeLimits;
            string dailyDescription = config.WeeklyConfiguration == null ? localizer["Scheduler:String:AndWithSpace"] : string.Empty;
            string timeStartLimit = limits.StartTime.ToString("HH:mm:ss", CultureInfo.CurrentCulture);
            string timeEndLimit = limits.EndTime.ToString("HH:mm:ss", CultureInfo.CurrentCulture);

            return dailyDescription + GetDailyFrecuencyMessage(config) + timeStartLimit + localizer["Scheduler:String:AndWithSpaces"] + timeEndLimit + Space;
        }

        private string GetDailyFrecuencyMessage(Configuration config)
        {
            string timeUnit = config.DailyConfiguration.DailyFrecuencyType switch
            {
                DailyFrecuency.Hours => localizer["Scheduler:String:Hours"],
                DailyFrecuency.Minutes => localizer["Scheduler:String:Minutes"],
                DailyFrecuency.Seconds => localizer["Scheduler:String:Seconds"],
                _ => throw new SchedulerException(localizer["DescriptionBuilder:Errors:NotSupportedDailyFrequency"]),
            };

            return localizer["Scheduler:String:EveryWithSpace"] + config.DailyConfiguration.Frecuency + Space + timeUnit + localizer["Scheduler:String:BetweenWithSpaces"];
        }
    }
}
