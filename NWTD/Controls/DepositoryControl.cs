using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mediachase.Cms.Web.UI.Controls;
using NWTD.InfoManager;
using Mediachase.Cms.Pages;

namespace NWTD.Controls {
	
	/// <summary>
	/// You can inherit fromt his control and have a control that stores a setting for the depository it pertains to.
	/// </summary>
	public class DepositoryControl : System.Web.UI.UserControl, ICmsDataAdapter {
		
		/// <summary>
		/// The Depository we'll be passing when we get the list
		/// </summary>
		protected Depository Depository { get; set; }

		#region ICmsDataAdapter Members
		/// <summary>
		/// Grabs the parameter information from the CMS Control Settings
		/// </summary>
		/// <param name="param"></param>
		public void SetParamInfo(object param) {
			try {
				ControlSettings settings = (ControlSettings)param;

				if (settings != null && settings.Params != null) {
					if (settings.Params["Depository"] != null)
						this.Depository = (Depository)Enum.Parse(typeof(Depository), settings.Params["Depository"].ToString());
				}

				this.DataBind();
			}
			catch {
			}
		}

		#endregion

	}
}
