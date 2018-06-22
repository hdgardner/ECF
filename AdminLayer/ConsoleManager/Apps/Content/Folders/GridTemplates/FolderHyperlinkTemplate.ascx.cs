using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Mediachase.Web.Console.Interfaces;

namespace Mediachase.Commerce.Manager.Apps.Content.Folders.GridTemplates
{
	public partial class FolderHyperlinkTemplate : System.Web.UI.UserControl, IEcfListViewTemplate
	{
		private object _DataItem;

        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		protected void Page_Load(object sender, EventArgs e)
		{
			bool needSetUrl = false;
			object objUrl = DataBinder.Eval(DataItem, "[ImageUrl]");
			if (objUrl == null || objUrl == DBNull.Value || String.IsNullOrEmpty(objUrl.ToString()))
				needSetUrl = true;
			else
				ObjectImage.ImageUrl = objUrl.ToString();

			bool isFolder = (bool)DataBinder.Eval(DataItem, "[IsFolder]");

			if (isFolder)
			{
				FolderLink.Visible = true;
				PageLabel.Visible = false;
				if (needSetUrl)
					ObjectImage.ImageUrl = Page.ResolveUrl("~/Apps/Content/images/Folder.gif");
			}
			else
			{
				FolderLink.Visible = false;
				PageLabel.Visible = true;
				if (needSetUrl)
					ObjectImage.ImageUrl = Page.ResolveUrl("~/Apps/Content/images/Html16.gif");
			}
		}

		#region IEcfListViewTemplate Members

        /// <summary>
        /// Gets or sets the data item.
        /// </summary>
        /// <value>The data item.</value>
		public object DataItem
		{
			get
			{
				return _DataItem;
			}
			set
			{
				_DataItem = value;
			}
		}

		#endregion
	}
}