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
using Mediachase.Web.Console.BaseClasses;
using Mediachase.Web.Console;
using System.Threading;

namespace Mediachase.Commerce.Manager.Dashboard.Modules
{
	public partial class NewsModule : BaseUserControl
	{
        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		protected void Page_Load(object sender, EventArgs e)
		{
			BindRss();
		}

        /// <summary>
        /// Binds the RSS.
        /// </summary>
		private void BindRss()
		{
			try
			{
				
				RssItem item = GetItemToBind();
				if (item != null)
				{
					string scriptString = "CreateRssNews('" + item.UrlPath + "', " + item.NewsCount + ", 'divNews');";
					Page.ClientScript.RegisterStartupScript(this.GetType(), "_rssload", scriptString, true);
				}
			}
			catch
			{
			}
		}

        /// <summary>
        /// Returns RssItem for the current admin language
        /// </summary>
        /// <returns></returns>
		private RssItem GetItemToBind()
		{
			// TODO: get item for a current language
			//Thread.CurrentThread.CurrentUICulture
			if (RssConfiguration.Instance.RssItems.Count > 0)
				return RssConfiguration.Instance.RssItems[0];

			return null;
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
}