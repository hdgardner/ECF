using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Mediachase.Cms.Pages;
using Mediachase.Cms.Managers;
using Mediachase.Cms.Dto;
using System.Data;
using Mediachase.Cms.Util;
using Mediachase.Commerce.Catalog;
using Mediachase.Commerce.Catalog.Dto;
using Mediachase.Commerce;

namespace Mediachase.Cms.Website.Structure.Base.Controls.Menu
{
    public partial class PP_SiteMenu : System.Web.UI.UserControl, IPropertyPage
    {
        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            DataSourceList.SelectedIndexChanged += new EventHandler(DataSourceList_SelectedIndexChanged);
        }

        /// <summary>
        /// Handles the SelectedIndexChanged event of the DataSourceList control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        void DataSourceList_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindSourceMembers(DataSourceList.SelectedValue);
        }

        #region IPropertyPage Members

        /// <summary>
        /// Gets the title.
        /// </summary>
        /// <value>The title.</value>
        public string Title
        {
            get { return "Site Menu View"; }
        }

        /// <summary>
        /// Gets the description.
        /// </summary>
        /// <value>The description.</value>
        public string Description
        {
            get { return "Displays site menu using template specified."; }
        }

        /// <summary>
        /// Loads the specified node uid.
        /// </summary>
        /// <param name="NodeUid">The node uid.</param>
        /// <param name="ControlUid">The control uid.</param>
        void IPropertyPage.Load(string NodeUid, string ControlUid)
        {
            ControlSettings settings = new ControlSettings();

            DynamicNode dNode = PageDocument.Current.DynamicNodes.LoadByUID(NodeUid);
            if (dNode != null)
                settings = dNode.GetSettings(NodeUid);
            else
                settings = PageDocument.Current.StaticNode.Controls[NodeUid];

            // Bind templates
            DisplayTemplate.Items.Clear();
            DisplayTemplate.Items.Add(new ListItem("(use default)", ""));
            TemplateDto templates = DictionaryManager.GetTemplateDto();
            if (templates.main_Templates.Count > 0)
            {
                DataView view = templates.main_Templates.DefaultView;
                view.RowFilter = "TemplateType = 'menu'";

                foreach (DataRowView row in view)
                {
                    DisplayTemplate.Items.Add(new ListItem(row["FriendlyName"].ToString(), row["Name"].ToString()));
                }

                DisplayTemplate.DataBind();
            }

            string selectedMember = String.Empty;
            if (settings != null && settings.Params != null)
            {
                Param prm = settings.Params;

                if ((prm["DisplayTemplate"] != null) && (prm["DisplayTemplate"] is string))
                {
                    CommonHelper.SelectListItem(DisplayTemplate, prm["DisplayTemplate"].ToString());
                }

                if ((prm["DataSource"] != null) && (prm["DataSource"] is string))
                {
                    CommonHelper.SelectListItem(DataSourceList, prm["DataSource"].ToString());
                }

                if ((prm["DataMember"] != null) && (prm["DataMember"] is string))
                {
                    selectedMember = prm["DataMember"].ToString();
                }
            }

            BindSourceMembers(selectedMember);
        }

        /// <summary>
        /// Binds the source members.
        /// </summary>
        /// <param name="source">The source.</param>
        private void BindSourceMembers(string source)
        {
            // Bind Data source members
            DSMemberList.Items.Clear();
            if (DataSourceList.SelectedValue.Equals("catalog", StringComparison.OrdinalIgnoreCase))
            {
                CatalogDto catalogs = CatalogContext.Current.GetCatalogDto(CMSContext.Current.SiteId);
                if (catalogs.Catalog.Count > 0)
                {
                    foreach (CatalogDto.CatalogRow row in catalogs.Catalog)
                    {
                        if (row.IsActive && row.StartDate <= FrameworkContext.Current.CurrentDateTime && row.EndDate >= FrameworkContext.Current.CurrentDateTime)
                            DSMemberList.Items.Add(new ListItem(row.Name, row.Name));
                    }

                    DSMemberList.DataBind();
                }
            }
            else
            {
                IDataReader reader = MenuItem.LoadAllRoot(CMSContext.Current.SiteId);
                while (reader.Read())
                {
                    DSMemberList.Items.Add(new ListItem(reader["Text"].ToString(), reader["MenuId"].ToString()));
                }
                reader.Close();
                DSMemberList.DataBind();
            }

            if (!String.IsNullOrEmpty(source))
                CommonHelper.SelectListItem(DSMemberList, source);
        }

        /// <summary>
        /// Saves the specified node uid.
        /// </summary>
        /// <param name="NodeUid">The node uid.</param>
        /// <param name="ControlUid">The control uid.</param>
        public void Save(string NodeUid, string ControlUid)
        {
            ControlSettings settings = new ControlSettings();

            DynamicNode dNode = PageDocument.Current.DynamicNodes.LoadByUID(NodeUid);
            if (dNode != null)
            {
                settings = dNode.GetSettings(dNode.NodeUID);
                dNode.IsModified = true;
            }
            else
            {
                settings = PageDocument.Current.StaticNode.Controls[NodeUid];
                if (settings == null)
                {
                    settings = new ControlSettings();
                    PageDocument.Current.StaticNode.Controls.Add(NodeUid, settings);
                }
            }

            if (settings.Params == null)
            {
                settings.Params = new Param();
            }

            settings.IsModified = true;

            CommonHelper.SaveParameter(settings, "DisplayTemplate", DisplayTemplate.SelectedValue);
            CommonHelper.SaveParameter(settings, "DataSource", DataSourceList.SelectedValue);
            CommonHelper.SaveParameter(settings, "DataMember", DSMemberList.SelectedValue);
        }

        #endregion
    }
}