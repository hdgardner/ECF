using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Web.Script.Serialization;
using System.Xml.XPath;
using Mediachase.Ibn.Web.UI.WebControls;
using System.Web.Hosting;
using System.Configuration;
using System.Xml;
using System.IO;
using Mediachase.Web.Console.Common;
using Mediachase.Web.Console.BaseClasses;
using Mediachase.Ibn.XmlTools;

namespace Mediachase.Commerce.Manager.Apps.Shell.Pages
{
	public partial class TreeSource : BasePage
	{
        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		protected void Page_Load(object sender, EventArgs e)
		{
			if (Request["mode"] == "full")
			{
				BindFullTree();
			}
			else if (Request.Form["node"] != "leftTemplate_tree_rootId")
			{
				BindNode(Request.Form["node"]);
			}
			else
			{
				BindJsTree();
			}
		}

		#region BindJsTree
        /// <summary>
        /// Binds the js tree.
        /// </summary>
		private void BindJsTree()
		{
			string tabId = String.Empty;
			if (Request["tab"] != null)
				tabId = Request["tab"];
			if (String.IsNullOrEmpty(tabId))
				return;

			IXPathNavigable navigable = Mediachase.Ibn.XmlTools.XmlBuilder.GetXml(StructureType.Navigation);

			XPathNavigator links = navigable.CreateNavigator().SelectSingleNode(string.Format("Navigation/Tabs/Tab[@id='{0}']", tabId));

			List<JsonTreeNode> nodes = new List<JsonTreeNode>();
			if (links != null)
				BindRecursive(nodes, links);

			WriteArray(nodes);
		}
		#endregion

		#region BindRecursive
        /// <summary>
        /// Determines whether [is enable command] [the specified class name].
        /// </summary>
        /// <param name="className">Name of the class.</param>
        /// <param name="viewName">Name of the view.</param>
        /// <param name="placeName">Name of the place.</param>
        /// <param name="commandName">Name of the command.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns>
        /// 	<c>true</c> if [is enable command] [the specified class name]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsEnableCommand(string className, string viewName, string placeName, string commandName, Dictionary<string, string> parameters)
        {
            bool flag = true;
            XmlCommand command = XmlCommand.GetCommandByName(className, viewName, placeName, commandName);
            if ((command == null) || string.IsNullOrEmpty(command.EnableHandler))
            {
                return flag;
            }
            object obj2 = AssemblyUtil.LoadObject(command.EnableHandler);
            if (!(obj2 is ICommandEnableHandler))
            {
                return flag;
            }
            CommandParameters element = new CommandParameters(commandName);
            element.CommandArguments = parameters;
            if (!string.IsNullOrEmpty(command.Params))
            {
                if (element.CommandArguments == null)
                {
                    element.CommandArguments = new Dictionary<string, string>();
                }
                element.CommandArguments.Add("_commandParamsKey", command.Params);
            }

            return ((ICommandEnableHandler)obj2).IsEnable(null, element);
        }

        /// <summary>
        /// Binds the recursive.
        /// </summary>
        /// <param name="nodes">The nodes.</param>
        /// <param name="linkItem">The link item.</param>
        /// <returns></returns>
		private int BindRecursive(List<JsonTreeNode> nodes, XPathNavigator linkItem)
		{
			int retVal = 0;
			foreach (XPathNavigator subItem in linkItem.SelectChildren(string.Empty, string.Empty))
			{
				JsonTreeNode node = new JsonTreeNode();

				string text = UtilHelper.GetResFileString(subItem.GetAttribute("text", string.Empty));
				string id = subItem.GetAttribute("id", string.Empty);

                node.text = text;
				node.id = id;

				node.cls = "nodeCls";

				node.iconCls = "iconNodeCls";

				string iconUrl = subItem.GetAttribute("iconUrl", string.Empty);
				if (!String.IsNullOrEmpty(iconUrl))
					node.icon = ResolveClientUrl(iconUrl);

				string iconCss = subItem.GetAttribute("iconCss", string.Empty);
				if (!String.IsNullOrEmpty(iconCss))
					node.iconCls = iconCss;
				
				string command = subItem.GetAttribute("command", string.Empty);
				if (!String.IsNullOrEmpty(command))
				{
                    Dictionary<string, string> parameters = new Dictionary<string, string>();
                    parameters.Add("permissions", subItem.GetAttribute("permissions", string.Empty));

                    if (!IsEnableCommand("", "", "", command, parameters))
                        continue;

					string cmd = CommandManager.GetCommandString(command, null);
					cmd = cmd.Replace("\"", "&quot;");
					node.href = String.Format("javascript:{0}", cmd);
				}

                string expanded = subItem.GetAttribute("expanded", string.Empty);
				if (!String.IsNullOrEmpty(expanded) && expanded.ToLower().Equals("true"))
					node.expanded = true;

				string type = subItem.GetAttribute("type", string.Empty);
				if (!String.IsNullOrEmpty(type))
					node.type = type;

				bool isAsyncLoad = false;
				string treeLoader = subItem.GetAttribute("treeLoader", string.Empty);
				string treeLoaderPath = subItem.GetAttribute("treeLoaderPath", string.Empty);
				if (!String.IsNullOrEmpty(treeLoader))
				{
					isAsyncLoad = true;
				}
				if (!String.IsNullOrEmpty(treeLoaderPath))
				{
					isAsyncLoad = true;
					node.treeLoader = ResolveClientUrl(treeLoaderPath);
				}

				if (!isAsyncLoad)
				{
					node.children = new List<JsonTreeNode>();
					int count = BindRecursive(node.children, subItem);
					if (count == 0)
					{
						node.leaf = true;
						node.children = null;
					}
				}

				nodes.Add(node);
				retVal++;
			}
			return retVal;
		}
		#endregion

		#region BindNode
        /// <summary>
        /// Binds the node.
        /// </summary>
        /// <param name="returnNodeId">The return node id.</param>
		private void BindNode(string returnNodeId)
		{
			string tabId = String.Empty;
			if (Request["tab"] != null)
				tabId = Request["tab"];
			if (String.IsNullOrEmpty(tabId))
				return;

			string staticNodeId = returnNodeId;
			string innerNodeId = returnNodeId;
			if (Request.Form["id"] != null && !String.IsNullOrEmpty(Request.Form["id"]) && Request.Form["id"] != "null")
				innerNodeId = Request.Form["id"].ToString();
			if (Request.Form["staticParentId"] != null && !String.IsNullOrEmpty(Request.Form["staticParentId"]) && Request.Form["staticParentId"] != "null")
				staticNodeId = Request.Form["staticParentId"].ToString();

			IXPathNavigable navigable = Mediachase.Ibn.XmlTools.XmlBuilder.GetXml(StructureType.Navigation);
			XPathNavigator link = navigable.CreateNavigator().SelectSingleNode(String.Format("//Link[@id='{0}']", staticNodeId));
			if (link != null)
			{
				string treeLoader = link.GetAttribute("treeLoader", string.Empty);
				if (!String.IsNullOrEmpty(treeLoader))
				{
					IJsonHandler jsHandler = (IJsonHandler)AssemblyUtil.LoadObject(treeLoader);
					WriteArray(jsHandler.GetJsonDataSource(innerNodeId));
				}
			}
		}
		#endregion

		#region WriteArray
        /// <summary>
        /// Writes the array.
        /// </summary>
        /// <param name="nodes">The nodes.</param>
		private void WriteArray(List<JsonTreeNode> nodes)
		{
            string json = JsonSerializer.Serialize(nodes);
			Response.Write(json);
		}

        /// <summary>
        /// Writes the array.
        /// </summary>
        /// <param name="json">The json.</param>
		private void WriteArray(string json)
		{
			Response.CacheControl = "no-cache";
			Response.AddHeader("Pragma", "no-cache");
			Response.Expires = -1;
			
			Response.Write(json);
		}
		#endregion

		#region BindFullTree
		private void BindFullTree()
		{
			IXPathNavigable navigable = Mediachase.Ibn.XmlTools.XmlBuilder.GetXml(StructureType.Navigation);

			XPathNavigator tabsNode = navigable.CreateNavigator().SelectSingleNode("Navigation/Tabs");
			List<JsonTreeNode> nodes = new List<JsonTreeNode>();
			foreach (XPathNavigator subItem in tabsNode.SelectChildren("Tab", string.Empty))
			{
				JsonTreeNode node = new JsonTreeNode();

				node.id = subItem.GetAttribute("id", string.Empty);
				node.text = UtilHelper.GetResFileString(subItem.GetAttribute("text", string.Empty));

				string order = subItem.GetAttribute("order", string.Empty);
				if (!string.IsNullOrEmpty(order))
					node.text += String.Concat("<span class=\"rightColumn\">", order, "</span><span class=\"clearColumn\"></span>");

				node.cls = "nodeCls";

				string iconUrl = subItem.GetAttribute("imageUrl", string.Empty);
				if (!String.IsNullOrEmpty(iconUrl))
					node.icon = ResolveClientUrl(iconUrl);

				node.children = new List<JsonTreeNode>();
				int count = BindRecursiveNoAsync(node.children, subItem);
				if (count == 0)
				{
					node.leaf = true;
					node.children = null;
				}

				nodes.Add(node);
			}

			WriteArray(nodes);
		}
		#endregion

		#region BindRecursiveNoAsync
		private int BindRecursiveNoAsync(List<JsonTreeNode> nodes, XPathNavigator linkItem)
		{
			int retVal = 0;
			foreach (XPathNavigator subItem in linkItem.SelectChildren(string.Empty, string.Empty))
			{
				JsonTreeNode node = new JsonTreeNode();

				node.id = subItem.GetAttribute("id", string.Empty);

				node.text = UtilHelper.GetResFileString(subItem.GetAttribute("text", string.Empty));

				string order = subItem.GetAttribute("order", string.Empty);
				if (!string.IsNullOrEmpty(order))
					node.text = String.Concat("<span class=\"rightColumn\">", order, "</span><span class=\"leftColumn\">", node.text, "</span>");

				node.cls = "nodeCls";
				node.iconCls = "iconNodeCls";

				string iconUrl = subItem.GetAttribute("iconUrl", string.Empty);
				if (!String.IsNullOrEmpty(iconUrl))
					node.icon = ResolveClientUrl(iconUrl);

				string iconCss = subItem.GetAttribute("iconCss", string.Empty);
				if (!String.IsNullOrEmpty(iconCss))
					node.iconCls = iconCss;

				node.children = new List<JsonTreeNode>();
				int count = BindRecursiveNoAsync(node.children, subItem);
				if (count == 0)
				{
					node.leaf = true;
					node.children = null;
				}

				nodes.Add(node);
				retVal++;
			}
			return retVal;
		}
		#endregion
	}
}