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

namespace Mediachase.Commerce.Manager.Catalog.Tabs
{
    public partial class NodeMetaEditTab : BaseUserControl, IAdminTabControl
    {
        /// <summary>
        /// Gets the catalog node id.
        /// </summary>
        /// <value>The catalog node id.</value>
        public int CatalogNodeId
        {
            get
            {
                return ManagementHelper.GetIntFromQueryString("NodeId");
            }
        }

        /// <summary>
        /// Gets the meta class id.
        /// </summary>
        /// <value>The meta class id.</value>
        public int MetaClassId
        {
            get
            {
                return ManagementHelper.GetIntFromQueryString("MetaClassId");
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
            //if (!this.IsPostBack)
                BindForm();
        }

        /// <summary>
        /// Binds the form.
        /// </summary>
        private void BindForm()
        {
            CatalogDto catalogDto = null;
            if (MetaClassId > 0)
            {
                MetaDataTab.MetaClassId = MetaClassId;
                catalogDto = CatalogContext.Current.GetCatalogDto(ParentCatalogId);
            }
            else if (MetaClassId == 0 && CatalogNodeId == 0 && Session["CatalogNode-MetaClassId"]!=null)
            {
                if (!String.IsNullOrEmpty(Session["CatalogNode-MetaClassId"].ToString()))
                {
                    MetaDataTab.MetaClassId = Int32.Parse(Session["CatalogNode-MetaClassId"].ToString());
                    catalogDto = CatalogContext.Current.GetCatalogDto(ParentCatalogId);
                }
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

            if (HttpContext.Current.Items["CatalogNode-MetaClassId"] != null)
                MetaDataTab.MetaClassId = Int32.Parse(HttpContext.Current.Items["CatalogNode-MetaClassId"].ToString());

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

                    MetaDataTab.Languages = list.ToArray();
                }
                MetaDataTab.DataBind();
            }
        }

        /// <summary>
        /// Binds a data source to the invoked server control and all its child controls.
        /// </summary>
        public override void DataBind()
        {
            base.DataBind();
            //if(this.IsPostBack)
            BindForm();   
        }


        #region IAdminTabControl Members
        /// <summary>
        /// Saves the changes.
        /// </summary>
        /// <param name="context">The context.</param>
        public void SaveChanges(IDictionary context)
        {
            CatalogNodeDto dto = (CatalogNodeDto)context["CatalogNodeDto"];
            dto.CatalogNode.RowChanged += new DataRowChangeEventHandler(CatalogNode_RowChanged);
            //MetaDataTab.SaveChanges(context);
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
                MetaDataTab.SaveChanges(null);
            }
            //throw new Exception("The method or operation is not implemented.");
        }
        #endregion
    }
}