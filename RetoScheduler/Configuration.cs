using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RetoScheduler
{
    public class Configuration
    {
        public DateTime CurrentDate { get; set; }
        public ConfigType Type { get; set; }
        public bool Enabled {  get; set; }
        public DateTime? ConfigDateTime { get; set; }
        public Occurs Occurs { get; set; }
        public int FrecuencyInDays{ get; set; }
        public Configuration(DateTime currentDate,ConfigType type,bool enabled,DateTime? configDataTime, Occurs occurs,int frecuencyInDays) //bool tal vez por defecto true
        {
            CurrentDate = currentDate;
            Enabled = enabled;
            ConfigDateTime = configDataTime;
            Type = type;
            Occurs = occurs;    
            FrecuencyInDays = frecuencyInDays;
        }
    }
    public enum ConfigType
    {
        Once = 1,
        Recurring = 2
    }
    public enum Occurs
    {
        Daily = 1,
    }
}
