using System;
using System.Collections.Generic;
using System.Text;

namespace TravelTimeAgent.Resources
{
    public class TravelTimeResult
    {
        public TracerResponse Tracer_Response { get; set; }
        public Dictionary<string, Equation> Equations { get; set; }

        public TravelTimeResult()
        {
            Tracer_Response = new TracerResponse();
        }
    }
    public class TracerResponse
    {
        public Dictionary<string, ConcentrationTime> LeadingEdge { get; set; }
        public Dictionary<string, ConcentrationTime> PeakConcentration { get; set; }        
        public Dictionary<string, ConcentrationTime> TrailingEdge { get; set; } 
    }
}
