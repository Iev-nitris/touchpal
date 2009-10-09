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
    public class CockpitControlInstance
    {
        private CockpitPanel panel;
        private ICockpitControl control;
        private Point location;
        private Rectangle rectangle;

        public CockpitControlInstance(CockpitPanel panel, ICockpitControl control, Point location)
        {
            this.panel = panel;
            this.control = control;
            this.location = location;
            this.rectangle = new Rectangle(location, control.Size);
        }

        public ICockpitControl Control
        {
            get
            {
                return control;
            }
        }

        public Point Location
        {
            get
            {
                return location;
            }
        }

        public Size Size
        {
            get
            {
                return control.Size;
            }
        }

        public Rectangle Rectangle
        {
            get
            {
                return rectangle;
            }
        }

        public void Invalidate()
        {
            panel.Invalidate(rectangle);
        }

        public void Paint(Graphics gfx)
        {
            control.Paint(gfx, rectangle);
        }
    }
}
