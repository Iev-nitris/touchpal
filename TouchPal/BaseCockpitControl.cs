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
    abstract class BaseCockpitControl : ICockpitControl
    {
        private List<CockpitControlInstance> instances = new List<CockpitControlInstance>();

        private List<IAction> pushedActions = new List<IAction>();
        private List<IAction> releaseActions = new List<IAction>();

        private ControlManager manager;

        private int networkID;
        private string name;
        private string currentValue;
        private Size size;

        public BaseCockpitControl(ControlManager manager, string name, int width, int height, int networkid, string[] pushedActions, string[] releaseActions)
        {
            this.manager = manager;
            this.size = new Size(width, height);
            this.name = name;
            this.networkID = networkid;

            if (pushedActions != null)
            {
                foreach (string action in pushedActions)
                {
                    this.pushedActions.Add(ActionFactory.CreateAction(action));
                }
            }

            if (releaseActions != null)
            {
                foreach (string action in releaseActions)
                {
                    this.releaseActions.Add(ActionFactory.CreateAction(action));
                }
            }
        }

        protected ControlManager Manager
        {
            get
            {
                return manager;
            }
        }

        protected void ExecutePushedActions()
        {
            foreach (IAction action in pushedActions)
            {
                if (action != null)
                    action.Execute(Manager);
            }
        }

        protected void ExecuteReleaseActions()
        {
            foreach (IAction action in releaseActions)
            {
                if (action != null)
                    action.Execute(Manager);
            }
        }

        public string Name
        {
            get
            {
                return name;
            }
        }

        public int NetworkID
        {
            get
            {
                return networkID;
            }
        }

        public Size Size
        {
            get
            {
                return size;
            }
        }

        public virtual string Value
        {
            get
            {
                return this.currentValue;
            }
            set
            {
                if (currentValue == null || !currentValue.Equals(value))
                {
                    TouchPal.Debug("Control (NetworkID=" + networkID + ") value updated to " + value);
                    this.currentValue = value;
                }
            }
        }

        public void AddInstance(CockpitControlInstance instance)
        {
            if (!instances.Contains(instance))
                instances.Add(instance);
        }

        public void RemoveInstance(CockpitControlInstance instance)
        {
            if (instances.Contains(instance))
                instances.Remove(instance);
        }

        public void Invalidate()
        {
            foreach (CockpitControlInstance instance in instances)
                instance.Invalidate();
        }

        public abstract void Pushed();
        public abstract void Released();
        public abstract void Reset();
        public abstract void Paint(Graphics gfx, Rectangle rectangle);
    }
}
