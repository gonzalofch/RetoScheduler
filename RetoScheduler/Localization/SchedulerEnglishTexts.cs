﻿namespace RetoScheduler.Localization
{
    public static class SchedulerEnglishTexts
    {
        public static Dictionary<string, string> Traductions = new Dictionary<string, string>()
        {
            {"Scheduler:Errors:NotEnabled","You need to check field to run the Scheduler"},
            {"Scheduler:Errors:NullDateLimits","Limits Can`t be null"},
            {"Scheduler:Errors:EndDateEarlierThanStartDate","The end date cannot be earlier than the initial date"},
            {"Scheduler:Errors:EndTimeEarlierThanStartTime","The EndTime cannot be earlier than StartTime"},
            {"Scheduler:Errors:RequiredConfigDateTimeInOnce","Once Types requires an obligatory DateTime"},
            {"Scheduler:Errors:NegativeDailyFrecuency","Don't should put negative numbers or zero in number field"},
            {"Scheduler:Errors:DateOutOfRanges","DateTime can't be out of start and end range field"},
            {"Scheduler:Errors:SelectedDaysIndexOutOfBounds","The index is greater than the number of days"},
            {"Scheduler:Errors:NotSupportedSelectedWeekDay","The selected Kind of Day is not supported"},
            {"Scheduler:Errors:NotSupportedOrdinal","Selected Ordinal is not supported"},
            {"Scheduler:Errors:ExecutionEarlierThanCurrentTime","The execution time cannot be earlier than the Current Time"},
            {"DescriptionBuilder:Errors:NotSupportedMonthlyDayNumber","Not supported action for monthly day number message"},
            {"DescriptionBuilder:Errors:NotSupportedMonthlyFrequency","Not supported action for monthly frecuency message"},
            {"DescriptionBuilder:Errors:NotSupportedWeeklyFrequency","Not supported action for monthly frecuency message"},
            {"DescriptionBuilder:Errors:NotSupportedDailyFrequency","Not supported action for daily frecuency message"},
            {"Scheduler:String:OccursWithSpace","Occurs "},
            {"Scheduler:String:OnceAtWithSpace","once at "},
            {"Scheduler:String:StartingOnWithSpace","starting on "},
            {"Scheduler:String:AndFinishingOnWithSpaces"," and finishing on "},
            {"Scheduler:String:TheWithSpace","the "},
            {"Scheduler:String:OfVeryWithSpace","of very "},
            {"Scheduler:String:OrdinalStWithSpace","st "},
            {"Scheduler:String:OrdinalNdWithSpace","nd "},
            {"Scheduler:String:OrdinalRdWithSpace","rd "},
            {"Scheduler:String:OrdinalThWithSpace","th "},
            {"Scheduler:String:MonthsWithSpace","months "},
            {"Scheduler:String:MonthsWithSpaces"," months "},
            {"Scheduler:String:MonthWithSpaces"," month "},
            {"Scheduler:String:WeeksWithSpaces"," weeks "},
            {"Scheduler:String:WeekWithSpaces"," week "},
            {"Scheduler:String:WeekWithSpace","week "},
            {"Scheduler:String:Hours","hours"},
            {"Scheduler:String:Minutes","minutes"},
            {"Scheduler:String:Seconds","seconds"},
            {"Scheduler:String:EveryWithSpace","every "},
            {"Scheduler:String:AndWithSpaces"," and "},
            {"Scheduler:String:AndWithSpace","and "},
            {"Scheduler:String:On","on"},
            {"Scheduler:String:OneTimeAtWithSpace","one time at "},
            {"Scheduler:String:BetweenWithSpaces"," between "},
            {"Scheduler:String:Day","day "},
            {"Scheduler:DayOfWeek:Monday","monday" },
            {"Scheduler:DayOfWeek:Tuesday","tuesday" },
            {"Scheduler:DayOfWeek:Wednesday","wednesday" },
            {"Scheduler:DayOfWeek:Thursday","thursday" },
            {"Scheduler:DayOfWeek:Friday","friday" },
            {"Scheduler:DayOfWeek:Saturday","saturday" },
            {"Scheduler:DayOfWeek:Sunday","sunday" },
            {"Scheduler:KindOfDay:Day","day" },
            {"Scheduler:KindOfDay:WeekDay" ,"weekday"},
            {"Scheduler:KindOfDay:WeekEndDay" ,"weekendday"},
            { "Scheduler:String:OrdinalFirst", "first"},
            { "Scheduler:String:OrdinalSecond", "second"},
            { "Scheduler:String:OrdinalThird", "third"},
            { "Scheduler:String:OrdinalFourth", "fourth"},
            { "Scheduler:String:OrdinalLast", "last"},
        };
    }
}
