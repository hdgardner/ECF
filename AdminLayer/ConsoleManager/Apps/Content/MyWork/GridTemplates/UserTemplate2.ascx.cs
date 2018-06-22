using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Mediachase.Web.Console.Interfaces;
using Mediachase.Web.Console.Common;

namespace Mediachase.Commerce.Manager.Apps.Content.MyWork.GridTemplates
{
    public partial class UserTemplate2 : System.Web.UI.UserControl, IEcfListViewTemplate
    {
        private object _DataItem;
        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// Gets the name of the user.
        /// </summary>
        /// <returns></returns>
        protected string GetUserName()
        {
            Guid editorGuid = (Guid)DataBinder.Eval(DataItem, "EditorUID");
            return ManagementHelper.GetUserName(editorGuid);
        }

        #region IEcfListViewTemplate Members

        /// <summary>
        /// Gets or sets the data item.
        /// </summary>
        /// <value>The data item.</value>
        public object DataItem
        {
            get
            {
                return _DataItem;
            }
            set
            {
                _DataItem = value;
            }
        }

        #endregion
    }
}