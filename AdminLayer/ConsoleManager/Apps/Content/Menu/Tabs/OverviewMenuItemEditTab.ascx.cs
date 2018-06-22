using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Threading;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using mc = Mediachase.Cms;
using Mediachase.Web.Console.BaseClasses;
using Mediachase.Web.Console.Common;
using Mediachase.Web.Console.Interfaces;

namespace Mediachase.Commerce.Manager.Menu.Tabs
{
    public partial class OverviewEditTab : BaseUserControl, IAdminTabControl
    {
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
        /// Gets the menu id.
        /// </summary>
        /// <value>The menu id.</value>
        protected int MenuId
        {
            get
            {
                using (IDataReader item = mc.MenuItem.LoadById(MenuItemId))
                {
                    if (item.Read())
                    {
                        return (int)item["MenuId"];
                    }
                    item.Close();
                }
                return -1;
            }
        }

        #region ResourceExists Property
        /// <summary>
        /// Gets a value indicating whether this instance is resource exists.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is resource exists; otherwise, <c>false</c>.
        /// </value>
        protected bool ResourceExists
        {
            get
            {
                using (IDataReader reader = mc.MenuItem.LoadById(MenuItemId, LanguageId))
                {
                    if (reader.Read())
                    {
                        reader.Close();
                        return true;
                    }
                    reader.Close();
                }
                return false;
            }
        }
        #endregion 

        #region ParentId Property
        /// <summary>
        /// Gets the parent id.
        /// </summary>
        /// <value>The parent id.</value>
        protected int ParentId
        {
            get
            {
                using (IDataReader item = mc.MenuItem.LoadParent(MenuItemId))
                {
                    if (item.Read())
                    {
                        int menuItemId = (int)item["MenuItemId"];
                        item.Close();
                        return menuItemId;
                    }
                    item.Close();
                }
                return -1;
            }
        }
        #endregion

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
        /// Gets the language id.
        /// </summary>
        /// <value>The language id.</value>
        public int LanguageId
        {
            get
            {
                return ManagementHelper.GetIntFromQueryString("LangId");
                /*
                using (IDataReader reader = mc.Language.GetLangByName(Thread.CurrentThread.CurrentCulture.Name))
                {
                    if (reader.Read())
                    {
                        return (int)reader["LangId"];
                    }
                }
                return 1;
                 * */
            }
        }

        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            if(!this.IsPostBack)
                BindForm();

            ddNavigationItems.SelectedIndexChanged += new EventHandler(ddNavigationItems_SelectedIndexChanged);
        }

        protected void ddNavigationItems_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindFields();
            BindParams();
        }

        /// <summary>
        /// Binds the form.
        /// </summary>
        private void BindForm()
        {
            BindDropDownList();
            BindCmdNames();
            BindFields();
            BindParams();
        }

		/// <summary>
		/// Checks if entered name is unique.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="args">The <see cref="System.Web.UI.WebControls.ServerValidateEventArgs"/> instance containing the event data.</param>
		public void NameCheck(object sender, ServerValidateEventArgs args)
		{
			bool itemExists = false;

			using (IDataReader menuItemReader = Mediachase.Cms.MenuItem.LoadSubMenu(Int32.Parse(ParentMenuItem.SelectedValue), LanguageId))
			{
				int menuItemId = 0;
				string name = String.Empty;
				while (menuItemReader.Read())
				{
					menuItemId = (int)menuItemReader["MenuItemId"];
					name = (string)menuItemReader["Text"];
					if ((String.Compare(name, Name.Text, StringComparison.OrdinalIgnoreCase) == 0) && (menuItemId != MenuItemId))
					{
						itemExists = true;
						break;
					}
				}
				menuItemReader.Close();
			}

			args.IsValid = !itemExists;
		}

        /// <summary>
        /// Offsets the specified level.
        /// </summary>
        /// <param name="level">The level.</param>
        /// <returns></returns>
        protected string Offset(int level)
        {
            string ret = "";
            for (int i = 0; i < level; i++)
                ret += "-";
            return ret;
        }

        #region Bind DropDownList
        /// <summary>
        /// Binds the drop down list.
        /// </summary>
        protected void BindDropDownList()
        {
            if (IsNew)
            {
                using (IDataReader items = mc.MenuItem.LoadByMenuId(MenuId, LanguageId))
                {
                    ParentMenuItem.Items.Clear();
                    while (items.Read())
                    {
                        ListItem newItem = new ListItem(Offset((int)items["OutlineLevel"]) + items["Text"].ToString(), items["MenuItemId"].ToString());
                        ParentMenuItem.Items.Insert(ParentMenuItem.Items.Count, newItem);
                    }
                    items.Close();
                }
                using (IDataReader root = Mediachase.Cms.MenuItem.LoadMenuRoot(MenuId))
                {
                    if (root.Read())
                    {
                        ListItem newItem = new ListItem(root["Text"].ToString(), root["MenuItemId"].ToString());
                        ParentMenuItem.Items.Insert(0, newItem);
                    }
                    root.Close();
                }

                
                //ParentMenuItem.SelectedValue = MenuItemId.ToString();
                ManagementHelper.SelectListItem(ParentMenuItem, MenuItemId);
                ParentMenuItem.DataBind();
                
            }
            else
            {
                //gets all childs and current node
                ArrayList child = new ArrayList();
                child.Add(MenuItemId);
                using (IDataReader items = mc.MenuItem.LoadAllChild(MenuItemId))
                {
                    while (items.Read())
                    {
                        child.Add((int)items["MenuItemId"]);
                    }

                    items.Close();
                }

                using (IDataReader items = mc.MenuItem.LoadByMenuId(MenuId, LanguageId))
                {
                    ParentMenuItem.Items.Clear();
                    while (items.Read())
                    {
                        if (!child.Contains((int)items["MenuItemId"]))
                        {
                            ListItem newItem = new ListItem(Offset((int)items["OutlineLevel"]) + items["Text"].ToString(), items["MenuItemId"].ToString());
                            ParentMenuItem.Items.Insert(ParentMenuItem.Items.Count, newItem);
                        }
                    }
                    items.Close();
                }
                using (IDataReader root = Mediachase.Cms.MenuItem.LoadMenuRoot(MenuId))
                {
                    if (root.Read())
                    {
                        ListItem newItem = new ListItem(root["Text"].ToString(), root["MenuItemId"].ToString());
                        ParentMenuItem.Items.Insert(0, newItem);
                    }
                    root.Close();
                }

                if (ParentId != -1)
                {
                    ParentMenuItem.SelectedValue = ParentId.ToString();
                }


                ParentMenuItem.DataBind();
            }
        }
        #endregion

        /// <summary>
        /// Binds the fields.
        /// </summary>
        protected void BindFields()
        {
            string toolTip = String.Empty;
            string text = String.Empty;
            string id = String.Empty;
            string imageUrl = String.Empty;
            bool validation = false;

            rbNone.Checked = true;

            if (!IsNew)
            {
                validation = false;
                using (IDataReader item = mc.MenuItem.LoadById(MenuItemId))
                {
                    if (item.Read())
                    {
                        tbCommand.Text = item["CommandText"].ToString().Trim();
                        int commandType = Int32.Parse(item["CommandType"].ToString());
						if (IsPostBack)
							commandType = GetCommandType(Int32.Parse(item["CommandType"].ToString()));

                        NavigationText.Attributes.Add("style", "display: none");
                        NavigationTitle.Attributes.Add("style", "display: none");

                        switch (commandType)
                        {
                            case 0: rbNone.Checked = true;
                                break;
                            case 1: rbUrl.Checked = true;
                                CommandTitle.Attributes.Add("style", "display: block");
                                CommandText.Attributes.Add("style", "display: block");
                                break;
                            case 2:
                                rbScript.Checked = true;
                                CommandTitle.Attributes.Add("style", "display: block");
                                CommandText.Attributes.Add("style", "display: block");
                                break;
                            case 3:
                                rbNavigation.Checked = true;
                                NavigationText.Attributes.Add("style", "display: block");
                                NavigationTitle.Attributes.Add("style", "display: block");
                                CommandTitle.Attributes.Add("style", "display: none");
                                CommandText.Attributes.Add("style", "display: none");
                                break;
                        }
                        IsVisible.IsSelected = (bool)item["IsVisible"];
						SortOrder.Text = item["Order"].ToString();
                    }

                    item.Close();
                }
                using (IDataReader item = mc.MenuItem.LoadById(MenuItemId, LanguageId))
                {
                    if (item.Read())
                    {
                        text = item["Text"].ToString().Trim();
                        toolTip = item["ToolTip"].ToString().Trim();
                    }
                    item.Close();
                }
                using (IDataReader reader = mc.Language.LoadLanguage(LanguageId))
                {
                    if (reader.Read())
                    {
                        //ImageUrl = Mediachase.Cms.Util.CommonHelper.GetAbsoluteThemedPath("/images/" + reader["LangName"].ToString().Substring(0, 2) + ".gif", Page.Theme);
                        validation = (bool)reader["IsDefault"];
                    }
                    reader.Close();
                }

            }
            else
            {
                //ImageUrl = Mediachase.Cms.Util.CommonHelper.GetAbsoluteThemedPath("/images/" + Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName + ".gif", Page.Theme);
            }

            Name.Text = text;
            if (rbNavigation.Checked && tbCommand.Text.Contains("?"))
            {
                if (!IsPostBack)
                {
                    ddNavigationItems.SelectedValue = tbCommand.Text.Split('^')[0];
                    BindParams();
                }
                string values = tbCommand.Text.Split('?')[1];
                txtValues.Text = string.Empty;
                while (values.Contains("="))
                {
                    string s = string.Empty;
                    if (values.Contains("&"))
                    {
                        s = values.Substring(values.IndexOf('=') + 1, values.IndexOf('&') - values.IndexOf('=') - 1);
                        values = values.Split('&')[1];
                    }
                    else
                    {
                        s = values.Substring(values.IndexOf('=') + 1, values.Length - values.IndexOf('=') - 1);
                        values = "";
                    }
                    txtValues.Text += s + ";";
                }
            }
            ToolTip.Text = toolTip;
            NameValidation.Enabled = true; //Validation;
            /*
            imgFlag.Height = 10;
            imgFlag.BorderWidth = 1;
            imgFlag.Width = 18;
            imgFlag2.Height = 10;
            imgFlag2.Width = 18;
            imgFlag2.BorderWidth = 1;
            imgFlag.ImageUrl = ImageUrl;
            imgFlag2.ImageUrl = ImageUrl;
             * */
        }

        #region Bind Params
        /// <summary>
        /// Binds the params.
        /// </summary>
        private void BindParams()
        {
            if(ddNavigationItems.Items.Count == 0)
                return;

            string text = string.Empty;//"<b>"+Resources.Admin.Parameters + ":</b> <br>";
            using (IDataReader reader = mc.NavigationManager.GetParamsByItemId(Convert.ToInt32(ddNavigationItems.SelectedValue)))
            {
                while (reader.Read())
                {
                    if ((bool)reader["IsRequired"])
                        text += "<b>" + (string)reader["Name"] + "</b><br>";
                    else
                        text += (string)reader["Name"] + "<br>";
                }
                reader.Close();
            }
            divParams.InnerHtml = text;
        }
        #endregion

        /// <summary>
        /// Binds the CMS names.
        /// </summary>
        private void BindCmdNames()
        {
            ddNavigationItems.DataSource = mc.NavigationManager.GetAllItemsDT();
            ddNavigationItems.DataTextField = "ItemName";
            ddNavigationItems.DataValueField = "ItemId";
            ddNavigationItems.DataBind();
            BindParams();
        }

		private int GetCommandType(int defaultValue)
		{
			int cmdType = defaultValue;
			if (rbUrl.Checked)
				cmdType = 1;
			else if (rbScript.Checked)
				cmdType = 2;
			else if (rbNavigation.Checked)
				cmdType = 3;

			return cmdType;
		}

        #region IAdminTabControl Members
        /// <summary>
        /// Saves the changes.
        /// </summary>
        /// <param name="context">The context.</param>
        public void SaveChanges(IDictionary context)
        {
			int sortOrder = Int32.Parse(SortOrder.Text);

            //NEW
            int id = MenuItemId;
			int commandType = GetCommandType(0);
            if (IsNew)
            {
                //add menu item
                if (!rbNavigation.Checked)
					id = mc.MenuItem.Add(MenuId, Name.Text.Trim(), tbCommand.Text, commandType, IsVisible.IsSelected, sortOrder);
                else
					id = mc.MenuItem.Add(MenuId, Name.Text.Trim(), GenerateCommand(), commandType, IsVisible.IsSelected, sortOrder);

                mc.MenuItem.MoveTo(id, Int32.Parse(ParentMenuItem.SelectedValue));
                //add resource
                mc.MenuItem.AddResource(id, LanguageId, Name.Text.Trim(), ToolTip.Text.Trim());
            }
            //OLD
            else
            {
				if (!rbNavigation.Checked)
					mc.MenuItem.Update(MenuItemId, Name.Text, tbCommand.Text, commandType, IsVisible.IsSelected, sortOrder);
				else
					mc.MenuItem.Update(MenuItemId, Name.Text, GenerateCommand(), commandType, IsVisible.IsSelected, sortOrder);

                if (ParentId != Int32.Parse(ParentMenuItem.SelectedValue))
                    mc.MenuItem.MoveTo(MenuItemId, Int32.Parse(ParentMenuItem.SelectedValue));

                if (!ResourceExists)
                    mc.MenuItem.AddResource(id, LanguageId, Name.Text.Trim(), ToolTip.Text.Trim());
                else
                    mc.MenuItem.UpdateResource(MenuItemId, LanguageId, Name.Text.Trim(), ToolTip.Text.Trim());
            }
        }
        #endregion

        #region GenerateCommand
        /// <summary>
        /// Generates the command (for CommandType = 3).
        /// </summary>
        /// <returns></returns>
        private string GenerateCommand()
        {
            string cmdText = string.Empty;
            string cmdValues = txtValues.Text;
            if (cmdValues.EndsWith(";", StringComparison.InvariantCultureIgnoreCase))
                cmdValues = cmdValues.Substring(0, cmdValues.Length - 1);

            using (IDataReader reader = mc.NavigationManager.GetParamsByItemId(Convert.ToInt32(ddNavigationItems.SelectedValue)))
            {
                while (reader.Read())
                {
                    if (cmdText == String.Empty)
                        cmdText += "?" + (string)reader["Name"];
                    else
                        cmdText += "&" + (string)reader["Name"];

                    if (cmdValues.Contains(";"))
                    {
                        cmdText += "=" + cmdValues.Split(';')[0];
                        cmdValues = cmdValues.Split(';')[1];
                    }
                    else
                    {
                        //cmdValues = cmdValues.Replace(";", String.Empty);
                        cmdText += "=" + cmdValues;
                        break;
                    }
                }
                reader.Close();
            }
            return ddNavigationItems.Text + '^' + cmdText;
        }
        #endregion
    }
}