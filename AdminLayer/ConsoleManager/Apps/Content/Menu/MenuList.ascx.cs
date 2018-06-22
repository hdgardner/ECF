using System;
using System.Data;
using System.Web.UI;
using Mediachase.Ibn.Web.UI.WebControls;
using Mediachase.Web.Console.BaseClasses;
using Mediachase.Web.Console.Common;
using Mediachase.Web.Console.Controls;
using mc = Mediachase.Cms;

namespace Mediachase.Commerce.Manager.Content.Menu
{
	public partial class MenuList : BaseUserControl
	{
		#region Publc Properties
		/// <summary>
        /// Gets the language id.
        /// </summary>
        /// <value>The language id.</value>
        public int LanguageId
        {
            get
            {
                return ManagementHelper.GetIntFromQueryString("langid");
            }
        }

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
        /// Gets the menu item id.
        /// </summary>
        /// <value>The menu item id.</value>
        public int MenuItemId
        {
            get
            {
				return ManagementHelper.GetIntFromQueryString("MenuItemId");
            }
        }

		/// <summary>
		/// Gets the parent id.
		/// </summary>
		/// <value>The parent id.</value>
		public int ParentItemId
		{
			get
			{
				return ManagementHelper.GetIntFromQueryString("parentid");
			}
		}
		#endregion

		/// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack || String.Compare(Request.Form["__EVENTTARGET"], CommandManager.GetCurrent(this.Page).ID, false) == 0)
				LoadDataAndDataBind();
        }

        /// <summary>
        /// Raises the <see cref="E:Init"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		protected override void OnInit(EventArgs e)
		{
			Page.LoadComplete += new EventHandler(Page_LoadComplete);

            MyListView.CurrentListView.PagePropertiesChanged += new EventHandler(CurrentListView_PagePropertiesChanged);
			MyListView.ViewId = Parameters["_v"].ToString();

			base.OnInit(e);
		}

        /// <summary>
        /// Handles the PagePropertiesChanged event of the CurrentListView control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        void CurrentListView_PagePropertiesChanged(object sender, EventArgs e)
        {
            LoadDataAndDataBind();
        }

        /// <summary>
        /// Loads the data and data bind.
        /// </summary>
        private void LoadDataAndDataBind()
        {
			MyListView.CurrentListView.PrimaryKeyId = EcfListView.MakePrimaryKeyIdString("MenuItemId", "MenuId", "IsRoot");

			DataTable dtSource = null;

            if (MenuItemId == 0)
            {
                IDataReader reader = mc.MenuItem.LoadAllRoot(SiteId);
                dtSource = ManagementHelper.ConvertToTable(reader);
                reader.Close();
            }
            else
            {
                DataTable menuTable = mc.MenuItem.LoadSubMenuDT(MenuItemId);
                foreach (DataRow row in menuTable.Rows)
                {
                    int itemId = (int)row["MenuItemId"];
                    using (IDataReader reader = mc.MenuItem.LoadById(itemId, LanguageId))
                    {
                        if (reader.Read())
                            row["Text"] = reader["Text"];
                        else
                            row["Text"] = "<span style='color:red;'>no localized version</span>";

                        reader.Close();
                    }
                }
                menuTable.Columns.Add(new DataColumn("IsDirectory", typeof(bool)));
                using (IDataReader par = mc.MenuItem.LoadParent(MenuItemId))
                {
                    if (par.Read())
                    {
                        DataRow newRow = menuTable.NewRow();
                        newRow["Text"] = "[..]";
                        newRow["MenuItemId"] = (int)par["MenuItemId"];
                        newRow["Order"] = -1;
                        newRow["IsDirectory"] = true;
                        menuTable.Rows.InsertAt(newRow, 0);
                    }
                    par.Close();
                }

                dtSource = menuTable;
            }

			if (dtSource != null)
				MyListView.DataSource = dtSource;

			DataBind();
        }

		/// <summary>
		/// Handles the LoadComplete event of the Page control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		void Page_LoadComplete(object sender, EventArgs e)
		{
			if (IsPostBack &&
				(ManagementHelper.GetBindGridFlag(MyListView.CurrentListView.ID) || String.Compare(Request.Form["__EVENTTARGET"], CommandManager.GetCurrent(this.Page).ID, false) == 0))
			{
				LoadDataAndDataBind();
				MyListView.MainUpdatePanel.Update();
			}
		}
    }
}