using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;

namespace Mediachase.Cms.Website.Structure.Base.Controls.Common
{
	public partial class UtilScriptsLoader : System.Web.UI.UserControl, IScriptControl
	{
		protected void Page_Load(object sender, EventArgs e)
		{
		}

		protected override void OnPreRender(EventArgs e)
		{

			if (!this.DesignMode)
			{
				ScriptManager sm = ScriptManager.GetCurrent(this.Page);
				if (sm != null)
				{
					sm.RegisterScriptControl(this);
					sm.RegisterScriptDescriptors(this);
				}
			}
			base.OnPreRender(e);
		}
		
		#region IScriptControl Members

		System.Collections.Generic.IEnumerable<ScriptDescriptor> IScriptControl.GetScriptDescriptors()
		{
			ScriptComponentDescriptor sd = new ScriptComponentDescriptor("Mediachase.Cms.Util");
			sd.ID = "MediachaseCmsUtil";
			sd.AddProperty("IsDesignMode", CMSContext.Current.IsDesignMode);
			return new ScriptDescriptor[] { sd };
		}

		System.Collections.Generic.IEnumerable<ScriptReference> IScriptControl.GetScriptReferences()
		{
			ScriptReference sr = new ScriptReference(Mediachase.Commerce.Shared.CommerceHelper.GetAbsolutePath("~/Scripts/CMS_Scripts.js"));
			return new ScriptReference[] { sr };
		}

		#endregion
	}
}