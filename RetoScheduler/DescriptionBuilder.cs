using Microsoft.Extensions.Localization;
using RetoScheduler.Configurations;
using RetoScheduler.Enums;
using RetoScheduler.Exceptions;
using RetoScheduler.Localization;
using System.Globalization;
using System.Text;
namespace RetoScheduler
{
    public class DescriptionBuilder
    {
        private const string Space = " ";

        private readonly IStringLocalizer _stringLocalizer;

        public DescriptionBuilder(IStringLocalizer stringLocalizer)
        {
            _stringLocalizer = stringLocalizer;
        }

        public string CalculateDescription(DateTime dateTime, Configuration config)
        {
            StringBuilder description = new StringBuilder(_stringLocalizer["Scheduler:String:OccursWithSpace"]);

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
            return _stringLocalizer["Scheduler:String:OnceAtWithSpace"] + dateTime.ToShortDateString() + Space;
        }

        private string GetDateLimitsDescription(Configuration config)
        {
            string startDate = config.DateLimits.StartDate.Date.ToShortDateString();
            string endDate = config.DateLimits.EndDate?.ToShortDateString();

            return config.DateLimits.EndDate.HasValue
                ? _stringLocalizer["Scheduler:String:StartingOnWithSpace"] + startDate + _stringLocalizer["Scheduler:String:AndFinishingOnWithSpaces"] + endDate
                : _stringLocalizer["Scheduler:String:StartingOnWithSpace"] + startDate;
        }

        private string GetMonthlyDescription(Configuration config)
        {
            string monthlyDescription = _stringLocalizer["Scheduler:String:TheWithSpace"];

            monthlyDescription += config.MonthlyConfiguration.Type == MonthlyConfigType.DayNumberOption
                ? GetMonthlyDayOfNumber(config)
                : GetMonthlyWeekdaysMessage(config);

            monthlyDescription += _stringLocalizer["Scheduler:String:OfVeryWithSpace"];
            monthlyDescription += GetMonthlyFrecuencyMessage(config);

            return monthlyDescription;
        }

        private string GetMonthlyDayOfNumber(Configuration config)
        {
            int dayNumber = config.MonthlyConfiguration.DayNumber;
            return config.MonthlyConfiguration.DayNumber switch
            {
                1 or 21 or 31 => dayNumber + _stringLocalizer["Scheduler:String:OrdinalStWithSpace"],
                2 => dayNumber + _stringLocalizer["Scheduler:String:OrdinalNdWithSpace"],
                3 => dayNumber + _stringLocalizer["Scheduler:String:OrdinalRdWithSpace"],
                > 3 and < 32 => dayNumber + _stringLocalizer["Scheduler:String:OrdinalThWithSpace"],
                _ => throw new SchedulerException(_stringLocalizer["DescriptionBuilder:Errors:NotSupportedMonthlyDayNumber"]),
            };
        }

        private string GetMonthlyWeekdaysMessage(Configuration config)
        {
            string ordinal = GetOrdinalInString(config) + Space;
            string selectedWeekDay = GetKindOfDayInString(config) + Space;

            return ordinal + selectedWeekDay;
        }

        private string GetOrdinalInString(Configuration config)
        {
            return config.MonthlyConfiguration.OrdinalNumber switch
            {
                Ordinal.First => _stringLocalizer["Scheduler:String:OrdinalFirst"],
                Ordinal.Second => _stringLocalizer["Scheduler:String:OrdinalSecond"],
                Ordinal.Third => _stringLocalizer["Scheduler:String:OrdinalThird"],
                Ordinal.Fourth => _stringLocalizer["Scheduler:String:OrdinalFourth"],
                Ordinal.Last => _stringLocalizer["Scheduler:String:OrdinalLast"],
                _ => throw new SchedulerException(_stringLocalizer["DescriptionBuilder:Errors:NotSupportedMonthlyDayNumber"]),
            };
        }

        private string GetKindOfDayInString(Configuration config)
        {
            return config.MonthlyConfiguration.SelectedDay switch
            {
                KindOfDay.Sunday => _stringLocalizer["Scheduler:DayOfWeek:Sunday"] ,
                KindOfDay.Monday => _stringLocalizer["Scheduler:DayOfWeek:Monday"] ,
                KindOfDay.Tuesday => _stringLocalizer["Scheduler:DayOfWeek:Tuesday"],
                KindOfDay.Wednesday => _stringLocalizer["Scheduler:DayOfWeek:Wednesday"],
                KindOfDay.Thursday => _stringLocalizer["Scheduler:DayOfWeek:Thursday"],
                KindOfDay.Friday => _stringLocalizer["Scheduler:DayOfWeek:Friday"],
                KindOfDay.Saturday => _stringLocalizer["Scheduler:DayOfWeek:Saturday"],
                KindOfDay.Day => _stringLocalizer["Scheduler:KindOfDay:Day"],
                KindOfDay.WeekDay => _stringLocalizer["Scheduler:KindOfDay:WeekDay"],
                KindOfDay.WeekEndDay => _stringLocalizer["Scheduler:KindOfDay:WeekEndDay"],
                _ => throw new SchedulerException(_stringLocalizer["DescriptionBuilder:Errors:NotSupportedWeeklyFrequency"]),
            };
        }

        private string GetMonthlyFrecuencyMessage(Configuration config)
        {
            return config.MonthlyConfiguration.Frecuency switch
            {
                0 => _stringLocalizer["Scheduler:String:MonthsWithSpace"],
                1 => config.MonthlyConfiguration.Frecuency + _stringLocalizer["Scheduler:String:MonthWithSpaces"],
                > 1 => config.MonthlyConfiguration.Frecuency + _stringLocalizer["Scheduler:String:MonthsWithSpaces"],
                _ => throw new SchedulerException(_stringLocalizer["DescriptionBuilder:Errors:NotSupportedMonthlyFrequency"]),
            };
        }

        private string GetWeeklyDescription(Configuration config)
        {
            return config.WeeklyConfiguration != null
                ? _stringLocalizer["Scheduler:String:EveryWithSpace"] + GetWeeklyFrecuencyMessage(config)
                : _stringLocalizer["Scheduler:String:EveryWithSpace"] + _stringLocalizer["Scheduler:String:Day"];
        }

        private string GetWeeklyFrecuencyMessage(Configuration config)
        {
            string weeklyMessage = config.WeeklyConfiguration.FrecuencyInWeeks switch
            {
                0 => _stringLocalizer["Scheduler:String:WeekWithSpace"],
                1 => config.WeeklyConfiguration.FrecuencyInWeeks + _stringLocalizer["Scheduler:String:WeekWithSpaces"],
                > 1 => config.WeeklyConfiguration.FrecuencyInWeeks + _stringLocalizer["Scheduler:String:WeeksWithSpaces"],
                _ => throw new SchedulerException(_stringLocalizer["DescriptionBuilder:Errors:NotSupportedWeeklyFrequency"]),
            };
            return weeklyMessage + GetListDayOfWeekInString(config.WeeklyConfiguration.SelectedDays);
        }

        private string GetListDayOfWeekInString(List<DayOfWeek> selectedDays)
        {
            StringBuilder formattedList = new StringBuilder(_stringLocalizer["Scheduler:String:On"]);

            foreach (var item in selectedDays)
            {
                string dayOfWeek = GetDayOfWeekInString(item);
                if (item == selectedDays.Last() && selectedDays.Count() >= 2)
                {
                    formattedList.Append(_stringLocalizer["Scheduler:String:AndWithSpaces"]);
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

        private string GetDayOfWeekInString(DayOfWeek item)
        {
            return item switch
            {
                DayOfWeek.Monday => _stringLocalizer["Scheduler:DayOfWeek:Monday"],
                DayOfWeek.Tuesday => _stringLocalizer["Scheduler:DayOfWeek:Tuesday"],
                DayOfWeek.Wednesday => _stringLocalizer["Scheduler:DayOfWeek:Wednesday"],
                DayOfWeek.Thursday => _stringLocalizer["Scheduler:DayOfWeek:Thursday"],
                DayOfWeek.Friday => _stringLocalizer["Scheduler:DayOfWeek:Friday"],
                DayOfWeek.Saturday => _stringLocalizer["Scheduler:DayOfWeek:Saturday"],
                DayOfWeek.Sunday => _stringLocalizer["Scheduler:DayOfWeek:Sunday"],
                _ => throw new SchedulerException(_stringLocalizer["DescriptionBuilder:Errors:NotSupportedWeeklyFrequency"]),
            };
        }

        private string GetDailyDescription(Configuration config)
        {
            if (config.DailyConfiguration.Type == DailyConfigType.Once && config.DailyConfiguration.OnceAt != TimeOnly.MinValue)
            {
                string dailyExecutionTime = config.DailyConfiguration.OnceAt.ToString("HH:mm:ss", CultureInfo.CurrentCulture);
                return _stringLocalizer["Scheduler:String:OneTimeAtWithSpace"] + dailyExecutionTime + Space;
            }

            var limits = config.DailyConfiguration.TimeLimits;
            string dailyDescription = config.WeeklyConfiguration == null ? _stringLocalizer["Scheduler:String:AndWithSpace"] : string.Empty;
            string timeStartLimit = limits.StartTime.ToString("HH:mm:ss", CultureInfo.CurrentCulture);
            string timeEndLimit = limits.EndTime.ToString("HH:mm:ss", CultureInfo.CurrentCulture);

            return dailyDescription + GetDailyFrecuencyMessage(config) + timeStartLimit + _stringLocalizer["Scheduler:String:AndWithSpaces"] + timeEndLimit + Space;
        }

        private string GetDailyFrecuencyMessage(Configuration config)
        {
            string timeUnit = config.DailyConfiguration.DailyFrecuencyType switch
            {
                DailyFrecuency.Hours => _stringLocalizer["Scheduler:String:Hours"],
                DailyFrecuency.Minutes => _stringLocalizer["Scheduler:String:Minutes"],
                DailyFrecuency.Seconds => _stringLocalizer["Scheduler:String:Seconds"],
                _ => throw new SchedulerException(_stringLocalizer["DescriptionBuilder:Errors:NotSupportedDailyFrequency"]),
            };

            return _stringLocalizer["Scheduler:String:EveryWithSpace"] + config.DailyConfiguration.Frecuency + Space + timeUnit + _stringLocalizer["Scheduler:String:BetweenWithSpaces"];
        }
    }
}
