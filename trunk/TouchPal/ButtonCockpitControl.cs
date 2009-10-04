/*
 * This file is part of the TouchPal project hosted on Google Code
 * (http://code.google.com/p/touchpal). See the accompanying license.txt file for 
 * applicable licenses.
 */

/*
 * (c) Copyright Craig Courtney 2009 All Rights Reserved.
 */
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace TouchPal
{
    class ButtonCockpitControl : BaseCockpitControl
    {
        private List<ButtonControlState> controlStates = new List<ButtonControlState>();
        private ButtonControlState currentState;
   
        // Base Control Images
        private Image background = null;
        private Image pushedBackground = null;
        private Image defaultImage = null;
        private Image defaultPushedImage = null;

        private bool pushed;
        private string defaultValue;

        public ButtonCockpitControl(ControlManager manager, CockpitXML.CockpitControlsButton control) : base(manager, control.Name, control.Width, control.Height, control.NetworkID, control.PushedAction, control.ReleaseAction)
        {
            this.defaultValue = control.DefaultValue;

            background = manager.ImageCache.getImage(control.BackgroundImage);
            pushedBackground = manager.ImageCache.getImage(control.PushedBackgroundImage);
            defaultImage = manager.ImageCache.getImage(control.DefaultImage);
            defaultPushedImage = manager.ImageCache.getImage(control.DefaultPushedImage);

            if (control.State != null)
            {
                foreach (CockpitXML.CockpitControlsButtonState configState in control.State)
                {
                    ButtonControlState state = new ButtonControlState(manager.ImageCache, configState);
                    controlStates.Add(state);
                }
            }

            pushed = false;
            Value = defaultValue;
        }

        private bool HasPushedImage
        {
            get
            {
                return (pushedBackground != null || defaultPushedImage != null || (currentState != null && currentState.PushedImage != null));
            }
        }

        public override void Pushed()
        {
            if (!pushed)
            {
                if (currentState != null && currentState.PushedActions.Count > 0)
                {
                    foreach (IAction action in currentState.PushedActions)
                    {
                        if (action != null)
                            action.Execute(Manager);
                    }
                }
                else
                {
                    ExecutePushedActions();
                }

                pushed = true;

                if (HasPushedImage) Invalidate();
            }
        }

        public override void Released()
        {
            if (pushed)
            {
                if (currentState != null && currentState.ReleaseActions.Count > 0)
                {
                    foreach (IAction action in currentState.ReleaseActions)
                    {
                        if (action != null)
                            action.Execute(Manager);
                    }
                }
                else
                {
                    ExecuteReleaseActions();
                }

                pushed = false;

                if (HasPushedImage) Invalidate();
            }
        }

        public override string Value
        {
            get
            {
                return base.Value;
            }
            set
            {
                ButtonControlState oldState = this.currentState;
                base.Value = value;
                this.currentState = null;

                foreach (ButtonControlState state in controlStates)
                {
                    if (state.ContainsValue(value))
                        currentState = state;
                }

                if (oldState != currentState) Invalidate();
            }
        }

        public override void Reset()
        {
            Value = defaultValue;
        }

        public override void Paint(Graphics gfx, Rectangle rectangle)
        {
            if (pushed && pushedBackground != null)
                gfx.DrawImage(pushedBackground, rectangle);
            else if (background != null)
                gfx.DrawImage(background, rectangle);

            if (pushed && currentState != null && currentState.PushedImage != null)
                gfx.DrawImage(currentState.PushedImage, rectangle);
            else if (currentState != null && currentState.Image != null)
                gfx.DrawImage(currentState.Image, rectangle);
            else if (pushed && defaultPushedImage != null)
                gfx.DrawImage(defaultPushedImage, rectangle);
            else if (defaultImage != null)
                gfx.DrawImage(defaultImage, rectangle);
        }
    }
}
