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

namespace TravelTimeServices.Controllers
{
    [Route("[controller]")]
    public class JobsonsController : WIM.Services.Controllers.ControllerBase
    {
        public ITravelTimeAgent agent { get; set; }
        public JobsonsController(ITravelTimeAgent agent ) : base()
        {
            this.agent = agent;
        }
        #region METHODS
        [HttpGet()]
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
        [HttpPost()][HttpGet("Execute")]
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
