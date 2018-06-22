using System;
using System.ComponentModel;
using System.Data;
using System.Resources;
using System.Drawing;
using System.Web;
using System.Text;
using System.Collections;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Xml;
using RssToolkit;
using RssToolkit.Rss;
using Mediachase.Web.Console.BaseClasses;
using Mediachase.Web.Console.Common;

namespace Mediachase.Commerce.Manager.Dashboard.Modules
{
	/// <summary>
	/// Summary description for RssNewsSource.
	/// </summary>
	public partial class RssNewsSource : BasePage
	{
        /// <summary>
        /// Gets the RSS path.
        /// </summary>
        /// <value>The RSS path.</value>
		public string RssPath
		{
			get
			{
				return ManagementHelper.GetValueFromQueryString("RssPath", String.Empty);
			}
		}

        /// <summary>
        /// Gets the RSS count.
        /// </summary>
        /// <value>The RSS count.</value>
		public int RssCount
		{
			get
			{
				return ManagementHelper.GetIntFromQueryString("RssCount");
			}
		}

        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		protected void Page_Load(object sender, System.EventArgs e)
		{
			Response.Cache.SetNoStore();

			BindRssNews();
		}

        /// <summary>
        /// Binds the RSS news.
        /// </summary>
		private void BindRssNews()
		{
			string ss = "";
			
			if (!String.IsNullOrEmpty(RssPath))
			{
				RssDocument rssDocument = null;

				try
				{
					rssDocument = RssDocument.Load(new Uri(RssPath));
				}
				catch
				{
					Response.ContentType = "text/xml";
					Response.Write("<div style='text-align:center;padding:10px;color:red;' class='text'>" + RM.GetString("RSS_ERROR") + "</div>");
					return;
				}

				// Create root element
				XmlDocument doc = new XmlDocument();
				XmlNode root = doc.AppendChild(doc.CreateElement("rssData"));
				XmlNode node = doc.CreateElement("htmlData");

				RssChannel channel = rssDocument.Channel;

				StringBuilder sb = new StringBuilder();
				sb.AppendFormat("<div>");
				sb.AppendFormat("<div style='padding-top: 3px;padding-bottom:3px;color: #444;font-size: 10pt;'><b>{0}</b></div>", channel.Title);
				for (int i = 0; i < channel.Items.Count && i < RssCount; i++)
				{
					sb.AppendFormat("<div style='padding-top: 3px;' class='text'><a href='{1}' target='_blank'>{0}</a></div>", channel.Items[i].Title, channel.Items[i].Link);
					sb.AppendFormat("<div style='color: gray;' class='text'>{0}</div>", channel.Items[i].PubDate);
					//sb.AppendFormat("<div style='color: black; font-family: tahoma; font-size: 12px;'>{0}</div>", channel.Items[i].Attributes["description"]);
				}
				sb.Append("</div>");
				XmlNode cdata = doc.CreateCDataSection(sb.ToString());
				//node.Name = "htmlData";
				node.AppendChild(cdata);
				root.AppendChild(node);
				//doc.DocumentElement.AppendChild(doc.CreateCDataSection(sb.ToString()));
				ss += doc.InnerText;
			}
			Response.ContentType = "text/xml";
			Response.Write(ss);
		}

		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			InitializeComponent();
			base.OnInit(e);
		}

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
		}
		#endregion
	}
}
