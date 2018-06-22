using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;

using Mediachase.Cms;
using Mediachase.Web.Console.BaseClasses;
using Mediachase.Web.Console.Common;
using Mediachase.Cms.Managers;
using Mediachase.Cms.Dto;
using Mediachase.Commerce.Profile;

namespace Mediachase.Commerce.Manager.Content.Folders
{
    public partial class LanguageMenu : BaseUserControl
    {
        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        #region PageId
        private int _pageId;
        /// <summary>
        /// Gets or sets the page id.
        /// </summary>
        /// <value>The page id.</value>
        public int PageId
        {
            get { return _pageId; }
            set { _pageId = value; }
        }
        #endregion

        private Guid _siteId;
        /// <summary>
        /// Gets or sets the site id.
        /// </summary>
        /// <value>The site id.</value>
        public Guid SiteId
        {
            get { return _siteId; }
            set { _siteId = value; }
        }

        #region PageOutline
        /// <summary>
        /// Gets the page outline.
        /// </summary>
        /// <value>The page outline.</value>
        public string PageOutline
        {
            get
            {
                using (IDataReader reader = FileTreeItem.GetItemById(PageId))
                {
                    if (reader.Read())
                    {
                        string outline = reader["Outline"].ToString();
                        reader.Close();
                        return outline;
                    }
                    reader.Close();
                }
                return "/";
            }
        }
        #endregion

        /// <summary>
        /// Binds a data source to the invoked server control and all its child controls.
        /// </summary>
        public override void DataBind()
        {
            base.DataBind();
            BindMenu();
        }

        #region BindMenu()
        /// <summary>
        /// Binds the menu.
        /// </summary>
        public void BindMenu()
        {
			//bool added = false;

            trLanguageMenu.Cells.Clear();

			// add only languages available for the site
			SiteDto siteDto = SiteManager.GetSite(SiteId);
			if (siteDto != null && siteDto.SiteLanguage.Count > 0)
			{
				EnumerableRowCollection<SiteDto.SiteLanguageRow> siteLangRows = siteDto.SiteLanguage.AsEnumerable();
				using (IDataReader reader = Language.GetAllLanguages())
				{
					while (reader.Read())
					{
						//added = true;
						string langName = reader["LangName"].ToString();
						var query = from row in siteLangRows
									where String.Compare(row.Field<string>("LanguageCode"), langName) == 0
									select row;

						// if language is available, add it to the menu
						if (query.Count() > 0)
							CreateLanguageItem(reader["LangName"].ToString(), (int)reader["LangId"]);
					}
                    reader.Close();
				}
			}

            //added = added;

			//if (!added)
			//{
			//    // if no controls have been added on the previous step, add an empty cell
			//    TableCell tc = new TableCell();
			//    tc.Text = "&nbsp;";
			//    trLanguageMenu.Cells.Add(tc);
			//}
        }
        #endregion

        #region CreateLanguageItem()
        /// <summary>
        /// Creates the language item.
        /// </summary>
        /// <param name="langName">Name of the lang.</param>
        /// <param name="langId">The lang id.</param>
        private void CreateLanguageItem(string langName, int langId)
        {
            if (ProfileContext.Current.CheckPermission("content:site:nav:mng:design"))
            {
                CreateFlagIcon(langName, langId);
                CreateMenu(langId);
            }
        }
        #endregion

        #region CreateMenu()
        /// <summary>
        /// Creates the menu.
        /// </summary>
        /// <param name="langId">The lang id.</param>
        private void CreateMenu(int langId)
        {
            //create menu
            TableCell cellMenu = new TableCell();
            //cellMenu.Style.Add("border-left", "solid 1px silver");
            cellMenu.Style.Add("width", "12px");
            System.Web.UI.WebControls.Menu menuLang = new System.Web.UI.WebControls.Menu();
            //set style
            menuLang.StaticMenuItemStyle.CssClass = menuPattern.StaticMenuItemStyle.CssClass;
            menuLang.DynamicMenuStyle.CssClass = menuPattern.DynamicMenuStyle.CssClass;
            //set templates
            //menuLang.StaticItemTemplate = menuPattern.StaticItemTemplate;
            menuLang.DynamicItemTemplate = menuPattern.DynamicItemTemplate;
            //disable default popup image
            menuLang.StaticEnableDefaultPopOutImage = false;
            //set orientation
            menuLang.Orientation = Orientation.Horizontal;
            System.Web.UI.WebControls.MenuItem root = new System.Web.UI.WebControls.MenuItem();
            root.Selectable = false;
            menuLang.Items.Add(root);

            //add other version
            //get archive status id
            int archiveStatusId = WorkflowStatus.GetArcStatus(0);
            //get allowed statusId
            ArrayList allowedStatusId = WorkflowAccess.LoadListByRoleId(Membership.GetUser().ProviderUserKey.ToString());
            using (IDataReader reader = PageVersion.GetVersionByLangId(PageId, langId))
            {
                while (reader.Read())
                {
                    int statusId = (int)reader["StatusId"];
                    string statusName = string.Empty;
                    using (IDataReader status = WorkflowStatus.LoadById(statusId))
                    {
                        if (status.Read())
                        {
                            if (statusId != archiveStatusId && allowedStatusId.Contains(statusId))
                            {
                                statusName = status["FriendlyName"].ToString();
                            }
                        }
                        status.Close();
                    }

                    //add user draft
                    Guid UserKey = (Guid)Membership.GetUser().ProviderUserKey;
                    Guid OwnerKey = new Guid(reader["EditorUID"].ToString());
                    if (statusId == WorkflowStatus.DraftId && UserKey == OwnerKey)
                    {
                        statusName = "draft";
                    }
                    //TODO: for refactoring
                    if (statusName != string.Empty)
                    {
                        System.Web.UI.WebControls.MenuItem newItem = new System.Web.UI.WebControls.MenuItem();
                        newItem.Text = "Version #" + reader["VersionId"] + "(" + statusName + ")";
                        newItem.Target = "_blank";
                        newItem.NavigateUrl = "http://" + Mediachase.Cms.GlobalVariable.GetVariable("url", SiteId) + PageOutline + "?VersionId=" + reader["VersionId"].ToString() +
                                              "&UserId=" + Membership.GetUser().ProviderUserKey.ToString();
                        root.ChildItems.Add(newItem);
                    }
                }
                reader.Close();
            }


			if (root.ChildItems.Count > 0)
			{
				//add menu to cell
				cellMenu.Controls.Add(menuLang);
			}
			
            //add cell to row
            trLanguageMenu.Cells.Add(cellMenu);
        }
        #endregion

        #region CreateFlagIcon()
        /// <summary>
        /// Creates the flag icon.
        /// </summary>
        /// <param name="langName">Name of the lang.</param>
        /// <param name="langId">The lang id.</param>
        private void CreateFlagIcon(string langName, int langId)
        {
            //create flag cell
            TableCell cellFlag = new TableCell();
            //create image
            Image imgFlag = new Image();
            imgFlag.Width = 18;
            imgFlag.Height = 12;
            imgFlag.BorderColor = System.Drawing.Color.Gray;
            imgFlag.BorderWidth = 1;
            //create hyperlink

            HyperLink hlFlag = new HyperLink();
            hlFlag.NavigateUrl = (Mediachase.Cms.GlobalVariable.GetVariable("url", SiteId).StartsWith("http://")) ? "" : "http://";
            CultureInfo culture = CultureInfo.CreateSpecificCulture(langName);
            if (PageHelper.HasLanguageVersion(PageId, langId))
            {
                imgFlag.ImageUrl = ManagementHelper.GetFlagIcon(CultureInfo.CreateSpecificCulture(langName));
                hlFlag.NavigateUrl += "javascript:CSManagementClient.OpenExternal('"+Mediachase.Cms.GlobalVariable.GetVariable("url", SiteId) + PageOutline + "?CurrentCulture=" + langName + "&_mode=edit" + "');";// +"&UserId=" + Membership.GetUser().ProviderUserKey.ToString();
                hlFlag.ToolTip = String.Format("{0} [{1}]", Resources.Admin.ViewPage, culture.DisplayName);
            }
            else
            {
                hlFlag.Attributes.Add("onclick", "return confirm('" + Resources.Admin.IsCreatePage + "');");
                hlFlag.ToolTip = String.Format("{0} [{1}]", Resources.Admin.CreatePage, culture.DisplayName);
                imgFlag.ImageUrl = ManagementHelper.GetFlagIcon(CultureInfo.CreateSpecificCulture(langName));
                imgFlag.CssClass = "DisabledIcon";
                hlFlag.NavigateUrl += "javascript:CSManagementClient.OpenExternal('" + Mediachase.Cms.GlobalVariable.GetVariable("url", SiteId) + PageOutline + "?VersionId=-2&CurrentCulture=" + langName + "&_mode=edit" + "');";// "&UserId=" + Membership.GetUser().ProviderUserKey.ToString() + "&CloseWindow=true";
            }
            //add image to hyperlink
            hlFlag.Controls.Add(imgFlag);
            //add flag to cell
            cellFlag.Controls.Add(hlFlag);
            //add cell to row
            trLanguageMenu.Cells.Add(cellFlag);
        }
        #endregion
    }
}
