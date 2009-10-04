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
using System.IO;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace TouchPal
{
    public partial class CockpitForm : Form
    {
        private IImageCache imageCache;
        private ControlManager manager;
        private List<CockpitControlInstance> controlInstances = new List<CockpitControlInstance>();
        private CockpitControlInstance pushedItem = null;

        public CockpitForm(ControlManager manager, IImageCache imageCache, CockpitXML.CockpitLayout layout)
        {
            this.manager = manager;
            this.imageCache = imageCache;
            manager.AddForm(this);

            InitializeComponent();

            // Now construct the list of controls that are actually used in the layout
            foreach (CockpitXML.CockpitLayoutControlLayout controlLayout in layout.ControlLayout)
            {
                if (manager.HasControl(controlLayout.ControlName))
                {
                    ICockpitControl control = manager.GetControl(controlLayout.ControlName);
                    CockpitControlInstance formItem = new CockpitControlInstance(this, control, new Point(controlLayout.X, controlLayout.Y));
                    controlInstances.Add(formItem);
                    control.AddInstance(formItem);
                }
            }

            this.Location = new Point(layout.X, layout.Y);
            this.Size = new Size(layout.Width, layout.Height);
            this.BackgroundImage = imageCache.getImage(layout.BackgroundImage);
            if (layout.TransparencyKey != null)
                this.TransparencyKey = Color.FromArgb(layout.TransparencyKey.Red, layout.TransparencyKey.Green, layout.TransparencyKey.Blue);

            this.MouseDown += new MouseEventHandler(CockpitForm_MouseDown);
            this.MouseUp += new MouseEventHandler(CockpitForm_MouseUp);
            this.Paint += new PaintEventHandler(this.CockpitForm_Paint);
        }


        private void CockpitForm_MouseDown(object sender, MouseEventArgs e)
        {
            // Loop through controls in reverse order check if click is within that button then execute action if so
            for (int i = controlInstances.Count()-1; i >= 0; i--)
            {
                CockpitControlInstance item = controlInstances[i];
                if (item.Rectangle.Contains(e.Location))
                {
                    pushedItem = item;
                    pushedItem.Control.Pushed();
                    i = -1;
                }
            }            
        }

        private void CockpitForm_MouseUp(object sender, MouseEventArgs e)
        {
            if (pushedItem != null)
            {
                pushedItem.Control.Released();
                pushedItem = null;
            }
        }

        protected override CreateParams CreateParams
        {
            get
            {
                const int WS_EX_NOACTIVATE = 0x8000000;

                CreateParams cp = base.CreateParams;
                cp.ExStyle |= WS_EX_NOACTIVATE;
                return cp;
            }
        }

        private void CockpitForm_Load(object sender, EventArgs e)
        {
        }

        private void CockpitForm_Paint(object sender, PaintEventArgs paintEvnt)
        {
            Graphics gfx = paintEvnt.Graphics;

            foreach (CockpitControlInstance control in controlInstances)
            {
                if (paintEvnt.ClipRectangle.IntersectsWith(control.Rectangle))
                    control.Paint(gfx);
            }
        }
    }
}
