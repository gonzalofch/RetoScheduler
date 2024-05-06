using RetoScheduler;
using RetoScheduler.Configurations;
using RetoScheduler.Configurations.Limits;
using RetoScheduler.Enums;
using System.Collections;

namespace RetoSchedulerTest
{
    public class SchedulerTestSpanishTexts : IEnumerable<object[]>
    {
        public readonly List<object[]> data = new List<object[]>()
        {
            new object[]{
                new Configuration
                (new DateTime(2020,1,1,0,0,0), ConfigType.Recurring,true,null,Occurs.Weekly,null,new WeeklyConfiguration(2, new List<DayOfWeek>(){DayOfWeek.Monday,DayOfWeek.Thursday,DayOfWeek.Friday}),
                    DailyConfiguration.Recurring(2,DailyFrecuency.Hours,new TimeLimits(new TimeOnly(4,0,0),new TimeOnly(8,0,0))),new DateLimits(new DateTime(2020,1,1)),Cultures.es_ES),
                new OutPut(new DateTime(2020,1,2,4,0,0),"Ocurre cada 2 semanas en lunes, jueves y viernes cada 2 horas entre las 04:00:00 y 08:00:00 comenzando el 01/01/2020")
            },

            new object[]{
                new Configuration
                (new DateTime(2020,1,4), ConfigType.Once, true,new DateTime(2020,1,8),Occurs.Daily,null,null
                    ,DailyConfiguration.Once(new TimeOnly(14,0,0),null),
                    new DateLimits(new DateTime(2020,1,1)),Cultures.es_ES),

                new OutPut(new DateTime(2020,1,8,14,0,0),"Ocurre una vez el 08/01/2020 una vez a las 14:00:00 comenzando el 01/01/2020")
            },

            new object[]
            {
                new Configuration(new DateTime(2020, 1, 4,0,0,0), ConfigType.Recurring, true , null, Occurs.Daily,null,null,
                    DailyConfiguration.Recurring(3,DailyFrecuency.Hours,new TimeLimits(new TimeOnly(10,0,0),new TimeOnly(15,0,0))),
                    new DateLimits(new DateTime(2020,1,1),null),Cultures.es_ES),
                new OutPut(new DateTime(2020, 1, 4,10,0,0),"Ocurre cada día y cada 3 horas entre las 10:00:00 y 15:00:00 comenzando el 01/01/2020")
            },

            new object[]
            {
                new Configuration
                (new DateTime (2024,4,4), ConfigType.Once, true , new DateTime(2024,05,10),Occurs.Daily,null,null,
                    DailyConfiguration.Once(new TimeOnly(16,0,0),null),new DateLimits(new DateTime(2020,5,12)),Cultures.es_ES),
                new OutPut (new DateTime(2024,5,10,16,0,0),"Ocurre una vez el 10/05/2024 una vez a las 16:00:00 comenzando el 12/05/2020")
            },

            new object[]
            {
                new Configuration(new DateTime(2023,12,24), ConfigType.Recurring, true , null, Occurs.Daily,null,null,
                    DailyConfiguration.Once(new TimeOnly(16,0,0),null),new DateLimits(new DateTime(2023,12,30),new DateTime(2024,1,1)),Cultures.es_ES),
                new OutPut(new DateTime(2023,12,30,16,0,0),"Ocurre cada día una vez a las 16:00:00 comenzando el 30/12/2023 y terminando el 01/01/2024")
            },

            new object[]
            {
                new Configuration
                (new DateTime (2024,4,4,10,0,0), ConfigType.Once, true , new DateTime(2024,5,10),Occurs.Daily,null,null,
                    DailyConfiguration.Recurring(30,DailyFrecuency.Minutes,new TimeLimits(new TimeOnly(15,0,0),new TimeOnly(19,0,0))),
                    new DateLimits(new DateTime(2020,5,10),null),Cultures.es_ES),
                new OutPut (new DateTime(2024,5,10,15,0,0),"Ocurre una vez el 10/05/2024 y cada 30 minutos entre las 15:00:00 y 19:00:00 comenzando el 10/05/2020")
            },

            new object[]
            {
                new Configuration(new DateTime(2023,12,24), ConfigType.Recurring, true , null, Occurs.Daily,null,null,
                    DailyConfiguration.Once(new TimeOnly(10,0,0),null),new DateLimits(new DateTime(2020,12,27)),Cultures.es_ES),
                new OutPut(new DateTime(2023,12,24,10,0,0),"Ocurre cada día una vez a las 10:00:00 comenzando el 27/12/2020")
            },

            new object[]
            {

                new Configuration(
                new DateTime(2020, 1, 4), ConfigType.Recurring, true, new DateTime(2020, 1, 8), Occurs.Daily,null,null,
                DailyConfiguration.Once(new TimeOnly(14, 0, 0),null),new DateLimits(new DateTime(2020, 1, 7),new DateTime(2020,1,9)),Cultures.es_ES),
                new OutPut(new DateTime(2020,1,7,14, 0, 0),"Ocurre cada día una vez a las 14:00:00 comenzando el 07/01/2020 y terminando el 09/01/2020")
            },

            new object[]
            {
                new Configuration(
                    new DateTime(2020, 1, 4), ConfigType.Recurring, true, new DateTime(2020, 1, 8), Occurs.Daily,null,
                    null,DailyConfiguration.Once(new TimeOnly(14,0,0),null),
                    new DateLimits(new DateTime(2020, 1, 9),new DateTime(2020,1,10)),Cultures.es_ES),
                    new OutPut(new DateTime(2020,1,9,14,0,0),"Ocurre cada día una vez a las 14:00:00 comenzando el 09/01/2020 y terminando el 10/01/2020")
            },

            //10
            new object[]
            {
                new Configuration(
                    new DateTime(2024, 4,11),ConfigType.Once,true, new DateTime(2024, 4, 11),Occurs.Daily,null,
                    null,
                    DailyConfiguration.Once(new TimeOnly(16,55,55),null),
                    new DateLimits(new DateTime(2024,4,11)),Cultures.es_ES),
                    new OutPut(new DateTime(2024,4,11,16,55,55),"Ocurre una vez el 11/04/2024 una vez a las 16:55:55 comenzando el 11/04/2024")
            },
            new object[]
            {
                    new Configuration(
                    new DateTime(2024, 4,11,0,0,0),ConfigType.Recurring,true,null,Occurs.Daily,null,null,
                    DailyConfiguration.Once(new TimeOnly(8,30,55),null),
                    new DateLimits(new DateTime(2024,4,11),new DateTime(2024,4,18)),Cultures.es_ES),
                    new OutPut(new DateTime(2024,4,11,8,30,55),"Ocurre cada día una vez a las 08:30:55 comenzando el 11/04/2024 y terminando el 18/04/2024")
            },

            new object[]
            {
                new Configuration(
                    new DateTime(2024, 4,11,12,46,30),ConfigType.Recurring,true,null,Occurs.Daily,null,
                    null,
                    DailyConfiguration.Once(new TimeOnly(8,30,55),null),
                    new DateLimits(new DateTime(2024,4,14),new DateTime(2024,4,18)),Cultures.es_ES),
                    new OutPut(new DateTime(2024,4,14,8,30,55),"Ocurre cada día una vez a las 08:30:55 comenzando el 14/04/2024 y terminando el 18/04/2024")
            },
            new object[]{
                new Configuration
                (new DateTime(2020,1,1,0,0,0), ConfigType.Recurring,true,null,Occurs.Weekly,null,new WeeklyConfiguration(1,
                    new List<DayOfWeek>(){DayOfWeek.Monday,DayOfWeek.Thursday,DayOfWeek.Friday}),
                    DailyConfiguration.Recurring(2,DailyFrecuency.Hours,
                        new TimeLimits(new TimeOnly(4,0,0),new TimeOnly(8,0,0))),new DateLimits(new DateTime(2020,1,1)),Cultures.es_ES),
                new OutPut(new DateTime(2020,1,2,4,0,0),"Ocurre cada 1 semana en lunes, jueves y viernes cada 2 horas entre las 04:00:00 y 08:00:00 comenzando el 01/01/2020")
            },

            //MONTHLY CONFIGURATIONS 14 - FINAL

            new object[]{
                new Configuration(new DateTime(2020, 1, 1), ConfigType.Recurring, true, null, Occurs.Monthly, MonthlyConfiguration.WeekDayOption(  Ordinal.First, KindOfDay.Monday, 3), null,
            DailyConfiguration.Recurring( 1, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(3, 0, 0), new TimeOnly(6, 0, 0))), new DateLimits(new DateTime(2020, 1, 1)),Cultures.es_ES),
                new OutPut(new DateTime(2020,1,6,3,0,0),"Ocurre el primer lunes de cada 3 meses y cada 1 horas entre las 03:00:00 y 06:00:00 comenzando el 01/01/2020")
            },

            new object[]{
                new Configuration(new DateTime(2020, 1, 1), ConfigType.Recurring, true, null, Occurs.Monthly, MonthlyConfiguration.WeekDayOption(  Ordinal.Second, KindOfDay.Tuesday, 3), null,
            DailyConfiguration.Recurring( 1, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(3, 0, 0), new TimeOnly(6, 0, 0))), new DateLimits(new DateTime(2020, 1, 1)),Cultures.es_ES),
                new OutPut(new DateTime(2020,1,14,3,0,0),"Ocurre el segundo martes de cada 3 meses y cada 1 horas entre las 03:00:00 y 06:00:00 comenzando el 01/01/2020")
            },

            new object[]{
                new Configuration(new DateTime(2020, 1, 1), ConfigType.Recurring, true, null, Occurs.Monthly, MonthlyConfiguration.WeekDayOption( Ordinal.Third, KindOfDay.Wednesday, 3), null,
            DailyConfiguration.Recurring( 1, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(3, 0, 0), new TimeOnly(6, 0, 0))), new DateLimits(new DateTime(2020, 1, 1)),Cultures.es_ES),
                new OutPut(new DateTime(2020,1,15,3,0,0),"Ocurre el tercer miercoles de cada 3 meses y cada 1 horas entre las 03:00:00 y 06:00:00 comenzando el 01/01/2020")
            },

             new object[]{
                new Configuration(new DateTime(2020, 1, 1), ConfigType.Recurring, true, null, Occurs.Monthly, MonthlyConfiguration.WeekDayOption(  Ordinal.Fourth, KindOfDay.Thursday, 3), null,
            DailyConfiguration.Recurring( 1, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(3, 0, 0), new TimeOnly(6, 0, 0))), new DateLimits(new DateTime(2020, 1, 1)),Cultures.es_ES),
                new OutPut(new DateTime(2020,1,23,3,0,0),"Ocurre el cuarto jueves de cada 3 meses y cada 1 horas entre las 03:00:00 y 06:00:00 comenzando el 01/01/2020")
            },

             new object[]{
                new Configuration(new DateTime(2020, 1, 1), ConfigType.Recurring, true, null, Occurs.Monthly, MonthlyConfiguration.WeekDayOption(  Ordinal.Last, KindOfDay.Friday, 3), null,
            DailyConfiguration.Recurring( 1, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(3, 0, 0), new TimeOnly(6, 0, 0))), new DateLimits(new DateTime(2020, 1, 1)),Cultures.es_ES),
                new OutPut(new DateTime(2020,1,31,3,0,0),"Ocurre el último viernes de cada 3 meses y cada 1 horas entre las 03:00:00 y 06:00:00 comenzando el 01/01/2020")
            },

             new object[]{
                new Configuration(new DateTime(2020, 1, 1), ConfigType.Recurring, true, null, Occurs.Monthly, MonthlyConfiguration.WeekDayOption(  Ordinal.First, KindOfDay.Saturday, 3), null,
            DailyConfiguration.Recurring( 1, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(3, 0, 0), new TimeOnly(6, 0, 0))), new DateLimits(new DateTime(2020, 1, 1)),Cultures.es_ES),
                new OutPut(new DateTime(2020,1,4,3,0,0),"Ocurre el primer sabado de cada 3 meses y cada 1 horas entre las 03:00:00 y 06:00:00 comenzando el 01/01/2020")
            },

             //20
              new object[]{
                new Configuration(new DateTime(2020, 1, 1), ConfigType.Recurring, true, null, Occurs.Monthly, MonthlyConfiguration.WeekDayOption(  Ordinal.Last, KindOfDay.Sunday, 3), null,
            DailyConfiguration.Recurring( 1, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(3, 0, 0), new TimeOnly(6, 0, 0))), new DateLimits(new DateTime(2020, 1, 1)),Cultures.es_ES),
                new OutPut(new DateTime(2020,1,26,3,0,0),"Ocurre el último domingo de cada 3 meses y cada 1 horas entre las 03:00:00 y 06:00:00 comenzando el 01/01/2020")
            },

              new object[]{
                new Configuration(new DateTime(2020, 1, 1), ConfigType.Recurring, true, null, Occurs.Monthly, MonthlyConfiguration.WeekDayOption(  Ordinal.First, KindOfDay.Day, 3), null,
            DailyConfiguration.Recurring( 1, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(3, 0, 0), new TimeOnly(6, 0, 0))), new DateLimits(new DateTime(2020, 1, 1)),Cultures.es_ES),
                new OutPut(new DateTime(2020,1,1,3,0,0),"Ocurre el primer día de cada 3 meses y cada 1 horas entre las 03:00:00 y 06:00:00 comenzando el 01/01/2020")
            },
              new object[]{
                new Configuration(new DateTime(2020, 1, 1), ConfigType.Recurring, true, null, Occurs.Monthly, MonthlyConfiguration.WeekDayOption(  Ordinal.Last, KindOfDay.Day, 3), null,
            DailyConfiguration.Recurring( 1, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(3, 0, 0), new TimeOnly(6, 0, 0))), new DateLimits(new DateTime(2020, 1, 1)),Cultures.es_ES),
                new OutPut(new DateTime(2020,1,31,3,0,0),"Ocurre el último día de cada 3 meses y cada 1 horas entre las 03:00:00 y 06:00:00 comenzando el 01/01/2020")
            },

              new object[]{
                new Configuration(new DateTime(2020, 1, 1), ConfigType.Recurring, true, null, Occurs.Monthly, MonthlyConfiguration.WeekDayOption(  Ordinal.Second, KindOfDay.WeekDay, 3), null,
            DailyConfiguration.Recurring( 1, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(3, 0, 0), new TimeOnly(6, 0, 0))), new DateLimits(new DateTime(2020, 1, 1)),Cultures.es_ES),
                new OutPut(new DateTime(2020,1,2,3,0,0),"Ocurre el segundo día de semana de cada 3 meses y cada 1 horas entre las 03:00:00 y 06:00:00 comenzando el 01/01/2020")
            },

            new object[]{
                new Configuration(new DateTime(2020, 1, 1), ConfigType.Recurring, true, null, Occurs.Monthly, MonthlyConfiguration.WeekDayOption(  Ordinal.Third, KindOfDay.WeekDay, 3), null,
            DailyConfiguration.Recurring( 1, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(3, 0, 0), new TimeOnly(6, 0, 0))), new DateLimits(new DateTime(2020, 1, 1)),Cultures.es_ES),
                new OutPut(new DateTime(2020,1,3,3,0,0),"Ocurre el tercer día de semana de cada 3 meses y cada 1 horas entre las 03:00:00 y 06:00:00 comenzando el 01/01/2020")
            },
            new object[]{
                new Configuration(new DateTime(2020, 1, 1), ConfigType.Recurring, true, null, Occurs.Monthly, MonthlyConfiguration.WeekDayOption( Ordinal.Fourth, KindOfDay.WeekEndDay, 3), null,
            DailyConfiguration.Recurring( 1, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(3, 0, 0), new TimeOnly(6, 0, 0))), new DateLimits(new DateTime(2020, 1, 1)),Cultures.es_ES),
                new OutPut(new DateTime(2020,1,12,3,0,0),"Ocurre el cuarto día de fin de semana de cada 3 meses y cada 1 horas entre las 03:00:00 y 06:00:00 comenzando el 01/01/2020")
            },

            new object[]{
                new Configuration(new DateTime(2020, 1, 1), ConfigType.Recurring, true, null, Occurs.Monthly, MonthlyConfiguration.WeekDayOption( Ordinal.Last, KindOfDay.WeekEndDay, 3), null,
            DailyConfiguration.Recurring( 1, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(3, 0, 0), new TimeOnly(6, 0, 0))), new DateLimits(new DateTime(2020, 1, 1)),Cultures.es_ES),
                new OutPut(new DateTime(2020,1,26,3, 0, 0),"Ocurre el último día de fin de semana de cada 3 meses y cada 1 horas entre las 03:00:00 y 06:00:00 comenzando el 01/01/2020")
            },

            //DAYNUMBER MONTHLY 27 - 
            new object[]{
                new Configuration(new DateTime(2020, 1, 1), ConfigType.Recurring, true, null,
                    Occurs.Monthly,
                     MonthlyConfiguration.DayOption( 5,  5), null,
            DailyConfiguration.Recurring( 5, DailyFrecuency.Hours,
                new TimeLimits(new TimeOnly(6, 0, 0), new TimeOnly(16, 0, 0))), new DateLimits(new DateTime(2020, 1, 4)),Cultures.es_ES),
                new OutPut(new DateTime(2020,1,5,6, 0, 0),"Ocurre el 5º de cada 5 meses y cada 5 horas entre las 06:00:00 y 16:00:00 comenzando el 04/01/2020")
            },

            new object[]{
                new Configuration(new DateTime(2020, 1, 1), ConfigType.Recurring, true, null, Occurs.Monthly,
                     MonthlyConfiguration.DayOption (1, 5), null,
            DailyConfiguration.Recurring( 5, DailyFrecuency.Hours,
                new TimeLimits(new TimeOnly(6, 0, 0), new TimeOnly(16, 0, 0))), new DateLimits(new DateTime(2020, 1, 3)),Cultures.es_ES),
                new OutPut(new DateTime(2020,2,1,6, 0, 0),"Ocurre el 1ero de cada 5 meses y cada 5 horas entre las 06:00:00 y 16:00:00 comenzando el 03/01/2020")
            },

            new object[]{
                new Configuration(new DateTime(2020, 1, 1), ConfigType.Recurring, true, null, Occurs.Monthly,
                     MonthlyConfiguration.DayOption(5, 5), null,
            DailyConfiguration.Once( new TimeOnly(17, 0, 0), null), new DateLimits(new DateTime(2020, 1, 4)),Cultures.es_ES),
                new OutPut(new DateTime(2020,1,5,17, 0, 0),"Ocurre el 5º de cada 5 meses una vez a las 17:00:00 comenzando el 04/01/2020")
            },

            new object[]{
                new Configuration(new DateTime(2020, 1, 1), ConfigType.Recurring, true, null, Occurs.Monthly,
                    MonthlyConfiguration.DayOption(3, 5), null,
            DailyConfiguration.Once( new TimeOnly(3, 0, 0),null), new DateLimits(new DateTime(2020, 1, 3)),Cultures.es_ES),
                new OutPut(new DateTime(2020,1,3,3, 0, 0),"Ocurre el 3ero de cada 5 meses una vez a las 03:00:00 comenzando el 03/01/2020")
            },
    };
        public IEnumerator<object[]> GetEnumerator()
        {
            return data.GetEnumerator();

        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return data.GetEnumerator();
        }
    }
}