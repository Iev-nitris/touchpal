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
    class ButtonControlState
    {
        private string[] stateValues;
        private Image stateImage;
        private Image pushedStateImage;
        private List<IAction> pushedActions = new List<IAction>();
        private List<IAction> releaseActions = new List<IAction>();

        public ButtonControlState(IImageCache imageCache, CockpitXML.CockpitControlsButtonState state)
        {
            if (state.StateValue == null)
                stateValues = new string[0];
            else
                stateValues = state.StateValue; 
            
            this.stateImage = imageCache.getImage(state.Image);
            this.pushedStateImage = imageCache.getImage(state.PushedImage);

            if (state.PushedAction != null)
            {
                foreach (string action in state.PushedAction)
                {
                    pushedActions.Add(ActionFactory.CreateAction(action));
                }
            }

            if (state.ReleaseAction != null)
            {
                foreach (string action in state.ReleaseAction)
                {
                    releaseActions.Add(ActionFactory.CreateAction(action));
                }
            }
        }

        public bool ContainsValue(string value)
        {
            return (stateValues.Contains(value));
        }

        public Image Image
        {
            get
            {
                return stateImage;
            }
        }

        public Image PushedImage
        {
            get
            {
                return pushedStateImage;
            }
        }

        public List<IAction> PushedActions
        {
            get
            {
                return pushedActions;
            }
        }

        public List<IAction> ReleaseActions
        {
            get
            {
                return releaseActions;
            }
        }
    }
}
