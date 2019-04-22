using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using TravelTimeAgent;
using TravelTimeAgent.Resources;
using WIM.Resources;
using Newtonsoft.Json;

namespace TravelTimeDB.Test
{
    [TestClass]
    public class TravelTimeAgentTest
    {


        [TestMethod]
        public void JobsonsConfigureTest()
        {
            try
            {
                Jobson jobsonstest = new Jobson();
                var valid = jobsonstest.IsValid;
                Assert.IsNotNull(jobsonstest);
                Assert.IsFalse(jobsonstest.IsValid);
            }
            catch (Exception ex)
            {
                Assert.IsTrue(false, ex.Message);
            }
        }
        [TestMethod]
        public void JobsonsExample1LimitedDataTest()
        {
        //Assume that a truck runs off the road and instantaneously spills 6,000 kg of a corrosive chemical into
        //an ungaged stream.Estimate the most probable and the expected worst case effects of the spill on the water
        //intake for a town that is located 15 km downstream.The worst case should occur for the shortest probable
        //traveltime.
        //No data exist for the stream receiving the spill, but topographic maps show that the drainage area is
        //350 km at the spill site and 430 km at the intake for the town. A review of available data also indicates
        //that a gaging station exists for a nearby stream with a drainage area of 452 km and a mean - annual flow of
        //5.22 m3 / s.At the time of the spill the flow at the gaging station was 3.88 m3 / s.The hydrology and weather
        //are assumed to be fairly uniform within the area so it will be assumed that the stream carrying the spill is
        //flowing at about 3.88(390 / 452) = 3.35 m3 / s, assuming the average drainage area for the reach is
        // (350 + 430)72 = 390 km2.Likewise, the mean - annual flow of the ungaged stream is estimated to be about
        // 5.22(390 / 452) = 4.50 m3 / s.
             try
            {
                List<Parameter> toremove = new List<Parameter>();
                Jobson jobsonstest = new Jobson();
                jobsonstest.Reaches[0].Parameters.ForEach(p =>
                {
                    switch (p.Code)
                    {
                        case "Q_a":                            
                            p.Value = 5.22 * (350.0 / 452.0);//m2
                            break;
                        case "Q":
                            p.Value = 3.88 * (350.0 / 452.0);//m^3/s
                            break;
                        case "D_a":
                            p.Value = 350.0 * Constants.CF_sqrkm2sqrm;//m2
                            break;
                        case "L":
                            p.Value = 0.0;
                            break;
                        case "R_r":
                            //keep default
                            break;
                        default:
                            toremove.Add(p);
                            break;
                    }//end switch
                });
                toremove.ForEach(p => jobsonstest.Reaches[0].Parameters.Remove(p));
                jobsonstest.Reaches.Add(15, new Reach()
                {
                    Name = "intake",
                    Description = "Intake 15 km downstream from spill",
                    ID = 15,
                    Parameters = new List<Parameter>()
                     {
                         new Parameter(){ Code = "Q_a", Unit = new Units(){Unit = "cubic meters per second", Abbr = "cms" },Value=5.22 * (430.0 / 452.0) },
                         new Parameter(){ Code = "Q", Unit = new Units(){Unit = "cubic meters per second", Abbr = "cms"  },Value=3.88 * (430.0 / 452.0) },
                         new Parameter(){ Code = "D_a", Unit = new Units(){Unit = "square meters", Abbr = "m^2"  },Value=430.0*Constants.CF_sqrkm2sqrm },
                         new Parameter(){ Code = "L", Unit = new Units(){Unit = "meters", Abbr = "m^2"  },Value=15.0*Constants.CF_km2m },
                         new Parameter(){ Code = "R_r", Unit = new Units(){Unit = "Diminsionless", Abbr = "dim" },Value=1.0 },
                    }
                });
                //instantaneously spills 6,000 kg of a corrosive chemical
                jobsonstest.Execute(6000);

                Assert.IsNotNull(jobsonstest.Reaches);
                //need better assert

            }
            catch (Exception ex)
            {
                Assert.IsTrue(false, ex.Message);
            }
        }
        [TestMethod]
        public void JobsonsExample3RhineRiver()
        {
            //With a catchment area of 180,000 ckm, the Rhine River is a very important Eropean river. Because of the high 
            //population density and heavy use, there is always the potential that the river will be accidently polluted. 
            //The IRC has been set up to help reduce the danger of accidents and to help respond to them if they occur. The 
            //Commission developed, calibrated, and verified the Alarm model to be used in responding to accidental spills. 
            //As part of the calibration process, the response to a slug injection near river km 59 was measured at Eglisau 
            //(km 78.7) and Birsfelden (km 163.8). In this example the measured response curves will first be predicted based 
            //on the river discharge and drainage area. To illustrate the value of time-of-travel data, improved predictions 
            //of the unit-peak concentration, as well as the time of the leading edge, and time of passage of the cloud will 
            //then be made using the traveltime measured for the peak concentration.
            //The mean annual flow of the Rhine River is 0.0152 cm/s/sqkm. Because the drainage area is approximately 16,000 
            //and 48,000 sqkm at river km 59 and 163.8, respectively, the mean annual flow can be estimated as 240 cms at the 
            //injection point and 730 cms at Birsfelden.
            //The response function characteristics at Eglisau and Birsfelden are first estimated without the aid of traveltime 
            //information. Assuming the drainage area at Eglisau is the same as at the injection site...
            //During the test, the river flow was 490 cms at the injection point...
            //The flow at Birsfelden during the test was 1,068 cms...
            try
            {
                List<Parameter> toremove = new List<Parameter>();
                Jobson jobsonstest = new Jobson();
                jobsonstest.Reaches[0].Parameters.ForEach(p =>
                {
                    switch (p.Code)
                    {
                        /*case "Q_a":
                            p.Value = 240;//m2
                            break;*/
                        case "Q":
                            p.Value = 490;//m^3/s
                            break;
                        case "D_a":
                            p.Value = 16000 * Constants.CF_sqrkm2sqrm;//m2
                            break;
                        case "L":
                            p.Value = 0.0;
                            break;
                        case "R_r":
                            //keep default
                            break;
                        default:
                            toremove.Add(p);
                            break;
                    }//end switch
                });
                toremove.ForEach(p => jobsonstest.Reaches[0].Parameters.Remove(p));
                jobsonstest.Reaches.Add(19, new Reach()
                {
                    Name = "Eglisau (km 78.7)",
                    Description = "Town 19.7 km downstream from injection site (km 59)",
                    ID = 19,
                    Parameters = new List<Parameter>()
                     {
                         /*new Parameter(){ Code = "Q_a", Unit = new Units(){Unit = "cubic meters per second", Abbr = "cms" },Value=240 },//((730-240)/(163.8-59))*(78.7-59.0)+240 },*/
                         new Parameter(){ Code = "Q", Unit = new Units(){Unit = "cubic meters per second", Abbr = "cms"  },Value=490 },
                         new Parameter(){ Code = "D_a", Unit = new Units(){Unit = "square meters", Abbr = "m^2"  },Value=16000*Constants.CF_sqrkm2sqrm },
                         new Parameter(){ Code = "L", Unit = new Units(){Unit = "meters", Abbr = "m^2"  },Value=19.7*Constants.CF_km2m },
                         new Parameter(){ Code = "R_r", Unit = new Units(){Unit = "Dimensionless", Abbr = "dim" },Value=1.0 },
                    }
                });
                jobsonstest.Reaches.Add(104.8, new Reach()
                {
                    Name = "Birsfelden (163.8)",
                    Description = "Town 104.8 km downstream from injection site (km 59)",
                    ID = 104,
                    Parameters = new List<Parameter>()
                     {
                         /*new Parameter(){ Code = "Q_a", Unit = new Units(){Unit = "cubic meters per second", Abbr = "cms" },Value=730 },*/
                         new Parameter(){ Code = "Q", Unit = new Units(){Unit = "cubic meters per second", Abbr = "cms"  },Value=1068 },
                         new Parameter(){ Code = "D_a", Unit = new Units(){Unit = "square meters", Abbr = "m^2"  },Value=48000*Constants.CF_sqrkm2sqrm },
                         new Parameter(){ Code = "L", Unit = new Units(){Unit = "meters", Abbr = "m^2"  },Value=104.8*Constants.CF_km2m },
                         new Parameter(){ Code = "R_r", Unit = new Units(){Unit = "Diminsionless", Abbr = "dim" },Value=1.0 },
                    }
                });
                //instantaneously spills 6,000 kg of a corrosive chemical
                jobsonstest.Execute(6000);

                Assert.IsNotNull(jobsonstest.Reaches);
                //need better assert

            }
            catch (Exception ex)
            {
                Assert.IsTrue(false, ex.Message);
            }
        }
        [TestMethod]
        public void JobsonsSerializeTest()
        {
            try
            {
                Jobson jobsonstest = new Jobson();
                var valid = jobsonstest.IsValid;
                var json = JsonConvert.SerializeObject(jobsonstest);
                var obj = JsonConvert.DeserializeObject<Jobson>(json);
                Assert.IsNotNull(jobsonstest);
                Assert.IsFalse(jobsonstest.IsValid);
            }
            catch (Exception ex)
            {
                Assert.IsTrue(false, ex.Message);
            }
        }
        [TestMethod]
        public void JobsonExampleRhineModified()
        {
            //With a catchment area of 180,000 ckm, the Rhine River is a very important Eropean river. Because of the high 
            //population density and heavy use, there is always the potential that the river will be accidently polluted. 
            //The IRC has been set up to help reduce the danger of accidents and to help respond to them if they occur. The 
            //Commission developed, calibrated, and verified the Alarm model to be used in responding to accidental spills. 
            //As part of the calibration process, the response to a slug injection near river km 59 was measured at Eglisau 
            //(km 78.7) and Birsfelden (km 163.8). In this example the measured response curves will first be predicted based 
            //on the river discharge and drainage area. To illustrate the value of time-of-travel data, improved predictions 
            //of the unit-peak concentration, as well as the time of the leading edge, and time of passage of the cloud will 
            //then be made using the traveltime measured for the peak concentration.
            //The mean annual flow of the Rhine River is 0.0152 cm/s/sqkm. Because the drainage area is approximately 16,000 
            //and 48,000 sqkm at river km 59 and 163.8, respectively, the mean annual flow can be estimated as 240 cms at the 
            //injection point and 730 cms at Birsfelden.
            //The response function characteristics at Eglisau and Birsfelden are first estimated without the aid of traveltime 
            //information. Assuming the drainage area at Eglisau is the same as at the injection site...
            //During the test, the river flow was 490 cms at the injection point...
            //The flow at Birsfelden during the test was 1,068 cms...
            try
            {
                List<Parameter> toremove = new List<Parameter>();
                Jobson jobsonstest = new Jobson();
                jobsonstest.Reaches[0].Parameters.ForEach(p =>
                {
                    switch (p.Code)
                    {
                        case "Q_a":
                            p.Value = 240;//m2
                            break;
                        case "Q":
                            p.Value = 490;//m^3/s
                            break;
                        case "S":
                            p.Value = 0;
                            break;
                        case "D_a":
                            p.Value = 16000 * Constants.CF_sqrkm2sqrm;//m2
                            break;
                        case "L":
                            p.Value = 0.0;
                            break;
                        case "R_r":
                            //keep default
                            break;
                        default:
                            toremove.Add(p);
                            break;
                    }//end switch
                });
                toremove.ForEach(p => jobsonstest.Reaches[0].Parameters.Remove(p));
                jobsonstest.Reaches.Add(19, new Reach()
                {
                    Name = "Eglisau (km 78.7)",
                    Description = "Town 19.7 km downstream from injection site (km 59)",
                    ID = 19,
                    Parameters = new List<Parameter>()
                    {
                         new Parameter(){ Code = "Q_a", Unit = new Units(){Unit = "cubic meters per second", Abbr = "cms" },Value=240 },//((730-240)/(163.8-59))*(78.7-59.0)+240 },
                         new Parameter(){ Code = "Q", Unit = new Units(){Unit = "cubic meters per second", Abbr = "cms"  },Value=490 },
                         new Parameter(){Code = "S", Unit = new Units(){Unit = "meter per meter", Abbr = "m/m"}, Value = 0.003},
                         new Parameter(){ Code = "D_a", Unit = new Units(){Unit = "square meters", Abbr = "m^2"  },Value=16000*Constants.CF_sqrkm2sqrm },
                         new Parameter(){ Code = "L", Unit = new Units(){Unit = "meters", Abbr = "m^2"  },Value=19.7*Constants.CF_km2m },
                         new Parameter(){ Code = "R_r", Unit = new Units(){Unit = "Dimensionless", Abbr = "dim" },Value=1.0 },
                    }
                });
                jobsonstest.Reaches.Add(104.8, new Reach()
                {
                    Name = "Birsfelden (163.8)",
                    Description = "Town 104.8 km downstream from injection site (km 59)",
                    ID = 104,
                    Parameters = new List<Parameter>()
                     {
                         new Parameter(){ Code = "Q_a", Unit = new Units(){Unit = "cubic meters per second", Abbr = "cms" },Value=730 },
                         new Parameter(){ Code = "Q", Unit = new Units(){Unit = "cubic meters per second", Abbr = "cms"  },Value=1068 },
                         new Parameter(){Code = "S", Unit = new Units(){Unit = "meter per meter", Abbr = "m/m"}, Value = 0.003},
                         new Parameter(){ Code = "D_a", Unit = new Units(){Unit = "square meters", Abbr = "m^2"  },Value=48000*Constants.CF_sqrkm2sqrm },
                         new Parameter(){ Code = "L", Unit = new Units(){Unit = "meters", Abbr = "m^2"  },Value=104.8*Constants.CF_km2m },
                         new Parameter(){ Code = "R_r", Unit = new Units(){Unit = "Diminsionless", Abbr = "dim" },Value=1.0 },
                    }
                });
                //instantaneously spills 6,000 kg of a corrosive chemical
                jobsonstest.Execute(6000);

                Assert.IsNotNull(jobsonstest.Reaches);
                //need better assert

            }
            catch (Exception ex)
            {
                Assert.IsTrue(false, ex.Message);
            }
        }
    }
}