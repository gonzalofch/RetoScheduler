using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RetoScheduler.Localization
{
    public static class SchedulerSpanishTexts
    {

        public static Dictionary<string, string> Traductions = new Dictionary<string, string>
        {
            {"Scheduler:Errors:NotEnabled", "Necesitas marcar el campo para ejecutar el Programador"},
            {"Scheduler:Errors:NullDateLimits", "Los límites no pueden ser nulos"},
            {"Scheduler:Errors:EndDateEarlierThanStartDate", "La fecha de fin no puede ser anterior a la fecha de inicio"},
            {"Scheduler:Errors:EndTimeEarlierThanStartTime", "La hora de fin no puede ser anterior a la hora de inicio"},
            {"Scheduler:Errors:RequiredConfigDateTimeInOnce", "Los tipos de Ocurrencia requieren una fecha y hora obligatoria"},
            {"Scheduler:Errors:NegativeDailyFrecuency", "No se deben introducir números negativos o cero en el campo de número"},
            {"Scheduler:Errors:DateOutOfRanges", "La fecha y hora no pueden estar fuera del rango de inicio y fin"},
            {"Scheduler:Errors:SelectedDaysIndexOutOfBounds", "El índice es mayor que el número de días"},
            {"Scheduler:Errors:NotSupportedSelectedWeekDay", "El tipo de día seleccionado no es compatible"},
            {"Scheduler:Errors:NotSupportedOrdinal", "El ordinal seleccionado no es compatible"},
            {"DescriptionBuilder:Errors:NotSupportedMonthlyDayNumber", "Acción no compatible para el número de día mensual"},
            {"DescriptionBuilder:Errors:NotSupportedMonthlyFrequency", "Acción no compatible para la frecuencia mensual"},
            {"DescriptionBuilder:Errors:NotSupportedWeeklyFrequency", "Acción no compatible para la frecuencia semanal"},
            {"DescriptionBuilder:Errors:NotSupportedDailyFrequency", "Acción no compatible para la frecuencia diaria"},
            {"Scheduler:String:OccursWithSpace", "Ocurre "},
            {"Scheduler:String:OnceAtWithSpace", "una vez el "},
            {"Scheduler:String:StartingOnWithSpace", "comenzando el "},
            {"Scheduler:String:AndFinishingOnWithSpaces", " y terminando el "},
            {"Scheduler:String:TheWithSpace", "el "},
            {"Scheduler:String:OfVeryWithSpace", "de cada "},
            {"Scheduler:String:OrdinalStWithSpace", "ero "},
            {"Scheduler:String:OrdinalNdWithSpace", "do "},
            {"Scheduler:String:OrdinalRdWithSpace", "ero "},
            {"Scheduler:String:OrdinalThWithSpace", "º "},
            {"Scheduler:String:MonthsWithSpace", "meses "},
            {"Scheduler:String:MonthsWithSpaces", " meses "},
            {"Scheduler:String:MonthWithSpaces", " mes "},
            {"Scheduler:String:WeeksWithSpaces", " semanas "},
            {"Scheduler:String:WeekWithSpaces", " semana "},
            {"Scheduler:String:Hours", "horas"},
            {"Scheduler:String:Minutes", "minutos"},
            {"Scheduler:String:Seconds", "segundos"},
            {"Scheduler:String:EveryWithSpace", "cada "},
            {"Scheduler:String:AndWithSpaces", " y "},
            {"Scheduler:String:AndWithSpace","y "},
            {"Scheduler:String:On", "en"},
            {"Scheduler:String:OneTimeAtWithSpace", "una vez a las "},
            {"Scheduler:String:BetweenWithSpaces", " entre las "},
            {"Scheduler:String:Day", "día "},

            {"Scheduler:DayOfWeek:Monday","lunes" },
            {"Scheduler:DayOfWeek:Tuesday","martes" },
            {"Scheduler:DayOfWeek:Wednesday","miercoles" },
            {"Scheduler:DayOfWeek:Thursday","jueves" },
            {"Scheduler:DayOfWeek:Friday","viernes" },
            {"Scheduler:DayOfWeek:Saturday","sabado" },
            {"Scheduler:DayOfWeek:Sunday","domingo" },
            {"Scheduler:KindOfDay:Day","día" },
            {"Scheduler:KindOfDay:WeekDay" ,"día de semana"},
            {"Scheduler:KindOfDay:WeekEndDay" ,"día de fin de semana"},

            { "Scheduler:String:OrdinalFirst", "primer"},
            { "Scheduler:String:OrdinalSecond", "segundo"},
            { "Scheduler:String:OrdinalThird", "tercer"},
            { "Scheduler:String:OrdinalFourth", "cuarto"},
            { "Scheduler:String:OrdinalLast", "último"},
        };
    }
}
