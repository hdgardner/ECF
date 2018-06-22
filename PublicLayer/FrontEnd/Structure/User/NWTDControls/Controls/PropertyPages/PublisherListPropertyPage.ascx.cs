using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Mediachase.Cms.Pages;

namespace Mediachase.Cms.Website.Structure.User.NWTDControls.Controls.PropertyPages {
	
	/// <summary>
	/// This page makes it possible to choose which depository a control will be using.
	/// </summary>
	public partial class PublisherListPropertyPage : System.Web.UI.UserControl, IPropertyPage {
		protected void Page_Load(object sender, EventArgs e) {
			this.ddlDepos.DataSource = Enum.GetNames(typeof(NWTD.InfoManager.Depository));
			this.ddlDepos.DataBind();
		}

		protected const string DEPOSITORY_KEY = "Depository";

		#region IPropertyPage Members

		public string Title {
			get { return "Publisher Settings"; }
		}

		public string Description {
			get { return "Configures Publisher Settings"; }
		}

		public new void Load(string NodeUid, string ControlUid) {
			ControlSettings settings = new ControlSettings();
			DynamicNode dNode = PageDocument.Current.DynamicNodes.LoadByUID(NodeUid);
			if (dNode != null)
				settings = dNode.GetSettings(NodeUid);
			if (settings != null && settings.Params != null) {
				if (settings.Params[DEPOSITORY_KEY] == null) settings.Params[DEPOSITORY_KEY] = NWTD.InfoManager.Depository.NWTD.ToString();		
				this.ddlDepos.SelectedValue = settings.Params[DEPOSITORY_KEY].ToString(); 
			}
			
		}

		public void Save(string NodeUid, string ControlUid) {
			ControlSettings settings = new ControlSettings();

			DynamicNode dNode = PageDocument.Current.DynamicNodes.LoadByUID(NodeUid);
			if (dNode != null) {
				settings = dNode.GetSettings(dNode.NodeUID);
				dNode.IsModified = true;
			}

			if (settings.Params == null) {
				settings.Params = new Param();
			}

			settings.IsModified = true;
			settings.Params.Remove(DEPOSITORY_KEY);
			settings.Params[DEPOSITORY_KEY] = this.ddlDepos.SelectedValue;
		}

		#endregion
	}
}