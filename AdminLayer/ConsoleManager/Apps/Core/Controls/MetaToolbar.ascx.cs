using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Hosting;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Xml.XPath;
using Mediachase.Ibn.Web.UI.WebControls;
using Mediachase.Commerce.Profile;
using Mediachase.Ibn.XmlTools;
using System.Globalization;

namespace Mediachase.Commerce.Manager.Core.Controls
{
	public partial class MetaToolbar : System.Web.UI.UserControl
	{
		private const string defaultClientHandler = "defaultToolbarOnClick";

		#region ClassName
		/// <summary>
		/// Gets or sets the name of the class.
		/// </summary>
		/// <value>The name of the class.</value>
		public string ClassName
		{
			get
			{
				string retval = String.Empty;
				if (ViewState["ClassName"] != null)
					retval = ViewState["ClassName"].ToString();
				return retval;
			}
			set
			{
				ViewState["ClassName"] = value;
			}
		}
		#endregion

		#region ViewName
		/// <summary>
		/// Gets or sets the name of the view.
		/// </summary>
		/// <value>The name of the view.</value>
		public string ViewName
		{
			get
			{
				string retval = String.Empty;
				if (ViewState["ViewName"] != null)
					retval = ViewState["ViewName"].ToString();
				return retval;
			}
			set
			{
				ViewState["ViewName"] = value;
			}
		}
		#endregion

		#region PlaceName
		/// <summary>
		/// Gets or sets the name of the place.
		/// </summary>
		/// <value>The name of the place.</value>
		public string PlaceName
		{
			get
			{
				string retval = String.Empty;
				if (ViewState["PlaceName"] != null)
					retval = ViewState["PlaceName"].ToString();
				return retval;
			}
			set
			{
				ViewState["PlaceName"] = value;
			}
		}
		#endregion

		#region prop: CssClassGeneral
		/// <summary>
		/// Gets or sets the CSS class general.
		/// </summary>
		/// <value>The CSS class general.</value>
		public string CssClassGeneral
		{
			get
			{
				if (ViewState["_CssClassGeneral"] == null)
					return string.Empty;

				return ViewState["_CssClassGeneral"].ToString();
			}
			set
			{
				ViewState["_CssClassGeneral"] = value;
			}
		}
		#endregion

		#region prop: BlankImageUrl
		/// <summary>
		/// Gets or sets the blank image URL.
		/// </summary>
		/// <value>The blank image URL.</value>
		public string BlankImageUrl
		{
			get
			{
				if (ViewState["_BlankImageUrl"] == null)
					return this.ResolveUrl("~/App_Themes/Default/Images/ext/default/s.gif");

				return ViewState["_BlankImageUrl"].ToString();
			}
			set
			{
				ViewState["_BlankImageUrl"] = value;
			}
		}
		#endregion

		#region prop: GridId
		/// <summary>
		/// Gets or sets the grid id.
		/// </summary>
		/// <value>The grid id.</value>
		public string GridId
		{
			get
			{
				if (ViewState["__GridId"] != null)
					return ViewState["__GridId"].ToString();

				return string.Empty;
			}
			set
			{
				ViewState["__GridId"] = value;
			}
		}
		#endregion

		/// <summary>
		/// Gets or sets the grid client id.
		/// </summary>
		/// <value>The grid client id.</value>
		public string GridClientId
		{
			get
			{
				if (ViewState["__GridClientId"] != null)
					return ViewState["__GridClientId"].ToString();

				return string.Empty;
			}
			set
			{
				ViewState["__GridClientId"] = value;
			}
		}

		#region GetJsToolbar
        /// <summary>
        /// Gets the js toolbar.
        /// </summary>
        /// <returns></returns>
		public JsToolbar GetJsToolbar()
		{
			return MainToolbar;
		}
		#endregion

        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		protected void Page_Load(object sender, EventArgs e)
		{
			Page.ClientScript.RegisterClientScriptBlock(this.Page.GetType(), "extStyle",
						String.Format("<link type=\"text/css\" rel=\"stylesheet\" href=\"{0}\" />", Page.ResolveClientUrl("~/App_Themes/Default/css/ext-all.css")));
			
			BindToolbar();
			MainToolbar.CssClassGeneral = this.CssClassGeneral;
			MainToolbar.BlankImageUrl = this.BlankImageUrl;
		}

        /// <summary>
        /// Binds the toolbar.
        /// </summary>
		private void BindToolbar()
		{
			Selector selector = new Selector(ClassName, ViewName, PlaceName);

			IXPathNavigable navigable = Mediachase.Ibn.XmlTools.XmlBuilder.GetXml(StructureType.MetaView, selector);
			string path = String.Format(CultureInfo.InvariantCulture, "{0}/Toolbar", StructureType.MetaView.ToString());
			XPathNavigator toolbar = navigable.CreateNavigator().SelectSingleNode(path);

			GetToolbarItemsFromXml(toolbar, MainToolbar.McToolbarItems);
		}

		#region GetToolbarItemsFromXml
        /// <summary>
        /// Gets the toolbar items from XML.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <param name="itemsCollection">The items collection.</param>
		private void GetToolbarItemsFromXml(XPathNavigator node, List<McToolbarItem> itemsCollection)
		{
			foreach (XPathNavigator toolbarItem in node.SelectChildren(string.Empty, string.Empty))
			{
				McToolbarItem item = new McToolbarItem();

				switch (toolbarItem.Name)
				{
					case "Text":
						{
							item.ItemType = McToolbarItemType.Text;
							break;
						}
					case "Splitter":
						{
							item.ItemType = McToolbarItemType.Splitter;
							break;
						}
					case "Button":
						{
							item.ItemType = McToolbarItemType.Button;
							break;
						}
					case "Menu":
						{
							item.ItemType = McToolbarItemType.Menu;
							break;
						}
					case "SplitButton":
						{
							item.ItemType = McToolbarItemType.SplitButton;
							break;
						}
					default:
						{
							throw new NotSupportedException(String.Format("Unknown nodeType: {0}", node.Name));
						}
				}

				string Id = toolbarItem.GetAttribute("id", string.Empty);
				string Text = toolbarItem.GetAttribute("text", string.Empty);
				string ImageUrl = toolbarItem.GetAttribute("imageUrl", string.Empty);
				string CssClass = toolbarItem.GetAttribute("cssClass", string.Empty);
				string Align = toolbarItem.GetAttribute("align", string.Empty);
				string Handler = toolbarItem.GetAttribute("handler", string.Empty);
				string CommandName = toolbarItem.GetAttribute("commandName", string.Empty);
				string ItemSplitter = toolbarItem.GetAttribute("itemSplitter", string.Empty);
				string Tooltip = toolbarItem.GetAttribute("tooltip", string.Empty);
                string permissions = toolbarItem.GetAttribute("permissions", string.Empty);

				McToolbarItemAlign itemAlign = McToolbarItemAlign.Left;
				McToolbarItemSplitter itemSplitter = McToolbarItemSplitter.None;

				#region Set enum: McToolbarItemAlign
				if (Align != string.Empty)
				{
					try
					{
						itemAlign = (McToolbarItemAlign)Enum.Parse(typeof(McToolbarItemAlign), Align);
					}
					catch
					{
						throw;
					}
				}
				#endregion

				#region Set enum: McToolbarItemSplitter
				if (ItemSplitter != string.Empty)
				{
					try
					{
						itemSplitter = (McToolbarItemSplitter)Enum.Parse(typeof(McToolbarItemSplitter), ItemSplitter);
					}
					catch
					{
						throw;
					}
				}
				#endregion

				item.Id = Id;
				item.Text = UtilHelper.GetResFileString(Text);

                if (ImageUrl != string.Empty && (item.ItemType == McToolbarItemType.Button || item.ItemType == McToolbarItemType.SplitButton || item.ItemType == McToolbarItemType.Menu))
					item.CssClass += "x-btn-wrap x-btn x-btn-text-icon ";

				item.ImageUrl = this.ResolveClientUrl(ImageUrl); // this.ResolveUrl dont work here
				item.CssClass += CssClass;
				item.ItemAlign = itemAlign;
				item.Handler = Handler;
				item.Tooltip = UtilHelper.GetResFileString(Tooltip);

				CommandParameters param = new CommandParameters(CommandName);

				Dictionary<string, string> dic = new Dictionary<string, string>();
				dic.Add("GridId", this.GridId);
				dic.Add("GridClientId", this.GridClientId);
                dic.Add("permissions", permissions);
				param.CommandArguments = dic;
				bool isEnable = true;

				if (CommandName != string.Empty)
				{
					string commandManagerScript = CommandManager.GetCurrent(this.Page).AddCommand(this.ClassName, this.ViewName, this.PlaceName, param, out isEnable);
					dic.Add("CommandManagerScript", commandManagerScript);

					item.Params = param.ToString();
					item.Handler = defaultClientHandler;
				}

				if (item.ItemType == McToolbarItemType.Menu || item.ItemType == McToolbarItemType.SplitButton)
					GetToolbarItemsFromXml(toolbarItem, item.Items);

				if (isEnable && (itemSplitter == McToolbarItemSplitter.Both || itemSplitter == McToolbarItemSplitter.Left))
				{
					McToolbarItem splitItem = new McToolbarItem(McToolbarItemType.Splitter);
					splitItem.ItemAlign = itemAlign;
					itemsCollection.Add(splitItem);
				}

				if (isEnable)
					itemsCollection.Add(item);

				if (isEnable && (itemSplitter == McToolbarItemSplitter.Both || itemSplitter == McToolbarItemSplitter.Right))
				{
					McToolbarItem splitItem = new McToolbarItem(McToolbarItemType.Splitter);
					splitItem.ItemAlign = itemAlign;
					itemsCollection.Add(splitItem);
				}
			}
		}
		#endregion
	}
}