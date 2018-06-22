using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;

namespace Mediachase.Cms.DataAdapter
{
    /// <summary>
    /// Base for control data adapters.
    /// </summary>
    public abstract class DataAdapterBase
    {
        #region Control
        private Control control;
        /// <summary>
        /// Gets or sets the control.
        /// </summary>
        /// <value>The control.</value>
        public Control Control
        {
            get { return control; }
            set { control = value; }
        } 
        #endregion

        #region +SetProperty()
        /// <summary>
        /// Sets the property.
        /// </summary>
        /// <param name="PropertyStorage">The property storage.</param>
        public abstract void SetProperty(object PropertyStorage);
        #endregion
    }
}