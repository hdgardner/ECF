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
using Mediachase.Web.Console.BaseClasses;
using Mediachase.Cms;
using Mediachase.Commerce.Profile;
using Mediachase.Commerce.Shared;
using Mediachase.Web.Console.Interfaces;
using Mediachase.Web.Console.Common;

namespace Mediachase.Commerce.Manager.Folders.Tabs
{
    public partial class CommandEditTab : BaseUserControl, IAdminTabControl
    {
		/// <summary>
		/// Gets the Page id.
		/// </summary>
		/// <value>The Page id.</value>
		public int PageId
		{
			get
			{
				return ManagementHelper.GetIntFromQueryString("PageId");
			}
		}

		/// <summary>
		/// Gets the Command id.
		/// </summary>
		/// <value>The Command id.</value>
		public int CommandId
		{
			get
			{
				return ManagementHelper.GetIntFromQueryString("CommandId");
			}
		}

        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            SecurityManager.CheckRolePermission("content:site:nav:mng:edit");
        }

        /// <summary>
        /// Binds the form.
        /// </summary>
        private void BindForm()
        {
			if (PageId == 0)
				return;

            BindDropDown();
			if (CommandId > 0)
			{
				//Bind data, if edit
				using (IDataReader reader = NavigationManager.GetCommandById(CommandId))
				{
					if (reader.Read())
					{
						tbQueryString.Text = (string)reader["Params"];
						if (!IsPostBack)
						{
							ddNavCmd.Items.FindByValue(((int)reader["ItemId"]).ToString()).Selected = true;
						}
					}
                    reader.Close();
				}
			}

            //imgAddCmd.Click += new ImageClickEventHandler(imgAddCmd_Click);
            //imgAddCmd.ImageUrl = Mediachase.Cms.Util.CommonHelper.GetAbsoluteThemedPath("/images/img_48.jpg", Page.Theme);
            imgAddCmd.ToolTip = Resources.Admin.AddCommand;
        }

        /// <summary>
        /// Binds the drop down.
        /// </summary>
        private void BindDropDown()
        {
            ddNavCmd.DataSource = NavigationManager.GetAllItemsDT();
            ddNavCmd.DataTextField = "ItemName";
            ddNavCmd.DataValueField = "ItemId";
            ddNavCmd.DataBind();
        }

        /// <summary>
        /// Binds a data source to the invoked server control and all its child controls.
        /// </summary>
        public override void DataBind()
        {
            base.DataBind();
            BindForm();
        }

        #region IAdminTabControl Members
        /// <summary>
        /// Saves the changes.
        /// </summary>
        /// <param name="context">The context.</param>
        public void SaveChanges(IDictionary context)
        {
			if (CommandId > 0)
				NavigationManager.UpdateCommand(CommandId, PageId.ToString(), Convert.ToInt32(ddNavCmd.SelectedValue), tbQueryString.Text, String.Empty);
			else
				NavigationManager.NewCommand(PageId.ToString(), Convert.ToInt32(ddNavCmd.SelectedValue), tbQueryString.Text, string.Empty);
            //Response.Redirect("NavigationCmd.aspx?PageId=" + Parameters["PageId"]);
        }
        #endregion
    }
}
