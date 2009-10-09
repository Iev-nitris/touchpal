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
    class ActionFactory
    {
        public static IAction CreateAction(string description)
        {
            if (description == null || description.Equals(""))
                return null;

            if (description[2].Equals(':'))
            {
                string command = description.Substring(0, 2);
                string commandValue = description.Substring(3);

                if (command.Equals("NS"))
                    return new NetworkSendAction(commandValue);
                if (command.Equals("KD"))
                    return new KeyAction(commandValue, true);
                if (command.Equals("KU"))
                    return new KeyAction(commandValue, false);
                if (command.Equals("CV"))
                    return new ChangeValueAction(commandValue);
                if (command.Equals("EX"))
                    return new ExitAction();
                if (command.Equals("RS"))
                    return new ResetAction();
                if (command.Equals("PH"))
                    return new PanelVisibleAction(commandValue, false);
                if (command.Equals("PS"))
                    return new PanelVisibleAction(commandValue, true);
            }

            TouchPal.Warn("Invalid action definition: " + description);
            return null;
        }
    }
}
