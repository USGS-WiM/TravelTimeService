﻿//------------------------------------------------------------------------------
//----- ServiceAgent -------------------------------------------------------
//------------------------------------------------------------------------------

//-------1---------2---------3---------4---------5---------6---------7---------8
//       01234567890123456789012345678901234567890123456789012345678901234567890
//-------+---------+---------+---------+---------+---------+---------+---------+

// copyright:   2017 WiM - USGS

//    authors:  Jeremy K. Newson USGS Web Informatics and Mapping
//              
//  
//   purpose:   The service agent is responsible for initiating the service call, 
//              capturing the data that's returned and forwarding the data back to 
//              the requestor.
//
//discussion:   
//
// 

using System;
using System.Collections.Generic;
using WiM.Resources;
using TravelTimeAgent.Resources;

namespace TravelTimeAgent
{
    public interface ITravelTimeAgent:IMessage
    {
        Jobson initialization();
        Jobson execute(Jobson ToT, Double? InitialMass_M_i_kg = null, DateTime? starttime = null);
    }

    public class TravelTimeAgent : ITravelTimeAgent
    {
        #region Properties
        public List<Message> Messages { get; set; }        

        #endregion
        #region Constructor
        public TravelTimeAgent()
        {
            this.Messages = new List<Message>();

        }
        #endregion
        #region Methods
        public Jobson initialization() {
            return new Jobson();
        }

        public Jobson execute(Jobson ToT, Double? InitialMass_M_i_kg = null, DateTime? starttime = null) {
            if (!ToT.IsValid) throw new Exception("Jobsons is not valid"); 

            ToT.Execute(InitialMass_M_i_kg, starttime);
            return ToT;
        }


        #endregion
        #region HELPER METHODS
        private void sm(string message, MessageType type = MessageType.info)
        {
            this.Messages.Add(new Message() { msg=message, type = type });
        }

        #endregion
    }

}