using System;
using System.Collections.Generic;
using System.Text;
using WIM.Resources;

namespace TravelTimeAgent.Resources
{
    public class Reach
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public List<Parameter> Parameters { get; set; }
        public TravelTimeResult Result { get; set; }
        public bool ShouldSerializeResult()
        { return Result != null; }
    }
}
