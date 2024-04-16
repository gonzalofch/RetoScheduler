using RetoScheduler;
using RetoScheduler.Configurations;
using RetoScheduler.Configurations.Limits;
using RetoScheduler.Enums;
using System;
using System.Collections;
using System.Collections.Generic;

namespace RetoSchedulerTest
{
    public class SchedulerTestConfiguration : IEnumerable<object[]>
    {
        public readonly List<object[]> data = new List<object[]>()
        {
            new object[]{
                new Configuration
                (new DateTime(2020,1,1,0,0,0), ConfigType.Recurring,true,null,Occurs.Weekly,null,new WeeklyConfiguration(2, new List<DayOfWeek>(){DayOfWeek.Monday,DayOfWeek.Thursday,DayOfWeek.Friday}),new DailyConfiguration(DailyConfigType.Recurring,TimeOnly.MinValue,2,DailyFrecuency.Hours,new TimeLimits(new TimeOnly(4,0,0),new TimeOnly(8,0,0))),new DateLimits(new DateTime(2020,1,1))),
                new OutPut(new DateTime(2020,1,2,4,0,0),"Occurs every 2 weeks on monday, thursday and friday every 2 hours between 4:00:00 AM and 8:00:00 AM starting on 01/01/2020")
            },

            new object[]{
                new Configuration
                (new DateTime(2020,1,4), ConfigType.Once, true,new DateTime(2020,1,8),Occurs.Daily,null,null,new DailyConfiguration(DailyConfigType.Once,new TimeOnly(14,0,0),null,null,null), new DateLimits(new DateTime(2020,1,1))),
                new OutPut(new DateTime(2020,1,8,14,0,0),"Occurs once at 08/01/2020 one time at 2:00:00 PM starting on 01/01/2020")
            },

            new object[]
            {
                new Configuration(new DateTime(2020, 1, 4,0,0,0), ConfigType.Recurring, true , null, Occurs.Daily,null,null,
                    new DailyConfiguration(DailyConfigType.Recurring,TimeOnly.MinValue,3,DailyFrecuency.Hours,new TimeLimits(new TimeOnly(10,0,0),new TimeOnly(15,0,0))), new DateLimits(new DateTime(2020,1,1),null)),
                new OutPut(new DateTime(2020, 1, 4,10,0,0),"Occurs every day and every 3 hours between 10:00:00 AM and 3:00:00 PM starting on 01/01/2020")
            },

            new object[]
            {
                new Configuration
                (new DateTime (2024,4,4), ConfigType.Once, true , new DateTime(2024,05,10),Occurs.Daily,null,null,
                    new DailyConfiguration(DailyConfigType.Once,new TimeOnly(16,0,0),null,null,null),new DateLimits(new DateTime(2020,5,12))),
                new OutPut (new DateTime(2024,5,10,16,0,0),"Occurs once at 10/05/2024 one time at 4:00:00 PM starting on 12/05/2020")
            },

            new object[]
            {
                new Configuration(new DateTime(2023,12,24), ConfigType.Recurring, true , null, Occurs.Daily,null,null,
                    new DailyConfiguration(DailyConfigType.Once,new TimeOnly(16,0,0),null,null,null),new DateLimits(new DateTime(2023,12,30),new DateTime(2024,1,1))),
                new OutPut(new DateTime(2023,12,30,16,0,0),"Occurs every day one time at 4:00:00 PM starting on 30/12/2023 and finishing on 01/01/2024")
            },

            new object[]
            {
                new Configuration
                (new DateTime (2024,4,4,10,0,0), ConfigType.Once, true , new DateTime(2024,5,10),Occurs.Daily,null,null,
                    new DailyConfiguration(DailyConfigType.Recurring,TimeOnly.MinValue,30,DailyFrecuency.Minutes,new TimeLimits(new TimeOnly(15,0,0),new TimeOnly(19,0,0))),new DateLimits(new DateTime(2020,5,10),null)),
                new OutPut (new DateTime(2024,5,10,15,0,0),"Occurs once at 10/05/2024 and every 30 minutes between 3:00:00 PM and 7:00:00 PM starting on 10/05/2020")
            },

            new object[]
            {
                new Configuration(new DateTime(2023,12,24), ConfigType.Recurring, true , null, Occurs.Daily,null,null,
                    new DailyConfiguration(DailyConfigType.Once,new TimeOnly(10,0,0),null,null,null),new DateLimits(new DateTime(2020,12,27))),
                new OutPut(new DateTime(2023,12,24,10,0,0),"Occurs every day one time at 10:00:00 AM starting on 27/12/2020")
            },

            new object[]
            {

                new Configuration(
                new DateTime(2020, 1, 4), ConfigType.Recurring, true, new DateTime(2020, 1, 8), Occurs.Daily,null,null,
                new DailyConfiguration(DailyConfigType.Once,new TimeOnly(14, 0, 0),null,null,null),new DateLimits(new DateTime(2020, 1, 7),new DateTime(2020,1,9))),
                new OutPut(new DateTime(2020,1,7,14, 0, 0),"Occurs every day one time at 2:00:00 PM starting on 07/01/2020 and finishing on 09/01/2020")
            },

            new object[]
            {
                new Configuration(
                    new DateTime(2020, 1, 4), ConfigType.Recurring, true, new DateTime(2020, 1, 8), Occurs.Daily,null,
                    null,new DailyConfiguration(DailyConfigType.Once,new TimeOnly(14,0,0),null,null,null),
                    new DateLimits(new DateTime(2020, 1, 9),new DateTime(2020,1,10))),
                    new OutPut(new DateTime(2020,1,9,14,0,0),"Occurs every day one time at 2:00:00 PM starting on 09/01/2020 and finishing on 10/01/2020")
            },

            //10
            new object[]
            {
                new Configuration(
                    new DateTime(2024, 4,11),ConfigType.Once,true, new DateTime(2024, 4, 11),Occurs.Daily,null,
                    null,
                    new DailyConfiguration(DailyConfigType.Once,new TimeOnly(16,55,55),null,null,null),
                    new DateLimits(new DateTime(2024,4,11))),
                    new OutPut(new DateTime(2024,4,11,16,55,55),"Occurs once at 11/04/2024 one time at 4:55:55 PM starting on 11/04/2024")
            },
            new object[]
            {
                    new Configuration(
                    new DateTime(2024, 4,11,0,0,0),ConfigType.Recurring,true,null,Occurs.Daily,null,null,
                    new DailyConfiguration(DailyConfigType.Once,new TimeOnly(8,30,55),null,null,null),
                    new DateLimits(new DateTime(2024,4,11),new DateTime(2024,4,18))),
                    new OutPut(new DateTime(2024,4,11,8,30,55),"Occurs every day one time at 8:30:55 AM starting on 11/04/2024 and finishing on 18/04/2024")
            },

            new object[]
            {
                new Configuration(
                    new DateTime(2024, 4,11,12,46,30),ConfigType.Recurring,true,null,Occurs.Daily,null,
                    null,
                    new DailyConfiguration(DailyConfigType.Once,new TimeOnly(8,30,55),null,null,null),
                    new DateLimits(new DateTime(2024,4,14),new DateTime(2024,4,18))),
                    new OutPut(new DateTime(2024,4,14,8,30,55),"Occurs every day one time at 8:30:55 AM starting on 14/04/2024 and finishing on 18/04/2024")
            },
            new object[]{
                new Configuration
                (new DateTime(2020,1,1,0,0,0), ConfigType.Recurring,true,null,Occurs.Weekly,null,new WeeklyConfiguration(1, new List<DayOfWeek>(){DayOfWeek.Monday,DayOfWeek.Thursday,DayOfWeek.Friday}),new DailyConfiguration(DailyConfigType.Recurring,TimeOnly.MinValue,2,DailyFrecuency.Hours,new TimeLimits(new TimeOnly(4,0,0),new TimeOnly(8,0,0))),new DateLimits(new DateTime(2020,1,1))),
                new OutPut(new DateTime(2020,1,2,4,0,0),"Occurs every 1 week on monday, thursday and friday every 2 hours between 4:00:00 AM and 8:00:00 AM starting on 01/01/2020")
            },

            //MONTHLYCONFIGURATIONS
            //MONDAY
            new object[]{
                new Configuration(new DateTime(2020, 1, 1), ConfigType.Recurring, true, null, Occurs.Monthly, new MonthlyConfiguration(MonthlyConfigType.WeekDayOption, 1, Ordinal.First, KindOfDay.Monday, 3), null,
            new DailyConfiguration(DailyConfigType.Recurring, TimeOnly.MinValue, 1, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(3, 0, 0), new TimeOnly(6, 0, 0))), new DateLimits(new DateTime(2020, 1, 1))),
                new OutPut(new DateTime(2020,1,6),"Occurs every 1 week on monday, thursday and friday every 2 hours between 4:00:00 AM and 8:00:00 AM starting on 01/01/2020")
            },

            new object[]{
                new Configuration(new DateTime(2020, 1, 1), ConfigType.Recurring, true, null, Occurs.Monthly, new MonthlyConfiguration(MonthlyConfigType.WeekDayOption, 1, Ordinal.Second, KindOfDay.Monday, 3), null,
            new DailyConfiguration(DailyConfigType.Recurring, TimeOnly.MinValue, 1, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(3, 0, 0), new TimeOnly(6, 0, 0))), new DateLimits(new DateTime(2020, 1, 1))),
                new OutPut(new DateTime(2020,1,13),"Occurs every 1 week on monday, thursday and friday every 2 hours between 4:00:00 AM and 8:00:00 AM starting on 01/01/2020")
            },

             new object[]{
                new Configuration(new DateTime(2020, 1, 1), ConfigType.Recurring, true, null, Occurs.Monthly, new MonthlyConfiguration(MonthlyConfigType.WeekDayOption, 1, Ordinal.Third, KindOfDay.Monday, 3), null,
            new DailyConfiguration(DailyConfigType.Recurring, TimeOnly.MinValue, 1, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(3, 0, 0), new TimeOnly(6, 0, 0))), new DateLimits(new DateTime(2020, 1, 1))),
                new OutPut(new DateTime(2020,1,20),"Occurs every 1 week on monday, thursday and friday every 2 hours between 4:00:00 AM and 8:00:00 AM starting on 01/01/2020")
            },

              new object[]{
                new Configuration(new DateTime(2020, 1, 1), ConfigType.Recurring, true, null, Occurs.Monthly, new MonthlyConfiguration(MonthlyConfigType.WeekDayOption, 1, Ordinal.Fourth, KindOfDay.Monday, 3), null,
            new DailyConfiguration(DailyConfigType.Recurring, TimeOnly.MinValue, 1, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(3, 0, 0), new TimeOnly(6, 0, 0))), new DateLimits(new DateTime(2020, 1, 1))),
                new OutPut(new DateTime(2020,1,27),"Occurs every 1 week on monday, thursday and friday every 2 hours between 4:00:00 AM and 8:00:00 AM starting on 01/01/2020")
            },

               new object[]{
                new Configuration(new DateTime(2020, 1, 1), ConfigType.Recurring, true, null, Occurs.Monthly, new MonthlyConfiguration(MonthlyConfigType.WeekDayOption, 1, Ordinal.Last, KindOfDay.Monday, 3), null,
            new DailyConfiguration(DailyConfigType.Recurring, TimeOnly.MinValue, 1, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(3, 0, 0), new TimeOnly(6, 0, 0))), new DateLimits(new DateTime(2020, 1, 1))),
                new OutPut(new DateTime(2020,1,27),"Occurs every 1 week on monday, thursday and friday every 2 hours between 4:00:00 AM and 8:00:00 AM starting on 01/01/2020")
            },

            //TUESDAY

            new object[]{
                new Configuration(new DateTime(2020, 1, 1), ConfigType.Recurring, true, null, Occurs.Monthly, new MonthlyConfiguration(MonthlyConfigType.WeekDayOption, 1, Ordinal.First, KindOfDay.Tuesday, 3), null,
            new DailyConfiguration(DailyConfigType.Recurring, TimeOnly.MinValue, 1, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(3, 0, 0), new TimeOnly(6, 0, 0))), new DateLimits(new DateTime(2020, 1, 1))),
                new OutPut(new DateTime(2020,1,7),"Occurs every 1 week on monday, thursday and friday every 2 hours between 4:00:00 AM and 8:00:00 AM starting on 01/01/2020")
            },

            new object[]{
                new Configuration(new DateTime(2020, 1, 1), ConfigType.Recurring, true, null, Occurs.Monthly, new MonthlyConfiguration(MonthlyConfigType.WeekDayOption, 1, Ordinal.Second, KindOfDay.Tuesday, 3), null,
            new DailyConfiguration(DailyConfigType.Recurring, TimeOnly.MinValue, 1, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(3, 0, 0), new TimeOnly(6, 0, 0))), new DateLimits(new DateTime(2020, 1, 1))),
                new OutPut(new DateTime(2020,1,14),"Occurs every 1 week on monday, thursday and friday every 2 hours between 4:00:00 AM and 8:00:00 AM starting on 01/01/2020")
            },

            new object[]{
                new Configuration(new DateTime(2020, 1, 1), ConfigType.Recurring, true, null, Occurs.Monthly, new MonthlyConfiguration(MonthlyConfigType.WeekDayOption, 1, Ordinal.Third, KindOfDay.Tuesday, 3), null,
            new DailyConfiguration(DailyConfigType.Recurring, TimeOnly.MinValue, 1, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(3, 0, 0), new TimeOnly(6, 0, 0))), new DateLimits(new DateTime(2020, 1, 1))),
                new OutPut(new DateTime(2020,1,21),"Occurs every 1 week on monday, thursday and friday every 2 hours between 4:00:00 AM and 8:00:00 AM starting on 01/01/2020")
            },

            new object[]{
                new Configuration(new DateTime(2020, 1, 1), ConfigType.Recurring, true, null, Occurs.Monthly, new MonthlyConfiguration(MonthlyConfigType.WeekDayOption, 1, Ordinal.Fourth, KindOfDay.Tuesday, 3), null,
            new DailyConfiguration(DailyConfigType.Recurring, TimeOnly.MinValue, 1, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(3, 0, 0), new TimeOnly(6, 0, 0))), new DateLimits(new DateTime(2020, 1, 1))),
                new OutPut(new DateTime(2020,1,28),"Occurs every 1 week on monday, thursday and friday every 2 hours between 4:00:00 AM and 8:00:00 AM starting on 01/01/2020")
            },

            new object[]{
                new Configuration(new DateTime(2020, 1, 1), ConfigType.Recurring, true, null, Occurs.Monthly, new MonthlyConfiguration(MonthlyConfigType.WeekDayOption, 1, Ordinal.Last, KindOfDay.Tuesday, 3), null,
            new DailyConfiguration(DailyConfigType.Recurring, TimeOnly.MinValue, 1, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(3, 0, 0), new TimeOnly(6, 0, 0))), new DateLimits(new DateTime(2020, 1, 1))),
                new OutPut(new DateTime(2020,1,28),"Occurs every 1 week on monday, thursday and friday every 2 hours between 4:00:00 AM and 8:00:00 AM starting on 01/01/2020")
            },

            //WEDNESDAY
            
            new object[]{
                new Configuration(new DateTime(2020, 1, 1), ConfigType.Recurring, true, null, Occurs.Monthly, new MonthlyConfiguration(MonthlyConfigType.WeekDayOption, 1, Ordinal.First, KindOfDay.Wednesday, 3), null,
            new DailyConfiguration(DailyConfigType.Recurring, TimeOnly.MinValue, 1, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(3, 0, 0), new TimeOnly(6, 0, 0))), new DateLimits(new DateTime(2020, 1, 1))),
                new OutPut(new DateTime(2020,1,1),"Occurs every 1 week on monday, thursday and friday every 2 hours between 4:00:00 AM and 8:00:00 AM starting on 01/01/2020")
            },

            new object[]{
                new Configuration(new DateTime(2020, 1, 1), ConfigType.Recurring, true, null, Occurs.Monthly, new MonthlyConfiguration(MonthlyConfigType.WeekDayOption, 1, Ordinal.Second, KindOfDay.Wednesday, 3), null,
            new DailyConfiguration(DailyConfigType.Recurring, TimeOnly.MinValue, 1, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(3, 0, 0), new TimeOnly(6, 0, 0))), new DateLimits(new DateTime(2020, 1, 1))),
                new OutPut(new DateTime(2020,1,8),"Occurs every 1 week on monday, thursday and friday every 2 hours between 4:00:00 AM and 8:00:00 AM starting on 01/01/2020")
            },

            new object[]{
                new Configuration(new DateTime(2020, 1, 1), ConfigType.Recurring, true, null, Occurs.Monthly, new MonthlyConfiguration(MonthlyConfigType.WeekDayOption, 1, Ordinal.Third, KindOfDay.Wednesday, 3), null,
            new DailyConfiguration(DailyConfigType.Recurring, TimeOnly.MinValue, 1, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(3, 0, 0), new TimeOnly(6, 0, 0))), new DateLimits(new DateTime(2020, 1, 1))),
                new OutPut(new DateTime(2020,1,15),"Occurs every 1 week on monday, thursday and friday every 2 hours between 4:00:00 AM and 8:00:00 AM starting on 01/01/2020")
            },

            new object[]{
                new Configuration(new DateTime(2020, 1, 1), ConfigType.Recurring, true, null, Occurs.Monthly, new MonthlyConfiguration(MonthlyConfigType.WeekDayOption, 1, Ordinal.Fourth, KindOfDay.Wednesday, 3), null,
            new DailyConfiguration(DailyConfigType.Recurring, TimeOnly.MinValue, 1, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(3, 0, 0), new TimeOnly(6, 0, 0))), new DateLimits(new DateTime(2020, 1, 1))),
                new OutPut(new DateTime(2020,1,22),"Occurs every 1 week on monday, thursday and friday every 2 hours between 4:00:00 AM and 8:00:00 AM starting on 01/01/2020")
            },

            new object[]{
                new Configuration(new DateTime(2020, 1, 1), ConfigType.Recurring, true, null, Occurs.Monthly, new MonthlyConfiguration(MonthlyConfigType.WeekDayOption, 1, Ordinal.Last, KindOfDay.Wednesday, 3), null,
            new DailyConfiguration(DailyConfigType.Recurring, TimeOnly.MinValue, 1, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(3, 0, 0), new TimeOnly(6, 0, 0))), new DateLimits(new DateTime(2020, 1, 1))),
                new OutPut(new DateTime(2020,1,29),"Occurs every 1 week on monday, thursday and friday every 2 hours between 4:00:00 AM and 8:00:00 AM starting on 01/01/2020")
            },

            //THURSDAY
            
            new object[]{
                new Configuration(new DateTime(2020, 1, 1), ConfigType.Recurring, true, null, Occurs.Monthly, new MonthlyConfiguration(MonthlyConfigType.WeekDayOption, 1, Ordinal.First, KindOfDay.Thursday, 3), null,
            new DailyConfiguration(DailyConfigType.Recurring, TimeOnly.MinValue, 1, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(3, 0, 0), new TimeOnly(6, 0, 0))), new DateLimits(new DateTime(2020, 1, 1))),
                new OutPut(new DateTime(2020,1,2),"Occurs every 1 week on monday, thursday and friday every 2 hours between 4:00:00 AM and 8:00:00 AM starting on 01/01/2020")
            },

            new object[]{
                new Configuration(new DateTime(2020, 1, 1), ConfigType.Recurring, true, null, Occurs.Monthly, new MonthlyConfiguration(MonthlyConfigType.WeekDayOption, 1, Ordinal.Second, KindOfDay.Thursday, 3), null,
            new DailyConfiguration(DailyConfigType.Recurring, TimeOnly.MinValue, 1, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(3, 0, 0), new TimeOnly(6, 0, 0))), new DateLimits(new DateTime(2020, 1, 1))),
                new OutPut(new DateTime(2020,1,9),"Occurs every 1 week on monday, thursday and friday every 2 hours between 4:00:00 AM and 8:00:00 AM starting on 01/01/2020")
            },

            new object[]{
                new Configuration(new DateTime(2020, 1, 1), ConfigType.Recurring, true, null, Occurs.Monthly, new MonthlyConfiguration(MonthlyConfigType.WeekDayOption, 1, Ordinal.Third, KindOfDay.Thursday, 3), null,
            new DailyConfiguration(DailyConfigType.Recurring, TimeOnly.MinValue, 1, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(3, 0, 0), new TimeOnly(6, 0, 0))), new DateLimits(new DateTime(2020, 1, 1))),
                new OutPut(new DateTime(2020,1,16),"Occurs every 1 week on monday, thursday and friday every 2 hours between 4:00:00 AM and 8:00:00 AM starting on 01/01/2020")
            },

            new object[]{
                new Configuration(new DateTime(2020, 1, 1), ConfigType.Recurring, true, null, Occurs.Monthly, new MonthlyConfiguration(MonthlyConfigType.WeekDayOption, 1, Ordinal.Fourth, KindOfDay.Thursday, 3), null,
            new DailyConfiguration(DailyConfigType.Recurring, TimeOnly.MinValue, 1, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(3, 0, 0), new TimeOnly(6, 0, 0))), new DateLimits(new DateTime(2020, 1, 1))),
                new OutPut(new DateTime(2020,1,23),"Occurs every 1 week on monday, thursday and friday every 2 hours between 4:00:00 AM and 8:00:00 AM starting on 01/01/2020")
            },

            new object[]{
                new Configuration(new DateTime(2020, 1, 1), ConfigType.Recurring, true, null, Occurs.Monthly, new MonthlyConfiguration(MonthlyConfigType.WeekDayOption, 1, Ordinal.Last, KindOfDay.Thursday, 3), null,
            new DailyConfiguration(DailyConfigType.Recurring, TimeOnly.MinValue, 1, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(3, 0, 0), new TimeOnly(6, 0, 0))), new DateLimits(new DateTime(2020, 1, 1))),
                new OutPut(new DateTime(2020,1,30),"Occurs every 1 week on monday, thursday and friday every 2 hours between 4:00:00 AM and 8:00:00 AM starting on 01/01/2020")
            },


            //FRIDAY
            
            new object[]{
                new Configuration(new DateTime(2020, 1, 1), ConfigType.Recurring, true, null, Occurs.Monthly, new MonthlyConfiguration(MonthlyConfigType.WeekDayOption, 1, Ordinal.First, KindOfDay.Friday, 3), null,
            new DailyConfiguration(DailyConfigType.Recurring, TimeOnly.MinValue, 1, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(3, 0, 0), new TimeOnly(6, 0, 0))), new DateLimits(new DateTime(2020, 1, 1))),
                new OutPut(new DateTime(2020,1,3),"Occurs every 1 week on monday, thursday and friday every 2 hours between 4:00:00 AM and 8:00:00 AM starting on 01/01/2020")
            },

            new object[]{
                new Configuration(new DateTime(2020, 1, 1), ConfigType.Recurring, true, null, Occurs.Monthly, new MonthlyConfiguration(MonthlyConfigType.WeekDayOption, 1, Ordinal.Second, KindOfDay.Friday, 3), null,
            new DailyConfiguration(DailyConfigType.Recurring, TimeOnly.MinValue, 1, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(3, 0, 0), new TimeOnly(6, 0, 0))), new DateLimits(new DateTime(2020, 1, 1))),
                new OutPut(new DateTime(2020,1,10),"Occurs every 1 week on monday, thursday and friday every 2 hours between 4:00:00 AM and 8:00:00 AM starting on 01/01/2020")
            },

            new object[]{
                new Configuration(new DateTime(2020, 1, 1), ConfigType.Recurring, true, null, Occurs.Monthly, new MonthlyConfiguration(MonthlyConfigType.WeekDayOption, 1, Ordinal.Third, KindOfDay.Friday, 3), null,
            new DailyConfiguration(DailyConfigType.Recurring, TimeOnly.MinValue, 1, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(3, 0, 0), new TimeOnly(6, 0, 0))), new DateLimits(new DateTime(2020, 1, 1))),
                new OutPut(new DateTime(2020,1,17),"Occurs every 1 week on monday, thursday and friday every 2 hours between 4:00:00 AM and 8:00:00 AM starting on 01/01/2020")
            },

            new object[]{
                new Configuration(new DateTime(2020, 1, 1), ConfigType.Recurring, true, null, Occurs.Monthly, new MonthlyConfiguration(MonthlyConfigType.WeekDayOption, 1, Ordinal.Fourth, KindOfDay.Friday, 3), null,
            new DailyConfiguration(DailyConfigType.Recurring, TimeOnly.MinValue, 1, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(3, 0, 0), new TimeOnly(6, 0, 0))), new DateLimits(new DateTime(2020, 1, 1))),
                new OutPut(new DateTime(2020,1,24),"Occurs every 1 week on monday, thursday and friday every 2 hours between 4:00:00 AM and 8:00:00 AM starting on 01/01/2020")
            },

            new object[]{
                new Configuration(new DateTime(2020, 1, 1), ConfigType.Recurring, true, null, Occurs.Monthly, new MonthlyConfiguration(MonthlyConfigType.WeekDayOption, 1, Ordinal.Last, KindOfDay.Friday, 3), null,
            new DailyConfiguration(DailyConfigType.Recurring, TimeOnly.MinValue, 1, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(3, 0, 0), new TimeOnly(6, 0, 0))), new DateLimits(new DateTime(2020, 1, 1))),
                new OutPut(new DateTime(2020,1,31),"Occurs every 1 week on monday, thursday and friday every 2 hours between 4:00:00 AM and 8:00:00 AM starting on 01/01/2020")
            },

            //SATURDAY
            
            new object[]{
                new Configuration(new DateTime(2020, 1, 1), ConfigType.Recurring, true, null, Occurs.Monthly, new MonthlyConfiguration(MonthlyConfigType.WeekDayOption, 1, Ordinal.First, KindOfDay.Saturday, 3), null,
            new DailyConfiguration(DailyConfigType.Recurring, TimeOnly.MinValue, 1, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(3, 0, 0), new TimeOnly(6, 0, 0))), new DateLimits(new DateTime(2020, 1, 1))),
                new OutPut(new DateTime(2020,1,4),"Occurs every 1 week on monday, thursday and friday every 2 hours between 4:00:00 AM and 8:00:00 AM starting on 01/01/2020")
            },

            new object[]{
                new Configuration(new DateTime(2020, 1, 1), ConfigType.Recurring, true, null, Occurs.Monthly, new MonthlyConfiguration(MonthlyConfigType.WeekDayOption, 1, Ordinal.Second, KindOfDay.Saturday, 3), null,
            new DailyConfiguration(DailyConfigType.Recurring, TimeOnly.MinValue, 1, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(3, 0, 0), new TimeOnly(6, 0, 0))), new DateLimits(new DateTime(2020, 1, 1))),
                new OutPut(new DateTime(2020,1,11),"Occurs every 1 week on monday, thursday and friday every 2 hours between 4:00:00 AM and 8:00:00 AM starting on 01/01/2020")
            },

            new object[]{
                new Configuration(new DateTime(2020, 1, 1), ConfigType.Recurring, true, null, Occurs.Monthly, new MonthlyConfiguration(MonthlyConfigType.WeekDayOption, 1, Ordinal.Third, KindOfDay.Saturday, 3), null,
            new DailyConfiguration(DailyConfigType.Recurring, TimeOnly.MinValue, 1, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(3, 0, 0), new TimeOnly(6, 0, 0))), new DateLimits(new DateTime(2020, 1, 1))),
                new OutPut(new DateTime(2020,1,18),"Occurs every 1 week on monday, thursday and friday every 2 hours between 4:00:00 AM and 8:00:00 AM starting on 01/01/2020")
            },

            new object[]{
                new Configuration(new DateTime(2020, 1, 1), ConfigType.Recurring, true, null, Occurs.Monthly, new MonthlyConfiguration(MonthlyConfigType.WeekDayOption, 1, Ordinal.Fourth, KindOfDay.Saturday, 3), null,
            new DailyConfiguration(DailyConfigType.Recurring, TimeOnly.MinValue, 1, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(3, 0, 0), new TimeOnly(6, 0, 0))), new DateLimits(new DateTime(2020, 1, 1))),
                new OutPut(new DateTime(2020,1,25),"Occurs every 1 week on monday, thursday and friday every 2 hours between 4:00:00 AM and 8:00:00 AM starting on 01/01/2020")
            },

            new object[]{
                new Configuration(new DateTime(2020, 1, 1), ConfigType.Recurring, true, null, Occurs.Monthly, new MonthlyConfiguration(MonthlyConfigType.WeekDayOption, 1, Ordinal.Last, KindOfDay.Saturday, 3), null,
            new DailyConfiguration(DailyConfigType.Recurring, TimeOnly.MinValue, 1, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(3, 0, 0), new TimeOnly(6, 0, 0))), new DateLimits(new DateTime(2020, 1, 1))),
                new OutPut(new DateTime(2020,1,25),"Occurs every 1 week on monday, thursday and friday every 2 hours between 4:00:00 AM and 8:00:00 AM starting on 01/01/2020")
            },

            //SUNDAY
            
            new object[]{
                new Configuration(new DateTime(2020, 1, 1), ConfigType.Recurring, true, null, Occurs.Monthly, new MonthlyConfiguration(MonthlyConfigType.WeekDayOption, 1, Ordinal.First, KindOfDay.Sunday, 3), null,
            new DailyConfiguration(DailyConfigType.Recurring, TimeOnly.MinValue, 1, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(3, 0, 0), new TimeOnly(6, 0, 0))), new DateLimits(new DateTime(2020, 1, 1))),
                new OutPut(new DateTime(2020,1,5),"Occurs every 1 week on monday, thursday and friday every 2 hours between 4:00:00 AM and 8:00:00 AM starting on 01/01/2020")
            },

            new object[]{
                new Configuration(new DateTime(2020, 1, 1), ConfigType.Recurring, true, null, Occurs.Monthly, new MonthlyConfiguration(MonthlyConfigType.WeekDayOption, 1, Ordinal.Second, KindOfDay.Sunday, 3), null,
            new DailyConfiguration(DailyConfigType.Recurring, TimeOnly.MinValue, 1, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(3, 0, 0), new TimeOnly(6, 0, 0))), new DateLimits(new DateTime(2020, 1, 1))),
                new OutPut(new DateTime(2020,1,12),"Occurs every 1 week on monday, thursday and friday every 2 hours between 4:00:00 AM and 8:00:00 AM starting on 01/01/2020")
            },

            new object[]{
                new Configuration(new DateTime(2020, 1, 1), ConfigType.Recurring, true, null, Occurs.Monthly, new MonthlyConfiguration(MonthlyConfigType.WeekDayOption, 1, Ordinal.Third, KindOfDay.Sunday, 3), null,
            new DailyConfiguration(DailyConfigType.Recurring, TimeOnly.MinValue, 1, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(3, 0, 0), new TimeOnly(6, 0, 0))), new DateLimits(new DateTime(2020, 1, 1))),
                new OutPut(new DateTime(2020,1,19),"Occurs every 1 week on monday, thursday and friday every 2 hours between 4:00:00 AM and 8:00:00 AM starting on 01/01/2020")
            },

            new object[]{
                new Configuration(new DateTime(2020, 1, 1), ConfigType.Recurring, true, null, Occurs.Monthly, new MonthlyConfiguration(MonthlyConfigType.WeekDayOption, 1, Ordinal.Fourth, KindOfDay.Sunday, 3), null,
            new DailyConfiguration(DailyConfigType.Recurring, TimeOnly.MinValue, 1, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(3, 0, 0), new TimeOnly(6, 0, 0))), new DateLimits(new DateTime(2020, 1, 1))),
                new OutPut(new DateTime(2020,1,26),"Occurs every 1 week on monday, thursday and friday every 2 hours between 4:00:00 AM and 8:00:00 AM starting on 01/01/2020")
            },

            new object[]{
                new Configuration(new DateTime(2020, 1, 1), ConfigType.Recurring, true, null, Occurs.Monthly, new MonthlyConfiguration(MonthlyConfigType.WeekDayOption, 1, Ordinal.Last, KindOfDay.Sunday, 3), null,
            new DailyConfiguration(DailyConfigType.Recurring, TimeOnly.MinValue, 1, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(3, 0, 0), new TimeOnly(6, 0, 0))), new DateLimits(new DateTime(2020, 1, 1))),
                new OutPut(new DateTime(2020,1,26),"Occurs every 1 week on monday, thursday and friday every 2 hours between 4:00:00 AM and 8:00:00 AM starting on 01/01/2020")
            },


            //DAY
            
            new object[]{
                new Configuration(new DateTime(2020, 1, 1), ConfigType.Recurring, true, null, Occurs.Monthly, new MonthlyConfiguration(MonthlyConfigType.WeekDayOption, 1, Ordinal.First, KindOfDay.Day, 3), null,
            new DailyConfiguration(DailyConfigType.Recurring, TimeOnly.MinValue, 1, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(3, 0, 0), new TimeOnly(6, 0, 0))), new DateLimits(new DateTime(2020, 1, 1))),
                new OutPut(new DateTime(2020,1,1),"Occurs every 1 week on monday, thursday and friday every 2 hours between 4:00:00 AM and 8:00:00 AM starting on 01/01/2020")
            },

            new object[]{
                new Configuration(new DateTime(2020, 1, 1), ConfigType.Recurring, true, null, Occurs.Monthly, new MonthlyConfiguration(MonthlyConfigType.WeekDayOption, 1, Ordinal.Second, KindOfDay.Day, 3), null,
            new DailyConfiguration(DailyConfigType.Recurring, TimeOnly.MinValue, 1, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(3, 0, 0), new TimeOnly(6, 0, 0))), new DateLimits(new DateTime(2020, 1, 1))),
                new OutPut(new DateTime(2020,1,2),"Occurs every 1 week on monday, thursday and friday every 2 hours between 4:00:00 AM and 8:00:00 AM starting on 01/01/2020")
            },

            new object[]{
                new Configuration(new DateTime(2020, 1, 1), ConfigType.Recurring, true, null, Occurs.Monthly, new MonthlyConfiguration(MonthlyConfigType.WeekDayOption, 1, Ordinal.Third, KindOfDay.Day, 3), null,
            new DailyConfiguration(DailyConfigType.Recurring, TimeOnly.MinValue, 1, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(3, 0, 0), new TimeOnly(6, 0, 0))), new DateLimits(new DateTime(2020, 1, 1))),
                new OutPut(new DateTime(2020,1,3),"Occurs every 1 week on monday, thursday and friday every 2 hours between 4:00:00 AM and 8:00:00 AM starting on 01/01/2020")
            },

            new object[]{
                new Configuration(new DateTime(2020, 1, 1), ConfigType.Recurring, true, null, Occurs.Monthly, new MonthlyConfiguration(MonthlyConfigType.WeekDayOption, 1, Ordinal.Fourth, KindOfDay.Day, 3), null,
            new DailyConfiguration(DailyConfigType.Recurring, TimeOnly.MinValue, 1, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(3, 0, 0), new TimeOnly(6, 0, 0))), new DateLimits(new DateTime(2020, 1, 1))),
                new OutPut(new DateTime(2020,1,4),"Occurs every 1 week on monday, thursday and friday every 2 hours between 4:00:00 AM and 8:00:00 AM starting on 01/01/2020")
            },

            new object[]{
                new Configuration(new DateTime(2020, 1, 1), ConfigType.Recurring, true, null, Occurs.Monthly, new MonthlyConfiguration(MonthlyConfigType.WeekDayOption, 1, Ordinal.Last, KindOfDay.Day, 3), null,
            new DailyConfiguration(DailyConfigType.Recurring, TimeOnly.MinValue, 1, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(3, 0, 0), new TimeOnly(6, 0, 0))), new DateLimits(new DateTime(2020, 1, 1))),
                new OutPut(new DateTime(2020,1,31),"Occurs every 1 week on monday, thursday and friday every 2 hours between 4:00:00 AM and 8:00:00 AM starting on 01/01/2020")
            },


            //WEEKDAY
            
            new object[]{
                new Configuration(new DateTime(2020, 1, 1), ConfigType.Recurring, true, null, Occurs.Monthly, new MonthlyConfiguration(MonthlyConfigType.WeekDayOption, 1, Ordinal.First, KindOfDay.WeekDay, 3), null,
            new DailyConfiguration(DailyConfigType.Recurring, TimeOnly.MinValue, 1, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(3, 0, 0), new TimeOnly(6, 0, 0))), new DateLimits(new DateTime(2020, 1, 1))),
                new OutPut(new DateTime(2020,1,1),"Occurs every 1 week on monday, thursday and friday every 2 hours between 4:00:00 AM and 8:00:00 AM starting on 01/01/2020")
            },

            new object[]{
                new Configuration(new DateTime(2020, 1, 1), ConfigType.Recurring, true, null, Occurs.Monthly, new MonthlyConfiguration(MonthlyConfigType.WeekDayOption, 1, Ordinal.Second, KindOfDay.WeekDay, 3), null,
            new DailyConfiguration(DailyConfigType.Recurring, TimeOnly.MinValue, 1, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(3, 0, 0), new TimeOnly(6, 0, 0))), new DateLimits(new DateTime(2020, 1, 1))),
                new OutPut(new DateTime(2020,1,2),"Occurs every 1 week on monday, thursday and friday every 2 hours between 4:00:00 AM and 8:00:00 AM starting on 01/01/2020")
            },

            new object[]{
                new Configuration(new DateTime(2020, 1, 1), ConfigType.Recurring, true, null, Occurs.Monthly, new MonthlyConfiguration(MonthlyConfigType.WeekDayOption, 1, Ordinal.Third, KindOfDay.WeekDay, 3), null,
            new DailyConfiguration(DailyConfigType.Recurring, TimeOnly.MinValue, 1, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(3, 0, 0), new TimeOnly(6, 0, 0))), new DateLimits(new DateTime(2020, 1, 1))),
                new OutPut(new DateTime(2020,1,3),"Occurs every 1 week on monday, thursday and friday every 2 hours between 4:00:00 AM and 8:00:00 AM starting on 01/01/2020")
            },

            new object[]{
                new Configuration(new DateTime(2020, 1, 1), ConfigType.Recurring, true, null, Occurs.Monthly, new MonthlyConfiguration(MonthlyConfigType.WeekDayOption, 1, Ordinal.Fourth, KindOfDay.WeekDay, 3), null,
            new DailyConfiguration(DailyConfigType.Recurring, TimeOnly.MinValue, 1, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(3, 0, 0), new TimeOnly(6, 0, 0))), new DateLimits(new DateTime(2020, 1, 1))),
                new OutPut(new DateTime(2020,1,6),"Occurs every 1 week on monday, thursday and friday every 2 hours between 4:00:00 AM and 8:00:00 AM starting on 01/01/2020")
            },

            new object[]{
                new Configuration(new DateTime(2020, 1, 1), ConfigType.Recurring, true, null, Occurs.Monthly, new MonthlyConfiguration(MonthlyConfigType.WeekDayOption, 1, Ordinal.Last, KindOfDay.WeekDay, 3), null,
            new DailyConfiguration(DailyConfigType.Recurring, TimeOnly.MinValue, 1, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(3, 0, 0), new TimeOnly(6, 0, 0))), new DateLimits(new DateTime(2020, 1, 1))),
                new OutPut(new DateTime(2020,1,31),"Occurs every 1 week on monday, thursday and friday every 2 hours between 4:00:00 AM and 8:00:00 AM starting on 01/01/2020")
            },


            //WEKENDDAY
            
            new object[]{
                new Configuration(new DateTime(2020, 1, 1), ConfigType.Recurring, true, null, Occurs.Monthly, new MonthlyConfiguration(MonthlyConfigType.WeekDayOption, 1, Ordinal.First, KindOfDay.WeekEndDay, 3), null,
            new DailyConfiguration(DailyConfigType.Recurring, TimeOnly.MinValue, 1, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(3, 0, 0), new TimeOnly(6, 0, 0))), new DateLimits(new DateTime(2020, 1, 1))),
                new OutPut(new DateTime(2020,1,4),"Occurs every 1 week on monday, thursday and friday every 2 hours between 4:00:00 AM and 8:00:00 AM starting on 01/01/2020")
            },

            new object[]{
                new Configuration(new DateTime(2020, 1, 1), ConfigType.Recurring, true, null, Occurs.Monthly, new MonthlyConfiguration(MonthlyConfigType.WeekDayOption, 1, Ordinal.Second, KindOfDay.WeekEndDay, 3), null,
            new DailyConfiguration(DailyConfigType.Recurring, TimeOnly.MinValue, 1, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(3, 0, 0), new TimeOnly(6, 0, 0))), new DateLimits(new DateTime(2020, 1, 1))),
                new OutPut(new DateTime(2020,1,5),"Occurs every 1 week on monday, thursday and friday every 2 hours between 4:00:00 AM and 8:00:00 AM starting on 01/01/2020")
            },

            new object[]{
                new Configuration(new DateTime(2020, 1, 1), ConfigType.Recurring, true, null, Occurs.Monthly, new MonthlyConfiguration(MonthlyConfigType.WeekDayOption, 1, Ordinal.Third, KindOfDay.WeekEndDay, 3), null,
            new DailyConfiguration(DailyConfigType.Recurring, TimeOnly.MinValue, 1, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(3, 0, 0), new TimeOnly(6, 0, 0))), new DateLimits(new DateTime(2020, 1, 1))),
                new OutPut(new DateTime(2020,1,11),"Occurs every 1 week on monday, thursday and friday every 2 hours between 4:00:00 AM and 8:00:00 AM starting on 01/01/2020")
            },

            new object[]{
                new Configuration(new DateTime(2020, 1, 1), ConfigType.Recurring, true, null, Occurs.Monthly, new MonthlyConfiguration(MonthlyConfigType.WeekDayOption, 1, Ordinal.Fourth, KindOfDay.WeekEndDay, 3), null,
            new DailyConfiguration(DailyConfigType.Recurring, TimeOnly.MinValue, 1, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(3, 0, 0), new TimeOnly(6, 0, 0))), new DateLimits(new DateTime(2020, 1, 1))),
                new OutPut(new DateTime(2020,1,12),"Occurs every 1 week on monday, thursday and friday every 2 hours between 4:00:00 AM and 8:00:00 AM starting on 01/01/2020")
            },

            new object[]{
                new Configuration(new DateTime(2020, 1, 1), ConfigType.Recurring, true, null, Occurs.Monthly, new MonthlyConfiguration(MonthlyConfigType.WeekDayOption, 1, Ordinal.Last, KindOfDay.WeekEndDay, 3), null,
            new DailyConfiguration(DailyConfigType.Recurring, TimeOnly.MinValue, 1, DailyFrecuency.Hours, new TimeLimits(new TimeOnly(3, 0, 0), new TimeOnly(6, 0, 0))), new DateLimits(new DateTime(2020, 1, 1))),
                new OutPut(new DateTime(2020,1,26),"Occurs every 1 week on monday, thursday and friday every 2 hours between 4:00:00 AM and 8:00:00 AM starting on 01/01/2020")
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
