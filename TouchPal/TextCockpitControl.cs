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
using System.Drawing.Text;
using System.Drawing;
using System.IO;

namespace TouchPal
{
    class TextCockpitControl : BaseCockpitControl
    {
        private static PrivateFontCollection fonts = new PrivateFontCollection();
        private static List<string> fontFiles = new List<string>();
        private Font textFont = null;
        private Brush textBrush = null;
        private Image background = null;
        private string defaultValue = "";

        public TextCockpitControl(ControlManager manager, CockpitXML.CockpitControlsText control)
            : base(manager, control.Name, control.Width, control.Height, control.NetworkID, control.PushedAction, control.ReleaseAction)
        {
            background = manager.ImageCache.getImage(control.BackgroundImage);
            defaultValue = control.DefaultValue;

            if (control.FontFile != null && File.Exists(control.FontFile))
            {
                if (!fontFiles.Contains(control.FontFile))
                {
                    fonts.AddFontFile(control.FontFile);
                    fontFiles.Add(control.FontFile);
                }
            }

            foreach (FontFamily ff in fonts.Families)
            {
                if (ff.Name.Equals(control.Font))
                {
                    textFont = new Font(ff, control.FontSize);
                    break;
                }
            }
            if (textFont == null)
                textFont = new Font(control.Font, control.FontSize);

            if (control.FontColor != null)
            {
                textBrush = new SolidBrush(Color.FromArgb(control.FontColor.Red, control.FontColor.Green, control.FontColor.Blue));
            }
            else
            {
                textBrush = Brushes.Yellow;
            }

            Value = defaultValue;
        }

        public override void Pushed()
        {
            throw new NotImplementedException();
        }

        public override void Released()
        {
            ExecutePushedActions();
        }

        public override void Reset()
        {
            Value = defaultValue;
        }

        public override string Value
        {
            get
            {
                  return base.Value;
            }
            set
            {
                if (Value == null || !Value.Equals(value))
                {
                    Invalidate();
                }
                base.Value = value;
            }
        }

        public override void Paint(System.Drawing.Graphics gfx, System.Drawing.Rectangle rectangle)
        {
            if (background != null)
                gfx.DrawImage(background, rectangle);
            gfx.DrawString(Value, textFont, textBrush, rectangle);
        }
    }
}
