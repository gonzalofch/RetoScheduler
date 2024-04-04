using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RetoScheduler
{
    internal class Activity
    {
        public enum Type
        {
            Once=1,
            Recurring =2
        } 
        public  int Frecuency { get; set; }//days
        public string Occurs{ get; set; }
        public DateTime SelectedDate { get; set; }


    }
}
