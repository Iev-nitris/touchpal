using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace TouchPal
{
    public class CockpitPanel
    {
        private string name;
        private CockpitForm form;
        private bool visible;
        private Point location;
        private Size size;
        private Rectangle rectangle;
        private Image backgroundImage;
        private List<CockpitControlInstance> controlInstances = new List<CockpitControlInstance>();

        public CockpitPanel(CockpitForm form, ControlManager manager, IImageCache imageCache, CockpitXML.CockpitLayoutPanel layout)
        {
            this.name = layout.Name;
            this.form = form;

            if (layout.ControlLayout != null)
            {
                // Now construct the list of controls that are actually used in the layout
                foreach (CockpitXML.CockpitLayoutPanelControlLayout controlLayout in layout.ControlLayout)
                {

                    if (manager.HasControl(controlLayout.ControlName))
                    {
                        ICockpitControl control = manager.GetControl(controlLayout.ControlName);
                        CockpitControlInstance formItem = new CockpitControlInstance(this, control, new Point(controlLayout.X, controlLayout.Y));
                        controlInstances.Add(formItem);
                        control.AddInstance(formItem);
                    }
                }
            }

            this.Location = new Point(layout.X, layout.Y);
            this.Size = new Size(layout.Width, layout.Height);
            this.backgroundImage = imageCache.getImage(layout.BackgroundImage);
            if (layout.VisibleSpecified)
                this.Visible = layout.Visible;
            else
                this.Visible = true;
        }

        public string Name
        {
            get { return name; }
        }

        public bool Visible
        {
            get { return visible; }
            set {
                if (this.visible != value)
                {
                    this.visible = value;
                    form.Invalidate(Rectangle);
                    TouchPal.Debug("Panel " + Name + " is now " + (value ? "visible." : "hidden."));
                }
            }
        }

        public Point Location
        {
            get { return location; }
            set { 
                location = value;
                this.Rectangle = new Rectangle(value, this.Size);
            }
        }

        public Rectangle Rectangle
        {
            get { return rectangle; }
            set { this.rectangle = value; }
        }

        public Size Size
        { 
            get { return size; }
            set 
            { 
                this.size = value;
                this.Rectangle = new Rectangle(this.Location, value);
            }
        }
        public CockpitControlInstance InstanceAt(Point location)
        {
            location.Offset(-Location.X,-Location.Y);
            // Loop through controls in reverse order check if click is within that button then execute action if so
            for (int i = controlInstances.Count() - 1; i >= 0; i--)
            {
                CockpitControlInstance item = controlInstances[i];
                if (item.Rectangle.Contains(location))
                {
                    return item;
                }
            }
            return null;
        }

        public void Invalidate(Rectangle rc)
        {
            rc.Offset(Location);
            form.Invalidate(rc);
        }

        public void Paint(Graphics gfx, Rectangle clipRectangle)
        {
            if (backgroundImage != null)
                gfx.DrawImage(backgroundImage, Rectangle);

            Matrix oldTransform = gfx.Transform;
            gfx.TranslateTransform(location.X, location.Y);
            clipRectangle.Intersect(this.Rectangle);
            clipRectangle.Offset(-Location.X, -Location.Y);

            foreach (CockpitControlInstance control in controlInstances)
            {
                if (clipRectangle.IntersectsWith(control.Rectangle))
                    control.Paint(gfx);
            }

            gfx.Transform = oldTransform;
        }
    }
}
