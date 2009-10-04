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
    public class ControlManager
    {
        private INetConnection connection;
        private IImageCache imageCache;
        Dictionary<string, ICockpitControl> controls = new Dictionary<string, ICockpitControl>();
        List<IAction> startActions = new List<IAction>();
        List<IAction> resetActions = new List<IAction>();
        List<CockpitForm> forms = new List<CockpitForm>();

        public ControlManager(INetConnection connection, IImageCache imageCache, string[] startActions, string[] resetActions, CockpitXML.CockpitControls controls)
        {
            this.connection = connection;
            this.imageCache = imageCache;

            connection.Manager = this;

            // Load start actions
            if (startActions != null)
                foreach (string action in startActions)
                    this.startActions.Add(ActionFactory.CreateAction(action));

            if (resetActions != null)
                foreach (string action in resetActions)
                    this.resetActions.Add(ActionFactory.CreateAction(action));

            // Load all of the controls defined
            if (controls.Button != null)
            {
                foreach (CockpitXML.CockpitControlsButton control in controls.Button)
                {
                    TouchPal.Debug("Loading button control: " + control.Name);
                    ButtonCockpitControl item = new ButtonCockpitControl(this, control);
                    this.controls.Add(control.Name, item);
                }
            }

            // Load all of the controls defined
            if (controls.Text != null)
            {
                foreach (CockpitXML.CockpitControlsText control in controls.Text)
                {
                    TouchPal.Debug("Loading text control: " + control.Name);
                    TextCockpitControl item = new TextCockpitControl(this, control);
                    this.controls.Add(control.Name, item);
                }
            }
        }

        public IImageCache ImageCache
        {
            get
            {
                return imageCache;
            }
        }

        public INetConnection NetConnection
        {
            get
            {
                return connection;
            }
        }

        public bool HasControl(string name)
        {
            return controls.ContainsKey(name);
        }

        public ICockpitControl GetControl(string name)
        {
            return controls[name];
        }

        public void UpdateControl(int networkID, string value)
        {
            foreach (KeyValuePair<string, ICockpitControl> kvp in controls)
            {
                if (kvp.Value.NetworkID == networkID && !kvp.Value.Value.Equals(value))
                {
                    kvp.Value.Value = value;
                }
            }
        }

        public void ClientConnected()
        {
            foreach (IAction action in startActions)
                action.Execute(this);
        }

        public void ResetControls()
        {
            foreach (KeyValuePair<string, ICockpitControl> kvp in controls)
            {
                kvp.Value.Reset();
            }
            foreach (IAction action in resetActions)
                action.Execute(this);
        }

        public void AddForm(CockpitForm form)
        {
            if (!forms.Contains(form))
                forms.Add(form);
        }

        public void Exit()
        {
            foreach (CockpitForm form in forms)
                form.Close();
        }
    }
}
