using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RetoScheduler
{
    public class Scheduler
    {

        public Scheduler() { 
        
        }
        public OutPut Init(Configuration config)
        {

            DateTime dateTime = DateTime.Now;
            string description = string.Empty;

            if (config.Type==ConfigType.Once)
            {
                if (config.ConfigDateTime.HasValue==false)
                {
                    throw new Exception("Once Types requires an obligatory DateTime");
                }
                
                dateTime = config.ConfigDateTime.Value;
                string d =dateTime.ToString("dd/MM/yyyy");
                string t = dateTime.ToString("HH:mm");
                description = "Occurs once. Schedule will be used on "+ d + " at "+ t + " starting on 01/01/2020";
            }
            else
            {
                dateTime = config.CurrentDate.AddDays(config.FrecuencyInDays);
                string d = dateTime.ToString("dd/MM/yyyy");
                string t = dateTime.ToString("HH:mm");
                description = "Occurs every day. Schedule will be used on " + d + " starting on 01/01/2020";

            }

            return new OutPut(dateTime, description);
            
        }


    }
}
