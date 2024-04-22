using RetoScheduler;
using RetoScheduler.Configurations;
using RetoScheduler.Configurations.Limits;
using RetoScheduler.Enums;
using System;
using System.Collections;
using System.Collections.Generic;

namespace RetoSchedulerTest
{
    public class SchedulerTestMonthlyConfiguration : IEnumerable<object[]>
    {
        public readonly List<object[]> data = new List<object[]>()
        {
            //PRUEBAS DE WEEKDAYOPTION DIAS DE LA SEMANA
            //MONDAY
            new object[]{
                new Configuration(new DateTime(2020, 1, 1), ConfigType.Recurring, true, null, Occurs.Monthly, new MonthlyConfiguration(MonthlyConfigType.WeekDayOption, 1, Ordinal.First, KindOfDay.Monday, 3), null,
            new DailyConfiguration(DailyConfigType.Recurring, TimeOnly.MinValue, 1, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(3, 0, 0), new TimeOnly(6, 0, 0))), new DateLimits(new DateTime(2020, 1, 1))),
                new OutPut(new DateTime(2020,1,6),"Occurs the first monday of very 3 months and every 1 hours between 3:00:00 AM and 6:00:00 AM starting on 01/01/2020")
            },

            new object[]{
                new Configuration(new DateTime(2020, 1, 1), ConfigType.Recurring, true, null, Occurs.Monthly, new MonthlyConfiguration(MonthlyConfigType.WeekDayOption, 1, Ordinal.Second, KindOfDay.Monday, 3), null,
            new DailyConfiguration(DailyConfigType.Recurring, TimeOnly.MinValue, 2, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(3, 0, 0), new TimeOnly(6, 0, 0))), new DateLimits(new DateTime(2020, 1, 2))),
                new OutPut(new DateTime(2020,1,13),"Occurs the second monday of very 3 months and every 2 hours between 3:00:00 AM and 6:00:00 AM starting on 02/01/2020")
            },

             new object[]{
                new Configuration(new DateTime(2020, 1, 1), ConfigType.Recurring, true, null, Occurs.Monthly, new MonthlyConfiguration(MonthlyConfigType.WeekDayOption, 1, Ordinal.Third, KindOfDay.Monday, 3), null,
            new DailyConfiguration(DailyConfigType.Recurring, TimeOnly.MinValue, 1, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(3, 0, 0), new TimeOnly(6, 0, 0))), new DateLimits(new DateTime(2020, 1, 1))),
                new OutPut(new DateTime(2020,1,20),"Occurs the third monday of very 3 months and every 1 hours between 3:00:00 AM and 6:00:00 AM starting on 01/01/2020")
            },

              new object[]{
                new Configuration(new DateTime(2020, 1, 1), ConfigType.Recurring, true, null, Occurs.Monthly, new MonthlyConfiguration(MonthlyConfigType.WeekDayOption, 1, Ordinal.Fourth, KindOfDay.Monday, 3), null,
            new DailyConfiguration(DailyConfigType.Recurring, TimeOnly.MinValue, 4, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(3, 0, 0), new TimeOnly(6, 0, 0))), new DateLimits(new DateTime(2020, 1, 1))),
                new OutPut(new DateTime(2020,1,27),"Occurs the fourth monday of very 3 months and every 4 hours between 3:00:00 AM and 6:00:00 AM starting on 01/01/2020")
            },

               new object[]{
                new Configuration(new DateTime(2020, 1, 1), ConfigType.Recurring, true, null, Occurs.Monthly, new MonthlyConfiguration(MonthlyConfigType.WeekDayOption, 1, Ordinal.Last, KindOfDay.Monday, 3), null,
            new DailyConfiguration(DailyConfigType.Recurring, TimeOnly.MinValue, 1, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(3, 0, 0), new TimeOnly(6, 0, 0))), new DateLimits(new DateTime(2020, 1, 1))),
                new OutPut(new DateTime(2020,1,27),"Occurs the last monday of very 3 months and every 1 hours between 3:00:00 AM and 6:00:00 AM starting on 01/01/2020")
            },

            //TUESDAY

            new object[]{
                new Configuration(new DateTime(2020, 1, 1), ConfigType.Recurring, true, null, Occurs.Monthly, new MonthlyConfiguration(MonthlyConfigType.WeekDayOption, 1, Ordinal.First, KindOfDay.Tuesday, 3), null,
            new DailyConfiguration(DailyConfigType.Recurring, TimeOnly.MinValue, 1, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(3, 0, 0), new TimeOnly(6, 0, 0))), new DateLimits(new DateTime(2020, 1, 1))),
                new OutPut(new DateTime(2020,1,7),"Occurs the first tuesday of very 3 months and every 1 hours between 3:00:00 AM and 6:00:00 AM starting on 01/01/2020")
            },

            new object[]{
                new Configuration(new DateTime(2020, 1, 1), ConfigType.Recurring, true, null, Occurs.Monthly, new MonthlyConfiguration(MonthlyConfigType.WeekDayOption, 1, Ordinal.Second, KindOfDay.Tuesday, 3), null,
            new DailyConfiguration(DailyConfigType.Recurring, TimeOnly.MinValue, 1, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(3, 0, 0), new TimeOnly(6, 0, 0))), new DateLimits(new DateTime(2020, 1, 1))),
                new OutPut(new DateTime(2020,1,14),"Occurs the second tuesday of very 3 months and every 1 hours between 3:00:00 AM and 6:00:00 AM starting on 01/01/2020")
            },

            new object[]{
                new Configuration(new DateTime(2020, 1, 1), ConfigType.Recurring, true, null, Occurs.Monthly, new MonthlyConfiguration(MonthlyConfigType.WeekDayOption, 1, Ordinal.Third, KindOfDay.Tuesday, 3), null,
            new DailyConfiguration(DailyConfigType.Recurring, TimeOnly.MinValue, 1, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(3, 0, 0), new TimeOnly(6, 0, 0))), new DateLimits(new DateTime(2020, 1, 1))),
                new OutPut(new DateTime(2020,1,21),"Occurs the third tuesday of very 3 months and every 1 hours between 3:00:00 AM and 6:00:00 AM starting on 01/01/2020")
            },

            new object[]{
                new Configuration(new DateTime(2020, 1, 1), ConfigType.Recurring, true, null, Occurs.Monthly, new MonthlyConfiguration(MonthlyConfigType.WeekDayOption, 1, Ordinal.Fourth, KindOfDay.Tuesday, 3), null,
            new DailyConfiguration(DailyConfigType.Recurring, TimeOnly.MinValue, 1, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(3, 0, 0), new TimeOnly(6, 0, 0))), new DateLimits(new DateTime(2020, 1, 1))),
                new OutPut(new DateTime(2020,1,28),"Occurs the fourth tuesday of very 3 months and every 1 hours between 3:00:00 AM and 6:00:00 AM starting on 01/01/2020")
            },

            new object[]{
                new Configuration(new DateTime(2020, 1, 1), ConfigType.Recurring, true, null, Occurs.Monthly, new MonthlyConfiguration(MonthlyConfigType.WeekDayOption, 1, Ordinal.Last, KindOfDay.Tuesday, 3), null,
            new DailyConfiguration(DailyConfigType.Recurring, TimeOnly.MinValue, 1, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(3, 0, 0), new TimeOnly(6, 0, 0))), new DateLimits(new DateTime(2020, 1, 1))),
                new OutPut(new DateTime(2020,1,28),"Occurs the last tuesday of very 3 months and every 1 hours between 3:00:00 AM and 6:00:00 AM starting on 01/01/2020")
            },

            //WEDNESDAY
            
            new object[]{
                new Configuration(new DateTime(2020, 1, 1), ConfigType.Recurring, true, null, Occurs.Monthly, new MonthlyConfiguration(MonthlyConfigType.WeekDayOption, 1, Ordinal.First, KindOfDay.Wednesday, 3), null,
            new DailyConfiguration(DailyConfigType.Recurring, TimeOnly.MinValue, 1, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(3, 0, 0), new TimeOnly(6, 0, 0))), new DateLimits(new DateTime(2020, 1, 1))),
                new OutPut(new DateTime(2020,1,1),"Occurs the first wednesday of very 3 months and every 1 hours between 3:00:00 AM and 6:00:00 AM starting on 01/01/2020")
            },

            new object[]{
                new Configuration(new DateTime(2020, 1, 1), ConfigType.Recurring, true, null, Occurs.Monthly, new MonthlyConfiguration(MonthlyConfigType.WeekDayOption, 1, Ordinal.Second, KindOfDay.Wednesday, 3), null,
            new DailyConfiguration(DailyConfigType.Recurring, TimeOnly.MinValue, 1, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(3, 0, 0), new TimeOnly(6, 0, 0))), new DateLimits(new DateTime(2020, 1, 1))),
                new OutPut(new DateTime(2020,1,8),"Occurs the second wednesday of very 3 months and every 1 hours between 3:00:00 AM and 6:00:00 AM starting on 01/01/2020")
            },

            new object[]{
                new Configuration(new DateTime(2020, 1, 1), ConfigType.Recurring, true, null, Occurs.Monthly, new MonthlyConfiguration(MonthlyConfigType.WeekDayOption, 1, Ordinal.Third, KindOfDay.Wednesday, 3), null,
            new DailyConfiguration(DailyConfigType.Recurring, TimeOnly.MinValue, 1, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(3, 0, 0), new TimeOnly(6, 0, 0))), new DateLimits(new DateTime(2020, 1, 1))),
                new OutPut(new DateTime(2020,1,15),"Occurs the third wednesday of very 3 months and every 1 hours between 3:00:00 AM and 6:00:00 AM starting on 01/01/2020")
            },

            new object[]{
                new Configuration(new DateTime(2020, 1, 1), ConfigType.Recurring, true, null, Occurs.Monthly, new MonthlyConfiguration(MonthlyConfigType.WeekDayOption, 1, Ordinal.Fourth, KindOfDay.Wednesday, 3), null,
            new DailyConfiguration(DailyConfigType.Recurring, TimeOnly.MinValue, 1, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(3, 0, 0), new TimeOnly(6, 0, 0))), new DateLimits(new DateTime(2020, 1, 1))),
                new OutPut(new DateTime(2020,1,22),"Occurs the fourth wednesday of very 3 months and every 1 hours between 3:00:00 AM and 6:00:00 AM starting on 01/01/2020")
            },

            new object[]{
                new Configuration(new DateTime(2020, 1, 1), ConfigType.Recurring, true, null, Occurs.Monthly, new MonthlyConfiguration(MonthlyConfigType.WeekDayOption, 1, Ordinal.Last, KindOfDay.Wednesday, 3), null,
            new DailyConfiguration(DailyConfigType.Recurring, TimeOnly.MinValue, 1, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(3, 0, 0), new TimeOnly(6, 0, 0))), new DateLimits(new DateTime(2020, 1, 1))),
                new OutPut(new DateTime(2020,1,29),"Occurs the last wednesday of very 3 months and every 1 hours between 3:00:00 AM and 6:00:00 AM starting on 01/01/2020")
            },

            //THURSDAY
            
            new object[]{
                new Configuration(new DateTime(2020, 1, 1), ConfigType.Recurring, true, null, Occurs.Monthly, new MonthlyConfiguration(MonthlyConfigType.WeekDayOption, 1, Ordinal.First, KindOfDay.Thursday, 3), null,
            new DailyConfiguration(DailyConfigType.Recurring, TimeOnly.MinValue, 1, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(3, 0, 0), new TimeOnly(6, 0, 0))), new DateLimits(new DateTime(2020, 1, 1))),
                new OutPut(new DateTime(2020,1,2),"Occurs the first thursday of very 3 months and every 1 hours between 3:00:00 AM and 6:00:00 AM starting on 01/01/2020")
            },

            new object[]{
                new Configuration(new DateTime(2020, 1, 1), ConfigType.Recurring, true, null, Occurs.Monthly, new MonthlyConfiguration(MonthlyConfigType.WeekDayOption, 1, Ordinal.Second, KindOfDay.Thursday, 3), null,
            new DailyConfiguration(DailyConfigType.Recurring, TimeOnly.MinValue, 1, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(3, 0, 0), new TimeOnly(6, 0, 0))), new DateLimits(new DateTime(2020, 1, 1))),
                new OutPut(new DateTime(2020,1,9),"Occurs the second thursday of very 3 months and every 1 hours between 3:00:00 AM and 6:00:00 AM starting on 01/01/2020")
            },

            new object[]{
                new Configuration(new DateTime(2020, 1, 1), ConfigType.Recurring, true, null, Occurs.Monthly, new MonthlyConfiguration(MonthlyConfigType.WeekDayOption, 1, Ordinal.Third, KindOfDay.Thursday, 3), null,
            new DailyConfiguration(DailyConfigType.Recurring, TimeOnly.MinValue, 1, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(3, 0, 0), new TimeOnly(6, 0, 0))), new DateLimits(new DateTime(2020, 1, 1))),
                new OutPut(new DateTime(2020,1,16),"Occurs the third thursday of very 3 months and every 1 hours between 3:00:00 AM and 6:00:00 AM starting on 01/01/2020")
            },

            new object[]{
                new Configuration(new DateTime(2020, 1, 1), ConfigType.Recurring, true, null, Occurs.Monthly, new MonthlyConfiguration(MonthlyConfigType.WeekDayOption, 1, Ordinal.Fourth, KindOfDay.Thursday, 3), null,
            new DailyConfiguration(DailyConfigType.Recurring, TimeOnly.MinValue, 1, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(3, 0, 0), new TimeOnly(6, 0, 0))), new DateLimits(new DateTime(2020, 1, 1))),
                new OutPut(new DateTime(2020,1,23),"Occurs the fourth thursday of very 3 months and every 1 hours between 3:00:00 AM and 6:00:00 AM starting on 01/01/2020")
            },

            new object[]{
                new Configuration(new DateTime(2020, 1, 1), ConfigType.Recurring, true, null, Occurs.Monthly, new MonthlyConfiguration(MonthlyConfigType.WeekDayOption, 1, Ordinal.Last, KindOfDay.Thursday, 3), null,
            new DailyConfiguration(DailyConfigType.Recurring, TimeOnly.MinValue, 1, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(3, 0, 0), new TimeOnly(6, 0, 0))), new DateLimits(new DateTime(2020, 1, 1))),
                new OutPut(new DateTime(2020,1,30),"Occurs the last thursday of very 3 months and every 1 hours between 3:00:00 AM and 6:00:00 AM starting on 01/01/2020")
            },


            //FRIDAY
            
            new object[]{
                new Configuration(new DateTime(2020, 1, 1), ConfigType.Recurring, true, null, Occurs.Monthly, new MonthlyConfiguration(MonthlyConfigType.WeekDayOption, 1, Ordinal.First, KindOfDay.Friday, 3), null,
            new DailyConfiguration(DailyConfigType.Recurring, TimeOnly.MinValue, 1, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(3, 0, 0), new TimeOnly(6, 0, 0))), new DateLimits(new DateTime(2020, 1, 1))),
                new OutPut(new DateTime(2020,1,3),"Occurs the first friday of very 3 months and every 1 hours between 3:00:00 AM and 6:00:00 AM starting on 01/01/2020")
            },

            new object[]{
                new Configuration(new DateTime(2020, 1, 1), ConfigType.Recurring, true, null, Occurs.Monthly, new MonthlyConfiguration(MonthlyConfigType.WeekDayOption, 1, Ordinal.Second, KindOfDay.Friday, 3), null,
            new DailyConfiguration(DailyConfigType.Recurring, TimeOnly.MinValue, 1, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(3, 0, 0), new TimeOnly(6, 0, 0))), new DateLimits(new DateTime(2020, 1, 1))),
                new OutPut(new DateTime(2020,1,10),"Occurs the second friday of very 3 months and every 1 hours between 3:00:00 AM and 6:00:00 AM starting on 01/01/2020")
            },

            new object[]{
                new Configuration(new DateTime(2020, 1, 1), ConfigType.Recurring, true, null, Occurs.Monthly, new MonthlyConfiguration(MonthlyConfigType.WeekDayOption, 1, Ordinal.Third, KindOfDay.Friday, 3), null,
            new DailyConfiguration(DailyConfigType.Recurring, TimeOnly.MinValue, 1, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(3, 0, 0), new TimeOnly(6, 0, 0))), new DateLimits(new DateTime(2020, 1, 1))),
                new OutPut(new DateTime(2020,1,17),"Occurs the third friday of very 3 months and every 1 hours between 3:00:00 AM and 6:00:00 AM starting on 01/01/2020")
            },

            new object[]{
                new Configuration(new DateTime(2020, 1, 1), ConfigType.Recurring, true, null, Occurs.Monthly, new MonthlyConfiguration(MonthlyConfigType.WeekDayOption, 1, Ordinal.Fourth, KindOfDay.Friday, 3), null,
            new DailyConfiguration(DailyConfigType.Recurring, TimeOnly.MinValue, 1, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(3, 0, 0), new TimeOnly(6, 0, 0))), new DateLimits(new DateTime(2020, 1, 1))),
                new OutPut(new DateTime(2020,1,24),"Occurs the fourth friday of very 3 months and every 1 hours between 3:00:00 AM and 6:00:00 AM starting on 01/01/2020")
            },

            new object[]{
                new Configuration(new DateTime(2020, 1, 1), ConfigType.Recurring, true, null, Occurs.Monthly, new MonthlyConfiguration(MonthlyConfigType.WeekDayOption, 1, Ordinal.Last, KindOfDay.Friday, 3), null,
            new DailyConfiguration(DailyConfigType.Recurring, TimeOnly.MinValue, 1, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(3, 0, 0), new TimeOnly(6, 0, 0))), new DateLimits(new DateTime(2020, 1, 1))),
                new OutPut(new DateTime(2020,1,31),"Occurs the last friday of very 3 months and every 1 hours between 3:00:00 AM and 6:00:00 AM starting on 01/01/2020")
            },

            //SATURDAY
            
            new object[]{
                new Configuration(new DateTime(2020, 1, 1), ConfigType.Recurring, true, null, Occurs.Monthly, new MonthlyConfiguration(MonthlyConfigType.WeekDayOption, 1, Ordinal.First, KindOfDay.Saturday, 3), null,
            new DailyConfiguration(DailyConfigType.Recurring, TimeOnly.MinValue, 1, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(3, 0, 0), new TimeOnly(6, 0, 0))), new DateLimits(new DateTime(2020, 1, 1))),
                new OutPut(new DateTime(2020,1,4),"Occurs the first saturday of very 3 months and every 1 hours between 3:00:00 AM and 6:00:00 AM starting on 01/01/2020")
            },

            new object[]{
                new Configuration(new DateTime(2020, 1, 1), ConfigType.Recurring, true, null, Occurs.Monthly, new MonthlyConfiguration(MonthlyConfigType.WeekDayOption, 1, Ordinal.Second, KindOfDay.Saturday, 3), null,
            new DailyConfiguration(DailyConfigType.Recurring, TimeOnly.MinValue, 1, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(3, 0, 0), new TimeOnly(6, 0, 0))), new DateLimits(new DateTime(2020, 1, 1))),
                new OutPut(new DateTime(2020,1,11),"Occurs the second saturday of very 3 months and every 1 hours between 3:00:00 AM and 6:00:00 AM starting on 01/01/2020")
            },

            new object[]{
                new Configuration(new DateTime(2020, 1, 1), ConfigType.Recurring, true, null, Occurs.Monthly, new MonthlyConfiguration(MonthlyConfigType.WeekDayOption, 1, Ordinal.Third, KindOfDay.Saturday, 3), null,
            new DailyConfiguration(DailyConfigType.Recurring, TimeOnly.MinValue, 1, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(3, 0, 0), new TimeOnly(6, 0, 0))), new DateLimits(new DateTime(2020, 1, 1))),
                new OutPut(new DateTime(2020,1,18),"Occurs the third saturday of very 3 months and every 1 hours between 3:00:00 AM and 6:00:00 AM starting on 01/01/2020")
            },

            new object[]{
                new Configuration(new DateTime(2020, 1, 1), ConfigType.Recurring, true, null, Occurs.Monthly, new MonthlyConfiguration(MonthlyConfigType.WeekDayOption, 1, Ordinal.Fourth, KindOfDay.Saturday, 3), null,
            new DailyConfiguration(DailyConfigType.Recurring, TimeOnly.MinValue, 1, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(3, 0, 0), new TimeOnly(6, 0, 0))), new DateLimits(new DateTime(2020, 1, 1))),
                new OutPut(new DateTime(2020,1,25),"Occurs the fourth saturday of very 3 months and every 1 hours between 3:00:00 AM and 6:00:00 AM starting on 01/01/2020")
            },

            new object[]{
                new Configuration(new DateTime(2020, 1, 1), ConfigType.Recurring, true, null, Occurs.Monthly, new MonthlyConfiguration(MonthlyConfigType.WeekDayOption, 1, Ordinal.Last, KindOfDay.Saturday, 3), null,
            new DailyConfiguration(DailyConfigType.Recurring, TimeOnly.MinValue, 1, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(3, 0, 0), new TimeOnly(6, 0, 0))), new DateLimits(new DateTime(2020, 1, 1))),
                new OutPut(new DateTime(2020,1,25),"Occurs the last saturday of very 3 months and every 1 hours between 3:00:00 AM and 6:00:00 AM starting on 01/01/2020")
            },

            //SUNDAY
            
            new object[]{
                new Configuration(new DateTime(2020, 1, 1), ConfigType.Recurring, true, null, Occurs.Monthly, new MonthlyConfiguration(MonthlyConfigType.WeekDayOption, 1, Ordinal.First, KindOfDay.Sunday, 3), null,
            new DailyConfiguration(DailyConfigType.Recurring, TimeOnly.MinValue, 1, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(3, 0, 0), new TimeOnly(6, 0, 0))), new DateLimits(new DateTime(2020, 1, 1))),
                    new OutPut(new DateTime(2020,1,5),"Occurs the first sunday of very 3 months and every 1 hours between 3:00:00 AM and 6:00:00 AM starting on 01/01/2020")
                },

            new object[]{
                new Configuration(new DateTime(2020, 1, 1), ConfigType.Recurring, true, null, Occurs.Monthly, new MonthlyConfiguration(MonthlyConfigType.WeekDayOption, 1, Ordinal.Second, KindOfDay.Sunday, 3), null,
            new DailyConfiguration(DailyConfigType.Recurring, TimeOnly.MinValue, 1, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(3, 0, 0), new TimeOnly(6, 0, 0))), new DateLimits(new DateTime(2020, 1, 1))),
                new OutPut(new DateTime(2020,1,12),"Occurs the second sunday of very 3 months and every 1 hours between 3:00:00 AM and 6:00:00 AM starting on 01/01/2020")
            },

            new object[]{
                new Configuration(new DateTime(2020, 1, 1), ConfigType.Recurring, true, null, Occurs.Monthly, new MonthlyConfiguration(MonthlyConfigType.WeekDayOption, 1, Ordinal.Third, KindOfDay.Sunday, 3), null,
            new DailyConfiguration(DailyConfigType.Recurring, TimeOnly.MinValue, 1, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(3, 0, 0), new TimeOnly(6, 0, 0))), new DateLimits(new DateTime(2020, 1, 1))),
                new OutPut(new DateTime(2020,1,19),"Occurs the third sunday of very 3 months and every 1 hours between 3:00:00 AM and 6:00:00 AM starting on 01/01/2020")
            },

            new object[]{
                new Configuration(new DateTime(2020, 1, 1), ConfigType.Recurring, true, null, Occurs.Monthly, new MonthlyConfiguration(MonthlyConfigType.WeekDayOption, 1, Ordinal.Fourth, KindOfDay.Sunday, 3), null,
            new DailyConfiguration(DailyConfigType.Recurring, TimeOnly.MinValue, 1, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(3, 0, 0), new TimeOnly(6, 0, 0))), new DateLimits(new DateTime(2020, 1, 1))),
                new OutPut(new DateTime(2020,1,26),"Occurs the fourth sunday of very 3 months and every 1 hours between 3:00:00 AM and 6:00:00 AM starting on 01/01/2020")
            },

            new object[]{
                new Configuration(new DateTime(2020, 1, 1), ConfigType.Recurring, true, null, Occurs.Monthly, new MonthlyConfiguration(MonthlyConfigType.WeekDayOption, 1, Ordinal.Last, KindOfDay.Sunday, 3), null,
            new DailyConfiguration(DailyConfigType.Recurring, TimeOnly.MinValue, 1, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(3, 0, 0), new TimeOnly(6, 0, 0))), new DateLimits(new DateTime(2020, 1, 1))),
                new OutPut(new DateTime(2020,1,26),"Occurs the last sunday of very 3 months and every 1 hours between 3:00:00 AM and 6:00:00 AM starting on 01/01/2020")
            },


            //DAY
            
            new object[]{
                new Configuration(new DateTime(2020, 1, 1), ConfigType.Recurring, true, null, Occurs.Monthly, new MonthlyConfiguration(MonthlyConfigType.WeekDayOption, 1, Ordinal.First, KindOfDay.Day, 3), null,
            new DailyConfiguration(DailyConfigType.Recurring, TimeOnly.MinValue, 1, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(3, 0, 0), new TimeOnly(6, 0, 0))), new DateLimits(new DateTime(2020, 1, 1))),
                new OutPut(new DateTime(2020,1,1),"Occurs the first day of very 3 months and every 1 hours between 3:00:00 AM and 6:00:00 AM starting on 01/01/2020")
            },

            new object[]{
                new Configuration(new DateTime(2020, 1, 1), ConfigType.Recurring, true, null, Occurs.Monthly, new MonthlyConfiguration(MonthlyConfigType.WeekDayOption, 1, Ordinal.Second, KindOfDay.Day, 3), null,
            new DailyConfiguration(DailyConfigType.Recurring, TimeOnly.MinValue, 1, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(3, 0, 0), new TimeOnly(6, 0, 0))), new DateLimits(new DateTime(2020, 1, 1))),
                new OutPut(new DateTime(2020,1,2),"Occurs the second day of very 3 months and every 1 hours between 3:00:00 AM and 6:00:00 AM starting on 01/01/2020")
            },

            new object[]{
                new Configuration(new DateTime(2020, 1, 1), ConfigType.Recurring, true, null, Occurs.Monthly, new MonthlyConfiguration(MonthlyConfigType.WeekDayOption, 1, Ordinal.Third, KindOfDay.Day, 3), null,
            new DailyConfiguration(DailyConfigType.Recurring, TimeOnly.MinValue, 1, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(3, 0, 0), new TimeOnly(6, 0, 0))), new DateLimits(new DateTime(2020, 1, 1))),
                new OutPut(new DateTime(2020,1,3),"Occurs the third day of very 3 months and every 1 hours between 3:00:00 AM and 6:00:00 AM starting on 01/01/2020")
            },

            new object[]{
                new Configuration(new DateTime(2020, 1, 1), ConfigType.Recurring, true, null, Occurs.Monthly, new MonthlyConfiguration(MonthlyConfigType.WeekDayOption, 1, Ordinal.Fourth, KindOfDay.Day, 3), null,
            new DailyConfiguration(DailyConfigType.Recurring, TimeOnly.MinValue, 1, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(3, 0, 0), new TimeOnly(6, 0, 0))), new DateLimits(new DateTime(2020, 1, 1))),
                new OutPut(new DateTime(2020,1,4),"Occurs the fourth day of very 3 months and every 1 hours between 3:00:00 AM and 6:00:00 AM starting on 01/01/2020")
            },

            new object[]{
                new Configuration(new DateTime(2020, 1, 1), ConfigType.Recurring, true, null, Occurs.Monthly, new MonthlyConfiguration(MonthlyConfigType.WeekDayOption, 1, Ordinal.Last, KindOfDay.Day, 3), null,
            new DailyConfiguration(DailyConfigType.Recurring, TimeOnly.MinValue, 1, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(3, 0, 0), new TimeOnly(6, 0, 0))), new DateLimits(new DateTime(2020, 1, 1))),
                new OutPut(new DateTime(2020,1,31),"Occurs the last day of very 3 months and every 1 hours between 3:00:00 AM and 6:00:00 AM starting on 01/01/2020")
            },


            //WEEKDAY
            
            new object[]{
                new Configuration(new DateTime(2020, 1, 1), ConfigType.Recurring, true, null, Occurs.Monthly, new MonthlyConfiguration(MonthlyConfigType.WeekDayOption, 1, Ordinal.First, KindOfDay.WeekDay, 3), null,
            new DailyConfiguration(DailyConfigType.Recurring, TimeOnly.MinValue, 1, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(3, 0, 0), new TimeOnly(6, 0, 0))), new DateLimits(new DateTime(2020, 1, 1))),
                new OutPut(new DateTime(2020,1,1),"Occurs the first weekday of very 3 months and every 1 hours between 3:00:00 AM and 6:00:00 AM starting on 01/01/2020")
            },

            new object[]{
                new Configuration(new DateTime(2020, 1, 1), ConfigType.Recurring, true, null, Occurs.Monthly, new MonthlyConfiguration(MonthlyConfigType.WeekDayOption, 1, Ordinal.Second, KindOfDay.WeekDay, 3), null,
            new DailyConfiguration(DailyConfigType.Recurring, TimeOnly.MinValue, 1, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(3, 0, 0), new TimeOnly(6, 0, 0))), new DateLimits(new DateTime(2020, 1, 1))),
                new OutPut(new DateTime(2020,1,2),"Occurs the second weekday of very 3 months and every 1 hours between 3:00:00 AM and 6:00:00 AM starting on 01/01/2020")
            },

            new object[]{
                new Configuration(new DateTime(2020, 1, 1), ConfigType.Recurring, true, null, Occurs.Monthly, new MonthlyConfiguration(MonthlyConfigType.WeekDayOption, 1, Ordinal.Third, KindOfDay.WeekDay, 3), null,
            new DailyConfiguration(DailyConfigType.Recurring, TimeOnly.MinValue, 1, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(3, 0, 0), new TimeOnly(6, 0, 0))), new DateLimits(new DateTime(2020, 1, 1))),
                new OutPut(new DateTime(2020,1,3),"Occurs the third weekday of very 3 months and every 1 hours between 3:00:00 AM and 6:00:00 AM starting on 01/01/2020")
            },

            new object[]{
                new Configuration(new DateTime(2020, 1, 1), ConfigType.Recurring, true, null, Occurs.Monthly, new MonthlyConfiguration(MonthlyConfigType.WeekDayOption, 1, Ordinal.Fourth, KindOfDay.WeekDay, 3), null,
            new DailyConfiguration(DailyConfigType.Recurring, TimeOnly.MinValue, 1, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(3, 0, 0), new TimeOnly(6, 0, 0))), new DateLimits(new DateTime(2020, 1, 1))),
                new OutPut(new DateTime(2020,1,6),"Occurs the fourth weekday of very 3 months and every 1 hours between 3:00:00 AM and 6:00:00 AM starting on 01/01/2020")
            },

            new object[]{
                new Configuration(new DateTime(2020, 1, 1), ConfigType.Recurring, true, null, Occurs.Monthly, new MonthlyConfiguration(MonthlyConfigType.WeekDayOption, 1, Ordinal.Last, KindOfDay.WeekDay, 3), null,
            new DailyConfiguration(DailyConfigType.Recurring, TimeOnly.MinValue, 1, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(3, 0, 0), new TimeOnly(6, 0, 0))), new DateLimits(new DateTime(2020, 1, 1))),
                new OutPut(new DateTime(2020,1,31),"Occurs the last weekday of very 3 months and every 1 hours between 3:00:00 AM and 6:00:00 AM starting on 01/01/2020")
            },


            //WEKENDDAY
            
            new object[]{
                new Configuration(new DateTime(2020, 1, 1), ConfigType.Recurring, true, null, Occurs.Monthly, new MonthlyConfiguration(MonthlyConfigType.WeekDayOption, 1, Ordinal.First, KindOfDay.WeekEndDay, 3), null,
            new DailyConfiguration(DailyConfigType.Recurring, TimeOnly.MinValue, 1, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(3, 0, 0), new TimeOnly(6, 0, 0))), new DateLimits(new DateTime(2020, 1, 1))),
                new OutPut(new DateTime(2020,1,4),"Occurs the first weekendday of very 3 months and every 1 hours between 3:00:00 AM and 6:00:00 AM starting on 01/01/2020")
            },

            new object[]{
                new Configuration(new DateTime(2020, 1, 1), ConfigType.Recurring, true, null, Occurs.Monthly, new MonthlyConfiguration(MonthlyConfigType.WeekDayOption, 1, Ordinal.Second, KindOfDay.WeekEndDay, 3), null,
            new DailyConfiguration(DailyConfigType.Recurring, TimeOnly.MinValue, 1, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(3, 0, 0), new TimeOnly(6, 0, 0))), new DateLimits(new DateTime(2020, 1, 1))),
                new OutPut(new DateTime(2020,1,5),"Occurs the second weekendday of very 3 months and every 1 hours between 3:00:00 AM and 6:00:00 AM starting on 01/01/2020")
            },

            new object[]{
                new Configuration(new DateTime(2020, 1, 1), ConfigType.Recurring, true, null, Occurs.Monthly, new MonthlyConfiguration(MonthlyConfigType.WeekDayOption, 1, Ordinal.Third, KindOfDay.WeekEndDay, 3), null,
            new DailyConfiguration(DailyConfigType.Recurring, TimeOnly.MinValue, 1, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(3, 0, 0), new TimeOnly(6, 0, 0))), new DateLimits(new DateTime(2020, 1, 1))),
                new OutPut(new DateTime(2020,1,11),"Occurs the third weekendday of very 3 months and every 1 hours between 3:00:00 AM and 6:00:00 AM starting on 01/01/2020")
            },

            new object[]{
                new Configuration(new DateTime(2020, 1, 1), ConfigType.Recurring, true, null, Occurs.Monthly, new MonthlyConfiguration(MonthlyConfigType.WeekDayOption, 1, Ordinal.Fourth, KindOfDay.WeekEndDay, 3), null,
            new DailyConfiguration(DailyConfigType.Recurring, TimeOnly.MinValue, 1, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(3, 0, 0), new TimeOnly(6, 0, 0))), new DateLimits(new DateTime(2020, 1, 1))),
                new OutPut(new DateTime(2020,1,12),"Occurs the fourth weekendday of very 3 months and every 1 hours between 3:00:00 AM and 6:00:00 AM starting on 01/01/2020")
            },

            new object[]{
                new Configuration(new DateTime(2020, 1, 1), ConfigType.Recurring, true, null, Occurs.Monthly, new MonthlyConfiguration(MonthlyConfigType.WeekDayOption, 1, Ordinal.Last, KindOfDay.WeekEndDay, 3), null,
            new DailyConfiguration(DailyConfigType.Recurring, TimeOnly.MinValue, 1, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(3, 0, 0), new TimeOnly(6, 0, 0))), new DateLimits(new DateTime(2020, 1, 1))),
                new OutPut(new DateTime(2020,1,26),"Occurs the last weekendday of very 3 months and every 1 hours between 3:00:00 AM and 6:00:00 AM starting on 01/01/2020")
            },

            //PRUEBAS DE DAYNUMBER NUMERO DE DIA
            new object[]{
                new Configuration(new DateTime(2020, 1, 1), ConfigType.Recurring, true, null, Occurs.Monthly, new MonthlyConfiguration(MonthlyConfigType.DayNumberOption, 5, Ordinal.Last, KindOfDay.WeekEndDay, 5), null,
            new DailyConfiguration(DailyConfigType.Recurring, TimeOnly.MinValue, 5, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(6, 0, 0), new TimeOnly(16, 0, 0))), new DateLimits(new DateTime(2020, 1, 4))),
                new OutPut(new DateTime(2020,1,5),"Occurs the 5th of very 5 months and every 5 hours between 6:00:00 AM and 4:00:00 PM starting on 04/01/2020")
            },

            new object[]{
                new Configuration(new DateTime(2020, 1, 1), ConfigType.Recurring, true, null, Occurs.Monthly, new MonthlyConfiguration(MonthlyConfigType.DayNumberOption, 1, Ordinal.Last, KindOfDay.WeekEndDay, 5), null,
            new DailyConfiguration(DailyConfigType.Recurring, TimeOnly.MinValue, 5, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(6, 0, 0), new TimeOnly(16, 0, 0))), new DateLimits(new DateTime(2020, 1, 3))),
                new OutPut(new DateTime(2020,2,1),"Occurs the 1st of very 5 months and every 5 hours between 6:00:00 AM and 4:00:00 PM starting on 03/01/2020")
            },

            new object[]{
                new Configuration(new DateTime(2020, 1, 1), ConfigType.Recurring, true, null, Occurs.Monthly, new MonthlyConfiguration(MonthlyConfigType.DayNumberOption, 5, Ordinal.Last, KindOfDay.WeekEndDay, 5), null,
            new DailyConfiguration(DailyConfigType.Once, new TimeOnly(17, 0, 0), 5, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(6, 0, 0), new TimeOnly(16, 0, 0))), new DateLimits(new DateTime(2020, 1, 4))),
                new OutPut(new DateTime(2020,1,5),"Occurs the 5th of very 5 months one time at 5:00:00 PM starting on 04/01/2020")
            },

            new object[]{
                new Configuration(new DateTime(2020, 1, 1), ConfigType.Recurring, true, null, Occurs.Monthly, new MonthlyConfiguration(MonthlyConfigType.DayNumberOption, 5, Ordinal.Last, KindOfDay.WeekEndDay, 5), null,
            new DailyConfiguration(DailyConfigType.Once, new TimeOnly(17, 0, 0), 5, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(6, 0, 0), new TimeOnly(16, 0, 0))), new DateLimits(new DateTime(2020, 1, 4))),
                new OutPut(new DateTime(2020,1,5),"Occurs the 5th of very 5 months one time at 5:00:00 PM starting on 04/01/2020")
            },

            new object[]{
                new Configuration(new DateTime(2020, 1, 1), ConfigType.Recurring, true, null, Occurs.Monthly, new MonthlyConfiguration(MonthlyConfigType.DayNumberOption, 3, Ordinal.Last, KindOfDay.WeekEndDay, 5), null,
            new DailyConfiguration(DailyConfigType.Once, new TimeOnly(3, 0, 0), 5, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(6, 0, 0), new TimeOnly(16, 0, 0))), new DateLimits(new DateTime(2020, 1, 4))),
                new OutPut(new DateTime(2020,2,3),"Occurs the 3rd of very 5 months one time at 3:00:00 AM starting on 04/01/2020")
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