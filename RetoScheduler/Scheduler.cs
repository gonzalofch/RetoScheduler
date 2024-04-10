using RetoScheduler.Configurations;
using RetoScheduler.Enums;
using RetoScheduler.Exceptions;
using System;
using System.Security.AccessControl;
using System.Security.Authentication.ExtendedProtection;
using System.Text;

namespace RetoScheduler
{
    public class Scheduler
    {

        public Scheduler()
        {

        }

        private bool Executed { get; set; }

        public OutPut Execute(Configuration config)
        {
            ValidateConfiguration(config);

            DateTime dateTime;
            if (config.Type == ConfigType.Once)
            {
                dateTime = InOnce(config);
            }
            else
            {
                dateTime = InRecurring(config);
            }
            var description = CalculateDescription(dateTime, config);
            Executed = true;
            return new OutPut(dateTime, description);
        }

        private void ValidateConfiguration(Configuration config)
        {
            if (!config.Enabled)
            {
                throw new SchedulerException("You need to check field to Run Program");
            }
            if (config.DateLimits == null)
            {
                throw new SchedulerException("Limits Can`t be null");
            }
            ValidateLimitsRange(config);
        }

        private void ValidateLimitsRange(Configuration config)
        {
            if (config.DateLimits.StartDate > config.DateLimits.EndDate)
            {
                throw new SchedulerException("The end date cannot be earlier than the initial date");
            }
        }

        private string CalculateDescription(DateTime dateTime, Configuration config)
        {
            StringBuilder description = new StringBuilder("Occurs ");

            string whenOccurs = "";
            string atThisTime;
            if (config.Type == ConfigType.Once)
            {
                whenOccurs += "once";
                string expectedTime = dateTime.ToString("HH:mm");
                atThisTime = " at " + expectedTime;
            }
            else
            {
                if (config.Occurs == Occurs.Weekly)
                {
                    whenOccurs += config.DailyConfiguration.Frecuency == 1
               ? "every week "
               : "every " + config.WeeklyConfiguration.FrecuencyInWeeks + " weeks ";
                    atThisTime = string.Empty;

                    whenOccurs += "on";
                    var selectedDays = config.WeeklyConfiguration.SelectedDays;
                    foreach (var item in selectedDays)
                    {
                        string dayInLower = item.ToString().ToLower();
                        if (item == selectedDays.Last() && selectedDays.Count() >= 2)
                        {
                            whenOccurs += " and " + dayInLower + " ";
                        }
                        else
                        {
                            if (item == selectedDays.First())
                            {
                                whenOccurs += " ";
                            }
                            else
                            {
                                whenOccurs += ", ";
                            }
                            whenOccurs += dayInLower;

                        }
                    }

                }
                //DAILY
                else
                {
                    //    whenOccurs+=config.DailyConfiguration.Frecuency==1
                    //     ? "every day"
                    //: "every " + config. + " days";
                    atThisTime = string.Empty;

                }

            }

            string atThisTimeInDay = string.Empty;
            if (config.DailyConfiguration.Type == DailyConfigType.Once)
            {
                var timeInDay = config.DailyConfiguration.OnceAt;
                atThisTimeInDay = "once at" + timeInDay;
            }
            else
            {


                TimeOnly startTime = config.DailyConfiguration.TimeLimits.StartTime; ;
                string startTimeFormat = (startTime.Hour >= 12) 
                ? (startTime.AddHours(-12)).ToString("h:mm tt", System.Globalization.CultureInfo.InvariantCulture)
                : startTime.ToString("h:mm tt", System.Globalization.CultureInfo.InvariantCulture);

                TimeOnly endTime = config.DailyConfiguration.TimeLimits.EndTime; ;
                string endTimeFormat = (endTime.Hour >= 12) 
                ? (endTime.AddHours(-12)).ToString("h:mm tt", System.Globalization.CultureInfo.InvariantCulture)  //(endTime.AddHours(-12)).ToString("h:mm tt", System.Globalization.CultureInfo.InvariantCulture) :
                : endTime.ToString("h:mm tt", System.Globalization.CultureInfo.InvariantCulture);

                atThisTimeInDay += config.DailyConfiguration.DailyFrecuencyType == DailyFrecuency.Hours
                    ? "every " + config.DailyConfiguration.Frecuency + " hours "
                    : "every " + config.DailyConfiguration.Frecuency + " minutes ";

                atThisTimeInDay += "between " + startTimeFormat + " and " + endTimeFormat;
            }

            string expectedDate = dateTime.ToString("dd/MM/yyyy");
            string expectedStartLimit = config.DateLimits.StartDate.ToString("dd/MM/yyyy");

            description.Append(whenOccurs)
                .Append(atThisTime)
                .Append(atThisTimeInDay)
                .Append(" starting on ")
                .Append(expectedStartLimit);

            return description.ToString();
        }

        private DateTime InOnce(Configuration config)
        {
            if (config.ConfigDateTime.HasValue == false)
            {
                throw new SchedulerException("Once Types requires an obligatory DateTime");
            }

            var dateTime = config.ConfigDateTime.Value;
            var occursOnceAt = config.DailyConfiguration.OnceAt;
            if (config.DailyConfiguration.Type==DailyConfigType.Once)
            {
                dateTime = dateTime + occursOnceAt.ToTimeSpan();
            }
            else
            {

            }

            return dateTime;
        }

        private DateTime InRecurring(Configuration config)
        {
            if (config.DailyConfiguration.Frecuency <= 0)
            {
                throw new SchedulerException("Don't should put negative numbers or zero in number field");
            }
            var dateTime = config.CurrentDate < config.DateLimits.StartDate
                ? config.DateLimits.StartDate
                : config.CurrentDate;

            if (Executed)
            {
                dateTime = dateTime.AddDays(1);

            }
            var dateBetweenLimits = dateTime >= config.DateLimits.StartDate && (config.DateLimits.EndDate.HasValue == false || dateTime <= config.DateLimits.EndDate);
            if (dateBetweenLimits == false)
            {
                throw new SchedulerException("DateTime can't be out of start and end range");
            }

            if ( config.WeeklyConfiguration!=null && config.WeeklyConfiguration.SelectedDays.Count() != 0)
            {
                do
                {
                    dateTime = dateTime.AddDays(1);
                } while (!config.WeeklyConfiguration.SelectedDays.Contains(dateTime.DayOfWeek));
                TimeOnly horaInicio = config.DailyConfiguration.TimeLimits.StartTime;
                dateTime = dateTime.AddHours(horaInicio.Hour).AddMinutes(horaInicio.Minute).AddSeconds(horaInicio.Second);
            }

            return dateTime;
        }
    }
}
