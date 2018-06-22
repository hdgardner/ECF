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
using Mediachase.Web.Console.Config;
using Mediachase.Web.Console;
using System.IO;
using Mediachase.Web.Console.Interfaces;
using System.Collections.Specialized;
using Mediachase.Web.Console.BaseClasses;
using Mediachase.Commerce.Profile;
using Mediachase.Web.Console.Common;

public partial class ContentFrame : BasePage
{
    /// <summary>
    /// Gets the view id.
    /// </summary>
    /// <value>The view id.</value>
    public string ViewId
    {
        get
        {
            if (!String.IsNullOrEmpty(this.Request.QueryString["_v"]))
                return this.Request.QueryString["_v"].ToString();

            return String.Empty;
        }
    }

    /// <summary>
    /// Gets the app id.
    /// </summary>
    /// <value>The app id.</value>
    public string AppId
    {
        get
        {
            if (!String.IsNullOrEmpty(this.Request.QueryString["_a"]))
                return this.Request.QueryString["_a"].ToString();

            return String.Empty;
        }
    }

    /// <summary>
    /// Handles the Load event of the Page control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    protected void Page_Load(object sender, EventArgs e)
    {
        BindScripts();
		ManagementHelper.RegisterBrowserStyles(this);

        if (!this.IsPostBack && !IsCallback) // determine if the request is call back
        {            
            //DataBind();
        }
    }

    private bool _isCallback = false;
    private bool _isCallbackProcessed = false;
    /// <summary>
    /// Gets a value indicating whether the page request is the result of a call back.
    /// </summary>
    /// <value></value>
    /// <returns>true if the page request is the result of a call back; otherwise, false.</returns>
    public new bool IsCallback
    {
        get
        {
            if (!_isCallbackProcessed)
            {
                if (Context != null && Context.Request != null)
                {
                    foreach (string key in Context.Request.Params.AllKeys)
                    {
                        if (key != null && key.StartsWith("Cart_") && key.IndexOf("_Callback") > 0)
                        {
                            _isCallback = true;
                            _isCallbackProcessed = true;
                            break;
                        }
                    }
                }

                _isCallbackProcessed = true;
            }

            return _isCallback;
        }
    }

    /// <summary>
    /// Processes the action events.
    /// </summary>
    private void ProcessActionEvents()
    {
        string action = Request.Form["_action"];

        if (String.IsNullOrEmpty(action))
            return;

        // Generate event
        ActionManager.GenerateAction(action, Request.Form["_params"]);
    }

    /// <summary>
    /// Binds the scripts.
    /// </summary>
    private void BindScripts()
    {
		Page.ClientScript.RegisterClientScriptInclude(this.Page.GetType(), "ExtBase", ResolveClientUrl("~/Scripts/ext/ext-base.js"));
		Page.ClientScript.RegisterClientScriptInclude(this.Page.GetType(), "ExtAll", ResolveClientUrl("~/Scripts/ext/ext-all.js"));
		Page.ClientScript.RegisterClientScriptInclude(this.Page.GetType(), "ManagementClientProxy", ResolveClientUrl("~/Scripts/ManagementClientProxy.js"));

        foreach (ModuleConfig module in ManagementContext.Current.Configs)
        {
            string url = String.Format("~/Apps/{0}/Scripts", module.Name);
            string path = Server.MapPath(url);
            if (Directory.Exists(path))
            {
                foreach (FileInfo file in new DirectoryInfo(path).GetFiles())
                {
                    this.ClientScript.RegisterClientScriptInclude(file.Name, this.ResolveClientUrl(String.Format("{0}/{1}", url, file.Name)));
                }
            }
        }
    }

    /// <summary>
    /// Changes the view.
    /// </summary>
    /// <param name="appId">The app id.</param>
    /// <param name="newViewId">The new view id.</param>
    private void ChangeView(string appId, string newViewId)
    {
        if (newViewId.Length == 0)
        {
            RedirectToSavedState();
            return;
        }
        
        Control contentCtrl = IbnMainLayout.FindControl("ContentHolderControl");

        if (contentCtrl == null)
            return;

        contentCtrl.Controls.Clear();

        AdminView view = ManagementContext.Current.FindView(appId, newViewId);
		if (view != null)
		{
            // Check view permissions
            if (view.Attributes.Contains("permissions"))
            {
                if (!ProfileContext.Current.CheckPermission(view.Attributes["permissions"].ToString()))
                    throw new UnauthorizedAccessException("Current user does not have enough rights to access the requested operation.");
            }

			string controlUrl = String.Format("~/Apps/{0}", view.ControlUrl);
			if (File.Exists(Server.MapPath(controlUrl)))
			{
				Control ctrl = this.LoadControl(controlUrl);
				contentCtrl.Controls.Add(ctrl);
			}

			//CenterContentPanel.Update();
			if (!this.IsPostBack && (!this.Request.QueryString.ToString().Contains("Callback=yes")))
				PersistNavigationState();
		}
		else
		{
			Mediachase.Web.Console.BaseClasses.ErrorManager.GenerateError(String.Format("View \"{0}\" in application \"{1}\" not found.", newViewId, appId));
		}
    }

    /// <summary>
    /// Redirects the state of to saved.
    /// </summary>
    void RedirectToSavedState()
    {
        //if (!String.IsNullOrEmpty(Profile.NavigationState.SelectedMenuId))
        //    Response.Redirect(String.Format("~/Apps/Shell/Pages/ContentFrame.aspx?_a={0}&_v={1}&{2}", Profile.NavigationState.SelectedMenuId, Profile.NavigationState.SelectedContentNameId, Profile.NavigationState.QueryString));
    }


    /// <summary>
    /// Persists the state of the navigation.
    /// </summary>
    private void PersistNavigationState()
    {
        /*
        string menuId = String.Empty;
        if (!String.IsNullOrEmpty(AppId))
            Profile.NavigationState.SelectedMenuId = AppId;

        string viewId = String.Empty;
        if (!String.IsNullOrEmpty(ViewId))
            Profile.NavigationState.SelectedContentNameId = ViewId;

        string queryString = String.Empty;
        
        NameValueCollection query = Request.QueryString;
        StringDictionary list = new StringDictionary();

        foreach (string key in query.Keys)
        {
            if (key != "_a" && key != "_v" && !String.IsNullOrEmpty(key))
                list.Add(key, query[key]);
        }

        if (list.Count > 0)
        {
            Profile.NavigationState.QueryString = CreateQueryString(list);
        }

        Profile.Save();
         * */
    }

    /// <summary>
    /// Creates the query string.
    /// </summary>
    /// <param name="dic">The dic.</param>
    /// <returns></returns>
    private string CreateQueryString(StringDictionary dic)
    {
        string returnString = String.Empty;

        foreach (string key in dic.Keys)
        {
            if (returnString.Length > 0)
                returnString += "&";

            returnString += String.Format("{0}={1}", key, dic[key]);
        }

        return returnString;
    }

    /// <summary>
    /// Creates the child controls tree.
    /// </summary>
    private void CreateChildControlsTree()
    {
        ChangeView(AppId, ViewId);
    }

    /// <summary>
    /// Raises the <see cref="E:System.Web.UI.Control.Init"></see> event to initialize the page.
    /// </summary>
    /// <param name="e">An <see cref="T:System.EventArgs"></see> that contains the event data.</param>
    override protected void OnInit(EventArgs e)
    {
        // Proceed with the rest
        base.OnInit(e);

        // Make sure controls are created before the viewstate is initialized
		CreateChildControlsTree();

		_action.ValueChanged += new EventHandler(_action_ValueChanged);
    }

    /// <summary>
    /// Handles the ValueChanged event of the _action control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    void _action_ValueChanged(object sender, EventArgs e)
    {
        // Process events
        ProcessActionEvents();

        // Reset the value back to empty
        _action.Value = String.Empty;
    }
}
