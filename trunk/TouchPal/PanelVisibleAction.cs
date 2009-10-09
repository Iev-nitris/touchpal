using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouchPal
{
    class PanelVisibleAction : IAction
    {
        private string panelName;
        private bool panelVisible;

        public PanelVisibleAction(string name, bool visible)
        {
            this.panelName = name;
            this.panelVisible = visible;
        }

        #region IAction Members

        void IAction.Execute(ControlManager manager)
        {
            manager.SetPanelVisibility(panelName, panelVisible);
        }

        #endregion
    }
}
