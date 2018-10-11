using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using TravelTimeAgent;
using TravelTimeAgent.Resources;
using WiM.Resources;

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
                List<IParameter> toremove = new List<IParameter>();
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
                    Parameters = new List<IParameter>()
                     {
                         new Parameter(){ Code = "Q_a", Unit = new Units(){Unit = "cubic meters per second", Abbr = "cms" },Value=5.22 * (430.0 / 452.0)},
                         new Parameter(){ Code = "Q", Unit = new Units(){Unit = "cubic meters per second", Abbr = "cms"  },Value=3.88 * (430.0 / 452.0) },
                         new Parameter(){ Code = "D_a", Unit = new Units(){Unit = "square meters", Abbr = "m^2"  },Value=430.0*Constants.CF_sqrkm2sqrm },
                         new Parameter(){ Code = "L", Unit = new Units(){Unit = "meters", Abbr = "m^2"  },Value=15.0*Constants.CF_km2m },
                     }
                });

                jobsonstest.Execute(6000);
                //Assert.IsNotNull(jobsonstest);
            }
            catch (Exception ex)
            {
                Assert.IsTrue(false, ex.Message);
            }
        }
    }
}
