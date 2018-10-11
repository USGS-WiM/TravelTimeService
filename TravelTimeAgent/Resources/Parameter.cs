using System;
using System.Collections.Generic;
using System.Text;
using WiM.Resources;

namespace TravelTimeAgent.Resources
{
    public class Parameter : IParameter
    {
        public Parameter()
        {
            Required = true;
        }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Code { get; set; }
        public IUnit Unit { get; set; }
        public double? Value { get; set; }

        public bool Required { get; set; }
        public bool ShouldSerializeRequired()
        { return Required; }

        public double? Default { get; set; }
        public bool ShouldSerializeDefault()
        { return Default.HasValue; }
    }

    public class Units : IUnit
    {
        public string Unit { get; set; }
        public string Abbr { get; set; }
    }
}
