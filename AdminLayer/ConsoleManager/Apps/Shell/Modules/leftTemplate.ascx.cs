using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Xml;
using System.Xml.XPath;
using System.Web.Hosting;
using Mediachase.Ibn.Web.UI.WebControls;
using Mediachase.Web.Console;
using Mediachase.Commerce.Profile;
using Mediachase.Ibn.XmlTools;

namespace Mediachase.Commerce.Manager.Apps.Shell.Modules
{
    public partial class LeftTemplate : System.Web.UI.UserControl
    {
        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add(new DataColumn("Title", typeof(string)));
            dt.Columns.Add(new DataColumn("ImageUrl", typeof(string)));
            DataRow dr;
			IXPathNavigable navigable = XmlBuilder.GetXml(StructureType.Navigation);

            XPathNavigator tabs = navigable.CreateNavigator().SelectSingleNode("Navigation/Tabs");

            foreach (XPathNavigator tabItem in tabs.SelectChildren(string.Empty, string.Empty))
            {
                dr = dt.NewRow();
				string title = UtilHelper.GetResFileString(tabItem.GetAttribute("text", string.Empty));
                string id = tabItem.GetAttribute("id", string.Empty);

                CommandParameters param = new CommandParameters(id);
                param.CommandArguments = new Dictionary<string, string>();
                param.CommandArguments.Add("permissions", tabItem.GetAttribute("permissions", string.Empty));

                string enableHandler = String.Empty;

                if (!ProfileConfiguration.Instance.EnablePermissions)
                    enableHandler = tabItem.GetAttribute("enableHandler", string.Empty);
                else
                    enableHandler = tabItem.GetAttribute("enableHandler2", string.Empty);

                if (!String.IsNullOrEmpty(enableHandler))
                {
                    ICommandEnableHandler enHandler = (ICommandEnableHandler)AssemblyUtil.LoadObject(enableHandler);
                    if (enHandler != null && !enHandler.IsEnable(sender, param))
                        continue;
                }

                string imageUrl = tabItem.GetAttribute("imageUrl", string.Empty);
                if (String.IsNullOrEmpty(imageUrl))
                    imageUrl = "~/App_Themes/Default/Images/ext/default/s.gif";

                string type = tabItem.GetAttribute("contentType", string.Empty).ToLower();
                if (String.IsNullOrEmpty(type))
                    type = "default";

                string configUrl = tabItem.GetAttribute("configUrl", string.Empty);
                string checkUrl = configUrl;
                if (checkUrl.IndexOf("?") >= 0)
                    checkUrl = checkUrl.Substring(0, checkUrl.IndexOf("?"));
                if (type.Equals("default") && String.IsNullOrEmpty(checkUrl))
                {
                    checkUrl = "~/Apps/Shell/Pages/TreeSource.aspx";
                    configUrl = "~/Apps/Shell/Pages/TreeSource.aspx?tab=" + id;
                }

                if (File.Exists(Server.MapPath(checkUrl)))
                {
                    switch (type)
                    {
                        case "default":
                            ClientScript.RegisterStartupScript(this.Page, this.Page.GetType(), Guid.NewGuid().ToString("N"), String.Format("leftTemplate_ECFAddMenuTab('{0}', '{1}', '{2}');", id, title, ResolveClientUrl(configUrl)), true);
                            break;
                        case "custom":
                            break;
                        default:
                            break;
                    }
                }

				dr["Title"] = title;
                dr["ImageUrl"] = imageUrl;
                dt.Rows.Add(dr);
            }
            TabItems.DataSource = dt.DefaultView;
            TabItems.DataBind();

            RegisterScripts();

            //Register navigation commands
			IList<XmlCommand> list = XmlCommand.GetListNavigationCommands("", "", "");
            CommandManager cm = CommandManager.GetCurrent(this.Page);
            foreach (XmlCommand cmd in list)
            {
                cm.AddCommand("", "", "", cmd.CommandName);
            }
        }

        #region RegisterScripts
        /// <summary>
        /// Registers the scripts.
        /// </summary>
        private void RegisterScripts()
        {
            //scripts
            Page.ClientScript.RegisterClientScriptInclude(this.Page.GetType(), "MainLeftTemplate", ResolveClientUrl("~/Scripts/Shell/mainLeftTemplate.js"));

            //styles
            Page.ClientScript.RegisterClientScriptBlock(this.Page.GetType(), Guid.NewGuid().ToString(),
                String.Format("<link type='text/css' rel='stylesheet' href='{0}' />", ResolveClientUrl("~/App_Themes/Default/css/Shell/mainLeftTemplate.css")));
        }
        #endregion
    }
}