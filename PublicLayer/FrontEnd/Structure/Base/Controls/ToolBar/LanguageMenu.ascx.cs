using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;

using Mediachase.Cms;
using Mediachase.Web.UI;
using Mediachase.Cms.Util;
using System.Collections.Specialized;
using Mediachase.Commerce.Profile;

public partial class Controls_ToolBar_LanguageMenu : BaseLocalizableControl
{
    /// <summary>
    /// Handles the Load event of the Page control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    protected void Page_Load(object sender, EventArgs e)
    {
        if (ProfileContext.Current.User != null)
            BindMenu();
    }

    #region PageId
    /// <summary>
    /// Gets the page id.
    /// </summary>
    /// <value>The page id.</value>
    public int PageId
    {
        get { return CMSContext.Current.PageId; }
    }
    #endregion

    #region PageOutline
    /// <summary>
    /// Gets the page outline.
    /// </summary>
    /// <value>The page outline.</value>
    public string PageOutline
    {
        get
        {
            return CMSContext.Current.Outline;
        }
    }
    #endregion

    #region BindMenu()
    /// <summary>
    /// Binds the menu.
    /// </summary>
    private void BindMenu()
    {
        trLanguageMenu.Cells.Clear();
        //bind default language
        string langName = LanguageName;
        int langId = LanguageId;
        CreateLanguageItem(langName, langId);
        //bind other language
        using (IDataReader reader = Language.GetAllLanguages())
        {
            while (reader.Read())
            {
                if ((int)reader["LangId"] != LanguageId)
                {
                    CreateLanguageItem(reader["LangName"].ToString(), (int)reader["LangId"]);
                }
            }

            reader.Close();
        }
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
        CreateFlagIcon(langName, langId);

        CreateMenu(langName, langId);
    }
    #endregion

    #region CreateMenu()
    /// <summary>
    /// Creates the menu.
    /// </summary>
    /// <param name="langName">Name of the lang.</param>
    /// <param name="langId">The lang id.</param>
    private void CreateMenu(string langName, int langId)
    {
        //create menu
        TableCell cellMenu = new TableCell();
        cellMenu.Style.Add("border-left", "solid 1px silver");
        cellMenu.Style.Add("width", "12px");
        System.Web.UI.WebControls.Menu menuLang = new System.Web.UI.WebControls.Menu();
        //set style
        menuLang.StaticMenuItemStyle.CssClass = menuPattern.StaticMenuItemStyle.CssClass;
        menuLang.DynamicMenuStyle.CssClass = menuPattern.DynamicMenuStyle.CssClass;
        //set templates
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
        ArrayList allowedStatusId = WorkflowAccess.LoadListByRoleId(ProfileContext.Current.User.ProviderUserKey.ToString());
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
                Guid UserKey = (Guid)ProfileContext.Current.User.ProviderUserKey;
                Guid OwnerKey = new Guid(reader["EditorUID"].ToString());
                if (statusId == WorkflowStatus.DraftId && UserKey == OwnerKey)
                {
                    statusName = "draft";
                }
                if (statusName != string.Empty)
                {
                    System.Web.UI.WebControls.MenuItem newItem = new System.Web.UI.WebControls.MenuItem();
                    newItem.Text = "Version #" + reader["VersionId"] + "(" + statusName + ")";


                    NameValueCollection vals = new NameValueCollection();
                    vals.Add("lang", langName);
                    vals.Add("VersionId", reader["VersionId"].ToString());
                    newItem.NavigateUrl = CommonHelper.FormatQueryString(CMSContext.Current.CurrentUrl, vals);
                    /*
                    newItem.NavigateUrl = "~" + PageOutline + "?VersionId=" + reader["VersionId"].ToString() +
                                          "&UserId=" + Membership.GetUser().ProviderUserKey.ToString() +
                                          "&lang=" + langName;
                     * */
                    root.ChildItems.Add(newItem);
                }
            }

            reader.Close();
        }


        if (root.ChildItems.Count > 0)
        {
            //add menu to cell
            cellMenu.Controls.Add(menuLang);
            //add cell to row
            trLanguageMenu.Cells.Add(cellMenu);
        }
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
        CultureInfo culture = CultureInfo.CreateSpecificCulture(langName);
        if (PageHelper.HasLanguageVersion(PageId, langId))
        {
            //create image
            Image imgFlag = new Image();
            imgFlag.ImageUrl = CommonHelper.GetFlagIcon(culture);
            //imgFlag.ImageUrl = "~/images/flags/" + CultureInfo.CreateSpecificCulture(langName).TwoLetterISOLanguageName + ".gif";
            imgFlag.Width = 18;
            imgFlag.AlternateText = culture.DisplayName;
            imgFlag.Height = 12;
            if (langId == LanguageId)
            {
                imgFlag.BorderColor = System.Drawing.Color.Gold;
                imgFlag.BorderWidth = 1;
            }
            else
            {
                imgFlag.BorderColor = System.Drawing.Color.White;
                imgFlag.BorderWidth = 1;
            }
            //create hyperlink
            HyperLink hlFlag = new HyperLink();

            NameValueCollection vals = new NameValueCollection();
            vals.Add("lang", langName);

            string navigateUrl = CMSContext.Current.CurrentUrl;
            if (navigateUrl.Contains("?"))
            {
                navigateUrl = navigateUrl.Substring(0, navigateUrl.IndexOf("?"));
            }

            hlFlag.NavigateUrl = CommonHelper.FormatQueryString(navigateUrl, vals);

            //hlFlag.NavigateUrl = "~" + PageOutline + "?lang=" + langName;// +"&UserId=" + Membership.GetUser().ProviderUserKey.ToString();
            //add image to hyperlink
            hlFlag.Controls.Add(imgFlag);
            //add flag to cell
            cellFlag.Controls.Add(hlFlag);
        }
        else
        {
            //create imagebutton
            ImageButton ibtnFlag = new ImageButton();
            ibtnFlag.ImageUrl = CommonHelper.GetFlagIcon(culture);
            ibtnFlag.AlternateText = culture.DisplayName;
            //ibtnFlag.ImageUrl = "~/images/flags/" + CultureInfo.CreateSpecificCulture(langName).TwoLetterISOLanguageName + "_gray.gif";
            ibtnFlag.Width = 18;
            ibtnFlag.Height = 12;
            ibtnFlag.CssClass = "cms-disabled";
            ibtnFlag.BorderColor = System.Drawing.Color.White;
            ibtnFlag.BorderWidth = 1;
            string onclickString = "if(confirm('Do you want to create page for this language?')){RunCommand('AddVersion', '" + PageId.ToString() + "," + langId.ToString() + "');}return false;";
            ibtnFlag.Attributes.Add("onclick", onclickString);
            //add commend argument
            //ibtnFlag.CommandArgument = PageId.ToString() + "," + langId.ToString();
            //ibtnFlag.CommandName = "AddVersion";
            //add handler
            //ibtnFlag.Click += new ImageClickEventHandler(ibtnFlag_Click);
            //add button to cell
            cellFlag.Controls.Add(ibtnFlag);
        }
        //add cell to row
        trLanguageMenu.Cells.Add(cellFlag);
    }
    #endregion

    #region Event Handlers
    /// <summary>
    /// Handles the Click event of the ibtnFlag control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.Web.UI.ImageClickEventArgs"/> instance containing the event data.</param>
    void ibtnFlag_Click(object sender, ImageClickEventArgs e)
    {
        string arg = ((ImageButton)sender).CommandArgument;
        string separator = ",";
        int pageId = int.Parse(arg.Split(separator.ToCharArray())[0]);
        int langId = int.Parse(arg.Split(separator.ToCharArray())[1]);
        string CurrentCulture = "en-us";
        using (IDataReader reader = Language.LoadLanguage(langId))
        {
            if (reader.Read())
            {
                CurrentCulture = (string)reader["LangName"];
            }

            reader.Close();
        }
        Response.Redirect(ResolveUrl(String.Format("~/{0}?VersionId=-2&lang={1}&PrevVersionId={2}", CMSContext.Current.Outline, CurrentCulture, CMSContext.Current.VersionId)));
    }
    #endregion
}
