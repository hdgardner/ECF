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

namespace Mediachase.Commerce.Manager.Catalog.Tabs
{
    public partial class NodeOverviewEditTab : CatalogBaseUserControl, IAdminTabControl, IPreCommit, IAdminContextControl
    {
        private const string _CatalogNodeDtoString = "CatalogNodeDto";
        private CatalogNodeDto _CatalogNodeDto = null;

		#region Public Properties
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
		/// Checks the node code.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="args">The <see cref="System.Web.UI.WebControls.ServerValidateEventArgs"/> instance containing the event data.</param>
		protected void NodeCodeCheck(object sender, ServerValidateEventArgs args)
		{
			CatalogNodeDto dto = CatalogContext.Current.GetCatalogNodeDto(CodeText.Text);

			if (dto.CatalogNode.Count > 0 && dto.CatalogNode[0].CatalogNodeId != CatalogNodeId)
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
            MetaClass catalogNode = MetaClass.Load(CatalogContext.MetaDataContext, "CatalogNode");
            MetaClassList.Items.Clear();
            if (catalogNode != null)
            {
                MetaClassCollection metaClasses = catalogNode.ChildClasses;
                foreach (MetaClass metaClass in metaClasses)
                {
                    MetaClassList.Items.Add(new ListItem(metaClass.FriendlyName, metaClass.Id.ToString()));
                }
                MetaClassList.DataBind();
            }

            // Bind Templates
            TemplateDto templates = DictionaryManager.GetTemplateDto();
            if (templates.main_Templates.Count > 0)
            {
                DataView view = templates.main_Templates.DefaultView;
                view.RowFilter = "TemplateType = 'node'";
                DisplayTemplate.DataSource = view;
                DisplayTemplate.DataBind();
            }

            if (CatalogNodeId > 0)
            {
                if (_CatalogNodeDto.CatalogNode.Count > 0)
                {
                    Name.Text = _CatalogNodeDto.CatalogNode[0].Name;
					AvailableFrom.Value = ManagementHelper.GetUserDateTime(_CatalogNodeDto.CatalogNode[0].StartDate);
                    ExpiresOn.Value = ManagementHelper.GetUserDateTime(_CatalogNodeDto.CatalogNode[0].EndDate);
                    CodeText.Text = _CatalogNodeDto.CatalogNode[0].Code;
                    SortOrder.Text = _CatalogNodeDto.CatalogNode[0].SortOrder.ToString();
                    IsCatalogNodeActive.IsSelected = _CatalogNodeDto.CatalogNode[0].IsActive;

                    ManagementHelper.SelectListItem(DisplayTemplate, _CatalogNodeDto.CatalogNode[0].TemplateName);
                    ManagementHelper.SelectListItem(MetaClassList, _CatalogNodeDto.CatalogNode[0].MetaClassId);
                }
            }
            else
            {
                this.AvailableFrom.Value = DateTime.Now;
                this.ExpiresOn.Value = DateTime.Now.AddYears(1);
                this.SortOrder.Text = "0";
            }
        }

        /// <summary>
        /// Binds the meta form.
        /// </summary>
        private void BindMetaForm()
        {
            CatalogDto catalogDto = null;

            if (CatalogNodeId == 0)
            {
                catalogDto = CatalogContext.Current.GetCatalogDto(ParentCatalogId);
            }
            else if (CatalogNodeId > 0)
            {
                MetaDataTab.ObjectId = CatalogNodeId;
                CatalogNodeDto dto = CatalogContext.Current.GetCatalogNodeDto(CatalogNodeId);
                if (dto.CatalogNode.Count > 0)
                {
                    catalogDto = CatalogContext.Current.GetCatalogDto(dto.CatalogNode[0].CatalogId);
                    MetaDataTab.MetaClassId = dto.CatalogNode[0].MetaClassId;
                }
            }

            //if (this.IsPostBack)
            MetaDataTab.MetaClassId = Int32.Parse(MetaClassList.SelectedValue);

			if (catalogDto != null && catalogDto.Catalog.Count > 0)
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
        }

        #region IAdminTabControl Members
        /// <summary>
        /// Saves the changes.
        /// </summary>
        /// <param name="context">The context.</param>
        public void SaveChanges(IDictionary context)
        {
            CatalogNodeDto dto = (CatalogNodeDto)context["CatalogNodeDto"];

            CatalogNodeDto.CatalogNodeRow row = null;

            if (dto.CatalogNode == null || dto.CatalogNode.Count == 0)
            {
                row = dto.CatalogNode.NewCatalogNodeRow();
                row.ApplicationId = CatalogConfiguration.Instance.ApplicationId;
            }
            else
            {
                row = dto.CatalogNode[0];
                if (row.MetaClassId != Int32.Parse(MetaClassList.SelectedValue))
                {
                    MetaObject.Delete(CatalogContext.MetaDataContext, row.CatalogNodeId, row.MetaClassId);
                }
            }

            row.Name = Name.Text;
            row.StartDate = AvailableFrom.Value.ToUniversalTime();
            row.EndDate = ExpiresOn.Value.ToUniversalTime();
            row.Code = CodeText.Text;
            row.SortOrder = Int32.Parse(SortOrder.Text);
            row.IsActive = IsCatalogNodeActive.IsSelected;

            if (ParentCatalogId > 0)
            {
                row.CatalogId = ParentCatalogId;

                if ((ParentCatalogNodeId > 0 && ParentCatalogNodeId != row.CatalogNodeId) || ParentCatalogNodeId == 0)
                    row.ParentNodeId = ParentCatalogNodeId;
            }

            row.TemplateName = DisplayTemplate.SelectedValue;
            row.MetaClassId = Int32.Parse(MetaClassList.SelectedValue);

            if (row.RowState == DataRowState.Detached)
                dto.CatalogNode.Rows.Add(row);

            dto.CatalogNode.RowChanged += new DataRowChangeEventHandler(CatalogNode_RowChanged);
        }

        /// <summary>
        /// Handles the RowChanged event of the CatalogNode control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Data.DataRowChangeEventArgs"/> instance containing the event data.</param>
        void CatalogNode_RowChanged(object sender, DataRowChangeEventArgs e)
        {
            CatalogNodeDto.CatalogNodeDataTable table = (CatalogNodeDto.CatalogNodeDataTable)sender;

            CatalogNodeDto.CatalogNodeRow row = (CatalogNodeDto.CatalogNodeRow)table.Rows[0];
            if (row.CatalogNodeId > 0)
            {
                MetaDataTab.MetaClassId = row.MetaClassId;
                MetaDataTab.ObjectId = row.CatalogNodeId;
            }
        }
        #endregion

        #region IPreCommit Members

        /// <summary>
        /// Pre-commit changes.
        /// </summary>
        /// <param name="context">The context.</param>
        public void PreCommitChanges(IDictionary context)
        {
            MetaDataTab.MDContext = CatalogContext.MetaDataContext;
            MetaDataTab.SaveChanges(null);
        }
        #endregion
		
        #region IAdminContextControl Members

        /// <summary>
        /// Loads the context.
        /// </summary>
        /// <param name="context">The context.</param>
        public void LoadContext(IDictionary context)
        {
            _CatalogNodeDto = (CatalogNodeDto)context[_CatalogNodeDtoString];
        }

        #endregion
    }
}