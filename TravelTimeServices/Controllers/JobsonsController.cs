//------------------------------------------------------------------------------
//----- HttpController ---------------------------------------------------------
//------------------------------------------------------------------------------

//-------1---------2---------3---------4---------5---------6---------7---------8
//       01234567890123456789012345678901234567890123456789012345678901234567890
//-------+---------+---------+---------+---------+---------+---------+---------+

// copyright:   2017 WIM - USGS

//    authors:  Jeremy K. Newson USGS Web Informatics and Mapping
//              
//  
//   purpose:   Handles resources through the HTTP uniform interface.
//
//discussion:   Controllers are objects which handle all interaction with resources. 
//              
//
// 

using Microsoft.AspNetCore.Mvc;
using System;
using TravelTimeAgent;
using System.Threading.Tasks;
using System.Collections.Generic;
using WIM.Resources;
//using WIM.Services.Attributes;

namespace TravelTimeServices.Controllers
{
    [Route("[controller]")]
    //[APIDescription(type = DescriptionType.e_string, Description = "The Jobsons resource represents the prediction of travel time as documented in Prediction of Traveltime and Longitudinal Dispersion in Rivers and Streams (Jobson, 1996.) Resultants return the calculated volume, accumulated travel times for the leading edge, peak, and trailing edge of a spill, the peak concentration, accumulated distance downstream, and associated input parameters.")]
    public class JobsonsController : WIM.Services.Controllers.ControllerBase
    {
        public ITravelTimeAgent agent { get; set; }
        public JobsonsController(ITravelTimeAgent agent ) : base()
        {
            this.agent = agent;
        }
        #region METHODS
        [HttpGet(Name = "Initialize")]
        //[APIDescription(type = DescriptionType.e_link, Description = "/Docs/Jobsons/initialize.md")]
        public async Task<IActionResult> Get()
        {
            //returns list of available Navigations
            try
            {
                var result = agent.initialization();                
                return Ok(result);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }
        [HttpPost("Execute", Name = "Compute Jobsons")][HttpGet("Execute", Name = "Compute Jobsons")]
        //[APIDescription(type = DescriptionType.e_link, Description = "/Docs/Jobsons/compute_jobsons.md")]
        public async Task<IActionResult> Execute([FromBody]Jobson configurations, [FromQuery]Double? initialmassconcentration = null, [FromQuery]DateTime? starttime = null)
        {
            try
            {
                var result = agent.execute(configurations, initialmassconcentration, starttime);                
                return Ok(result);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }
            #endregion
            #region HELPER METHODS
            private void sm(List<Message> messages)
        {
            if (messages.Count < 1) return;
            HttpContext.Items[WIM.Services.Middleware.X_MessagesExtensions.msgKey] = messages;
        }
        #endregion
    }
}
