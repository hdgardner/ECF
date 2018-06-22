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
using Mediachase.Cms;
using Mediachase.Cms.Dto;
using Mediachase.Cms.Managers;
using Mediachase.Commerce.Catalog;
using Mediachase.Commerce.Catalog.Dto;
using Mediachase.Cms.Pages;
using Mediachase.Cms.Util;
using Mediachase.Commerce;

namespace Mediachase.Cms.Website.Structure.User.NWTDControls.Controls.PropertyPages {
	public partial class PP_EntryView : System.Web.UI.UserControl, IPropertyPage {
		/// <summary>
		/// Handles the Load event of the Page control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		protected void Page_Load(object sender, EventArgs e) {
	
		}

		protected const string DEPOSITORY_KEY = "Depository";

		#region IPropertyPage Members

		/// <summary>
		/// Loads the specified node uid.
		/// </summary>
		/// <param name="NodeUid">The node uid.</param>
		/// <param name="ControlUid">The control uid.</param>
		void IPropertyPage.Load(string NodeUid, string ControlUid) {
			ControlSettings settings = new ControlSettings();

			DynamicNode dNode = PageDocument.Current.DynamicNodes.LoadByUID(NodeUid);
			if (dNode != null)
				settings = dNode.GetSettings(NodeUid);

			// Bind templates
			DisplayTemplate.Items.Clear();
			DisplayTemplate.Items.Add(new ListItem("(use default)", ""));
			TemplateDto templates = DictionaryManager.GetTemplateDto();
			if (templates.main_Templates.Count > 0) {
				DataView view = templates.main_Templates.DefaultView;
				view.RowFilter = "TemplateType = 'entry'";

				foreach (DataRowView row in view) {
					DisplayTemplate.Items.Add(new ListItem(row["FriendlyName"].ToString(), row["Name"].ToString()));
				}

				DisplayTemplate.DataBind();
			}

			// Bind catalogs
			CatalogList.Items.Clear();
			CatalogList.Items.Add(new ListItem("(use default)", ""));
			CatalogDto catalogs = CatalogContext.Current.GetCatalogDto(CMSContext.Current.SiteId);
			if (catalogs.Catalog.Count > 0) {
				foreach (CatalogDto.CatalogRow row in catalogs.Catalog) {
					if (row.IsActive && row.StartDate <= FrameworkContext.Current.CurrentDateTime && row.EndDate >= FrameworkContext.Current.CurrentDateTime)
						CatalogList.Items.Add(new ListItem(row.Name, row.Name));
				}

				CatalogList.DataBind();
			}

			//Bind Depositories
			this.ddlDepository.DataSource = Enum.GetNames(typeof(NWTD.InfoManager.Depository));
			this.ddlDepository.DataBind();


			if (settings != null && settings.Params != null) {
				Param prm = settings.Params;
				if ((prm["NodeCode"] != null) && (prm["NodeCode"] is string)) {
					NodeCode.Text = prm["NodeCode"].ToString();
				}

				if ((prm["EntryCode"] != null) && (prm["EntryCode"] is string)) {
					EntryCode.Text = prm["EntryCode"].ToString();
				}

				if ((prm["DisplayTemplate"] != null) && (prm["DisplayTemplate"] is string)) {
					CommonHelper.SelectListItem(DisplayTemplate, prm["DisplayTemplate"].ToString());
				}

				if ((prm["CatalogName"] != null) && (prm["CatalogName"] is string)) {
					CommonHelper.SelectListItem(CatalogList, prm["CatalogName"].ToString());
				}

				if (prm[DEPOSITORY_KEY] == null) prm[DEPOSITORY_KEY] = NWTD.InfoManager.Depository.NWTD.ToString();
					this.ddlDepository.SelectedValue = settings.Params[DEPOSITORY_KEY].ToString(); 

			}
		}

		/// <summary>
		/// Saves the specified node uid.
		/// </summary>
		/// <param name="NodeUid">The node uid.</param>
		/// <param name="ControlUid">The control uid.</param>
		void IPropertyPage.Save(string NodeUid, string ControlUid) {
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

			settings.Params.Remove("DisplayTemplate");

			if (!String.IsNullOrEmpty(DisplayTemplate.SelectedValue))
				settings.Params.Add("DisplayTemplate", DisplayTemplate.SelectedValue);

			settings.Params.Remove("NodeCode");

			if (!String.IsNullOrEmpty(NodeCode.Text))
				settings.Params.Add("NodeCode", NodeCode.Text);

			settings.Params.Remove("EntryCode");

			if (!String.IsNullOrEmpty(NodeCode.Text))
				settings.Params.Add("EntryCode", EntryCode.Text);

			settings.Params.Remove("CatalogName");

			if (!String.IsNullOrEmpty(CatalogList.SelectedValue))
				settings.Params.Add("CatalogName", CatalogList.SelectedValue);

			settings.Params.Remove(DEPOSITORY_KEY);
			if (!String.IsNullOrEmpty(this.ddlDepository.SelectedValue))
				settings.Params[DEPOSITORY_KEY] = this.ddlDepository.SelectedValue;

		}

		#endregion

		#region IPropertyPage Members

		/// <summary>
		/// Gets the title.
		/// </summary>
		/// <value>The title.</value>
		public string Title {
			get { return "Entry View"; }
		}

		/// <summary>
		/// Gets the description.
		/// </summary>
		/// <value>The description.</value>
		public string Description {
			get { return "Displays catalog entry like product on the page using the template specified."; }
		}

		#endregion
	}
}