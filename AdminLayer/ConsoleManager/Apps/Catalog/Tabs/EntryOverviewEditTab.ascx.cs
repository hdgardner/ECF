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
using Mediachase.MetaDataPlus;
using Mediachase.Cms;
using Mediachase.Cms.Dto;
using Mediachase.Cms.Managers;
using Mediachase.Data.Provider;
using Mediachase.Commerce.Storage;

namespace Mediachase.Commerce.Manager.Catalog.Tabs
{
    public partial class EntryOverviewEditTab : CatalogBaseUserControl, IAdminTabControl, IPreCommit, IAdminContextControl
    {
        private const string _CatalogEntryDtoString = "CatalogEntryDto";
        private const string _CatalogRelationDtoString = "CatalogRelationDto";
        private CatalogEntryDto _CatalogEntryDto = null;
        private CatalogRelationDto _CatalogRelationDto = null;

		#region Public Properties
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
        /// Gets the parent catalog node id.
        /// </summary>
        /// <value>The parent catalog node id.</value>
		public int ParentCatalogNodeId
		{
			get
			{
				return ManagementHelper.GetIntFromQueryString("catalognodeid");
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
        /// Gets the type of the entry.
        /// </summary>
        /// <value>The type of the entry.</value>
        public string EntryType
        {
            get
            {
                return Request.QueryString["type"];
            }
		}
		#endregion

		/// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            MetaClassList.SelectedIndexChanged += new EventHandler(MetaClassList_SelectedIndexChanged);

            if (!Page.IsPostBack)
                BindForm();

            //if(!this.IsPostBack)
            BindMetaForm();
        }

        /// <summary>
        /// Handles the SelectedIndexChanged event of the MetaClassList control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        void MetaClassList_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindMetaForm();
        }

        /// <summary>
        /// Checks the entry code.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="System.Web.UI.WebControls.ServerValidateEventArgs"/> instance containing the event data.</param>
        public void EntryCodeCheck(object sender, ServerValidateEventArgs args)
        {
            CatalogEntryDto entryDto = CatalogContext.Current.GetCatalogEntryDto(CodeText.Text);

            if (entryDto.CatalogEntry.Count > 0 && entryDto.CatalogEntry[0].CatalogEntryId != CatalogEntryId)
            {
                args.IsValid = false;
                return;
            }

            args.IsValid = true;
        }

        /// <summary>
        /// Binds the form.
        /// </summary>
        private void BindForm()
        {
            // Bind Meta classes
            MetaClass catalogEntry = MetaClass.Load(CatalogContext.MetaDataContext, "CatalogEntry");
            MetaClassList.Items.Clear();
            if (catalogEntry != null)
            {
                MetaClassCollection metaClasses = catalogEntry.ChildClasses;
                foreach (MetaClass metaClass in metaClasses)
                    MetaClassList.Items.Add(new ListItem(metaClass.FriendlyName, metaClass.Id.ToString()));

                MetaClassList.DataBind();
            }

            // Bind Templates
            TemplateDto templates = DictionaryManager.GetTemplateDto();
            if (templates.main_Templates.Count > 0)
            {
                DataView view = templates.main_Templates.DefaultView;
                view.RowFilter = "TemplateType = 'entry'";
                DisplayTemplate.DataSource = view;
                DisplayTemplate.DataBind();
            }

            if (CatalogEntryId > 0)
            {
                if (_CatalogEntryDto.CatalogEntry.Count > 0)
                {
                    Name.Text = _CatalogEntryDto.CatalogEntry[0].Name;
                    AvailableFrom.Value = ManagementHelper.GetUserDateTime(_CatalogEntryDto.CatalogEntry[0].StartDate);
					ExpiresOn.Value = ManagementHelper.GetUserDateTime(_CatalogEntryDto.CatalogEntry[0].EndDate);
                    CodeText.Text = _CatalogEntryDto.CatalogEntry[0].Code;
                    IsActive.IsSelected = _CatalogEntryDto.CatalogEntry[0].IsActive;

                    ManagementHelper.SelectListItem(DisplayTemplate, _CatalogEntryDto.CatalogEntry[0].TemplateName);
                    ManagementHelper.SelectListItem(MetaClassList, _CatalogEntryDto.CatalogEntry[0].MetaClassId);

                    // Bind Sort order
                    foreach (CatalogRelationDto.NodeEntryRelationRow row in _CatalogRelationDto.NodeEntryRelation)
                    {
                        if (row.CatalogEntryId == _CatalogEntryDto.CatalogEntry[0].CatalogEntryId
                            && row.CatalogId == this.ParentCatalogId
                            && row.CatalogNodeId == this.ParentCatalogNodeId)
                        {
                            SortOrder.Text = row.SortOrder.ToString();
                        }
                    }
                }
            }
            else
            {
                this.AvailableFrom.Value = ManagementHelper.GetUserDateTime(DateTime.UtcNow);
                this.ExpiresOn.Value = ManagementHelper.GetUserDateTime(DateTime.UtcNow).AddYears(1);
            }
        }

        /// <summary>
        /// Binds the meta form.
        /// </summary>
        private void BindMetaForm()
        {
            CatalogDto catalogDto = null;
            if (CatalogEntryId == 0)
            {
                catalogDto = CatalogContext.Current.GetCatalogDto(ParentCatalogId);
            }
            else if (CatalogEntryId > 0)
            {
                MetaDataTab.ObjectId = CatalogEntryId;
                CatalogEntryDto dto = CatalogContext.Current.GetCatalogEntryDto(CatalogEntryId);
                if (dto.CatalogEntry.Count > 0)
                {
                    catalogDto = CatalogContext.Current.GetCatalogDto(dto.CatalogEntry[0].CatalogId);
                    MetaDataTab.MetaClassId = dto.CatalogEntry[0].MetaClassId;
                }
            }

            //if (this.IsPostBack)
            if(!String.IsNullOrEmpty(MetaClassList.SelectedValue))
                MetaDataTab.MetaClassId = Int32.Parse(MetaClassList.SelectedValue);

            if (catalogDto != null)
            {
                List<string> list = new List<string>();
                list.Add(catalogDto.Catalog[0].DefaultLanguage);
                if (catalogDto.CatalogLanguage.Count > 0)
                {
                    foreach (CatalogDto.CatalogLanguageRow row in catalogDto.CatalogLanguage.Rows)
                    {
                        list.Add(row.LanguageCode);
                    }
                }

                MetaDataTab.Languages = list.ToArray();
                MetaDataTab.DataBind();
            }

            // add parent catalognode
            if (ParentCatalogNodeId > 0)
            {
            }
        }

		/// <summary>
		/// Handles the RowChanged event of the CatalogEntry control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.Data.DataRowChangeEventArgs"/> instance containing the event data.</param>
		void CatalogEntry_RowChanged(object sender, DataRowChangeEventArgs e)
		{
			CatalogEntryDto.CatalogEntryDataTable table = (CatalogEntryDto.CatalogEntryDataTable)sender;

			CatalogEntryDto.CatalogEntryRow row = (CatalogEntryDto.CatalogEntryRow)table.Rows[0];
			if (row.CatalogEntryId > 0)
			{
				MetaDataTab.MetaClassId = row.MetaClassId;
				MetaDataTab.ObjectId = row.CatalogEntryId;

			}
		}

        #region IAdminTabControl Members
        /// <summary>
        /// Saves the changes.
        /// </summary>
        /// <param name="context">The context.</param>
        public void SaveChanges(IDictionary context)
        {
            CatalogEntryDto dto = (CatalogEntryDto)context[_CatalogEntryDtoString];
            CatalogEntryDto.CatalogEntryRow row = null;

            if (dto.CatalogEntry == null || dto.CatalogEntry.Count == 0)
            {
                row = dto.CatalogEntry.NewCatalogEntryRow();
                row.ApplicationId = CatalogConfiguration.Instance.ApplicationId;
                row.ClassTypeId = EntryType;
            }
            else
            {
                row = dto.CatalogEntry[0];
                if (row.MetaClassId != Int32.Parse(MetaClassList.SelectedValue))
                {
                    MetaObject.Delete(CatalogContext.MetaDataContext, row.CatalogEntryId, row.MetaClassId);
                }
            }

            row.Name = Name.Text;
            row.StartDate = AvailableFrom.Value.ToUniversalTime();
            row.EndDate = ExpiresOn.Value.ToUniversalTime();
            row.Code = CodeText.Text;
            row.IsActive = IsActive.IsSelected;

            if (ParentCatalogId > 0 && row.RowState == DataRowState.Detached)
                row.CatalogId = ParentCatalogId;

            row.TemplateName = DisplayTemplate.SelectedValue;
            row.MetaClassId = Int32.Parse(MetaClassList.SelectedValue);

            if (row.RowState == DataRowState.Detached)
                dto.CatalogEntry.Rows.Add(row);

            dto.CatalogEntry.RowChanged += new DataRowChangeEventHandler(CatalogEntry_RowChanged);
        }
		#endregion

        #region IPreCommit Members
        /// <summary>
        /// Pre-commit changes.
        /// </summary>
        /// <param name="context">The context.</param>
        public void PreCommitChanges(IDictionary context)
        {
			CatalogEntryDto dto = (CatalogEntryDto)context[_CatalogEntryDtoString];
			CatalogRelationDto relationDto = (CatalogRelationDto)context[_CatalogRelationDtoString];

            // Save Meta Data
            IDictionary dic = new ListDictionary();
            MetaDataTab.MDContext = CatalogContext.MetaDataContext;
            MetaDataTab.SaveChanges(dic);
            byte[] serializedObject = ((MetaObjectSerialized)dic["MetaObjectSerialized"]).BinaryValue;
            dto.CatalogEntry[0][MetaObjectSerialized.SerializedFieldName] = serializedObject;

            // Update relations
            foreach (CatalogRelationDto.NodeEntryRelationRow row in relationDto.NodeEntryRelation)
            {
                if (row.CatalogEntryId == dto.CatalogEntry[0].CatalogEntryId 
                    && row.CatalogId == this.ParentCatalogId
                    && row.CatalogNodeId == this.ParentCatalogNodeId)
                {
                    row.SortOrder = Int32.Parse(SortOrder.Text);
                }
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
            _CatalogRelationDto = (CatalogRelationDto)context[_CatalogRelationDtoString];
        }
        #endregion
    }
}