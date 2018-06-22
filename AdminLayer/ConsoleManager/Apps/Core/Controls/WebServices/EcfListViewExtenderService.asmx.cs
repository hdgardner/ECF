using System;
using System.Web.Services;
using Mediachase.Commerce.Manager.Core.Controls;
using Mediachase.Commerce.Profile;
using Mediachase.Ibn.Web.UI.WebControls;
using Mediachase.Web.Console.Controls;
using Mediachase.Web.Console.Config;
using Mediachase.Web.Console;

namespace Mediachase.Commerce.Manager.Apps.Core.Controls.WebServices
{
	/// <summary>
	/// Summary description for EcfListViewExtenderService
	/// </summary>
	[WebService(Namespace = "http://mediachase.com/ecf50")]
	[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
	[System.ComponentModel.ToolboxItem(false)]
	// To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
	[System.Web.Script.Services.ScriptService]
	public class EcfListViewExtenderService : System.Web.Services.WebService
	{
		[WebMethod]
		public void ColumnResize(string ContextKey, int ColumnIndex, int NewSize)
		{
			EcfListViewContextKey key = UtilHelper.JsonDeserialize<EcfListViewContextKey>(ContextKey);
			try
			{
				// get old settings
				string gridSettingsKey = CMPageSettings.MakeGridSettingsKey(key.AppId, key.ViewId);
				string oldGridSettings = ProfileContext.Current.Profile.PageSettings.GetSettingString(gridSettingsKey);
				EcfListViewPreferences prefs = null;
				if (!String.IsNullOrEmpty(oldGridSettings))
					prefs = UtilHelper.JsonDeserialize<EcfListViewPreferences>(oldGridSettings);

				if (prefs == null)
					prefs = new EcfListViewPreferences(key.AppId, key.ViewId);

				int index = ColumnIndex;
				AdminView view = ManagementContext.Current.FindView(key.AppId, key.ViewId);
				if (view != null && view.Columns.Count>ColumnIndex)
				{
					// get real index of a column with visibleIndex==ColumnIndex
					// start search from column with index == ColumnIndex because its visibleIndex<=ColumnIndex
					for (int i = ColumnIndex; i < view.Columns.Count; i++)
					{
						if (view.Columns[i].ColumnVisibleIndex == ColumnIndex)
						{
							index = i;
							break;
						}
					}
				}
				prefs.ColumnProperties[index.ToString()] = NewSize.ToString();

				// save new settings
				ProfileContext.Current.Profile.PageSettings.SetSettingString(gridSettingsKey, UtilHelper.JsonSerialize(prefs));
				ProfileContext.Current.Profile.Save();
			}
			catch (Exception ex)
			{
				// TODO: handle exception
			}
		}
	}
}
