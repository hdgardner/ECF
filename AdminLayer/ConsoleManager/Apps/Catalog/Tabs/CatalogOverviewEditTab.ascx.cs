using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Mediachase.Cms;
using Mediachase.Cms.Dto;
using Mediachase.Commerce.Catalog;
using Mediachase.Commerce.Catalog.Dto;
using Mediachase.Commerce.Core.Managers;
using Mediachase.Web.Console.BaseClasses;
using Mediachase.Web.Console.Interfaces;
using Mediachase.Web.Console.Common;
using Mediachase.Commerce.Catalog.Managers;

namespace Mediachase.Commerce.Manager.Catalog.Tabs
{
    public partial class CatalogOverviewEditTab : BaseUserControl, IAdminTabControl
    {
        /// <summary>
        /// Gets the catalog id.
        /// </summary>
        /// <value>The catalog id.</value>
        public int CatalogId
        {
            get
            {
                return ManagementHelper.GetIntFromQueryString("CatalogId");
            }
        }
        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            if(!this.IsPostBack)
                BindForm();
        }

        /// <summary>
        /// Binds the form.
        /// </summary>
        private void BindForm()
        {
            // Bind Languages
            BindLanguages();

            // Bind Currencies
            BindCurrency();

            if (CatalogId > 0)
            {
                CatalogDto dto = CatalogContext.Current.GetCatalogDto(CatalogId);
                this.CatalogName.Text = dto.Catalog[0].Name;
                this.AvailableFrom.Value = ManagementHelper.GetUserDateTime(dto.Catalog[0].StartDate);
				this.ExpiresOn.Value = ManagementHelper.GetUserDateTime(dto.Catalog[0].EndDate);
                this.SortOrder.Text = dto.Catalog[0].SortOrder.ToString();

                // Bind other languages
                if (dto.CatalogLanguage.Count > 0)
                {
                    foreach (CatalogDto.CatalogLanguageRow row in dto.CatalogLanguage.Rows)
                    {
                        foreach (ListItem item in OtherLanguagesList.Items)
                        {
                            if (item.Value == row.LanguageCode)
                                item.Selected = true;
                        }
                    }
                }

                ManagementHelper.SelectListItem(DefaultLanguage, dto.Catalog[0].DefaultLanguage.ToLower());
                ManagementHelper.SelectListItem(DefaultCurrency, dto.Catalog[0].DefaultCurrency.ToLower());
                ManagementHelper.SelectListItem(BaseWeight, dto.Catalog[0].WeightBase);
                this.IsCatalogActive.IsSelected = dto.Catalog[0].IsActive;

                // Bind sites
                BindSites(dto);
            }
            else
            {
                this.AvailableFrom.Value = DateTime.Now;
                this.ExpiresOn.Value = DateTime.Now.AddYears(1);
                this.SortOrder.Text = "0";
				ManagementHelper.SelectListItem(DefaultCurrency, CommonSettingsManager.GetDefaultCurrency());
                ManagementHelper.SelectListItem(DefaultLanguage, CommonSettingsManager.GetDefaultLanguage());

                // Bind sites
                BindSites(null);
            }
        }

        /// <summary>
        /// Binds the currency.
        /// </summary>
        private void BindCurrency()
        {
            foreach (CurrencyDto.CurrencyRow row in CatalogContext.Current.GetCurrencyDto().Currency)
            {
                ListItem item = new ListItem(row.Name, row.CurrencyCode.ToLower());
                DefaultCurrency.Items.Add(item);
            }

        }

        /// <summary>
        /// Binds the languages.
        /// </summary>
        private void BindLanguages()
        {
            DataTable languages = Language.GetAllLanguagesDT();
            foreach (DataRow row in languages.Rows)
            {
                CultureInfo culture = CultureInfo.CreateSpecificCulture(row["LangName"].ToString());
                ListItem item = new ListItem(culture.DisplayName, culture.Name.ToLower());
                DefaultLanguage.Items.Add(item);
                ListItem item2 = new ListItem(culture.DisplayName, culture.Name.ToLower());
                OtherLanguagesList.Items.Add(item2);
            }
        }

        /// <summary>
        /// Binds the sites.
        /// </summary>
        /// <param name="catalog">The catalog.</param>
        private void BindSites(CatalogDto catalog)
        {
            // Bind sites
            SiteDto siteDto = CMSContext.Current.GetSitesDto(CmsConfiguration.Instance.ApplicationId);

            if (siteDto.Site.Count > 0)
            {
                foreach (SiteDto.SiteRow row in siteDto.Site.Rows)
                {
                    ListItem item = new ListItem(row.Name, row.SiteId.ToString());
                    if (catalog!= null && catalog.SiteCatalog.Count > 0 && CatalogId > 0)
                    {
                        if (catalog.SiteCatalog.Select(String.Format("CatalogId = {0} and SiteId = '{1}'", CatalogId, row.SiteId)).Length > 0)
                        {
                            item.Selected = true;
                        }
                    }

                    SiteList.Items.Add(item);
                }
            }
            SiteList.DataBind();
        }

		/// <summary>
		/// Checks if entered name is unique.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="args">The <see cref="System.Web.UI.WebControls.ServerValidateEventArgs"/> instance containing the event data.</param>
		public void NameCheck(object sender, ServerValidateEventArgs args)
		{
			// get all catalogs
			CatalogDto dto = CatalogContext.Current.GetCatalogDto();
			// select catalog with the same name as entered in the textbox
			CatalogDto.CatalogRow[] catalogs = (CatalogDto.CatalogRow[])dto.Catalog.Select(String.Format("Name='{0}'", args.Value));

			bool found = false;
			// check if catalog is found
			if (catalogs!=null && catalogs.Length>0)
			{
				if (catalogs[0].CatalogId != CatalogId)
					found = true;
			}

			args.IsValid = !found;
		}

        #region IAdminTabControl Members
        /// <summary>
        /// Saves the changes.
        /// </summary>
        /// <param name="context">The context.</param>
        public void SaveChanges(IDictionary context)
        {
            CatalogDto dto = (CatalogDto)context["Catalog"];
            CatalogDto.CatalogRow row = null;

            if (dto.Catalog.Count > 0)
            {
                row = dto.Catalog[0];
                row.Modified = DateTime.UtcNow;
            }
            else
            {
                row = dto.Catalog.NewCatalogRow();
                row.Created = DateTime.UtcNow;
                row.Modified = DateTime.UtcNow;
                row.IsPrimary = true;
            }

            row.Name = CatalogName.Text;
            row.DefaultCurrency = DefaultCurrency.SelectedValue;
            row.DefaultLanguage = DefaultLanguage.SelectedValue;
            row.WeightBase = BaseWeight.SelectedValue;
            row.StartDate = this.AvailableFrom.Value.ToUniversalTime();
            row.EndDate = this.ExpiresOn.Value.ToUniversalTime();
            row.IsActive = this.IsCatalogActive.IsSelected;
            row.SortOrder = Int32.Parse(this.SortOrder.Text);
            row.ApplicationId = CatalogConfiguration.Instance.ApplicationId;
                       
            if (row.RowState == DataRowState.Detached)
                dto.Catalog.Rows.Add(row);

            // Populate other languages
            foreach (ListItem item in OtherLanguagesList.Items)
            {
                if (item.Selected && !DefaultLanguage.SelectedValue.Equals(item.Value))
                {
                    if (dto.CatalogLanguage.Select(String.Format("CatalogId = {0} and LanguageCode = '{1}'", CatalogId, item.Value)).Length == 0)
                    {
                        CatalogDto.CatalogLanguageRow langRow = dto.CatalogLanguage.NewCatalogLanguageRow();
                        langRow.LanguageCode = item.Value;
                        langRow.CatalogId = row.CatalogId;
                        dto.CatalogLanguage.Rows.Add(langRow);
                    }
                }
                else
                {
                    DataRow[] rows = dto.CatalogLanguage.Select(String.Format("CatalogId = {0} and LanguageCode = '{1}'", CatalogId, item.Value));
                    if (rows.Length > 0)
                    {
                        foreach (CatalogDto.CatalogLanguageRow lrow in rows)
                            lrow.Delete();
                    }
                }
            }

            // Populate sites
            foreach (ListItem item in SiteList.Items)
            {
                if (item.Selected) // add row
                {
                    DataRow[] rows = dto.SiteCatalog.Select(String.Format("CatalogId = {0} and SiteId = '{1}'", CatalogId, item.Value));
                    if (rows.Length == 0)
                    {
                        CatalogDto.SiteCatalogRow siteRow = dto.SiteCatalog.NewSiteCatalogRow();
                        siteRow.SiteId = new Guid(item.Value);
                        siteRow.CatalogId = row.CatalogId;
                        dto.SiteCatalog.Rows.Add(siteRow);
                    }
                }
                else // delete row
                {
                    DataRow[] rows = dto.SiteCatalog.Select(String.Format("CatalogId = {0} and SiteId = '{1}'", CatalogId, item.Value));
                    if (rows.Length > 0)
                    {
                        foreach (CatalogDto.SiteCatalogRow srow in rows)
                        {
                            srow.Delete();
                        }
                    }
                }
            }

            // Re add sites
        }
        #endregion
    }
}