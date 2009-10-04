/*
 * This file is part of the TouchPal project hosted on Google Code
 * (http://code.google.com/p/touchpal). See the accompanying license.txt file for 
 * applicable licenses.
 */

/*
 * (c) Copyright Craig Courtney 2009 All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

namespace TouchPal
{
    class NetworkSendAction : IAction
    {
        public NetworkSendAction(string data)
        {
            this.data = data;
        }

        #region IAction Members

        private string data;

        public void Execute(ControlManager manager)
        {
            manager.NetConnection.SendData(data);
        }

        #endregion
    }
}
