using System;
using System.Collections.Generic;
using System.Text;

namespace TravelTimeAgent.Resources
{
    public class ConcentrationTime
    {
        public String ReachTime { get; set; }

        public DateTime Date { get; set; }
        public Double Concentration { get; set; }
        public String CumTime { get; set; }
        public String Comments { get; set; }
    }
}
