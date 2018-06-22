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
using System.Threading;

namespace Mediachase.Ibn.Web.UI
{
	public partial class BlockHeader : System.Web.UI.UserControl
	{
		ArrayList items = new ArrayList();

		#region Public Properties: Title
		private string title = "";
        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        /// <value>The title.</value>
		public string Title
		{
			set
			{
				title = value;
				lblBlockTitle.Text = title;
				if (title == "")
					lblBlockTitle.Visible = false;
				else
					lblBlockTitle.Visible = true;
			}
			get
			{
				return title;
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
			BindData();
		}

		#region PreRender
        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.PreRender"/> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> object that contains the event data.</param>
		protected override void OnPreRender(EventArgs e)
		{
			bool setseparator = false;
			foreach (LinkItem1 li in items)
			{
				if (setseparator)
				{
					AddSeparatorInternal();
				}
				else setseparator = true;

				HtmlAnchor link = new HtmlAnchor();
				link.InnerHtml = li._text;
				link.HRef = li._url;
				link.Attributes.Add("class", "ibn-toolbar");

				HtmlTableCell cell = new HtmlTableCell();
				cell.Style.Add("padding-right", "5");
				cell.NoWrap = true;
				cell.Controls.Add(link);
				if (title == "")
					trMain.Cells.Insert(trMain.Cells.Count - 1, cell);
				else
					trMain.Cells.Insert(trMain.Cells.Count, cell);
			}
		}
		#endregion

		#region BindData
        /// <summary>
        /// Binds the data.
        /// </summary>
		private void BindData()
		{
			lblBlockTitle.Text = title;
		}
		#endregion

		#region AddLink
        /// <summary>
        /// Adds the link.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="url">The URL.</param>
		public void AddLink(string text, string url)
		{
			items.Add(new LinkItem1(text, url));
		}
		#endregion

		#region AddImageLink
        /// <summary>
        /// Adds the image link.
        /// </summary>
        /// <param name="ImageUrl">The image URL.</param>
        /// <param name="text">The text.</param>
        /// <param name="url">The URL.</param>
		public void AddImageLink(string ImageUrl, string text, string url)
		{
			items.Add(new LinkItem1("<img align='absmiddle' border='0' src='" + ImageUrl + "' />&nbsp;" + text, url));
		}
		#endregion

		#region AddThemedImageLink
        /// <summary>
        /// Adds the themed image link.
        /// </summary>
        /// <param name="ImageUrl">The image URL.</param>
        /// <param name="text">The text.</param>
        /// <param name="url">The URL.</param>
		public void AddThemedImageLink(string ImageUrl, string text, string url)
		{
			items.Add(new LinkItem1("<img align='absmiddle' border='0' src='" + CHelper.GetAbsolutePath(ImageUrl) + "' />&nbsp;" + text, url));
		}
		#endregion

		#region AddLinkAt
        /// <summary>
        /// Adds the link at.
        /// </summary>
        /// <param name="position">The position.</param>
        /// <param name="text">The text.</param>
        /// <param name="url">The URL.</param>
		public void AddLinkAt(int position, string text, string url)
		{
			items.Insert(position, new LinkItem1(text, url));
		}
		#endregion

		#region AddSeparatorInternal()
        /// <summary>
        /// Adds the separator internal.
        /// </summary>
		public void AddSeparatorInternal()
		{
			HtmlTableCell cell = new HtmlTableCell();
			cell.Attributes.Add("class", "ibn-separator");
			cell.InnerText = "|";
			if (title == "")
				trMain.Cells.Insert(trMain.Cells.Count - 1, cell);
			else
				trMain.Cells.Insert(trMain.Cells.Count, cell);
		}
		#endregion

		#region AddSeparator
        /// <summary>
        /// Adds the separator.
        /// </summary>
		public void AddSeparator()
		{
		}
		#endregion

		#region AddSeparatorAt
        /// <summary>
        /// Adds the separator at.
        /// </summary>
        /// <param name="position">The position.</param>
		public void AddSeparatorAt(int position)
		{
		}
		#endregion
	}

	public interface IGetHTML
	{
		string GetHTML();
	}

	public class LinkItem1 : IGetHTML
	{
		public string _text;
		public string _url;

        /// <summary>
        /// Initializes a new instance of the <see cref="LinkItem1"/> class.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="url">The URL.</param>
		public LinkItem1(string text, string url)
		{
			_text = text;
			_url = url;
		}

		#region Implementation of IGetHTML
        /// <summary>
        /// Gets the HTML.
        /// </summary>
        /// <returns></returns>
		public string GetHTML()
		{
			return String.Format("<a href='{0}'>{1}</a>", _url, _text);
		}
		#endregion
	}
}