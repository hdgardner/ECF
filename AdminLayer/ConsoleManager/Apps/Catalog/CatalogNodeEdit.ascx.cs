using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using Mediachase.Commerce.Catalog;
using Mediachase.Commerce.Catalog.Dto;
using Mediachase.Commerce.Catalog.Managers;
using Mediachase.Commerce.Manager.Core;
using Mediachase.Data.Provider;
using Mediachase.Web.Console.BaseClasses;
using Mediachase.Web.Console.Common;

namespace Mediachase.Commerce.Manager.Catalog
{
    public partial class CatalogNodeEdit : CatalogBaseUserControl
    {
        private const string _CatalogNodeDtoString = "CatalogNodeDto";

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

        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            //if (!this.IsPostBack)
            LoadContext();
        }

        /// <summary>
        /// Loads the fresh node.
        /// </summary>
        /// <returns></returns>
        private CatalogNodeDto LoadFreshNode()
        {
            CatalogNodeDto node = CatalogContext.Current.GetCatalogNodeDto(CatalogNodeId, new CatalogNodeResponseGroup(CatalogNodeResponseGroup.ResponseGroup.CatalogNodeFull));

            // persist in session
            Session[_CatalogNodeDtoString] = node;

            return node;
        }

        /// <summary>
        /// Loads the context.
        /// </summary>
        private void LoadContext()
        {
            CatalogNodeDto node = null;

            if (CatalogNodeId > 0)
            {
                if (!this.IsPostBack && (!this.Request.QueryString.ToString().Contains("Callback=yes"))) // load fresh on initial load
                {
                    node = LoadFreshNode();

                    if (node == null)
                        node = new CatalogNodeDto();
                }
                else // load from session
                {
                    node = (CatalogNodeDto)Session[_CatalogNodeDtoString];

                    if (node == null)
                        node = LoadFreshNode();
                }
            }
            else
            {
                node = (CatalogNodeDto)Session[_CatalogNodeDtoString];

                // create new dto objects if they are null
                if(node == null)
                    node = new CatalogNodeDto();
            }


            if (CatalogNodeId > 0 && node.CatalogNode.Count == 0)
                Response.Redirect("ContentFrame.aspx?_a=Catalog&_v=Catalog-List");

            // Put a dictionary key that can be used by other tabs
            IDictionary dic = new ListDictionary();
            dic.Add(_CatalogNodeDtoString, node);

            // Call tabs load context
            ViewControl.LoadContext(dic);
        }


        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Init"/> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> object that contains the event data.</param>
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            EditSaveControl.SaveChanges += new EventHandler<SaveControl.SaveEventArgs>(EditSaveControl_SaveChanges);
        }

        /// <summary>
        /// Handles the SaveChanges event of the EditSaveControl control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		void EditSaveControl_SaveChanges(object sender, SaveControl.SaveEventArgs e)
        {
			if (!this.Page.IsValid)
			{
				e.RunScript = false;
				return;
			}

            CatalogNodeDto dto = null;

            if (CatalogNodeId > 0)
                dto = CatalogContext.Current.GetCatalogNodeDto(CatalogNodeId);
            else
            {
                dto = new CatalogNodeDto();                
            }

            // Put a dictionary key that can be used by other tabs
            IDictionary dic = new ListDictionary();
            dic.Add(_CatalogNodeDtoString, dto);

            using (TransactionScope scope = new TransactionScope())
            {
                // Call tabs save
                ViewControl.SaveChanges(dic);

                // Save modifications
                if (dto.HasChanges())
                    CatalogContext.Current.SaveCatalogNode(dto);

				ViewControl.CommitChanges(dic);

                scope.Complete();
            }

			// we don't need to store Dto in session any more
			Session.Remove(_CatalogNodeDtoString);
        }
    }
}