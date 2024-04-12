using Microsoft.VisualBasic;
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

            DateTime dateTime = config.Type == ConfigType.Once
                ? InOnce(config)
                : InRecurring(config);

            ValidateNextExecutionIsBetweenDateLimits(config, dateTime);
            Executed = true;

            return new OutPut(dateTime, CalculateDescription(dateTime, config));
        }

        private void ValidateConfiguration(Configuration config)
        {
            ValidadIsEnabled(config);
            ValidateLimitsRange(config);
            ValidateLimitsTimeRange(config);
        }

        private static void ValidadIsEnabled(Configuration config)
        {
            if (!config.Enabled)
            {
                throw new SchedulerException("You need to check field to Run Program");
            }
        }

        private void ValidateLimitsRange(Configuration config)
        {
            if (config.DateLimits == null)
            {
                throw new SchedulerException("Limits Can`t be null");
            }
            if (config.DateLimits.StartDate > config.DateLimits.EndDate)
            {
                throw new SchedulerException("The end date cannot be earlier than the initial date");
            }
        }

        private void ValidateLimitsTimeRange(Configuration config)
        {
            if (config.DailyConfiguration.Type == DailyConfigType.Recurring)
            {
                if (config.DailyConfiguration.TimeLimits.EndTime.ToTimeSpan() < config.CurrentDate.TimeOfDay)
                {
                    throw new SchedulerException("The EndTime cannot be earlier than Current Time");
                }

                if (config.DailyConfiguration.TimeLimits.EndTime < config.DailyConfiguration.TimeLimits.StartTime)
                {
                    throw new SchedulerException("The EndTime cannot be earlier than StartTime");
                }
            }
        }
 
        private string CalculateDescription(DateTime dateTime, Configuration config)
        {
            //ACOTACIONES NECESARIAS 
            /** 
             * CASOS POSIBLES:
             * 
             * CONFIGURACION:
             * --------ONCE
             * SE LEE ONCE TIME AT  (SOLO FECHA)
             * PARA ESTE CASO TAMBIEN TENGO QUE AGREGAR NEXT EXECUTION TIME (CUANDO SE UTILIZARA EL SCHEDULER)
             * 
             * --------RECURRING
             * 
             * ||| si es que hay-------------- WEEKLY
             * |||SE LEE FRECUENCY IN WEEKS
             * |||SE LEEN LOS DIAS DE LA SEMANA DE LA LISTA
             * 
             * siempre-------------- DAILY
             * -------------------- OCCURS ONCE
             * SE LEE EL TIEMPO
             * -------------------- OCCURS EVERY
             * SE LEE LA FRECUENCIA, FRECUENCIA EN (HORAS,MINS,SECS)
             * 
             * siempre SE LEE START TIME 
             * ||| SE LEE END TIME SI ES QUE HAY
             * 
             **/

            StringBuilder description = new StringBuilder();
            description.Append("Occurs ");
            if (config.Type == ConfigType.Once)
            {
                //CONFIGURATION ONCE
                description.Append("once at ");
                var configurationOnceTimeAt = config.ConfigDateTime.Value.ToString("dd/MM/yyyy");
                var fechaDeEjecucion = dateTime.Date.ToString("dd/MM/yyyy");
                description.Append(configurationOnceTimeAt + " ");
            }
            else
            {
                description.Append("every ");
                //CONFIGURATION RECURRING

                if (config.WeeklyConfiguration != null)
                {

                    //weeklyconfig
                    var frecuenciaSemanal = config.WeeklyConfiguration.FrecuencyInWeeks;
                    string listaDeDíasFormateada = string.Empty;
                    listaDeDíasFormateada += "on";
                    var selectedDays = config.WeeklyConfiguration.SelectedDays;
                    //foreach para volver string de que dias estan seleccionados
                    foreach (var item in selectedDays)
                    {
                        string dayInLower = item.ToString().ToLower();
                        if (item == selectedDays.Last() && selectedDays.Count() >= 2)
                        {
                            listaDeDíasFormateada += " and " + dayInLower + " ";
                        }
                        else
                        {
                            if (item == selectedDays.First())
                            {
                                listaDeDíasFormateada += " ";
                            }
                            else
                            {
                                listaDeDíasFormateada += ", ";
                            }
                            listaDeDíasFormateada += dayInLower;
                        }
                    }
                    if (config.WeeklyConfiguration.FrecuencyInWeeks == 0)
                    {
                        description.Append("week ");
                    }
                    else if (config.WeeklyConfiguration.FrecuencyInWeeks == 1)
                    {
                        description.Append(frecuenciaSemanal + " week ");
                    }
                    else
                    {
                        description.Append(frecuenciaSemanal + " weeks ");
                    }
                    description.Append(listaDeDíasFormateada);
                }
                else
                {
                    //NO HAY WEEK CONFIG (daily diario)
                    description.Append("day ");
                }
            }

            //EN TODOS LOS CASOS HACER ESTO:

            //dailyconfig
            if (config.DailyConfiguration.Type == DailyConfigType.Once)
            {

                //OCCURS ONCE AT
                var tiempoDeEjecucion = config.DailyConfiguration.OnceAt.parseAmPm();
                description.Append("one time at " + tiempoDeEjecucion+ " ");
            }
            else
            {
                //OCCURS EVERY
                var frecuencyInDay = config.DailyConfiguration.Frecuency;
                string frecuenciaSeleccionadaFormateada = string.Empty;
                frecuenciaSeleccionadaFormateada = config.DailyConfiguration.DailyFrecuencyType switch
                {
                    DailyFrecuency.Hours => "hours",
                    DailyFrecuency.Minutes => "minutes",
                    DailyFrecuency.Seconds => "seconds",
                    _ => "ERROR",
                };
                var horaLimiteInicio= config.DailyConfiguration.TimeLimits.StartTime.parseAmPm();
                var horaLimiteFin =   config.DailyConfiguration.TimeLimits.EndTime.parseAmPm();
                if (config.WeeklyConfiguration == null)
                {
                    description.Append("and ");
                }
                description.Append("every " + frecuencyInDay + " " + frecuenciaSeleccionadaFormateada + " between " + horaLimiteInicio + " and " + horaLimiteFin + " ");
            }
            //LIMITES FECHA

            var fechaLimiteInicio = config.DateLimits.StartDate.Date.ToString("dd/MM/yyyy");
            

            description.Append("starting on " + fechaLimiteInicio);
            if (config.DateLimits.EndDate.HasValue)
            {
                description.Append(" and finishing on " + config.DateLimits.EndDate.Value.ToString("dd/MM/yyyy"));
            }

            return description.ToString();
        }

        private DateTime InOnce(Configuration config)
        {
            if (config.ConfigDateTime.HasValue == false)
            {
                throw new SchedulerException("Once Types requires an obligatory DateTime");
            }

            return config.DailyConfiguration.Type == DailyConfigType.Once
                ? config.ConfigDateTime.Value + config.DailyConfiguration.OnceAt.ToTimeSpan()
                : config.ConfigDateTime.Value + config.DailyConfiguration.TimeLimits.StartTime.ToTimeSpan();
        }

        private DateTime GetFirstExecutionDate(Configuration config)
        {

            return config.CurrentDate < config.DateLimits.StartDate
           ? config.DateLimits.StartDate
           : config.CurrentDate;
        }

        private DateTime InRecurring(Configuration config)
        {
            if (config.DailyConfiguration.Frecuency <= 0)
            {
                throw new SchedulerException("Don't should put negative numbers or zero in number field");
            }

            var dateTime = Executed
                ? config.CurrentDate
                : GetFirstExecutionDate(config);

            dateTime = NextDayExecution(config, dateTime);

            TimeOnly dateTimeTime = TimeOnly.FromDateTime(dateTime);
            if (config.DailyConfiguration.Type == DailyConfigType.Once)
            {
                return dateTime.Date.Add(config.DailyConfiguration.OnceAt.ToTimeSpan());
            }

            TimeOnly nextExecutionTime = AddOccursEveryUnit(config, dateTimeTime);

            return nextExecutionTime < config.DailyConfiguration.TimeLimits.StartTime
                ? dateTime.Date + config.DailyConfiguration.TimeLimits.StartTime.ToTimeSpan()
                : dateTime.Date + nextExecutionTime.ToTimeSpan();
        }

        private static void ValidateNextExecutionIsBetweenDateLimits(Configuration config, DateTime dateTime)
        {
            var dateBetweenLimits = dateTime >= config.DateLimits.StartDate && (config.DateLimits.EndDate.HasValue == false || dateTime <= config.DateLimits.EndDate);
            if (dateBetweenLimits is false)
            {
                throw new SchedulerException("DateTime can't be out of start and end range");
            }
            if (dateTime < config.CurrentDate)
            {
                throw new SchedulerException("Execution Time can't be earlier than Current Date");
            }
        }

        private DateTime NextDayExecution(Configuration config, DateTime dateTime)
        {
            if (config.WeeklyConfiguration == null || !config.WeeklyConfiguration.SelectedDays.Any())
            {
                return dateTime;
            }

            var sortedDays = config.WeeklyConfiguration.SelectedDays.OrderBy(_ => _).ToList();
            var actualDay = config.CurrentDate.DayOfWeek;
            var hasLimits = config.DailyConfiguration.TimeLimits != null;
            var excedsLimits = hasLimits && config.CurrentDate.TimeOfDay >= config.DailyConfiguration.TimeLimits.EndTime.ToTimeSpan();
            var skipDay = excedsLimits || (config.DailyConfiguration.Type == DailyConfigType.Once && Executed);

            var firstDayOfWeek = skipDay
                ? sortedDays.FirstOrDefault(_ => _ > actualDay)
                : sortedDays.FirstOrDefault(_ => _ >= actualDay);

            return skipDay
                ? GetNextDayInWeek(sortedDays, actualDay, dateTime, config)
                : NextDay(sortedDays, actualDay, dateTime);
        }

        private static DateTime NextDay(List<DayOfWeek> sortedDays, DayOfWeek actualDay, DateTime dateTime)
        {
            var nextDay = sortedDays.FirstOrDefault(_ => _ >= actualDay);

            return dateTime.NextDayOfWeek(nextDay);
        }

        private static DateTime GetNextDayInWeek(List<DayOfWeek> sortedDays, DayOfWeek actualDay, DateTime dateTime, Configuration config)
        {
            var nextDay = sortedDays.FirstOrDefault(_ => _ > actualDay);
            if (!sortedDays.Contains(nextDay))
            {
                nextDay = sortedDays.FirstOrDefault(_ => _ > dateTime.NextDayOfWeek(nextDay).DayOfWeek);
                dateTime = dateTime.AddWeeks(config.WeeklyConfiguration.FrecuencyInWeeks);
            }

            return dateTime.NextDayOfWeek(nextDay).Date;
        }

        private static TimeOnly AddOccursEveryUnit(Configuration config, TimeOnly dateTimeTime)
        {
            if (dateTimeTime == TimeOnly.MinValue)
            {
                return config.DailyConfiguration.TimeLimits.StartTime;
            }

            return config.DailyConfiguration.DailyFrecuencyType switch
            {
                DailyFrecuency.Hours => dateTimeTime.AddHours(config.DailyConfiguration.Frecuency.Value),
                DailyFrecuency.Minutes => dateTimeTime.AddMinutes(config.DailyConfiguration.Frecuency.Value),
                DailyFrecuency.Seconds => dateTimeTime.AddSeconds(config.DailyConfiguration.Frecuency.Value),
                _ => dateTimeTime,
            };
        }
    }
}
