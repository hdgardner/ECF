using System;
using System.Data;
using System.Data.Sql;
using System.Data.SqlTypes;
using System.Data.SqlClient;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Threading;
using System.Xml;

using mc = Mediachase.Cms;


public partial class Controls_Menu_MainMenu : Mediachase.Web.UI.BaseLocalizableControl
{
    #region MenuId
    private int menuid = 0;
    /// <summary>
    /// Gets or sets the menu id.
    /// </summary>
    /// <value>The menu id.</value>
    public int MenuId
    {
        get
        {
           return menuid;
        }
        set
        {
            menuid = value;
        }
    }
    #endregion

	#region SkinId
	private string _skinId = string.Empty;
    /// <summary>
    /// Gets or sets the skin id.
    /// </summary>
    /// <value>The skin id.</value>
	public string SkinId
	{
		get { return _skinId; }
		set { _skinId = value; }
	}
	#endregion

	#region BindMenu()
    /// <summary>
    /// Binds the menu.
    /// </summary>
	public void BindMenu()
    {
		if (MenuId <= 0)
		{
			this.Visible = false;
			return;
		}
		mcMainMenu.Items.Clear();
		//mcMainMenu.SkinID = SkinId;

		//get menu root
        int RootId = mc.MenuItem.LoadRootId(MenuId, mc.CMSContext.Current.SiteId);
        //get first level menu
        DataTable MenuTable = mc.MenuItem.LoadSubMenuDT(RootId);
        foreach (DataRow row in MenuTable.Rows)
        {
            int MenuItemId = (int)row["MenuItemId"];
            using (IDataReader reader = mc.MenuItem.LoadById(MenuItemId, LanguageId))
            {
                if (reader.Read())
                {
                    //translation exists
                    MenuItem newItem = new MenuItem();
                    newItem.Selectable = true;
                    newItem.Text = reader["Text"].ToString();
                    switch ((int)reader["CommandType"])
                    {
                        case 1: newItem.NavigateUrl = reader["CommandText"].ToString();
                            //highlight if selected
                            Regex RegexObj = new Regex(@"[\x20/~]*(?<url>[\w\x2F]*)[\x20]*");
                            string NavigateUrl = RegexObj.Match(reader["CommandText"].ToString()).Groups["url"].Value;
                            string CurrentUrl = RegexObj.Match(mc.CMSContext.Current.Outline).Groups["url"].Value;
                            if(NavigateUrl.ToUpper() == CurrentUrl.ToUpper())
                                newItem.Selected = true;
                            break;
                        case 2: newItem.NavigateUrl = "javascript:" + reader["CommandText"].ToString();
                            break;
                        case 3:
                            newItem.NavigateUrl = Mediachase.Commerce.Shared.CommerceHelper.GetAbsolutePath(GetNavigate((string)reader["CommandText"]));
                            break;
                    }
                    
                    //add to main menu
                    mcMainMenu.Items.Add(newItem);
                    //bind sub menu
                    BindSubMenu(newItem, (int)reader["MenuItemId"]);
                }

                reader.Close();
            }
        }
        if (MenuTable.Rows.Count == 0)
            mcMenu.Visible = false;
    }

    /// <summary>
    /// Gets the navigate.
    /// </summary>
    /// <param name="Text">The text.</param>
    /// <returns></returns>
    private string GetNavigate(string Text)
    {
        int itemId = Convert.ToInt32(Text.Split('^')[0]);
        string param = Text.Split('^')[1]; //.Replace('?', '&');

        ArrayList Params = new ArrayList(10);
        ArrayList Values = new ArrayList(10);
        //int i = 0;

        while (true)
        {
            string s = param.Split('&')[0];
            Params.Add(param.Substring(1, param.IndexOf('=') - 1));
            Values.Add(param.Substring(s.IndexOf('=') + 1, Math.Max(s.LastIndexOf('&'), s.Length) - s.IndexOf('=') - 1));
            if (!param.Contains("&"))
                break;
            param = param.Substring(param.IndexOf('&'), param.Length - param.IndexOf('&'));
        }

        return mc.NavigationManager.GetUrl(mc.NavigationManager.GetItemNameById(itemId), Params, Values);
    }

    /// <summary>
    /// Binds the sub menu.
    /// </summary>
    /// <param name="RootItem">The root item.</param>
    /// <param name="RootId">The root id.</param>
    protected void BindSubMenu(MenuItem RootItem, int RootId)
    {
        DataTable MenuTable = mc.MenuItem.LoadSubMenuDT(RootId);
        foreach (DataRow row in MenuTable.Rows)
        {
            int MenuItemId = (int)row["MenuItemId"];
            using (IDataReader reader = mc.MenuItem.LoadById(MenuItemId, this.LanguageId))
            {
                if (reader.Read())
                {
                    //translation exists
                    MenuItem newItem = new MenuItem();
                    newItem.Selectable = true;
                    newItem.Text = reader["Text"].ToString();
                    switch ((int)reader["CommandType"])
                    {
                        case 1: newItem.NavigateUrl = reader["CommandText"].ToString();
                            break;
                        case 2: newItem.NavigateUrl = "javascript:" + reader["CommandText"].ToString();
                            break;
                        case 3:
                            newItem.NavigateUrl = Mediachase.Commerce.Shared.CommerceHelper.GetAbsolutePath(GetNavigate((string)reader["CommandText"]));
                            break;
                    }
                    //add to main menu
                    RootItem.ChildItems.Add(newItem);
                    //recursive call
                    BindSubMenu(newItem, (int)reader["MenuItemId"]);
                }

                reader.Close();
            }
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
       BindMenu();
    }

    /// <summary>
    /// Raises the <see cref="E:System.Web.UI.Control.Init"/> event.
    /// </summary>
    /// <param name="e">An <see cref="T:System.EventArgs"/> object that contains the event data.</param>
    override protected void OnInit(EventArgs e)
    {
        base.OnInit(e);
    }
	       
   
}
