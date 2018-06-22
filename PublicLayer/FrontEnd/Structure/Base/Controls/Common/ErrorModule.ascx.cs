using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using Mediachase.Cms.WebUtility.UI;

namespace Mediachase.Cms.Controls.Common
{
    /// <summary>
    /// This module is used to report errors back to the UI. Do not use it to report any code traces and such. 
    /// Instead always provide human explanation and way for user to correct something.
    /// </summary>
    public partial class ErrorModule : UserControl
    {
        ArrayList _Messages = null;
        /// <summary>
        /// Initializes a new instance of the <see cref="T:ErrorModule"/> class.
        /// </summary>
        public ErrorModule()
        {
            ErrorManager.Instance.Error += new ErrorEventHandler(Instance_Error);
        }

        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="T:System.EventArgs"/> instance containing the event data.</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (_Messages==null || _Messages.Count == 0)
                ErrorMessages.Visible = false;
        }

        /// <summary>
        /// Handles the Error event of the Instance control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.IO.ErrorEventArgs"/> instance containing the event data.</param>
        private void Instance_Error(object sender, ErrorEventArgs e)
        {
            if (_Messages == null)
                _Messages = new ArrayList();
            _Messages.Add(e.Message);
            ErrorMessages.DataSource = _Messages;
            ErrorMessages.DataBind();
            ErrorMessages.Visible = true;
        }
    }
}