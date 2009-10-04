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

namespace TouchPal
{
    class ChangeValueAction : IAction
    {
        int networkID = 0;
        string value = null;

        public ChangeValueAction(string change)
        {
            string[] values = change.Split(new Char[] { '=' });
            try
            {
                networkID = Convert.ToInt32(values[0]);
                value = values[1];
            }
            catch (FormatException fe)
            {
                TouchPal.Warn("Bad network id value (" + values[0] + ")");
            }
        }

        #region IAction Members
       
        void IAction.Execute(ControlManager manager)
        {
            manager.UpdateControl(networkID, value);
        }

        #endregion
    }
}
