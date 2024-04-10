using RetoScheduler.Configurations;
using RetoScheduler.Enums;
using RetoScheduler.Exceptions;
using RetoScheduler.Extensions;
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

        private static DateTime AddWeeks(DateTime dateTime, int weeks)
        {
            return dateTime.AddDays(weeks*7);
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
            if (config.DailyConfiguration.Type == DailyConfigType.Once)
            {
                var occursOnceAt = config.DailyConfiguration.OnceAt;

                dateTime = dateTime + occursOnceAt.ToTimeSpan();
            }
            else
            {
                var startingTime = config.DailyConfiguration.TimeLimits.StartTime;
                dateTime = dateTime + startingTime.ToTimeSpan();
            }

            return dateTime;
        }

        private DateTime InRecurring(Configuration config)
        {
            if (config.DailyConfiguration.Frecuency <= 0)
            {
                throw new SchedulerException("Don't should put negative numbers or zero in number field");
            }
            DateTime dateTime;
            if (Executed)
            {
                dateTime = config.CurrentDate;
            }
            else
            {
                dateTime = config.CurrentDate < config.DateLimits.StartDate
               ? config.DateLimits.StartDate
               : config.CurrentDate;
            }

            //if (Executed)
            //{
            //    dateTime = dateTime.AddDays(1);

            //}
            //else
            //{

            //}

            var dateBetweenLimits = dateTime >= config.DateLimits.StartDate && (config.DateLimits.EndDate.HasValue == false || dateTime <= config.DateLimits.EndDate);
            if (dateBetweenLimits == false)
            {
                throw new SchedulerException("DateTime can't be out of start and end range");
            }

            if (config.WeeklyConfiguration != null && config.WeeklyConfiguration.SelectedDays.Count() != 0)
            {
                //identificar en que dia estas, si es dayofweek seleccionado, sino sumar 1 día 
                //luego identificar que el tiempo este dentro de timeLimits, si no esta, ir al timeLimits.startTime y que cambie el dateTime por ese.
                //revisar si ya se ejecuto antes, si ya se ejecuto antes, sumar las horas O MINUTOS del config.DailyConfig.Frecuency y que cambie el dateTime por ese valor.
                bool ejecutado = false;

                do
                {
                    if (config.WeeklyConfiguration.SelectedDays.Contains(dateTime.DayOfWeek))
                    {
                        var dateTimeTime = new TimeOnly(dateTime.Hour, dateTime.Minute, dateTime.Second);
                        if (dateTimeTime < config.DailyConfiguration.TimeLimits.StartTime)
                        {
                            dateTime = dateTime + config.DailyConfiguration.TimeLimits.StartTime.ToTimeSpan();
                            ejecutado = true;
                        }
                        else
                        {
                            //realizar operacion de suma y luego verificar si esta dentro de los tiempos limite, si esta dentro delos tiempos limite, retornar ese dateTime
                            //en caso de que la operacion sumar horas o minutos se salga del rango de tiempos limite, se suma un día y se realiza el do while otra vez (ejecutado =false)
                            //cuando llegue al domingo, va a saltar la ejecucion de 2 semanas en adelante, osea cambia el currentDate por dos semanas en adelante al primer lunes que encuentre

                            //if ( dateTimeTime > config.DailyConfiguration.TimeLimits.StartTime && dateTimeTime < config.DailyConfiguration.TimeLimits.EndTime )
                            //{
                            if (config.DailyConfiguration.DailyFrecuencyType == DailyFrecuency.Hours)
                            {
                                dateTime = dateTime.AddHours(config.DailyConfiguration.Frecuency.Value);
                                dateTimeTime = new TimeOnly(dateTime.Hour, dateTime.Minute, dateTime.Second); ;
                                if (dateTimeTime <= config.DailyConfiguration.TimeLimits.EndTime)
                                {
                                    ejecutado = true;
                                }
                                else
                                {
                                    if (dateTime.DayOfWeek == DayOfWeek.Sunday)
                                    {
                                        dateTime.Date.AddDays((7 * (config.WeeklyConfiguration.FrecuencyInWeeks)) + 1);
                                    }
                                    else
                                    {
                                        dateTime = dateTime.Date.AddDays(1);
                                    }
                                }
                            }
                            else
                            {
                                dateTime = dateTime.AddMinutes(config.DailyConfiguration.Frecuency.Value);
                                ejecutado = true;
                                dateTimeTime = new TimeOnly(dateTime.Hour, dateTime.Minute, dateTime.Second); ;

                                if (dateTimeTime <= config.DailyConfiguration.TimeLimits.EndTime)
                                {
                                    ejecutado = true;
                                }
                                else
                                {
                                    dateTime = dateTime.Date.AddDays(1);
                                }

                            }
                        }
                    }
                    else
                    {
                        dateTime = dateTime.Date.AddDays(1);
                        if (dateTime.DayOfWeek == DayOfWeek.Monday)
                        {
                            dateTime = dateTime.AddWeeks(config.WeeklyConfiguration.FrecuencyInWeeks);
                        }
                    }
                } while (!ejecutado);
            }
            else
            {
                if (config.DailyConfiguration.Type == DailyConfigType.Once)
                {
                    TimeOnly dateTimeTime = new TimeOnly(dateTime.Hour, dateTime.Minute, dateTime.Second);
                    if (dateTimeTime > config.DailyConfiguration.OnceAt)
                    {
                        throw new SchedulerException("Once time execution time can't be before than Current Time");
                    }
                    dateTime = dateTime.Date.Add(dateTimeTime.ToTimeSpan());
                }
                else
                {
                    var dateTimeTime = new TimeOnly(dateTime.Hour, dateTime.Minute, dateTime.Second);
                    TimeOnly nextExecutionTime;
                    //if (dateTimeTime <config.DailyConfiguration.TimeLimits.StartTime && dateTimeTime >config.DailyConfiguration.TimeLimits.EndTime)
                    //{
                    if (config.DailyConfiguration.DailyFrecuencyType == DailyFrecuency.Hours)
                    {
                        nextExecutionTime = dateTimeTime.AddHours(config.DailyConfiguration.Frecuency.Value);
                    }
                    else if(config.DailyConfiguration.DailyFrecuencyType == DailyFrecuency.Minutes)
                    {
                        nextExecutionTime = dateTimeTime.AddMinutes(config.DailyConfiguration.Frecuency.Value);
                    }
                    else
                    {
                        nextExecutionTime = dateTimeTime.AddSeconds(config.DailyConfiguration.Frecuency.Value);
                    }

                    if (nextExecutionTime < config.DailyConfiguration.TimeLimits.StartTime)
                    {
                        dateTime = dateTime.Date + config.DailyConfiguration.TimeLimits.StartTime.ToTimeSpan();
                    }
                    else
                    {
                        dateTime = dateTime.Date + nextExecutionTime.ToTimeSpan();
                    }
                }
                //}
            }
            return dateTime;
        }
    }
}
