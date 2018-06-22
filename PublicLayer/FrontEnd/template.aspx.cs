using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;
using System.Web.Configuration;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;

using ComponentArt.Web.UI;

using Mediachase.Commerce.Shared;
using Mediachase.Cms;
using Mediachase.Cms.Dto;
using Mediachase.Cms.Managers;
using Mediachase.Cms.Pages;
using Mediachase.Cms.ResourceHandler;
using Mediachase.Cms.Util;
using Mediachase.Web.UI;
using Mediachase.Cms.WebUtility.UI;
using System.Collections.Specialized;
using Mediachase.Commerce.Profile;

namespace Mediachase.Cms.Web
{
    /// <summary>
    /// The main cms page that is responsible for rendering all cms related pages. Contains logic for handling
    /// two distinct views, Design view and Public view. Design view is used by the CMS Content Editor and designers to
    /// create pages and modify the content. The Public view is used to display that content to the end user / customer.
    /// 
    /// Security is checked to protect unauthorized users accessing the management functions. Workflow for publishing content
    /// is also inforced.
    /// </summary>
    public partial class template : PageBase, IMasterPage
    {
        #region UserUID Property
        /// <summary>
        /// Gets the user UID.
        /// </summary>
        /// <value>The user UID.</value>
        public Guid UserUID
        {
            get
            {
                if (this.Page.User.Identity.IsAuthenticated)
                    return (Guid)ProfileContext.Current.User.ProviderUserKey;
                else
                    return Guid.Empty;
            }

        }

        /// <summary>
        /// Gets a value indicating whether this user can edit.
        /// </summary>
        /// <value><c>true</c> if this instance can edit; otherwise, <c>false</c>.</value>
        private bool CanEdit
        {
            get
            {
                if (!Page.User.Identity.IsAuthenticated)
                    return false;

                bool enable = false;
                if (ProfileConfiguration.Instance.EnablePermissions)
                {
                    enable = ProfileContext.Current.CheckPermission("content:site:nav:mng:design");
                }
                else
                {
                    enable = SecurityManager.CheckPermission(new string[] { CmsRoles.EditorRole });
                }

                return enable;
            }
        }

        #endregion

        #region Page_PreInit()
        /// <summary>
        /// Handles the PreInit event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Page_PreInit(object sender, EventArgs e)
        {
            // Apply context
            FillContext();

            // Apply theme
            ApplyTheme();
        }
        #endregion

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Init"></see> event to initialize the page.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"></see> that contains the event data.</param>
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
        }

        #region Page_Load()
        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            // Will only process change template command
            ProcessCommands();

            ToolbarInit();

            LoadTemplate();

            LoadStylesAndScripts();

            SetHeaders();

            EnsureID();
        }

        /// <summary>
        /// Initialize Toolbars.
        /// </summary>
        private void ToolbarInit()
        {
            if (CanEdit)
            {
                DesignDiv.Controls.Add(LoadControl("~/Structure/Base/Controls/ToolBar/ToolBar.ascx"));
                DesignDiv.Controls.Add(LoadControl("~/Structure/Base/Controls/ToolBox/ToolBox.ascx"));
                if (ToolBoxVisible.Value.Trim() == string.Empty)
                    ToolBoxVisible.Value = CMSContext.Current.ToolBoxVisible.ToString();

                if (ToolBarVisible.Value.Trim() == string.Empty)
                    ToolBarVisible.Value = CMSContext.Current.ToolBarVisible.ToString();
            }
            else
            {
            }
        }
        #endregion

        #region Page Render
        /// <summary>
        /// We override Render to swap out the default HtmlTextWriter for our own. Our own Writer's sole purpose is to
        /// change the the action attribute on the form tag to the vanity Url. This way all postbacks occur on the vanity url
        /// </summary>
        /// <param name="writer">The <see cref="T:System.Web.UI.HtmlTextWriter"></see> that receives the page content.</param>
        protected override void Render(HtmlTextWriter writer)
        {
            HttpCookie cookie;
            cookie = Request.Cookies["ToolBoxVisibleCookie"];
            if (cookie != null && !String.IsNullOrEmpty(cookie.Value))
            {
                if (cookie.Value == "True")
                    ToolBoxVisible.Value = cookie.Value;
            }

            if(!CMSContext.Current.IsDesignMode)
                ToolBoxVisible.Value = "False";

            // Hide Design UI if not in design mode
            //HideDesignUI();

            //GA 25.06.2006
            //if (CMSContext.Current.IsDesignMode) form1.Attributes.Add("onSubmit", "recordTbLocation();");
            bool isVisible = false;
            if (CanEdit)
                isVisible = true;

			/*if (isVisible && !ClientScript.IsStartupScriptRegistered("ToolBoxScript"))
				ClientScript.RegisterStartupScript(this.GetType(), "ToolBoxScript", " initToolBox('" + ToolBoxVisible.ClientID + "');", true);
            if (isVisible && !ClientScript.IsClientScriptBlockRegistered("editableWrapper"))
                ClientScript.RegisterClientScriptBlock(this.GetType(), "editableWrapper", "<script type='text/javascript' language='javascript' src='" + Mediachase.Commerce.Shared.CommerceHelper.GetAbsolutePath("/Scripts/editableWrapper.js") + "'></script>");
			*/
            base.Render(writer);

            if (ToolBoxVisible.Value != string.Empty)
            {
                cookie = new HttpCookie("ToolBoxVisibleCookie");
                cookie.Expires = DateTime.MaxValue;
                cookie.Value = ToolBoxVisible.Value;
                Response.Cookies.Add(cookie);
            }
        }
        #endregion

        /// <summary>
        /// Hides non used design elements from being rendered.
        /// </summary>
        private void HideDesignUI()
        {
            DesignDiv.Visible = false;
            SysInfoHolder1.Visible = false;
            DesignScripts.Visible = false;
        }

        #region Page_PreRender
        /// <summary>
        /// Handles the PreRender event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Page_PreRender(object sender, EventArgs e)
        {
            CheckUserAccess();
            
            // Set correct form url
            CmsMainForm.Attributes["action"] = CMSContext.Current.CurrentUrl;
			
        }


        #endregion

        #region CheckUserAccess
        /// <summary>
        /// Checks the user access.
        /// </summary>
        private void CheckUserAccess()
        {
			bool accessGranted = CMSContext.Current.CanAccessPage;

            //REDIRECT TO LOGIN IF NEEDED
            if (!accessGranted && Page.User.Identity.IsAuthenticated)
            {
                Response.Redirect(ResolveUrl("~/accessdenied.aspx"));
            }
            
            if (!accessGranted)
            {
                // can not use "FormsAuthentication.LoginUrl" because it will return full path including ApplicationName, which might be incorrect for a current CMS Site context
                Response.Redirect(ResolveUrl(String.Format("{0}?ReturnUrl={1}", "~/login.aspx", Request.RawUrl.Replace("/", "%2f"))));
            }
            //Response.Redirect(Mediachase.Cms.AppStart.GetAbsolutePath("~/Login.aspx?ReturnUrl=" + Request.RawUrl.Replace("/", "%2f"), Request.ServerVariables["SERVER_PORT"]));
        }
        #endregion

        #region FillContext()
        /// <summary>
        /// Fills the context.
        /// </summary>
        private void FillContext()
        {
            // Reset the DesignMode since we could have saved it in a cookie and logged out since then
            if (CMSContext.Current.IsDesignMode && !CanEdit)
                CMSContext.Current.IsDesignMode = false;

            //SHOW  REQUESTED VERSION
            if (!String.IsNullOrEmpty(Request.QueryString["VersionId"]) && Page.User.Identity.IsAuthenticated)
            {
                bool versionExist = (int.Parse(Request.QueryString["VersionId"]) > 0);

                //Work with exist. version
                if (versionExist)
                {
                    int versionLangId = 1;
                    CMSContext.Current.VersionId = int.Parse(Request.QueryString["VersionId"]);

                    using (IDataReader reader = PageVersion.GetVersionById(CMSContext.Current.VersionId))
                    {
                        if (reader.Read())
                        {
                            CMSContext.Current.TemplateId = (int)reader["TemplateId"];
                            versionLangId = (int)reader["LangId"];
                        }

                        reader.Close();
                    }
                    //set CurrentCulture to version Culture
                    using (IDataReader lang = Language.LoadLanguage(versionLangId))
                    {
                        if (lang.Read())
                        {
                            Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(lang["LangName"].ToString());
                            Thread.CurrentThread.CurrentUICulture = CultureInfo.CreateSpecificCulture(lang["LangName"].ToString());
                        }

                        lang.Close();
                    }
                }
                //Work with temp version
                else
                {
                    CMSContext.Current.VersionId = int.Parse(Request.QueryString["VersionId"]);
                    CMSContext.Current.IsDesignMode = true;
                    if (Request.QueryString["TemplateId"] != null)
                    {
                        CMSContext.Current.TemplateId = int.Parse(Request.QueryString["TemplateId"]);
                    }
                    else
                    {
                        CMSContext.Current.TemplateId = int.Parse(GlobalVariable.GetVariable("default_template", CMSContext.Current.SiteId).ToString());
                    }
                }
            }

            //SHOW PUBLISHED VERSION
            else
            {
                LoadPublishedVersionContext(!CMSContext.Current.IsDesignMode);
            }

            // If we are opening page in design mode, make sure we can actually edit the version
            if (CMSContext.Current.IsDesignMode && CMSContext.Current.VersionId != -2 && !PageHelper.HasLanguageVersion(CMSContext.Current.PageId, LanguageId, CMSContext.Current.VersionId))
            {
                // simply switch design mode off
                CMSContext.Current.IsDesignMode = false;
            }

            // Check current mode and set if toolbar and toolbox is visible
            if (CanEdit)
            {
                // Redirect to version list page, only for users who have appropriate permissions
                if (CMSContext.Current.VersionId == -1)
                {
                    if (CMSContext.Current.IsDesignMode)
                    {
                        LoadPublishedVersionContext(true);
                        _CommandValue.Value = CMSContext.Current.PageId.ToString() + "," + LanguageId.ToString();
                        RunCommand("AddVersion");
                    }
                    //Response.Redirect("~/Structure/Base/Controls/VersionList.aspx?PageId=" + CMSContext.Current.PageId + "&LanguageId=" + LanguageId);
                }

                // Check if any specific tab was clicked
                if (Request.QueryString["tab"] != null && !this.IsPostBack)
                {
                    if (Request.QueryString["tab"] == "design")
                    {
                        CMSContext.Current.IsDesignMode = true;
                        RunCommand("Edit");
                    }
                    else if (Request.QueryString["tab"] == "public")
                    {
                        CMSContext.Current.IsDesignMode = false;
                        RunCommand("Cancel");
                    }
                }
            }
            else
            {
                // Display default language page and show friendly error saying specified language page is not available
                if (CMSContext.Current.VersionId == -1)
                {
                    //Response.Cookies.Clear();
                    if (Response.Cookies["MediachaseCMSCurrentCulture"] != null)
                        Response.Cookies["MediachaseCMSCurrentCulture"].Expires = DateTime.Now.AddDays(-10);

                    if (!String.IsNullOrEmpty(Request.QueryString["_mode"]) && Request.QueryString["_mode"] == "edit")
                    {
                        // can not use "FormsAuthentication.LoginUrl" because it will return full path including ApplicationName, which might be incorrect for a current CMS Site context
                        Response.Redirect(String.Format("{0}?ReturnUrl={1}", ResolveUrl("~/login.aspx"), HttpUtility.UrlEncode(CMSContext.Current.CurrentUrl)));
                    }

					//throw new HttpException(404, String.Empty);
                }
                else if (!String.IsNullOrEmpty(Request.QueryString["_mode"]) && Request.QueryString["_mode"] == "edit")
                {
                    // can not use "FormsAuthentication.LoginUrl" because it will return full path including ApplicationName, which might be incorrect for a current CMS Site context
                    Response.Redirect(String.Format("{0}?ReturnUrl={1}", ResolveUrl("~/login.aspx"), HttpUtility.UrlEncode(CMSContext.Current.CurrentUrl)));
                }


                HideDesignUI();
                CMSContext.Current.IsDesignMode = false;
            }

//            if (!CMSContext.Current.IsDesignMode)
//                HideDesignUI();

            // Initialize current PageDocument
            if (CMSContext.Current.IsDesignMode)
                PageDocument.Current = PageDocument.Open(CMSContext.Current.VersionId, OpenMode.Design, UserUID);
            else
                PageDocument.Current = PageDocument.Open(CMSContext.Current.VersionId, OpenMode.View, UserUID);
        }

        /// <summary>
        /// Sets the theme specified in site settings, if any.
        /// </summary>
        private void ApplyTheme()
        {
            string theme = GlobalVariable.GetVariable("sitetheme", CMSContext.Current.SiteId);
            if (!String.IsNullOrEmpty(theme))
                Page.Theme = theme;
        }

        /// <summary>
        /// Loads the published version context.
        /// </summary>
        /// <param name="loadDefaults">if set to <c>true</c> [load defaults].</param>
        private void LoadPublishedVersionContext(bool loadDefaults)
        {
            //GET PUBLISHED VERSION
            int statusId = WorkflowStatus.GetLast();

            string cacheKey = CmsCache.CreateCacheKey("page-published", CMSContext.Current.PageId.ToString(), statusId.ToString(), loadDefaults.ToString(), Thread.CurrentThread.CurrentCulture.Name);

            // check cache first
            object cachedObject = CmsCache.Get(cacheKey);
            int[] versionArray = null;

            if (cachedObject != null)
                versionArray = (int[])cachedObject;

            // Load the object
            if (versionArray == null)
            {
                lock (CmsCache.GetLock(cacheKey))
                {
                    cachedObject = CmsCache.Get(cacheKey);
                    if (cachedObject != null)
                    {
                        versionArray = (int[])cachedObject;
                    }
                    else
                    {
                        //using (IDataReader reader = PageVersion.GetByLangIdAndStatusId(CMSContext.Current.PageId, LanguageId, statusId))
                        using (IDataReader reader = PageVersion.GetVersionByStatusId(CMSContext.Current.PageId, statusId))
                        {
                            while (reader.Read())
                            {
                                // Load first language that we encounter, possibly should be the default language
                                if (CMSContext.Current.VersionId == -1 && loadDefaults)
                                {
                                    versionArray = new int[] { (int)reader["VersionId"], (int)reader["TemplateId"] };
                                }

                                if ((int)reader["LangId"] == LanguageId)
                                {
                                    versionArray = new int[] { (int)reader["VersionId"], (int)reader["TemplateId"] };
                                    break;
                                }
                            }

                            reader.Close();
                        }

                        CmsCache.Insert(cacheKey, versionArray, CmsConfiguration.Instance.Cache.PageDocumentTimeout);
                    }
                }
            }

            // Populate info from version array
            if (versionArray != null)
            {
                CMSContext.Current.VersionId = versionArray[0];
                CMSContext.Current.TemplateId = versionArray[1];
            }
        }
        #endregion

        #region LoadTemplate()
        /// <summary>
        /// Loads the template.
        /// </summary>
        private void LoadTemplate()
        {
            string templatePath = DictionaryManager.GetTemplatePath(CMSContext.Current.TemplateId);

            if (!templatePath.StartsWith("~"))
                templatePath = "~" + templatePath;

            // Check if template exists first
            if (!File.Exists(Server.MapPath(templatePath)))
            {
                // Generate template file not found exception
                ErrorManager.GenerateError(String.Format("Template file \"{0}\" was not found. Using default template instead.", templatePath));

                // get default template
                CMSContext.Current.TemplateId = int.Parse(GlobalVariable.GetVariable("default_template", CMSContext.Current.SiteId).ToString());
                templatePath = DictionaryManager.GetTemplatePath(CMSContext.Current.TemplateId);
            }

            if (!templatePath.StartsWith("~"))
                templatePath = "~" + templatePath;

            Control ctrl = this.Page.LoadControl(templatePath);
            CMSContext.Current.ControlPlaces = ((Mediachase.Cms.Util.IPublicTemplate)ctrl).ControlPlaces;
            Content.Controls.Add(ctrl);
        }
        #endregion

        #region LoadStylesAndScripts()
        /// <summary>
        /// Loads the styles and scripts.
        /// </summary>
        private void LoadStylesAndScripts()
        {
            //LOAD STYLES AND SCRIPTS
            if (CMSContext.Current.IsDesignMode)
            {
                //REGISTER SCRIPT
                this.Page.ClientScript.RegisterClientScriptBlock(this.GetType(), Guid.NewGuid().ToString(), "<script type='text/javascript' language='javascript' src='" + Mediachase.Commerce.Shared.CommerceHelper.GetAbsolutePath("/Scripts/jQuery.js") + "'></script>");
                /*done in CMS_Scripts.js
				this.Page.ClientScript.RegisterClientScriptBlock(this.GetType(), Guid.NewGuid().ToString(), "<script type='text/javascript' language='javascript' src='" + Mediachase.Commerce.Shared.CommerceHelper.GetAbsolutePath("/Scripts/ActionSet.js") + "'></script>");
				*/
                //APPLY STYLE
                CommonHelper.AddLinkedStyleSheet(this, "~/Styles/snapStyle.css");
                CommonHelper.AddLinkedStyleSheet(this, "~/Styles/ActionSet.css");
                CommonHelper.AddLinkedStyleSheet(this, "~/styles/drop_variation.css");
            }
        }
        #endregion

        #region SetHeaders
        /// <summary>
        /// Sets the headers.
        /// </summary>
        private void SetHeaders()
        {
            string cacheKey = CmsCache.CreateCacheKey("page-headers", CMSContext.Current.PageId.ToString());

            // check cache first
            object cachedObject = CmsCache.Get(cacheKey);
            string[] headersArray = null;

            if (cachedObject != null)
                headersArray = (string[])cachedObject;

            // Load the object
            if (headersArray == null)
            {
                lock (CmsCache.GetLock(cacheKey))
                {
                    cachedObject = CmsCache.Get(cacheKey);
                    if (cachedObject != null)
                    {
                        headersArray = (string[])cachedObject;
                    }
                    else
                    {
                        using (IDataReader reader = PageAttributes.GetByPageId(CMSContext.Current.PageId))
                        {
                            if (reader.Read())
                            {
                                headersArray = new string[] { (string)reader["MetaKeys"], (string)reader["MetaDescriptions"], (string)reader["Title"]};
                                CmsCache.Insert(cacheKey, headersArray, CmsConfiguration.Instance.Cache.PageDocumentTimeout);
                            }

                            reader.Close();
                        }
                    }
                }
            }

            if (headersArray != null)
            {
                MetaKeyWord.Attributes.Add("content", headersArray[0]);
                MetaKeyWord.Attributes.Add("name", "keywords");

                MetaDescription.Attributes.Add("content", headersArray[1]);
                MetaDescription.Attributes.Add("name", "description");

                Head1.Title = headersArray[2];
                Page.Title = headersArray[2];

                // Set footer as well
                PageInclude.Text = GlobalVariable.GetVariable("page_include", CMSContext.Current.SiteId);
            }
        }
        #endregion

        #region IMasterPage Members

        /// <summary>
        /// Gets the get PH template.
        /// </summary>
        /// <value>The get PH template.</value>
        public PlaceHolder GetPHTemplate
        {
            get { return Content; }
        }

        #endregion

        #region TOOLBAR
        #region ToolBarEvent: ToolbarButton_Click
        /// <summary>
        /// Called when [command].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void OnCommand(object sender, EventArgs e)
        {

            HiddenField field = (HiddenField)sender;
            if (field != null)
                RunCommand(field.Value);
        }

        /// <summary>
        /// Processes the commands. This is special case and occurs before the template is loaded. This is done to avoid
        /// viewstate exception that can happen when for instance the template used has changed.
        /// </summary>
        private void ProcessCommands()
        {
            if (!String.IsNullOrEmpty(Request.Form["_CommandField"]))
            {
                if (Request.Form["_CommandField"] == "ChangeTemplate")
                    RunCommand(Request.Form["_CommandField"]);
                else if (Request.Form["_CommandField"] == "Edit")
                    RunCommand(Request.Form["_CommandField"]);
            }
        }

        /// <summary>
        /// Runs the command.
        /// </summary>
        /// <param name="commandName">Name of the command.</param>
        private void RunCommand(string commandName)
        {
            FileSystemResourceProvider prov = (FileSystemResourceProvider)ResourceHandler.ResourceHandler.ResourceProvider;
            {
                //EXIT FROM DESIGN-MODE
                if (commandName == "Cancel")
                {
                    // delete temp pagedocument
					DeleteFromTemp();
					this.ViewState.Clear();

                    CMSContext.Current.IsDesignMode = false;

                    //back to prev page
                    if (Request.QueryString["PrevVersionId"] != null)
                    {
                        NameValueCollection vals = new NameValueCollection();

                        if (Int32.Parse(Request.QueryString["PrevVersionId"]) > 0)
                            vals.Add("VersionId", Request.QueryString["PrevVersionId"]);
                        else
                            vals.Add("VersionId", "");

                        string url = CommonHelper.FormatQueryString(CMSContext.Current.CurrentUrl, vals);

                        Response.Redirect(url);
                        //Response.Redirect("~" + CMSContext.Current.Outline + "?VersionId=" + Request.QueryString["PrevVersionId"]);
                    }
                    else
                        Response.Redirect(Request.RawUrl, true);

                }

                //ENTER DESIGN-MODE
                if (commandName == "Edit")
                {
                    //GA: delete temp pagedocument
                    DeleteFromTemp();

                    // Check if we have access to the current version, if not, the new version for a current language will
                    // need to be created for "edit" command
                    if (!PageHelper.HasLanguageVersion(CMSContext.Current.PageId, LanguageId, CMSContext.Current.VersionId))
                    {
                        // add new version
                        _CommandValue.Value = String.Format("{0},{1}", CMSContext.Current.PageId, LanguageId);
                        RunCommand("AddVersion");
                    }

                    //back to prev page
                    if (Request.QueryString["PrevVersionId"] != null)
                    {
                        NameValueCollection vals = new NameValueCollection();
                        vals.Add("PrevVersionId", Request.QueryString["PrevVersionId"]);
                        string url = CommonHelper.FormatQueryString(CMSContext.Current.CurrentUrl, vals);
                        Response.Redirect(url);
                        //Response.Redirect("~" + CMSContext.Current.Outline + "?VersionId=" + Request.QueryString["PrevVersionId"]);
                    }
                    else
                        Response.Redirect(Request.RawUrl, true);

                }

                //SAVE AS DRAFT
                if (commandName == "SaveDraft")
                {
                    if (CMSContext.Current.VersionId == -2)
                    {
                        CMSContext.Current.VersionId = PageVersion.AddDraft(CMSContext.Current.PageId, CMSContext.Current.TemplateId, LanguageId, (Guid)ProfileContext.Current.User.ProviderUserKey);
                        PageDocument.Current.PageVersionId = CMSContext.Current.VersionId;
                        //copy resources
                        DirectoryInfo resDir = new DirectoryInfo(MapPath("~/" + prov.Archive + "/" + ProfileContext.Current.User.UserName + "/"));
                        CommonHelper.CopyDirectory(resDir.FullName, MapPath("~/" + prov.Archive + "/" + CMSContext.Current.VersionId.ToString() + "/"));
                        if (resDir.Exists)
                            resDir.Delete(true);

                    }
                    PageDocument.Current.IsModified = true;

                    using (IDataReader reader = PageVersion.GetVersionById(CMSContext.Current.VersionId))
                    {
                        if (reader.Read())
                        {
                            int verId = -1;
                            bool addVersion = false;
                            using (IDataReader r = PageVersion.GetVersionById(CMSContext.Current.VersionId))
                            {
                                if (r.Read())
                                {
                                    addVersion = (int)r["StatusId"] != -1;
                                    if (!addVersion)
                                    {
                                        verId = CMSContext.Current.VersionId;
                                        PageVersion.UpdatePageVersion(verId, (int)r["TemplateId"], (int)r["LangId"], -1, WorkflowStatus.DraftId, (Guid)ProfileContext.Current.User.ProviderUserKey, 1, string.Empty);
                                    }
                                }

                                r.Close();
                            }

                            if (addVersion)
                            {
                                verId = PageVersion.AddPageVersion((int)reader["PageId"], (int)reader["TemplateId"], (int)reader["LangId"], (Guid)ProfileContext.Current.User.ProviderUserKey, 1, string.Empty);
                                using (IDataReader reader2 = PageVersion.GetVersionById(verId))
                                {
                                    if (reader2.Read())
                                        PageVersion.UpdatePageVersion(verId, (int)reader2["TemplateId"], (int)reader2["LangId"], (int)reader2["StatusId"], WorkflowStatus.DraftId, (Guid)ProfileContext.Current.User.ProviderUserKey, 1, string.Empty);

                                    reader2.Close();
                                }

                                //copy resources
                                //todo: feature suspended, reimplement in the future
                                //CommonHelper.CopyDirectory(MapPath("~/" + prov.Archive + "/" + CMSContext.Current.VersionId.ToString() + "/"), MapPath("~/" + prov.Archive + "/" + verId.ToString() + "/"));

                                CMSContext.Current.VersionId = verId;
                            }
                        }

                        reader.Close();
                    }
                    //SAVE DRAFT TO PERSIST STORAGE
                    PageDocument.Current.Save(CMSContext.Current.VersionId, SaveMode.PersistentStorage, Guid.Empty);
                    PageDocument.Current.ResetModified();

                    //delete from temporary storage
                    DeleteFromTemp();
                    this.ViewState.Clear();

                    //SWITCH TO VIEW MODE
                    EnableViewMode();

                    RedirectToNewPage();
                }

                //PUBLISH
                if (commandName == "Publish")
                {
					bool firstVersion = false;
                    if (CMSContext.Current.VersionId == -2)
                    {
						firstVersion = true;
                        CMSContext.Current.VersionId = PageVersion.AddDraft(CMSContext.Current.PageId, CMSContext.Current.TemplateId, LanguageId, (Guid)ProfileContext.Current.User.ProviderUserKey);
                        PageDocument.Current.PageVersionId = CMSContext.Current.VersionId;
                        //copy resources
                        DirectoryInfo resDir = new DirectoryInfo(MapPath("~/" + prov.Archive + "/" + ProfileContext.Current.User.UserName + "/"));
                        CommonHelper.CopyDirectory(resDir.FullName, MapPath("~/" + prov.Archive + "/" + CMSContext.Current.VersionId.ToString() + "/"));
                        if (resDir.Exists)
                            resDir.Delete(true);
                    }
                    using (IDataReader reader = PageVersion.GetVersionById(CMSContext.Current.VersionId))
                    {
                        if (reader.Read())
                        {
                            int langId = (int)reader["LangId"];
                            if (WorkflowStatus.GetNext((int)reader["StatusId"]) != -1 || (int)reader["StatusId"] == -1)
                                PageVersion.UpdatePageVersion(CMSContext.Current.VersionId, (int)reader["TemplateId"], (int)reader["LangId"], (int)reader["StatusId"], WorkflowAccess.GetMaxStatus(Roles.GetRolesForUser(), WorkflowStatus.GetLast((int)reader["StatusId"])), (Guid)ProfileContext.Current.User.ProviderUserKey, 1, string.Empty);

                            // if we publish version
                            if (WorkflowStatus.GetLast((int)reader["StatusId"]) == WorkflowAccess.GetMaxStatus(Roles.GetRolesForUser(), WorkflowStatus.GetLast((int)reader["StatusId"])))
                            {
                                //find old published and put to archive
                                using (IDataReader reader2 = PageVersion.GetVersionByStatusId((int)reader["PageId"], WorkflowStatus.GetLast((int)reader["StatusId"])))
                                {
                                    while (reader2.Read())
                                    {
                                        if ((int)reader2["LangId"] == langId)
                                        {
                                            if (CMSContext.Current.VersionId != (int)reader2["VersionId"])
                                                PageVersion.UpdatePageVersion((int)reader2["VersionId"], (int)reader2["TemplateId"], (int)reader2["LangId"], (int)reader2["StatusId"], WorkflowStatus.GetArcStatus((int)reader2["StatusId"]), (Guid)ProfileContext.Current.User.ProviderUserKey, 2, "sent to archieve");
                                        }
                                    }

                                    reader2.Close();
                                }
                            }
                        }
                        reader.Close();
                    }

                    //SAVE TO PERSIST STORAGE
                    PageDocument.Current.Save(CMSContext.Current.VersionId, SaveMode.PersistentStorage, Guid.Empty);
                    PageDocument.Current.ResetModified();

                    //delete from temporary storage
                    DeleteFromTemp();
                    this.ViewState.Clear();

                    //SWITCH TO VIEW MODE
                    EnableViewMode();

					// replace VersionId in query string with a new id
					string url = Request.RawUrl;
					if (firstVersion)
					{
						// if the page has just been created and is being published, need to remove "VersionId=-2" from the queryString to load published version
						NameValueCollection vals = new NameValueCollection();
						vals.Add("VersionId", String.Empty);
						url = CommonHelper.FormatQueryString(url, vals);
					}
                    Response.Redirect(url, true);
                }

                if (commandName == "Approve")
                {
                    using (IDataReader reader = PageVersion.GetVersionById(CMSContext.Current.VersionId))
                    {
                        if (reader.Read())
                        {
                            PageVersion.UpdatePageVersion(CMSContext.Current.VersionId, (int)reader["TemplateId"], (int)reader["LangId"], (int)reader["StatusId"], WorkflowAccess.GetMaxStatus(Roles.GetRolesForUser(), WorkflowStatus.GetLast((int)reader["StatusId"])), (Guid)ProfileContext.Current.User.ProviderUserKey, 1, /*ddText.TextValue*/"");
                            int langId = (int)reader["langId"];

                            // if we publish version
                            if (WorkflowStatus.GetLast((int)reader["StatusId"]) == WorkflowAccess.GetMaxStatus(Roles.GetRolesForUser(), WorkflowStatus.GetLast((int)reader["StatusId"])))
                            {
                                //find old publishd and put to archive
                                using (IDataReader reader2 = PageVersion.GetVersionByStatusId((int)reader["PageId"], WorkflowStatus.GetLast((int)reader["StatusId"])))
                                {
                                    while (reader2.Read())
                                    {
                                        if (langId == (int)reader2["LangId"])
                                        {
                                            if (CMSContext.Current.VersionId != (int)reader2["VersionId"])
                                                PageVersion.UpdatePageVersion((int)reader2["VersionId"], (int)reader2["TemplateId"], (int)reader2["LangId"], (int)reader2["StatusId"], WorkflowStatus.GetArcStatus((int)reader2["StatusId"]), (Guid)ProfileContext.Current.User.ProviderUserKey, 1, "Archieved");
                                        }
                                    }

                                    reader2.Close();
                                }
                            }
                            //PageVersion.DeletePageVersion(CMSContext.Current.VersionId);
                            //PageVersion.AddPageVerion((int)reader["PageId"], (int)reader["TemplateId"], (int)reader["LangId"], (Guid)Membership.GetUser(Page.User.Identity.Name).ProviderUserKey, 1, ddText.TextValue);
                        }

                        reader.Close();
                    }

                    Response.Redirect(Request.RawUrl, true);
                    //RedirectToNewPage();
              }

                if (commandName == "Deny")
                {
                    using (IDataReader reader = PageVersion.GetVersionById(CMSContext.Current.VersionId))
                    {
                        if (reader.Read())
                        {
                            PageVersion.UpdatePageVersion(CMSContext.Current.VersionId, (int)reader["TemplateId"], (int)reader["LangId"], (int)reader["StatusId"], WorkflowStatus.GetPrevious((int)reader["StatusId"]), (Guid)ProfileContext.Current.User.ProviderUserKey, 2, /*ddText.TextValue*/"");
                            //PageVersion.AddPageVerion((int)reader["PageId"], (int)reader["TemplateId"], (int)reader["LangId"], (Guid)Membership.GetUser(Page.User.Identity.Name).ProviderUserKey, 1, ddText.TextValue);
                        }

                        reader.Close();
                    }
                    string url = Page.Request.Url.ToString();

                    if (!url.Contains("VersionId"))
                    {
                        NameValueCollection vals = new NameValueCollection();
                        vals.Add("VersionId", CMSContext.Current.VersionId.ToString());
                        url = CommonHelper.FormatQueryString(url, vals);
                        Response.Redirect(url);
                    }
                }

                if (commandName == "ChangeTemplate")
                {
                    // Clear cache
                    CmsCache.Clear();

                    CMSContext mcContext = CMSContext.Current;
                    using (IDataReader reader = PageVersion.GetVersionById(CMSContext.Current.VersionId))
                    {
                        if (reader.Read())
                        {
                            int pageTemplateId;

                            if (Int32.TryParse(_CommandValue.Value, out pageTemplateId))
                                PageVersion.UpdatePageVersion(CMSContext.Current.VersionId, int.Parse(_CommandValue.Value), this.LanguageId, (int)reader["StatusId"], (int)reader["StatusId"], (Guid)ProfileContext.Current.User.ProviderUserKey, 1, string.Empty);
                        }

                        reader.Close();
                    }

                    if (mcContext.VersionId == -2)
                    {
                        NameValueCollection vals = new NameValueCollection();
                        vals.Add("TemplateId", _CommandValue.Value);
                        string url = CommonHelper.FormatQueryString(Request.RawUrl, vals);
                        Response.Redirect(url, true);
                    }
                    else
                    {
                        Response.Redirect(Request.RawUrl, true);
                    }

                }

                if (commandName == "AddVersion")
                {
                    string separator = ",";
                    int pageId = int.Parse(_CommandValue.Value.Split(separator.ToCharArray())[0]);
                    int langId = int.Parse(_CommandValue.Value.Split(separator.ToCharArray())[1]);
                    string currentCulture = "en-us";
                    using (IDataReader reader = Language.LoadLanguage(langId))
                    {
                        if (reader.Read())
                        {
                            currentCulture = (string)reader["LangName"];
                        }

                        reader.Close();
                    }

                    NameValueCollection vals = new NameValueCollection();
                    vals.Add("lang", currentCulture);
                    vals.Add("VersionId", "-2");
                    vals.Add("PrevVersionId", CMSContext.Current.VersionId.ToString());
                    string url = CommonHelper.FormatQueryString(CMSContext.Current.CurrentUrl, vals);
                    Response.Redirect(url);
                }               
                return;
            }
        }

        #region RedirectToNewPage()
        /// <summary>
        /// Redirects to new page.
        /// </summary>
        private void RedirectToNewPage()
        {
            NameValueCollection vals = new NameValueCollection();
            vals.Add("VersionId", CMSContext.Current.VersionId.ToString());
            string url = CommonHelper.FormatQueryString(CMSContext.Current.CurrentUrl, vals);
            Response.Redirect(url);
        }
        #endregion

        #region DeleteFromTemp()
        /// <summary>
        /// Deletes from temp.
        /// </summary>
        private static void DeleteFromTemp()
        {
            if (PageDocument.Current != null)
            {
                PageDocument.Current.Delete(PageDocument.Current.PageVersionId, (Guid)ProfileContext.Current.User.ProviderUserKey, DeleteMode.TemporaryStorage);
                PageDocument.Current.Delete(CMSContext.Current.VersionId, (Guid)ProfileContext.Current.User.ProviderUserKey, DeleteMode.TemporaryStorage);
                PageDocument.Current.Delete(-2, (Guid)ProfileContext.Current.User.ProviderUserKey, DeleteMode.TemporaryStorage);
                PageDocument.Current.ResetModified();
            }

            // Clear cache
            CmsCache.Clear();
        }
        #endregion

        #region EnableViewMode()
        /// <summary>
        /// Enables the view mode.
        /// </summary>
        private void EnableViewMode()
        {
            HttpCookie cookie = new HttpCookie("MediachaseCMSDesign");
            cookie.Expires = DateTime.MaxValue;
            cookie.Value = "False";
            Response.Cookies.Add(cookie);
        }
        #endregion

        #endregion

        #endregion
		
	}
}
