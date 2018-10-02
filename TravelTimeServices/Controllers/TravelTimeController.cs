//------------------------------------------------------------------------------
//----- HttpController ---------------------------------------------------------
//------------------------------------------------------------------------------

//-------1---------2---------3---------4---------5---------6---------7---------8
//       01234567890123456789012345678901234567890123456789012345678901234567890
//-------+---------+---------+---------+---------+---------+---------+---------+

// copyright:   2017 WiM - USGS

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
using WiM.Resources;

namespace TravelTimeServices.Controllers
{
    [Route("[controller]")]
    public class TravelTimeController : WiM.Services.Controllers.ControllerBase
    {
        public ITravelTimeAgent agent { get; set; }
        public TravelTimeController(ITravelTimeAgent agent ) : base()
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
                var result = agent.method();
                sm(agent.Messages);
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
            HttpContext.Items[WiM.Services.Middleware.X_MessagesExtensions.msgKey] = messages;
        }
        #endregion
    }
}
