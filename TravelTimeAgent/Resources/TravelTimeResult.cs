using System;
using System.Collections.Generic;
using System.Text;

namespace TravelTimeAgent.Resources
{
    public class TravelTimeResult
    {
        public ConcentrationTime LeadingEdge { get; set; }
        public ConcentrationTime PeakConcentration { get; set; }        
        public ConcentrationTime TrailingEdge { get; set; }
    }
}
