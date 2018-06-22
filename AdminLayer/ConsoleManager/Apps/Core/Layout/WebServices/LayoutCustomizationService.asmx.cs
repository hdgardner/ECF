using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.Services;
using System.Web.Services.Protocols;
using Mediachase.Commerce.Profile;
using Mediachase.Ibn.Web.UI.Layout;
using Mediachase.Web.Console.Common;

namespace Mediachase.Commerce.Manager.WebServices
{
	/// <summary>
	/// Summary description for LayoutCustomizationService
	/// </summary>
	[WebService(Namespace = "http://mediachase.com/ecf50")]
	[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
	[ToolboxItem(false)]
	// To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
	[System.Web.Script.Services.ScriptService]
	public class LayoutCustomizationService : System.Web.Services.WebService
	{
		[WebMethod]
		public void ChangePosition(string newLayout, string contextKey)
		{
			LayoutContextKey key = new JavaScriptSerializer().Deserialize<LayoutContextKey>(contextKey);

			try
			{
				ProfileContext.Current.Profile.PageSettings.SetSettingString(key.PageUid, newLayout);
				ProfileContext.Current.Profile.Save();
			}
			catch (Exception ex)
			{
				// TODO: handle exception
			}

			//if (!key.IsAdmin)
			//{
			//    Mediachase.IBN.Business.UserLightPropertyCollection pc = Mediachase.IBN.Business.Security.CurrentUser.Properties;
			//    pc[keyData] = newLayout;
			//}
			//else
			//{
			//    string _uid = DashboardPageProviderBase.GetPageWorkspaceUid(key.PageUid);
			//    string templateUid = string.Empty;
			//    using (IDataReader reader = Mediachase.IBN.Business.Common.GetWorkspaceSettings(_uid))
			//    {
			//        if (reader.Read())
			//        {
			//            templateUid = reader["TemplateUid"].ToString();
			//        }
			//    }

			//    if (templateUid != string.Empty)
			//        Mediachase.IBN.Business.Common.UpdateWorkspaceSettings(_uid, newLayout, templateUid);
			//    else
			//        throw new ArgumentException(String.Format("Cant read setting for page: {0}, uid: {1}", key.PageUid, _uid));
			//}
		}

		[WebMethod]
		public void Delete(string newLayout, string contextKey)
		{
			LayoutContextKey key = new JavaScriptSerializer().Deserialize<LayoutContextKey>(contextKey);

			try
			{
				ProfileContext.Current.Profile.PageSettings.SetSettingString(key.PageUid, newLayout);
				ProfileContext.Current.Profile.Save();
			}
			catch (Exception ex)
			{
				// TODO: handle exception
			}

			//if (!key.IsAdmin)
			//{
			//    Mediachase.IBN.Business.UserLightPropertyCollection pc = Mediachase.IBN.Business.Security.CurrentUser.Properties;
			//    pc[keyData] = newLayout;
			//}
			//else
			//{
			//    string _uid = DashboardPageProviderBase.GetPageWorkspaceUid(key.PageUid);
			//    string templateUid = string.Empty;
			//    using (IDataReader reader = Mediachase.IBN.Business.Common.GetWorkspaceSettings(_uid))
			//    {
			//        if (reader.Read())
			//        {
			//            templateUid = reader["TemplateUid"].ToString();
			//        }
			//    }

			//    if (templateUid != string.Empty)
			//        Mediachase.IBN.Business.Common.UpdateWorkspaceSettings(_uid, newLayout, templateUid);
			//    else
			//        throw new ArgumentException(String.Format("Cant read setting for page: {0}, uid: {1}", key.PageUid, _uid));
			//}
		}
	}
}
