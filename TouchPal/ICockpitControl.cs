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
using System.Drawing;

namespace TouchPal
{
    public interface ICockpitControl
    {
        string Name
        {
            get;
        }

        int NetworkID
        {
            get;
        }

        Size Size
        {
            get;
        }

        string Value
        {
            get;
            set;
        }

        void Invalidate();
        void Paint(Graphics gfx, Rectangle rectangle);
        void Pushed();
        void Released();
        void Reset();
        void AddInstance(CockpitControlInstance instance);
        void RemoveInstance(CockpitControlInstance instance);
    }
}
