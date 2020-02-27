using System;
using System.Collections.Generic;
using System.Text;

namespace TravelTimeAgent.Resources
{
    public class ConcentrationTime
    {
        public TimeSpan ReachTime { get; set; } //set to string to display days, hours, min

        public DateTime Date { get; set; }
        public Double Concentration { get; set; }
        public TimeSpan CumTime { get; set; } //set to string to display days, hours, min
        public String Comments { get; set; }
    }
}
