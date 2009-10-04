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
    class ResetAction : IAction
    {
        #region IAction Members

        void IAction.Execute(ControlManager manager)
        {
            manager.ResetControls();
        }

        #endregion
    }
}
