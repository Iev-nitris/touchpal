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
        private List<CockpitPanel> controlPanels = new List<CockpitPanel>();
        private CockpitControlInstance pushedItem = null;

        public CockpitForm(ControlManager manager, IImageCache imageCache, CockpitXML.CockpitLayout layout, int forceX, int forceY)
        {
            this.manager = manager;
            this.imageCache = imageCache;
            manager.AddForm(this);

            InitializeComponent();

            // Now construct the list of controls that are actually used in the layout
            foreach (CockpitXML.CockpitLayoutPanel controlPanel in layout.Panel)
            {
                controlPanels.Add(new CockpitPanel(this, manager, imageCache, controlPanel));
            }

            this.Location = new Point(forceX >= 0 ? forceX : layout.X, forceY >= 0 ? forceY : layout.Y);
            this.Size = new Size(layout.Width, layout.Height);
            this.BackgroundImage = imageCache.getImage(layout.BackgroundImage);
            if (layout.TransparencyKey != null)
                this.TransparencyKey = Color.FromArgb(layout.TransparencyKey.Red, layout.TransparencyKey.Green, layout.TransparencyKey.Blue);

            this.MouseDown += new MouseEventHandler(CockpitForm_MouseDown);
            this.MouseUp += new MouseEventHandler(CockpitForm_MouseUp);
            this.Paint += new PaintEventHandler(this.CockpitForm_Paint);
        }

        public void SetPanelVisibility(string name, bool value)
        {
            foreach (CockpitPanel panel in controlPanels)
            {
                if (panel.Name.Equals(name))
                    panel.Visible = value;
            }
        }

        private void CockpitForm_MouseDown(object sender, MouseEventArgs e)
        {
            // Loop through controls in reverse order check if click is within that button then execute action if so
            for (int i = controlPanels.Count()-1; i >= 0; i--)
            {
                if (controlPanels[i].Visible && controlPanels[i].Rectangle.Contains(e.Location))
                {
                    CockpitControlInstance item = controlPanels[i].InstanceAt(e.Location);
                    if (item != null)
                    {
                        pushedItem = item;
                        pushedItem.Control.Pushed();
                        i = -1;
                    }
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

            foreach (CockpitPanel panel in controlPanels)
            {
                if (panel.Visible && paintEvnt.ClipRectangle.IntersectsWith(panel.Rectangle))
                {
                    panel.Paint(gfx, paintEvnt.ClipRectangle);
                }              
            }
        }
    }
}
