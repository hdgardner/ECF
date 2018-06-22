using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Web.UI.WebControls;
using Mediachase.Cms;
using Mediachase.Commerce.Catalog;
using Mediachase.Commerce.Catalog.Dto;
using Mediachase.Commerce.Catalog.Managers;
using Mediachase.Commerce.Catalog.Objects;
using Mediachase.Commerce.Catalog.Search;
using Mediachase.Commerce.Core.Managers;
using Mediachase.Ibn.Web.UI.WebControls;
using Mediachase.Web.Console.BaseClasses;
using Mediachase.Web.Console.Common;

namespace Mediachase.Commerce.Manager.Marketing
{
	public partial class EntrySearch : MarketingBaseUserControl
	{
		private ListItemCollection _liCollection = new ListItemCollection();
		/// <summary>
		/// Gets or sets the selected items in right list box.
		/// </summary>
		/// <value>The items.</value>
		public ListItemCollection Items
		{
			get 
			{
				return _liCollection;
			}
			set 
			{
				_liCollection = value;
			}
		}

		public string ClientControlId
		{
			get
			{
				return ManagementHelper.GetValueFromQueryString("ControlId", String.Empty);
			}
		}

		public string CatalogEntryIds
		{
			get
			{
				return ManagementHelper.GetValueFromQueryString("CatalogEntryIds", String.Empty);
			}
			 
		}

		protected void Page_Init(object sender, EventArgs e)
		{
			this.btnSave.Click += new EventHandler(btnSave_Click);
			this.btnSearch.Click += new EventHandler(btnSearch_Click);
		}

		void btnSearch_Click(object sender, EventArgs e)
		{
			InitDataSource();
			lbSource.DataBind();
			for (int i = lbSource.Items.Count-1; i >= 0; i--)
			{
				//already in selected items
				if (ltlSelector.Items.FindByValue(lbSource.Items[i].Value) != null)
				{
					lbSource.Items.RemoveAt(i);
				}
				else//item is not in selected items
					ltlSelector.Items.Add(lbSource.Items[i]);
			}
		}

		protected void Page_Load(object sender, EventArgs e)
		{
			if (!IsPostBack)
			{
				BindTargetItems();
				BindLanguagesList();
				ManagementHelper.SelectListItem(ListLanguages, CommonSettingsManager.GetDefaultLanguage());

				BindCatalogsList();
			}
		}

		void btnSave_Click(object sender, EventArgs e)
		{
			bool error = false;
			try
			{
				ProcessCommand();
			}
			catch (Exception ex)
			{
				string errorMessage = ex.Message.Replace("'", "\\'").Replace(Environment.NewLine, "\\n");
				ClientScript.RegisterStartupScript(this.Page, this.GetType(), "__CatalogEntriesSelectFrameError",
					String.Format("alert('{0}{1}');", "Operation failed.\\nError: ", errorMessage), true);
				error = true;
			}

			// if operation succeeded, close dialog
			if (!error)
			{
				//CommandParameters cp = new CommandParameters("MC_Workspace_PropertyPage");
				Mediachase.Ibn.Web.UI.WebControls.CommandHandler.RegisterCloseOpenedFrameScript(this.Page, String.Empty); //cp.ToString());
			}
		}

		private void ProcessCommand()
		{
			List<ListItem> selItems = ltlSelector.GetSelectedItems();
			Dictionary<string, string> dicItems = new Dictionary<string, string>();
			if (selItems != null && selItems.Count > 0)
			{
				foreach (ListItem li in selItems)
				{
					dicItems.Add(li.Value, li.Text);
				}
			}
			Session["CatalogEntryFilter_SelectedItems_" + ClientControlId] = dicItems;
		}

		/// <summary>
		/// Binds the languages.
		/// </summary>
		private void BindLanguagesList()
		{
			ListLanguages.DataValueField = "LangId";
			ListLanguages.DataTextField = "LangName";

			DataTable languages = Language.GetAllLanguagesDT();
			foreach (DataRow row in languages.Rows)
			{
				CultureInfo culture = CultureInfo.CreateSpecificCulture(row["LangName"].ToString());
				ListItem item = new ListItem(culture.DisplayName, culture.Name.ToLower());
				ListLanguages.Items.Add(item);
			}
		}

		/// <summary>
		/// Binds the catalogs list.
		/// </summary>
		private void BindCatalogsList()
		{
			CatalogDto dto = CatalogContext.Current.GetCatalogDto();

			ListCatalogs.Items.Add(new ListItem("all catalogs", "[all]"));
			foreach (CatalogDto.CatalogRow row in dto.Catalog.Rows)
			{
				ListItem item = new ListItem(row.Name, row.Name);
				ListCatalogs.Items.Add(item);
			}
			ListCatalogs.Items[0].Selected = true;
			ListCatalogs.DataBind();
		}

		private void BindTargetItems()
		{
			if (!String.IsNullOrEmpty(CatalogEntryIds))
			{
				string[] ids = CatalogEntryIds.Split(',');
				Items.Clear();
				for (int i = 0; i < ids.Length; i++)
				{
					CatalogEntryDto ent = CatalogContext.Current.GetCatalogEntryDto(int.Parse(ids[i]));
					if (ent != null && ent.CatalogEntry.Count > 0)
					{
						CatalogEntryDto.CatalogEntryRow row = ent.CatalogEntry[0];
						Items.Add(new ListItem(row.Name, row.Code));
					}
				}
			}

			if (Items != null && Items.Count > 0)
			{
				lbTarget.Items.Clear();
				foreach (ListItem li in Items)
				{
					lbTarget.Items.Add(li);
					li.Selected = true;
					ltlSelector.Items.Add(li);
				}
			}
		}
		
		private void InitDataSource()
		{
			CatalogSearchParameters pars = new CatalogSearchParameters();
			
			// language filter
			pars.Language = ListLanguages.SelectedValue;

			// catalog filter
			//CatalogDto dto = CatalogContext.Current.GetCatalogDto();
			//foreach (CatalogDto.CatalogRow row in dto.Catalog.Rows)
			//{
			//    pars.CatalogNames.Add(row.Name);
			//}
			if (ListCatalogs.SelectedValue == "[all]")
			{
				foreach (ListItem item in ListCatalogs.Items)
				{
					if (item.Value == "[all]")
						continue;

					pars.CatalogNames.Add(item.Value);
				}
			}
			else if (ListCatalogs.SelectedIndex >= 0)// selected catalogs
			{
				for (int iTmp = 0; iTmp < ListCatalogs.Items.Count; iTmp++)
				{
					ListItem li = ListCatalogs.Items[iTmp];
					if (li.Selected)
						pars.CatalogNames.Add(li.Value);
				}
			}
			pars.FreeTextSearchPhrase = tbKeywords.Text;
			CatalogSearchOptions opts = new CatalogSearchOptions();
			opts.RecordsToRetrieve = Int32.MaxValue;
			opts.Namespace = "Mediachase.Commerce.Catalog";
			opts.StartingRecord = 0;
			opts.ReturnTotalCount = true;
			CatalogEntryResponseGroup respgroup = new CatalogEntryResponseGroup();
			int totalRecordsCount = 0;
			CatalogEntryDto entries = CatalogContext.Current.FindItemsDto(pars, opts, ref totalRecordsCount, respgroup);
			if (totalRecordsCount > 0)
				lbSource.DataSource = entries.CatalogEntry.Rows;
		}
	}
}