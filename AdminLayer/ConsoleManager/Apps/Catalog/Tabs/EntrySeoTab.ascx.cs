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
using System.Collections.Generic;
using System.Collections.Specialized;
using Mediachase.Web.Console.BaseClasses;
using Mediachase.Web.Console.Interfaces;
using Mediachase.Commerce.Catalog;
using Mediachase.Commerce.Catalog.Dto;
using Mediachase.Web.Console.Common;
using Mediachase.MetaDataPlus.Configurator;
using Mediachase.Commerce.Manager.Catalog.Modules;
using Mediachase.Commerce.Shared;

namespace Mediachase.Commerce.Manager.Catalog.Tabs
{
    public partial class EntrySeoTab : BaseUserControl, IAdminTabControl, IAdminContextControl
    {
        private const string _CatalogEntryDtoString = "CatalogEntryDto";
        private CatalogEntryDto _CatalogEntryDto = null;

        /// <summary>
        /// Gets the catalog entry id.
        /// </summary>
        /// <value>The catalog entry id.</value>
		public int CatalogEntryId
		{
			get
			{
				return ManagementHelper.GetIntFromQueryString("catalogentryid");
			}
		}

        /// <summary>
        /// Gets the parent catalog id.
        /// </summary>
        /// <value>The parent catalog id.</value>
		public int ParentCatalogId
		{
			get
			{
				return ManagementHelper.GetIntFromQueryString("catalogid");
			}
		}

        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            //if (!Page.IsPostBack)
                BindForm();
        }

        /// <summary>
        /// Binds the form.
        /// </summary>
        private void BindForm()
        {
            List<string> languageList = new List<string>();
            CatalogDto catalogDto = null;
            if (CatalogEntryId > 0)
            {
                // Get list of languages
                catalogDto = CatalogContext.Current.GetCatalogDto(_CatalogEntryDto.CatalogEntry[0].CatalogId);
            }
            else
            {
                // Get list of languages
                catalogDto = CatalogContext.Current.GetCatalogDto(ParentCatalogId);
            }

            languageList.Add(catalogDto.Catalog[0].DefaultLanguage);
            if (catalogDto.CatalogLanguage.Count > 0)
            {                
                foreach (CatalogDto.CatalogLanguageRow row in catalogDto.CatalogLanguage.Rows)
                {
                    languageList.Add(row.LanguageCode.ToLower());
                }
            }

            // Bind controls
            SeoCtrl.Controls.Clear();

            foreach (string langCode in languageList)
            {
                Control seoControl = LoadControl("../Modules/SeoTab.ascx");
                ((SeoTab)seoControl).LanguageCode = langCode;

                if (_CatalogEntryDto != null && _CatalogEntryDto.CatalogItemSeo.Count > 0)
                {
                    CatalogEntryDto.CatalogItemSeoRow[] rows = (CatalogEntryDto.CatalogItemSeoRow[])_CatalogEntryDto.CatalogItemSeo.Select(String.Format("LanguageCode='{0}'", langCode));
                    if (rows != null && rows.Length > 0)
                        ((SeoTab)seoControl).Row = rows[0];
                }

                SeoCtrl.Controls.Add(seoControl);
            }

            SeoCtrl.DataBind();
        }

        #region IAdminTabControl Members
        /// <summary>
        /// Saves the changes.
        /// </summary>
        /// <param name="context">The context.</param>
        public void SaveChanges(IDictionary context)
        {
            CatalogEntryDto dto = (CatalogEntryDto)context["CatalogEntryDto"];

            /*
			if (CatalogEntryId > 0 && dto == null)
				dto = CatalogContext.Current.GetCatalogEntryDto(CatalogEntryId);
			else if (CatalogEntryId == 0)
				dto = new CatalogEntryDto();
             * */

            foreach (UserControl ctrl in SeoCtrl.Controls)
            {
                IDictionary newContext = new ListDictionary();
                CatalogEntryDto.CatalogItemSeoRow seoRow = null;

                if (dto.CatalogItemSeo.Count > 0)
                {
                    // find appropriate row
                    DataRow[] rows = dto.CatalogItemSeo.Select(String.Format("LanguageCode = '{0}'", ((SeoTab)ctrl).LanguageCode));

                    if (rows.Length > 0)
                        seoRow = (CatalogEntryDto.CatalogItemSeoRow)rows[0];
                    else
                    {
                        seoRow = dto.CatalogItemSeo.NewCatalogItemSeoRow();
                        seoRow.ApplicationId = CatalogConfiguration.Instance.ApplicationId;
                        seoRow.CatalogEntryId = CatalogEntryId == 0 ? -1 : CatalogEntryId;
                        seoRow.Uri = String.Empty;
                    }
                }
                else
                {
                    seoRow = dto.CatalogItemSeo.NewCatalogItemSeoRow();
                    seoRow.ApplicationId = CatalogConfiguration.Instance.ApplicationId;
                    seoRow.CatalogEntryId = CatalogEntryId == 0 ? -1 : CatalogEntryId;
                    seoRow.Uri = String.Empty;
                }

                // Auto generate the URL if empty
                if (String.IsNullOrEmpty(seoRow.Uri))
                {
					string name = dto.CatalogEntry.Count > 0 ? dto.CatalogEntry[0].Name : "";
                    string langCode = ((SeoTab)ctrl).LanguageCode;
                    string url = String.Format("{0}.aspx", CommerceHelper.CleanUrlField(name));

                    int index = 1;
                    while (CatalogContext.Current.GetCatalogEntryByUriDto(url, langCode).CatalogEntry.Count != 0 || CatalogContext.Current.GetCatalogNodeDto(url, langCode).CatalogNode.Count != 0)
                    {
						url = String.Format("{0}-{1}.aspx", CommerceHelper.CleanUrlField(name), index.ToString());
                        index++;
                    }

                    // Check 
                    seoRow.Uri = url;
                }

                newContext.Add("CatalogItemSeoRow", seoRow);

                ((IAdminTabControl)ctrl).SaveChanges(newContext);

                if (seoRow.RowState == DataRowState.Detached)
                    dto.CatalogItemSeo.Rows.Add(seoRow);
            }
        }
        #endregion

        #region IAdminContextControl Members

        /// <summary>
        /// Loads the context.
        /// </summary>
        /// <param name="context">The context.</param>
        public void LoadContext(IDictionary context)
        {
            _CatalogEntryDto = (CatalogEntryDto)context[_CatalogEntryDtoString];
        }

        #endregion
    }
}