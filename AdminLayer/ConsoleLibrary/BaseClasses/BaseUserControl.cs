using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;
using System.Collections.Specialized;
using Mediachase.Web.Console.Interfaces;
using System.Globalization;
using System.Resources;
using Mediachase.Web.Console.Common;
using Mediachase.Commerce.Profile;
using Mediachase.Commerce.Shared;
using Mediachase.Commerce.Core;

namespace Mediachase.Web.Console.BaseClasses
{
    public class BaseUserControl : UserControl, IDynamicParamControl
    {
        /// <summary>
        /// Gets a value indicating whether the user control is being loaded in response to a client postback, or if it is being loaded and accessed for the first time.
        /// </summary>
        /// <value></value>
        /// <returns>true if the user control is being loaded in response to a client postback; otherwise, false.</returns>
        public new bool IsPostBack
        {
            get
            {
                //bool rendering = ScriptManager.GetCurrent(this.Page).isin.IsInPartialRenderingMode;
                return base.IsPostBack; //&& !rendering;
            }
        }

        /// <summary>
        /// Gets the RM.
        /// </summary>
        /// <value>The RM.</value>
        public ConsoleResourceManager RM
        {
            get
            {
                return new ConsoleResourceManager();
            }
        }

        #region IDynamicParamControl Members

        /// <summary>
        /// Gets or sets the parameters.
        /// </summary>
        /// <value>The parameters.</value>
        public NameValueCollection Parameters
        {
            get
            {
                return ManagementContext.Current.Parameters;
            }
            set
            {
                ManagementContext.Current.Parameters = value;
            }
        }

        #endregion

        /// <summary>
        /// Displays the error message.
        /// </summary>
        /// <param name="msg">The MSG.</param>
        protected void DisplayErrorMessage(string msg)
        {
            ErrorManager.GenerateError(msg);
        }

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Init"/> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> object that contains the event data.</param>
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
        }
    }
}
