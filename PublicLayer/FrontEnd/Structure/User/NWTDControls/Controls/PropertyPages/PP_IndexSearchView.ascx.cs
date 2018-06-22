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
using Mediachase.Commerce.Catalog;
using Mediachase.Commerce.Catalog.Dto;
using Mediachase.Commerce.Catalog.Objects;
using Mediachase.Cms;
using Mediachase.Cms.Dto;
using Mediachase.Cms.Managers;
using Mediachase.Cms.Pages;
using Mediachase.Cms.Util;
using Mediachase.MetaDataPlus.Configurator;
using Mediachase.MetaDataPlus;
using Mediachase.Commerce;

namespace Mediachase.Cms.Website.Structure.User.NWTDControls.Controls.PropertyPages {
	public partial class PP_IndexSearchView : System.Web.UI.UserControl, IPropertyPage {
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

			// Bind Meta Types
			// Bind Meta classes
			// MetaDataContext.Current = CatalogContext.MetaDataContext;
			MetaClass catalogEntry = MetaClass.Load(CatalogContext.MetaDataContext, "CatalogEntry");
			MetaClassList.Items.Clear();
			if (catalogEntry != null) {
				MetaClassCollection metaClasses = catalogEntry.ChildClasses;
				foreach (MetaClass metaClass in metaClasses) {
					MetaClassList.Items.Add(new ListItem(metaClass.FriendlyName, metaClass.Name));
				}
				MetaClassList.DataBind();
			}

			// Bind templates
			DisplayTemplate.Items.Clear();
			DisplayTemplate.Items.Add(new ListItem("(use default)", ""));
			TemplateDto templates = DictionaryManager.GetTemplateDto();
			if (templates.main_Templates.Count > 0) {
				DataView view = templates.main_Templates.DefaultView;
				view.RowFilter = "TemplateType = 'search-index'";

				foreach (DataRowView row in view) {
					DisplayTemplate.Items.Add(new ListItem(row["FriendlyName"].ToString(), row["Name"].ToString()));
				}

				DisplayTemplate.DataBind();
			}

			// Bind Types
			EntryTypeList.Items.Clear();
			EntryTypeList.Items.Add(new ListItem(EntryType.Product, EntryType.Product));
			EntryTypeList.Items.Add(new ListItem(EntryType.Package, EntryType.Package));
			EntryTypeList.Items.Add(new ListItem(EntryType.Bundle, EntryType.Bundle));
			EntryTypeList.Items.Add(new ListItem(EntryType.DynamicPackage, EntryType.DynamicPackage));
			EntryTypeList.Items.Add(new ListItem(EntryType.Variation, EntryType.Variation));
			EntryTypeList.DataBind();

			// Bind catalogs
			CatalogList.Items.Clear();
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

				CommonHelper.LoadTextBox(settings, "NodeCode", NodeCode);
				CommonHelper.LoadTextBox(settings, "RecordsPerPage", NumberOfRecords);

				/*
				CommonHelper.LoadTextBox(settings, "FTSPhrase", FTSPhrase);
				CommonHelper.LoadTextBox(settings, "AdvancedFTSPhrase", AdvancedFTSPhrase);
				CommonHelper.LoadTextBox(settings, "MetaSQLClause", MetaSQLClause);
				CommonHelper.LoadTextBox(settings, "SQLClause", SQLClause);
				 * */

				if ((prm["DisplayTemplate"] != null) && (prm["DisplayTemplate"] is string)) {
					CommonHelper.SelectListItem(DisplayTemplate, prm["DisplayTemplate"].ToString());
				}

				CommonHelper.SelectList(settings, "Catalogs", CatalogList);
				CommonHelper.SelectList(settings, "EntryClasses", MetaClassList);
				CommonHelper.SelectList(settings, "EntryTypes", EntryTypeList);

				// Orderby
				if ((prm["OrderBy"] != null) && (prm["OrderBy"] is string)) {
					string orderBy = prm["OrderBy"].ToString();
					bool isDesc = orderBy.Contains("DESC");

					string listOrderBy = orderBy.Replace(" DESC", "");
					listOrderBy = listOrderBy.Replace(" ASC", "");

					CommonHelper.SelectListItem(OrderByList, listOrderBy);

					if (!String.IsNullOrEmpty(OrderByList.SelectedValue)) {
						if (OrderByList.SelectedValue == "custom") {
							OrderBy.Text = orderBy;
						}
						else {
							OrderDesc.Checked = isDesc;
						}
					}
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
				settings.IsModified = true;
			}

			if (settings.Params == null) {
				settings.Params = new Param();
			}

			settings.IsModified = true;

			CommonHelper.SaveParameter(settings, "DisplayTemplate", DisplayTemplate.SelectedValue);
			CommonHelper.SaveParameter(settings, "NodeCode", NodeCode.Text);
			CommonHelper.SaveParameter(settings, "RecordsPerPage", NumberOfRecords.Text);
			CommonHelper.SaveParameter(settings, "Catalogs", CatalogList);
			CommonHelper.SaveParameter(settings, "EntryTypes", EntryTypeList);
			CommonHelper.SaveParameter(settings, "EntryClasses", MetaClassList);
			/*
			CommonHelper.SaveParameter(settings, "FTSPhrase", FTSPhrase.Text);
			CommonHelper.SaveParameter(settings, "AdvancedFTSPhrase", AdvancedFTSPhrase.Text);
			CommonHelper.SaveParameter(settings, "SQLClause", SQLClause.Text);
			CommonHelper.SaveParameter(settings, "MetaSQLClause", MetaSQLClause.Text);
			 * */

			// Orderby
			string orderByValue = String.Empty;
			if (!String.IsNullOrEmpty(OrderByList.SelectedValue)) {
				if (OrderByList.SelectedValue == "custom") {
					orderByValue = OrderBy.Text;
				}
				else {
					orderByValue = OrderByList.SelectedValue;
					if (OrderDesc.Checked)
						orderByValue = orderByValue + " DESC";
				}
			}
			CommonHelper.SaveParameter(settings, "OrderBy", orderByValue);


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
			get { return "Search View"; }
		}

		/// <summary>
		/// Gets the description.
		/// </summary>
		/// <value>The description.</value>
		public string Description {
			get { return "Displays entries withing configured parameters using template specified."; }
		}

		#endregion
	}

}