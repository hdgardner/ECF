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
    public partial class NodeSeoTab : BaseUserControl, IAdminTabControl
    {
        /// <summary>
        /// Gets the catalog node id.
        /// </summary>
        /// <value>The catalog node id.</value>
		public int CatalogNodeId
		{
			get
			{
				return ManagementHelper.GetIntFromQueryString("nodeid");
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
            BindForm();
        }

        /// <summary>
        /// Binds the form.
        /// </summary>
        private void BindForm()
        {
            List<string> languageList = new List<string>();
            CatalogDto catalogDto = null;
            CatalogNodeDto dto = null;
            if (CatalogNodeId > 0)
            {

                dto = CatalogContext.Current.GetCatalogNodeDto(CatalogNodeId);

                // Get list of languages
				if (dto.CatalogNode.Count > 0)
					catalogDto = CatalogContext.Current.GetCatalogDto(dto.CatalogNode[0].CatalogId);
            }
            else
            {
                // Get list of languages
                catalogDto = CatalogContext.Current.GetCatalogDto(ParentCatalogId);
            }

			if (catalogDto.Catalog.Count > 0)
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

                if (dto != null && dto.CatalogItemSeo.Count > 0)
                {
                    CatalogNodeDto.CatalogItemSeoRow[] rows = (CatalogNodeDto.CatalogItemSeoRow[])dto.CatalogItemSeo.Select(String.Format("LanguageCode='{0}'", langCode));
                    if (rows != null && rows.Length > 0)
                        ((SeoTab)seoControl).Row = rows[0];
                }

                SeoCtrl.Controls.Add(seoControl);
            }

            SeoCtrl.DataBind();
        }

        /// <summary>
        /// Binds a data source to the invoked server control and all its child controls.
        /// </summary>
        public override void DataBind()
        {
            base.DataBind();            
        }


        #region IAdminTabControl Members
        /// <summary>
        /// Saves the changes.
        /// </summary>
        /// <param name="context">The context.</param>
        public void SaveChanges(IDictionary context)
        {
            CatalogNodeDto dto = (CatalogNodeDto)context["CatalogNodeDto"];

            foreach (UserControl ctrl in SeoCtrl.Controls)
            {
                IDictionary newContext = new ListDictionary();
                CatalogNodeDto.CatalogItemSeoRow seoRow = null;

                if (dto.CatalogItemSeo.Count > 0)
                {
                    // find appropriate row
                    DataRow[] rows = dto.CatalogItemSeo.Select(String.Format("LanguageCode = '{0}'", ((SeoTab)ctrl).LanguageCode));

                    if (rows.Length > 0)
                        seoRow = (CatalogNodeDto.CatalogItemSeoRow)rows[0];
                    else
                    {
                        seoRow = dto.CatalogItemSeo.NewCatalogItemSeoRow();
                        seoRow.ApplicationId = CatalogConfiguration.Instance.ApplicationId;
                        seoRow.CatalogNodeId = dto.CatalogNode[0].CatalogNodeId;
                        seoRow.Uri = String.Empty;
                    }
                }
                else
                {
                    seoRow = dto.CatalogItemSeo.NewCatalogItemSeoRow();
                    seoRow.ApplicationId = CatalogConfiguration.Instance.ApplicationId;
                    seoRow.CatalogNodeId = dto.CatalogNode[0].CatalogNodeId;
                    seoRow.Uri = String.Empty;
                }

                // Auto generate the URL if empty
                if (String.IsNullOrEmpty(seoRow.Uri))
                {
                    string name = dto.CatalogNode[0].Name;
                    //string url = String.Format("{0}/{1}.aspx", ((SeoTab)ctrl).LanguageCode, CommerceHelper.CleanUrlField(name));
                    string url = String.Format("{0}.aspx", CommerceHelper.CleanUrlField(name));
                    string langCode = ((SeoTab)ctrl).LanguageCode;
                    int index = 1;
                    while (CatalogContext.Current.GetCatalogEntryByUriDto(url, langCode).CatalogEntry.Count != 0 || CatalogContext.Current.GetCatalogNodeDto(url, langCode).CatalogNode.Count != 0)
                    {
						url = String.Format("{0}-{1}.aspx", CommerceHelper.CleanUrlField(name), index.ToString());
                        index++;
                    }

                    seoRow.Uri = url;
                }

                newContext.Add("CatalogItemSeoRow", seoRow);

                ((IAdminTabControl)ctrl).SaveChanges(newContext);

                if (seoRow.RowState == DataRowState.Detached)
                    dto.CatalogItemSeo.Rows.Add(seoRow);
            }

        }
        #endregion
    }
}