using System;
using System.Collections.Generic;
using System.Text;

namespace TravelTimeAgent.Resources
{
    public class Parameter 
    {
        public Parameter()
        {
            Required = true;
        }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Code { get; set; }
        public Units Unit { get; set; }
        public double? Value { get; set; }

        public bool Required { get; set; }
        public bool ShouldSerializeRequired()
        { return Required; }

        public double? Default { get; set; }
        public bool ShouldSerializeDefault()
        { return Default.HasValue; }

        public Parameter Clone()
        {
            return new Parameter()
            {
                Name = this.Name,
                Description = this.Description,
                Code = this.Code,
                Unit = new Units()
                {
                    Unit = this.Unit.Unit,
                    Abbr = this.Unit.Abbr
                },
                Value = this.Value,
                Required = this.Required
            };
        }
    }

    public class Units
    {
        public string Unit { get; set; }
        public string Abbr { get; set; }
    }
}
