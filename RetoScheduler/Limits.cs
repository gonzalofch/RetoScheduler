using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RetoScheduler
{
    public class Limits
    {

        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public Limits(DateTime startDate, DateTime? endDate = null)
        {
            StartDate = startDate;
            EndDate = endDate; 
        }   
        
    }
}
