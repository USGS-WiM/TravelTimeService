using System;
using System.Collections.Generic;
using System.Text;

namespace TravelTimeAgent.Resources
{
    public class TravelTimeResult
    {
        public Dictionary<string,ConcentrationTime> LeadingEdge { get; set; }
        public Dictionary<string, ConcentrationTime> PeakConcentration { get; set; }        
        public Dictionary<string, ConcentrationTime> TrailingEdge { get; set; }
    }
}
