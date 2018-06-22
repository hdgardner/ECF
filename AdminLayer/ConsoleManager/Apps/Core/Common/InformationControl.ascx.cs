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
using Mediachase.Commerce.Catalog;
using Mediachase.Commerce.Core;
using Mediachase.Cms;
using Mediachase.Commerce.Marketing;
using Mediachase.Commerce.Orders;
using Mediachase.Commerce.Profile;
using Mediachase.Ibn.Library;

namespace Mediachase.Commerce.Manager.Core.Common
{
	public partial class InformationControl : CoreBaseUserControl
	{
		/// <summary>
		/// Handles the Load event of the Page control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		protected void Page_Load(object sender, EventArgs e)
		{
			try
			{
				int major = 0;
				int minor = 0;
				int patch = 0;
				DateTime installed = DateTime.MinValue;

				AppContext.GetApplicationSystemVersion(out major, out minor, out patch, out installed);
				ApplicationSystemVersionText.Text = GetVersionString(major, minor, patch, installed);

				AssetContext.GetAssetSystemVersion(out major, out minor, out patch, out installed);
				BusinessFoundationSystemVersionText.Text = GetVersionString(major, minor, patch, installed);

				CatalogContext.GetCatalogSystemVersion(out major, out minor, out patch, out installed);
				CatalogSystemVersionText.Text = GetVersionString(major, minor, patch, installed);

				CMSContext.GetContentSystemVersion(out major, out minor, out patch, out installed);
				ContentSystemVersionText.Text = GetVersionString(major, minor, patch, installed);

				MarketingContext.GetMarketingSystemVersion(out major, out minor, out patch, out installed);
				MarketingSystemVersionText.Text = GetVersionString(major, minor, patch, installed);

				OrderContext.GetOrderSystemVersion(out major, out minor, out patch, out installed);
				OrderSystemVersionText.Text = GetVersionString(major, minor, patch, installed);

				ProfileContext.GetProfileSystemVersion(out major, out minor, out patch, out installed);
				ProfileSystemVersionText.Text = GetVersionString(major, minor, patch, installed);
			}
			catch(Exception ex)
			{
				ErrorText.Visible = true;
				ErrorText.Text = String.Format("Error: {0}", ex.Message);
			}
		}

		private string GetVersionString(int major, int minor, int patch, DateTime installed)
		{
			string versionString = String.Empty;
			if (major == 0 && minor == 0 && patch == 0)
				versionString = "N/A";
			else
				versionString = String.Format("{0}.{1}.{2}&nbsp;&nbsp;&nbsp;Installed:&nbsp;{3}", major, minor, patch, installed);
			return versionString;
		}
	}
}