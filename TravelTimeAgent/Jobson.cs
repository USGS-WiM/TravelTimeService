//------------------------------------------------------------------------------
//----- Jobsons ----------------------------------------------------------------
//------------------------------------------------------------------------------

//-------1---------2---------3---------4---------5---------6---------7---------8
//       01234567890123456789012345678901234567890123456789012345678901234567890
//-------+---------+---------+---------+---------+---------+---------+---------+

// copyright:   2018 WIM - USGS

//    authors:  Jeremy K. Newson USGS Web Informatics and Mapping
//              
//  
//   purpose:   The Jobsons object contains methods and properties related to 
//              prediction of traveltime and longitudianl dispersion in rivers
//              and streams.
//              See Jobson, H.E., 1996, Prediction of Traveltime and Longitudinal
//                      Dispersion in Rivers and Streams: U.S. Geological Survey Water-Resources
//                      Investigations Report 96-4013,
//
//discussion:   
//+-+-+-+-+-+-+-+-+-+-+-+-+-+-+ Slope,annual Discharge, Drainage Area+-+-+-+-+-+-+-+-+-+-+-+-\\
//          in meters per second
//          V_p = 0.094+0.0143*(D'_a)^0.919*(Q'_a)^(-0.465)*S^(0.159)*Q/D_a  Eq 12
//
//          S, slope meter per meter

//+-+-+-+-+-+-+-+-+-+-+-+-+-+-+ annual Discharge,Drainage Area +-+-+-+-+-+-+-+-+-+-+-+-+-+-+\\
//          in meters per second
//          V_p = 0.020+0.051*(D'_a)^0.821*(Q'_a)^(-0.465)*Q/D_a              Eq 14

//+-+-+-+-+-+-+-+-+-+-+-+-+-+-+ Drainage Area +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+\\
//          in meters per second
//          V_p = 0.152+0.8.1(D''_a)^0.595*Q/D_a                              Eq 16
//
//          D''_a, dimensionless drainage area D''_a=(D_a^1.25*√g)/Q  Eq 10 (alternate)
//
//---------------------------------------------------------------------------------\\
//          D'_a, dimensionless drainage area D'_a=(D_a^1.25*√g)/Q_a          Eq 10
//          D_a, Drainage area
//          g, acceleration of gravity
//
//          Q'_a, the dimensionless relative discharge Q/Q_a
//          Q, discharge at time of measurement
//          Q_a, mean annual discharge
//
//
// the time of arrival of the leading edge of the pollutant indicates when tha local problem will first exist
//          T_l=0.890*T_p (Equ 18)        
//
//          T_p, elapsed time to the peak concentration;
//
//
//Duration from leading edge until tracer concentration has reduced to within 10 percent of the
//peak concentration;
//
//          T_10d=(2*10^6)/(C_up)  Eq 19
//
//          C_up, magnitude of the unit-peak concentration.
//          C_up=857*〖T_p〗^(〖-0.760(Q/(Q_a))〗^(-0.079) ) Eq 7
//
//          T_p, elapsed time to the peak concentration;
//          Q, discharge at time of measurement
//          Q_a, mean annual discharge
//
using System;
using System.Collections.Generic;
using System.Text;
using WIM.Resources.TimeSeries;
using WIM.Utilities;
using WIM.Resources;
using System.Linq;
using TravelTimeAgent.Resources;

namespace TravelTimeAgent
{
    public class Jobson
    {
        #region Properties and Fields
        private List<Parameter> _availableParameters = new List<Parameter>();

        public DateTime? InitialTimeStamp { get; private set; }
        public bool ShouldSerializeInitialTimeStamp()
        { return InitialTimeStamp.HasValue; }
        
        public SortedDictionary<Double, Reach> Reaches { get; set; }
        public bool IsValid {
            get {
                //Checks if reaches are valid
                try
                {
                    //must have at least 2 reaches to continue
                    if (this.Reaches.Count() < 1) return false;

                    var parameters = this.Reaches?.SelectMany(r => r.Value.Parameters).ToList();
                    if (parameters == null || parameters.Count < this._availableParameters.Where(par => par.Required == true).Count() * Reaches.Count())
                    {
                        return false;
                    }

                    foreach (var p in parameters)
                    {
                        //TODO: add finer grain validation based on param code
                        if (p.Required && (!p.Value.HasValue || p.Value.Value < 0))
                            return false;
                    }
                    
                    //finally
                    return true;
                }
                catch (Exception ex)
                {
                    return false;
                }
            }
        }

        public bool ShouldSerializeIsValid()
        { return false; }
        #endregion
        #region Constructors
        public Jobson()
        {
            init();            
        }
        #endregion
        #region Methods
        public bool Execute(Double? InitialMass_M_i_kg=null, DateTime? starttime = null)
        {
            try
            {            
                if (!IsValid) throw new Exception("Jobson parameters are invalid");
                if (starttime == null) InitialTimeStamp = DateTime.Today;
                            
                for (int i = 1; i < this.Reaches.Count; i++)
                {
                    var startingReach = this.Reaches.ElementAt(i - 1).Value;
                    var endingreach = this.Reaches.ElementAt(i).Value;

                    if (!loadEstimate(startingReach, endingreach, InitialMass_M_i_kg)) throw new Exception("Estimate failed to compute.");                    
                }//next i
                return true;
            }
            catch (Exception ex)
            {
                sm("Failed to execute jobsons Agent " + ex.Message, WIM.Resources.MessageType.error);
                return false;
            }
        }
        #endregion
        #region "Helper Methods"
        private void init() {
            //initial request, hands back everything you need to run jobsons equations           
            Enum.GetValues(typeof(parameterEnum)).Cast<parameterEnum>().Where(p=>
                (int)p < 10)
                .ToList().ForEach(p=>
                _availableParameters.Add(getParameters(p) as Parameter));

            //preload reach
            this.Reaches = new SortedDictionary<double, Reach>();
            this.Reaches.Add(0, new Reach()
            {
                 ID=0,
                 Description = "Initial injection reach",
                 Name = "Initial",
                 Parameters = this._availableParameters.Cast<Parameter>().ToList()
            });
        }
        private bool loadEstimate(Reach start, Reach end, double? initialMassConcentration = null)
        {
            try
            {
                //average start and end parameters
                List<Parameter> paramlist = new List<Parameter>(start.Parameters);
                paramlist.AddRange(end.Parameters);
                var aveParams = paramlist.GroupBy(k => k.Code).ToDictionary(k => k.FirstOrDefault().Code,
                    s => s.Aggregate((a, b) => aggregateParameters(a, b)).Value);
                //add to start concentration
                var m_i = getParameters(parameterEnum.e_M_i);
                m_i.Value = initialMassConcentration.Value * Constants.CF_kg2mg;
                start.Parameters.Add(m_i);
                aveParams.Add(m_i.Code,m_i.Value);

                // compute and solve travel time equations
                end.Result = getTraveltimeResult(aveParams,end.Parameters.FirstOrDefault(p=>p.Code =="Q").Value);
      
                return true;
            }
            catch (Exception ex)
            {
                sm("Failed to estimate for reaches " + start.Name + end.Name, MessageType.error);
                return false;
            }
        }
        private void computeConcentrations(Reach end, Dictionary<string,double?> parameters)
        {
            try
            {          
                var Cp = evaluate(EquationEnum.e_peakconcentration_C_p, parameters);
                var Cpmax = evaluate(EquationEnum.e_peakconcentration_C_pmax, parameters);
            }
            catch (Exception ex)
            {
                sm("Concentration failed to compute " + ex.Message);
            }
        }
        private TravelTimeResult getTraveltimeResult(Dictionary<string, double?> parms,double? Qend)
        {
            try
            {
                var dimda = evaluate(EquationEnum.e_dimdrainagearea_D_a_prime, parms);
                var dimqa = evaluate(EquationEnum.e_dimrelativedischarge_Q_a_prime, parms);
                var vp = evaluate(EquationEnum.e_velocity_V, parms);
                var tl = evaluate(EquationEnum.e_leadingedge, parms);
                var tlmax = evaluate(EquationEnum.e_leadingedgemax, parms);
                var pc = evaluate(EquationEnum.e_timepeakconcentration_T_p, parms);
                var pcmax = evaluate(EquationEnum.e_timepeakconcentration_T_pmax, parms);
                var Td10 = evaluate(EquationEnum.e_trailingedge, parms) * Constants.CF_sec2hrs;
                var Td10max = evaluate(EquationEnum.e_trailingedgemax, parms) * Constants.CF_sec2hrs;

                //change Q to end reach
                parms["Q"] = Qend;
                var Cp = evaluate(EquationEnum.e_peakconcentration_C_p, parms);
                var Cpmax = evaluate(EquationEnum.e_peakconcentration_C_pmax, parms);

                var result = new TravelTimeResult();
                result.LeadingEdge = new Dictionary<string, ConcentrationTime>() {
                    {getProbabilityName(probabilityTypeEnum.e_HighestProbable),new ConcentrationTime(){ Time =tl, Concentration=0  } },
                    {getProbabilityName(probabilityTypeEnum.e_MaximumProbable),new ConcentrationTime(){ Time =tlmax, Concentration=0   } },
                };
                result.PeakConcentration = new Dictionary<string, ConcentrationTime>() {
                    {getProbabilityName(probabilityTypeEnum.e_HighestProbable),new ConcentrationTime(){ Time =pc, Concentration=Cp   } },
                    {getProbabilityName(probabilityTypeEnum.e_MaximumProbable),new ConcentrationTime(){ Time =pcmax, Concentration=Cpmax   } },
                };
                result.TrailingEdge = new Dictionary<string, ConcentrationTime>() {
                    {getProbabilityName(probabilityTypeEnum.e_HighestProbable),new ConcentrationTime(){ Time =tl+Td10, Concentration=0.1*Cp   } },
                    {getProbabilityName(probabilityTypeEnum.e_MaximumProbable),new ConcentrationTime(){ Time =tlmax+Td10max , Concentration=0.1*Cpmax  } },
                };

                return result;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        private Double evaluate(EquationEnum equationType, Dictionary<string,double?> parameters)
        {
            try
            {
                //get required parameters
                var reqParams = getRequiredVariables(equationType).Select(e=>e.Code).Distinct().ToList();
                if (reqParams.Except(parameters.Select(s => s.Key)).Any()) throw new KeyNotFoundException("Missing required parameter(s) "+equationType);

                string expression = getExpression(equationType, parameters.Select(x=>x.Key));
                ExpressionOps eOps = new ExpressionOps(expression, parameters);
                if (!eOps.IsValid) throw new Exception("Expression failed to evaluate "+equationType);
                return eOps.Value;
            }
            catch (Exception)
            {

                throw;
            }
        }
        private string getExpression(EquationEnum expression,IEnumerable<string> availableparams)
        {
            List<string> args = getExpressionArguments(expression,availableparams);
            string equation = string.Empty;
            switch (expression)
            {
                case EquationEnum.e_velocity_V:
                case EquationEnum.e_velocity_Vmax:
                    //equation = "{1}+{2}*({0})^{3}*({4}){5}*Q/D_a";
                    equation = "{1}+{2}*({0}^{3})*(({4})^{6})*({5}^{7})*(Q/D_a)";
                    //equation = "({5}^{7})";
                    break;
                //case EquationEnum.e_velocity_Vmax:
                //    equation = "{1}+{2}*^"

                
                case EquationEnum.e_leadingedge:
                case EquationEnum.e_leadingedgemax:
                    equation = "0.890 * ({0})";                                          //Eq 18
                    break;
                case EquationEnum.e_trailingedge:
                case EquationEnum.e_trailingedgemax:
                    equation = "(2*10^6)/({0})";                                         //Eq 19
                    break;
                case EquationEnum.e_dimdrainagearea_D_a_prime:
                    equation = "(D_a^1.25*sqrt({0}))/Q_a";
                    break;
                case EquationEnum.e_unitpeakconcentration_C_up:
                case EquationEnum.e_unitpeakconcentration_C_upmax:
                   equation = "857*({1})^(-0.760*({0})^(-0.079) )";                      //Eq 7
                    break;
                case EquationEnum.e_timepeakconcentration_T_p:
                case EquationEnum.e_timepeakconcentration_T_pmax:
                    equation = "L/({0})*{1}";
                    break;
                case EquationEnum.e_dimrelativedischarge_Q_a_prime:
                    equation = "Q/Q_a";
                    break;
                case EquationEnum.e_peakconcentration_C_p:
                case EquationEnum.e_peakconcentration_C_pmax:
                    equation = "(({0})*R_r*M_i)/(1*10^6*(Q*{1}))";                               //eq 4 - rearranged to solve for C
                    break;
                default:
                    throw new NotImplementedException(expression.ToString());                   
            }//end switch

            return(args != null)? string.Format(equation, args.ToArray()):equation;
        }
        private List<string> getExpressionArguments(EquationEnum expression, IEnumerable<string> availableparams)
        {
            List<string> args = null;
            EquationEnum e_type;
            string varg = String.Empty;
            switch (expression)
            {
                case EquationEnum.e_velocity_V:
                case EquationEnum.e_velocity_Vmax:
                    
                    if (new List<string>() { "S", "Q_a" }.All(p => (availableparams).Contains(p))) //ask Jeremy
                    {
                        //+-+-+-+-+ Slope,Discharge,Drainage Area V +-+-+-+-+\\
                        args = getVelocityConstants(velocityPeakEnum.v_slope, expression == EquationEnum.e_velocity_Vmax);
                        args.Insert(0, availableparams.Contains("D_a_prime")? "D_a_prime": getExpression(EquationEnum.e_dimdrainagearea_D_a_prime, availableparams));
                        //args.Insert(4, string.Format("{0}^{{5}}", availableparams.Contains("Q_a_prime") ? "Q_a_prime" : getExpression(EquationEnum.e_dimrelativedischarge_Q_a_prime, availableparams)));
                        //args.Insert(5, "*S^({6})");
                        args.Insert(4, string.Format("{0}", availableparams.Contains("Q_a_prime") ? "Q_a_prime" : getExpression(EquationEnum.e_dimrelativedischarge_Q_a_prime, availableparams))); //NESTING is not working ?
                        args.Insert(5, "S"); //changed

                        //equation = "{2}+{3}*({0})^{4}*({1})^{5}*S^({6})*Q/D_a";
                        //           "{1}+{2}*({0})^{3}*({4})*({5})*Q/D_a";
                    }
                    else if (new List<string>() { "Q_a" }.All(p => availableparams.Contains(p)))
                    {
                        //+-+-+-+-+ Discharge,Drainage Area V +-+-+-+-+\\
                        args = getVelocityConstants(velocityPeakEnum.v_no_slope, expression == EquationEnum.e_velocity_Vmax);
                        args.Insert(0, availableparams.Contains("D_a_prime") ? "D_a_prime" : getExpression(EquationEnum.e_dimdrainagearea_D_a_prime, availableparams));
                        args.Insert(4, string.Format("({0})^{1}", availableparams.Contains("Q_a_prime") ? "Q_a_prime" : getExpression(EquationEnum.e_dimrelativedischarge_Q_a_prime, availableparams),args[4]));
                        args.Insert(5, "");//remove slope
                    }
                    else
                    {
                        //+-+-+-+-+ Drainage Area only V  +-+-+-+-+\\                        
                        args = getVelocityConstants(velocityPeakEnum.v_drainagearea, expression == EquationEnum.e_velocity_Vmax);
                        //D"a is defined by equation 10 except that the local discharge (Q) is used in place of the mean annual discharge(Qa).
                        args.Insert(0, availableparams.Contains("Q_a_dprime") ? "Q_a_dprime" : getExpression(EquationEnum.e_dimdrainagearea_D_a_prime, availableparams).Replace("Q_a", "Q"));
                        args.Insert(4, "");//remove Q'a
                        args.Insert(5, "");//remove slope
                    }
                    break;

                case EquationEnum.e_leadingedge:
                case EquationEnum.e_leadingedgemax:
                    e_type = (expression == EquationEnum.e_leadingedge) ? EquationEnum.e_timepeakconcentration_T_p : EquationEnum.e_timepeakconcentration_T_pmax;
                    varg = (expression == EquationEnum.e_leadingedge) ? "T_p" : "T_pmax";
                    args = new List<string>() { availableparams.Contains(varg) ? varg : getExpression(e_type, availableparams) }; 
                    break;

                case EquationEnum.e_trailingedge:
                case EquationEnum.e_trailingedgemax:
                    e_type = (expression == EquationEnum.e_trailingedge) ? EquationEnum.e_unitpeakconcentration_C_up : EquationEnum.e_unitpeakconcentration_C_upmax;
                    varg = (expression == EquationEnum.e_leadingedge) ? "C_up" : "C_upmax";
                    args = new List<string>() { availableparams.Contains(varg) ? varg : getExpression(e_type, availableparams) };
                    break;

                case EquationEnum.e_dimdrainagearea_D_a_prime:
                    args = new List<string>() { Constants.gravityacc_g.ToString() };
                    break;

                case EquationEnum.e_unitpeakconcentration_C_up:
                case EquationEnum.e_unitpeakconcentration_C_upmax:
                    e_type = (expression == EquationEnum.e_unitpeakconcentration_C_up) ? EquationEnum.e_timepeakconcentration_T_p : EquationEnum.e_timepeakconcentration_T_pmax;
                    varg = (expression == EquationEnum.e_unitpeakconcentration_C_up) ? "T_p" : "T_pmax";
                    args = new List<string>() { availableparams.Contains(varg)?varg:getExpression(EquationEnum.e_dimrelativedischarge_Q_a_prime, availableparams), availableparams.Contains(varg) ? varg : getExpression(e_type, availableparams) };
                    break;

                case EquationEnum.e_timepeakconcentration_T_p:
                case EquationEnum.e_timepeakconcentration_T_pmax:
                    e_type = (expression == EquationEnum.e_timepeakconcentration_T_p) ? EquationEnum.e_velocity_V : EquationEnum.e_velocity_Vmax;
                    varg = (expression == EquationEnum.e_timepeakconcentration_T_p) ? "V" : "Vmax";
                    args = new List<string>() { availableparams.Contains(varg) ? varg : getExpression(e_type, availableparams), Constants.CF_sec2hrs.ToString() };
                    break;

                case EquationEnum.e_dimrelativedischarge_Q_a_prime:
                    break;

                case EquationEnum.e_peakconcentration_C_p:
                case EquationEnum.e_peakconcentration_C_pmax:
                    e_type = (expression == EquationEnum.e_peakconcentration_C_p) ? EquationEnum.e_unitpeakconcentration_C_up : EquationEnum.e_unitpeakconcentration_C_upmax;
                    varg = (expression == EquationEnum.e_peakconcentration_C_p) ? "C_up" : "C_upmax";
                    args = new List<string>() { availableparams.Contains(varg) ? varg : getExpression(e_type, availableparams), Constants.CF_cms2lps.ToString() };
                   
                    break;

                default:
                    throw new NotImplementedException(expression.ToString());
            }//end switch

            return args;
        }
        private List<Parameter> getRequiredVariables(EquationEnum expression)
        {
            List<Parameter> variables = new List<Parameter>();
            switch (expression)
            {
                case EquationEnum.e_velocity_V:
                case EquationEnum.e_velocity_Vmax:
                    variables.Add(getParameters(parameterEnum.e_D_a));
                    variables.AddRange(getRequiredVariables(EquationEnum.e_dimdrainagearea_D_a_prime));
                    variables.Add(getParameters(parameterEnum.e_Q));
                    //variables.Add(getParameters(parameterEnum.e_Q_a));
                    variables.AddRange(getRequiredVariables(EquationEnum.e_dimrelativedischarge_Q_a_prime));
                    break;

                case EquationEnum.e_trailingedge:
                case EquationEnum.e_trailingedgemax:
                    variables.AddRange(getRequiredVariables(EquationEnum.e_unitpeakconcentration_C_up));
                    break;
                case EquationEnum.e_leadingedge:
                case EquationEnum.e_leadingedgemax:
                    variables.AddRange(getRequiredVariables(EquationEnum.e_timepeakconcentration_T_p));
                    break;
                case EquationEnum.e_dimdrainagearea_D_a_prime:
                    variables.Add(getParameters(parameterEnum.e_D_a));
                    variables.Add(getParameters(parameterEnum.e_Q_a));
                    break;
                case EquationEnum.e_unitpeakconcentration_C_up:
                case EquationEnum.e_unitpeakconcentration_C_upmax:
                    variables.AddRange(getRequiredVariables(EquationEnum.e_timepeakconcentration_T_p));
                    variables.Add(getParameters(parameterEnum.e_Q));
                    variables.Add(getParameters(parameterEnum.e_Q_a));
                    break;
                case EquationEnum.e_timepeakconcentration_T_p:
                case EquationEnum.e_timepeakconcentration_T_pmax:
                    variables.Add(getParameters(parameterEnum.e_distance));
                    variables.AddRange(getRequiredVariables(EquationEnum.e_velocity_V));
                    break;
                case EquationEnum.e_dimrelativedischarge_Q_a_prime:
                    variables.Add(getParameters(parameterEnum.e_Q));
                    variables.Add(getParameters(parameterEnum.e_Q_a));
                    break;
                case EquationEnum.e_peakconcentration_C_p:
                case EquationEnum.e_peakconcentration_C_pmax:
                    variables.Add(getParameters(parameterEnum.e_M_i));
                    variables.Add(getParameters(parameterEnum.e_R_r));
                    variables.Add(getParameters(parameterEnum.e_Q));
                    variables.AddRange(getRequiredVariables(EquationEnum.e_unitpeakconcentration_C_up));
                    break;

                default:
                    throw new NotImplementedException(expression.ToString());
            }

            return variables.ToList();
        }
        private string getProbabilityName(probabilityTypeEnum ptype)
        {
            switch (ptype)
            {
                case probabilityTypeEnum.e_HighestProbable:
                    return "HighestProbable";
                case probabilityTypeEnum.e_MaximumProbable:
                    return "MaximumProbable";
                default:
                    return "other";
            }
        }
        private List<string> getVelocityConstants(velocityPeakEnum velocityType,bool isMax = false)
        {
            //0.094+0.0143*(D'_a)^0.919*(Q'_a)^(-0.465)*S^(0.159)*Q/D_a       Eq 12
            //0.250+0.0200*(D'_a)^0.919*(Q'_a)^(-0.465)*S^(0.159)*Q/D_a //max    Eq 13
            List<double> velocityConstants = new List<double>();
            switch (velocityType)
            {
                case velocityPeakEnum.v_slope:
                    //0.094+0.0143*(D'_a)^0.919*(Q'_a)^(-0.465)*S^(0.159)*Q/D_a       Eq 12
                    //0.25+0.02*(D'_a)^0.919*(Q'_a)^(-0.465)*S^(0.159)*Q/D_a //max    Eq 13
                    if (!isMax)
                    {
                        velocityConstants.Add(0.094);//constA
                        velocityConstants.Add(0.0143);//constB
                    }
                    else
                    {
                        velocityConstants.Add(0.25);//constA
                        velocityConstants.Add(0.02);//constB
                    }//end if
                    velocityConstants.Add(0.919);//constC
                    velocityConstants.Add(-0.465);//constD
                    velocityConstants.Add(0.159); //constE
                    break;
                case velocityPeakEnum.v_no_slope:
                    //0.020+0.051*(D'_a)^0.821*(Q'_a)^(-0.465)*Q/D_a                  Eq 14                                                                                                                    
                    //0.2+0.093*(D'_a)^0.821*(Q'_a)^(-0.465)*Q/D_a //max              Eq 15 
                    if (!isMax)
                    {
                        velocityConstants.Add(0.020);//constA
                        velocityConstants.Add(0.051);//constB
                    }
                    else
                    {
                        velocityConstants.Add(0.20);//constA
                        velocityConstants.Add(0.093);//constB
                    }//end if
                    velocityConstants.Add(0.821);//constC
                    velocityConstants.Add(-0.465);//constD
                    break;

                case velocityPeakEnum.v_drainagearea:
                    break;
                default:
                    break;
            }//end switch
            return velocityConstants.Select(i => i.ToString()).ToList();
        }
        private Parameter aggregateParameters(Parameter p1, Parameter p2)
        {
            switch (p1.Code)
            {
                //case "L":
                //    return new Parameter() { Code = p1.Code, Value = p2.Value - p1.Value };
                default:
                    //return new Parameter() { Code = p1.Code, Value = (p1.Value + p2.Value) / 2 };
                    return new Parameter() { Code = p1.Code, Value = p2.Value };
            }
        }
        private Parameter getParameters(parameterEnum p)
        { 
            switch (p)
            {
                case parameterEnum.e_Q_a:
                
                    return new Parameter()
                    {
                        Code = "Q_a",
                        Name = "mean annual discharge",
                        Description = "mean annual discharge",
                        Required = false,
                        Unit = getUnit(p)
                    };
                case parameterEnum.e_Q:
                    return new Parameter()
                    {
                        Code = "Q",
                        Name = "discharge at time of measurement",
                        Description = "discharge at time of measurement",
                        Unit = getUnit(p)
                    };
                case parameterEnum.e_S:
                    return new Parameter()
                    {
                        Code = "S",
                        Name = "Slope",
                        Description = "Slope",
                        Unit = getUnit(p),
                        Required = false
                    };
                case parameterEnum.e_D_a:
                    return new Parameter()
                    {
                        Code = "D_a",
                        Name = "Drainage area",
                        Description = "drainage area",
                        Unit = getUnit(p)
                    };
                case parameterEnum.e_distance:
                    return new Parameter()
                    {
                        Code = "L",
                        Name = "Distance",
                        Description = "Distance",
                        Unit = getUnit(p)
                    };
                case parameterEnum.e_M_i:
                    return new Parameter()
                    {
                        Code = "M_i",
                        Name = "Mass of pollutant spilled",
                        Description = "actual mass of pollutant spilled",
                        Unit = getUnit(p)
                    };
                case parameterEnum.e_R_r:
                    return new Parameter()
                    {
                        Code = "R_r",
                        Name = "Recovery Ratio",
                        Description = "Recovery ratio (Mass recovered/ Mass initial)",
                        Unit = getUnit(p),
                        Value = 1.0
                    };
                default:
                    throw new NotImplementedException("Parameter not implemented " + p);
            }// end switch            
        }
        private Units getUnit(parameterEnum p)
        {
            switch (p)
            {
                case parameterEnum.e_Q_a:
                case parameterEnum.e_Q:
                    return new Units { Unit = "cubic meters per second", Abbr = "cms" };
                case parameterEnum.e_S:
                    return new Units { Unit = "meters per meters", Abbr = "m/m" };
                case parameterEnum.e_D_a:
                    return new Units { Unit = "square meters", Abbr = "m^2" };
                case parameterEnum.e_distance:
                    return new Units { Unit = "meters", Abbr = "m" };
                case parameterEnum.e_M_i:
                    return new Units { Unit = "milligrams", Abbr = "mg" };
                case parameterEnum.e_R_r:
                    return new Units { Unit = "Diminsionless", Abbr = "dim" };
                default:
                    throw new NotImplementedException("Parameter not implemented "+p);
            }//end switch
        }
        private void sm(string msg, MessageType type = MessageType.info) {
            
        }
        #endregion
        #region "Enumerated Constants"
        public enum EquationEnum
        {
            e_velocity_V,
            e_velocity_Vmax,
            e_trailingedge,
            e_trailingedgemax,
            e_leadingedge,
            e_leadingedgemax,
            e_dimdrainagearea_D_a_prime,
            e_unitpeakconcentration_C_up,
            e_unitpeakconcentration_C_upmax,
            e_peakconcentration_C_p,
            e_peakconcentration_C_pmax,
            e_timepeakconcentration_T_p,
            e_timepeakconcentration_T_pmax,
            e_dimrelativedischarge_Q_a_prime 
        }
        public enum parameterEnum
        {
            e_Q_a = 0 ,
            e_Q = 1,
            e_S = 2,
            e_D_a = 3,
            e_distance = 4,
            e_R_r = 5,
            e_M_i = 10,
            
        }
        public enum velocityPeakEnum
        {
            v_slope,
            v_no_slope,
            v_drainagearea
        }
        public enum probabilityTypeEnum
        {
            e_HighestProbable,
            e_MaximumProbable
        }
        #endregion
    }
}