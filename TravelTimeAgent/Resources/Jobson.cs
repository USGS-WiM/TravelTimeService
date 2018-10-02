using System;
using System.Collections.Generic;
using System.Text;
using WiM.Resources.TimeSeries;
using WiM.Utilities;

namespace TravelTimeAgent.Resources
{
    
    public class Jobson
    {
        public DateTime Initial { get; private set; }
        public ConcentrationTime? LeadingEdge
        {
            get { return getLeadingEdge(); }
        }

        public ConcentrationTime? PeakConcentration { get; private set; }
        public ConcentrationTime? TrailingEdge { get; private set; }
        public ConcentrationTimeSeries ConcentrationElapseTime { get; private set; }

        public Jobson(DateTime? initialtime = null)
        {
            if (!initialtime.HasValue) initialtime = DateTime.Now;
            this.Initial = initialtime.Value;
        }

        #region "Helper Methods"
        private ConcentrationTime? getLeadingEdge()
        {
            // the time of arrival of the leading edge of the pollutant indicates when tha local problem will first exist
            //T_l=0.890*T_p (Equ 18)        

            //T_p, elapsed time to the peak concentration;
            return null;
        }
        private ConcentrationTime? getTrailingEdge() {
            //Duration from leading edge until tracer concentration has reduced to within 10 percent of the
            //peak concentration;

            //T_10d=(2*10^6)/(C_up)  Eq 

            //C_up, magnitude of the unit-peak concentration.
            //C_up=857*〖T_p〗^(〖-0.760(Q/(Q_a))〗^(-0.079) ) Eq 7

            //T_p, elapsed time to the peak concentration;
            //Q, discharge at time of measurement
            //Q_a, mean annual discharge
            return null;
        }
        private double? VelocityOfPeakConcentration() {
            //+-+-+-+-+ Slope,Discharge,Drainage Area+-+-+-+-+\\
            //in meters per second
            //V_p = 0.094+0.0143*(D'_a)^0.919*(Q'_a)^(-0.465)*S^(0.159)*Q/D_a  Eq 12

            //S, slope meter per meter

            //+-+-+-+-+ Discharge,Drainage Area +-+-+-+-+\\
            //in meters per second
            //V_p = 0.020+0.051*(D'_a)^0.821*(Q'_a)^(-0.465)*Q/D_a              Eq 14

            //+-+-+-+-+ Drainage Area +-+-+-+-+\\
            //in meters per second
            //V_p = 0.152+0.8.1(D''_a)^0.595*Q/D_a                              Eq 16

            //D''_a, dimensionless drainage area D''_a=(D_a^1.25*√g)/Q  Eq 10 (alternate)

            //+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+\\
            //D'_a, dimensionless drainage area D'_a=(D_a^1.25*√g)/Q_a          Eq 10
            //D_a, Drainage area
            //g, acceleration of gravity

            //Q'_a, the dimensionless relative discharge Q/Q_a
            //Q, discharge at time of measurement
            //Q_a, mean annual discharge

            return null;
        }
        private double? ProbablyMaxVelocityOfPeakConcentration()
        {
            //+-+-+-+-+ Slope,Discharge,Drainage Area+-+-+-+-+\\
            //in meters per second
            //V_mp = 0.25+0.02*(D'_a)^0.919*(Q'_a)^(-0.465)*S^(0.159)*Q/D_a     Eq 13

            //S, slope meter per meter

            //+-+-+-+-+ Discharge,Drainage Area +-+-+-+-+\\
            //in meters per second
            //V_mp = 0.2+0.093*(D'_a)^0.821*(Q'_a)^(-0.465)*Q/D_a               Eq 15

            //+-+-+-+-+ Drainage Area +-+-+-+-+\\
            //in meters per second
            //V_mp = 0.2+40.0*(D''_a)^0.595*Q/D_a                                Eq 17

            //D''_a, dimensionless drainage area D''_a=(D_a^1.25*√g)/Q  Eq 10 (alternate)

            //+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+\\
            //D'_a, dimensionless drainage area D'_a=(D_a^1.25*√g)/Q_a 
            //D_a, Drainage area
            //g, acceleration of gravity

            //Q'_a, the dimensionless relative discharge Q/Q_a
            //Q, discharge at time of measurement
            //Q_a, mean annual discharge

            return null;
        }
        /// <summary>
        /// T_p Elapse time of peak concentration
        /// </summary>
        /// <param name="distance">Distance in meters</param>
        /// <returns>Elapse time </returns>
        private double? ElapseTimeOfPeakConcentration(double distance) {
            return distance / (VelocityOfPeakConcentration() * Constants.CF_Sec2Hrs);
        }
        
        #endregion
        #region "Structures"
        //A structure is a value type. When a structure is created, the variable to which the struct is assigned holds
        //the struct's actual data. When the struct is assigned to a new variable, it is copied. The new variable and
        //the original variable therefore contain two separate copies of the same data. Changes made to one copy do not
        //affect the other copy.

        //In general, classes are used to model more complex behavior, or data that is intended to be modified after a
        //class object is created. Structs are best suited for small data structures that contain primarily data that is
        //not intended to be modified after the struct is created.
        public struct ConcentrationTime
        {
            public DateTime DateTime { get; set; }
            public Double Concentration { get; set; }

        }
        #endregion
    }
}
