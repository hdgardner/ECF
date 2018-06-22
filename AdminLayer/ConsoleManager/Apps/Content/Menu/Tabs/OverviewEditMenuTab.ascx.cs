using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Threading;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using Mediachase.Web.Console.BaseClasses;
using mc = Mediachase.Cms;
using Mediachase.Web.Console.Common;
using Mediachase.Web.Console.Interfaces;

namespace Mediachase.Commerce.Manager.Menu.Tabs
{
    public partial class OverviewEditMenuTab : BaseUserControl, IAdminTabControl
    {
        /// <summary>
        /// Gets the site id.
        /// </summary>
        /// <value>The site id.</value>
        public Guid SiteId
        {
            get
            {
                return new Guid(Request.QueryString["SiteId"].ToString());
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is new.
        /// </summary>
        /// <value><c>true</c> if this instance is new; otherwise, <c>false</c>.</value>
        public bool IsNew
        {
            get
            {
                return !String.IsNullOrEmpty(Parameters["isnew"]);
            }
        }

        /// <summary>
        /// Gets the menu id.
        /// </summary>
        /// <value>The menu id.</value>
        public int MenuId
        {
            get
            {
                return ManagementHelper.GetIntFromQueryString("MenuId");
            }
        }

        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// Binds the form.
        /// </summary>
        private void BindForm()
        {
            if (IsNew)
                return;

            using (IDataReader menu = Mediachase.Cms.Menu.LoadById(MenuId))
            {
                if (menu.Read())
                    Name.Text = menu["FriendlyName"].ToString();
                menu.Close();
            }
        }

        /// <summary>
        /// Binds a data source to the invoked server control and all its child controls.
        /// </summary>
        public override void DataBind()
        {
            base.DataBind();
            BindForm();
        }

		/// <summary>
		/// Checks if entered name is unique.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="args">The <see cref="System.Web.UI.WebControls.ServerValidateEventArgs"/> instance containing the event data.</param>
		public void NameCheck(object sender, ServerValidateEventArgs args)
		{
			bool menuExists = false;

			using (IDataReader menuReader = Mediachase.Cms.Menu.LoadByName(Name.Text, SiteId))
			{
				int menuId = 0;
				if (menuReader.Read())
				{
					menuId = (int)menuReader["MenuId"];
					if (menuId != MenuId)
						menuExists = true;
				}
				menuReader.Close();
			}

			args.IsValid = !menuExists;
		}

        #region IAdminTabControl Members
        /// <summary>
        /// Saves the changes.
        /// </summary>
        /// <param name="context">The context.</param>
        public void SaveChanges(IDictionary context)
        {
            if(IsNew)
                Mediachase.Cms.Menu.Add(Name.Text, SiteId);
            else
                Mediachase.Cms.Menu.Update(MenuId, Name.Text);
        }
        #endregion
    }
}