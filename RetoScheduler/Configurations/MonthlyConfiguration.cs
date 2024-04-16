﻿using RetoScheduler.Configurations.Limits;
using RetoScheduler.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RetoScheduler.Configurations
{
    public class MonthlyConfiguration
    {

        public MonthlyConfiguration(MonthlyConfigType type, int dayNumber, Ordinal ordinalNumber, KindOfDay kindOfDay, int frecuency)
        {
            Type = type;
            DayNumber =  dayNumber;
            OrdinalNumber = ordinalNumber ;
            SelectedDay =   kindOfDay ;
            Frecuency = frecuency;
        }

        public MonthlyConfigType Type { get; set; }

        public int DayNumber { get; set; }

        public Ordinal OrdinalNumber { get; set; }

        public KindOfDay SelectedDay { get; set; }

        public int Frecuency { get; set; }
    }
}
