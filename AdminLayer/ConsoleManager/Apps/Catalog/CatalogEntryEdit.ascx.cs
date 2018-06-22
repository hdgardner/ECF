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
using Mediachase.Web.Console.Common;
using Mediachase.Commerce.Catalog.Dto;
using Mediachase.Commerce.Catalog;
using Mediachase.Commerce.Catalog.Managers;
using Mediachase.Data.Provider;
using Mediachase.Commerce.Manager.Core;

namespace Mediachase.Commerce.Manager.Catalog
{
    public partial class CatalogEntryEdit : CatalogBaseUserControl
    {
        private const string _CatalogEntryDtoString = "CatalogEntryDto";
		private const string _CatalogRelationDtoString = "CatalogRelationDto";
		private const string _CatalogAssociationDtoString = "CatalogAssociationDto";

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
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            LoadContext();
            //if (!this.IsPostBack)
            //    DataBind();
        }

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Init"/> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> object that contains the event data.</param>
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            EditSaveControl.SaveChanges += new EventHandler<SaveControl.SaveEventArgs>(EditSaveControl_SaveChanges);
            ViewControl.ViewId = Request.QueryString["_v"];
        }

        /// <summary>
        /// Loads the entry.
        /// </summary>
        /// <returns></returns>
		private CatalogEntryDto LoadEntry()
		{
			return CatalogContext.Current.GetCatalogEntryDto(CatalogEntryId, new CatalogEntryResponseGroup(CatalogEntryResponseGroup.ResponseGroup.CatalogEntryFull));
		}

        /// <summary>
        /// Loads the relation.
        /// </summary>
        /// <returns></returns>
		private CatalogRelationDto LoadRelation()
		{
			return CatalogContext.Current.GetCatalogRelationDto(ParentCatalogId, ParentCatalogNodeId, CatalogEntryId, String.Empty, new CatalogRelationResponseGroup(CatalogRelationResponseGroup.ResponseGroup.NodeEntry | CatalogRelationResponseGroup.ResponseGroup.CatalogEntry));
		}

        /// <summary>
        /// Loads the association.
        /// </summary>
        /// <returns></returns>
		private CatalogAssociationDto LoadAssociation()
		{
			return CatalogContext.Current.GetCatalogAssociationDtoByEntryId(CatalogEntryId);
		}

        /// <summary>
        /// Loads the fresh entry.
        /// </summary>
        /// <returns></returns>
        private CatalogEntryDto LoadFreshEntry()
        {
			CatalogEntryDto entry = LoadEntry();

			// persist in session
			Session[_CatalogEntryDtoString] = entry;

			return entry;
        }

        /// <summary>
        /// Loads the fresh relation.
        /// </summary>
        /// <returns></returns>
		private CatalogRelationDto LoadFreshRelation()
		{
			CatalogRelationDto relation = LoadRelation();

			// persist in session
			Session[_CatalogRelationDtoString] = relation;

			return relation;
		}

        /// <summary>
        /// Loads the fresh association.
        /// </summary>
        /// <returns></returns>
		private CatalogAssociationDto LoadFreshAssociation()
		{
			CatalogAssociationDto association = LoadAssociation();

			// persist in session
			Session[_CatalogAssociationDtoString] = association;

			return association;
		}

        /// <summary>
        /// Creates the empty dtos.
        /// </summary>
        /// <param name="entry">The entry.</param>
        /// <param name="relation">The relation.</param>
        /// <param name="association">The association.</param>
        /// <param name="persistInSession">if set to <c>true</c> [persist in session].</param>
		private void CreateEmptyDtos(ref CatalogEntryDto entry, ref CatalogRelationDto relation, ref CatalogAssociationDto association, bool persistInSession)
		{
			if (relation == null)
			{
				relation = new CatalogRelationDto();
				if (persistInSession)
					Session[_CatalogRelationDtoString] = relation;
			}

			if (association == null)
			{
				association = new CatalogAssociationDto();
				if (persistInSession)
					Session[_CatalogAssociationDtoString] = association;
			}

			if (entry == null)
			{
				entry = new CatalogEntryDto();
				if (persistInSession)
					Session[_CatalogEntryDtoString] = entry;
			}
		}

        /// <summary>
        /// Loads the context.
        /// </summary>
		private void LoadContext()
		{
			CatalogEntryDto entry = null;
			CatalogRelationDto relation = null;
			CatalogAssociationDto association = null;

			if (!this.IsPostBack && (!this.Request.QueryString.ToString().Contains("Callback=yes"))) // load fresh on initial load
			{
				entry = LoadFreshEntry();
				relation = LoadFreshRelation();
				association = LoadFreshAssociation();

				// if Dtos not loaded, create empty Dtos
				CreateEmptyDtos(ref entry, ref relation, ref association, true);
			}
			else // load from session
			{
				entry = (CatalogEntryDto)Session[_CatalogEntryDtoString];

				if (entry == null)
					entry = LoadFreshEntry();

				association = (CatalogAssociationDto)Session[_CatalogAssociationDtoString];

				if (association == null)
					association = LoadFreshAssociation();

				relation = (CatalogRelationDto)Session[_CatalogRelationDtoString];

				if (relation == null)
					relation = LoadFreshRelation();
			}

			if (CatalogEntryId > 0 && entry.CatalogEntry.Count == 0)
				Response.Redirect("ContentFrame.aspx?_a=Catalog&_v=Catalog-List");

			// Put a dictionary key that can be used by other tabs
			IDictionary dic = new ListDictionary();
			dic.Add(_CatalogEntryDtoString, entry);
			dic.Add(_CatalogRelationDtoString, relation);
			dic.Add(_CatalogAssociationDtoString, association);

			// Call tabs load context
			ViewControl.LoadContext(dic);
		}

        /// <summary>
        /// Handles the SaveChanges event of the EditSaveControl control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="Mediachase.Commerce.Manager.Core.SaveControl.SaveEventArgs"/> instance containing the event data.</param>
        void EditSaveControl_SaveChanges(object sender, SaveControl.SaveEventArgs e)
        {
            // Validate form
            if (!this.Page.IsValid)
            {
                e.RunScript = false;
                return;
            }

            CatalogEntryDto dto = null;
            CatalogRelationDto relation = null;
			CatalogAssociationDto association = null;

			using (TransactionScope scope = new TransactionScope())
			{
				if (CatalogEntryId > 0)
				{
					dto = (CatalogEntryDto)Session[_CatalogEntryDtoString];
					relation = (CatalogRelationDto)Session[_CatalogRelationDtoString];
					association = (CatalogAssociationDto)Session[_CatalogAssociationDtoString];
				}

				if (association == null && CatalogEntryId > 0)
					association = LoadAssociation();

				if (relation == null && CatalogEntryId > 0)
					relation = LoadRelation();

				if (dto == null && CatalogEntryId > 0)
					dto = LoadEntry();

				CreateEmptyDtos(ref dto, ref relation, ref association, true);

				// Put a dictionary key that can be used by other tabs
				IDictionary dic = new ListDictionary();
				dic.Add(_CatalogEntryDtoString, dto);
				dic.Add(_CatalogRelationDtoString, relation);
				dic.Add(_CatalogAssociationDtoString, association);

				// Call tabs save
				ViewControl.SaveChanges(dic);

				// Save modifications
				if (dto.HasChanges())
					CatalogContext.Current.SaveCatalogEntry(dto);

				// get current CatalogEntryId
				int currentCatalogEntryId = this.CatalogEntryId;
				if (dto.CatalogEntry != null && dto.CatalogEntry.Rows.Count > 0)
					currentCatalogEntryId = dto.CatalogEntry[0].CatalogEntryId;

				// Modify relationship
				CatalogRelationDto.NodeEntryRelationRow relRow = null;

				// Find existing row
				if (relation.NodeEntryRelation.Count > 0)
				{
					foreach (CatalogRelationDto.NodeEntryRelationRow row in relation.NodeEntryRelation.Rows)
					{
						if (row.CatalogEntryId == currentCatalogEntryId && row.CatalogId == ParentCatalogId && row.CatalogNodeId == ParentCatalogNodeId)
						{
							relRow = row;
							break;
						}
					}
				}

				// If no existing record found, create a new one
				if (ParentCatalogId > 0 && ParentCatalogNodeId > 0)
				{
					if (relRow == null)
					{
						relRow = relation.NodeEntryRelation.NewNodeEntryRelationRow();
					}

					if (ParentCatalogId > 0)
						relRow.CatalogId = ParentCatalogId;

					if (this.ParentCatalogNodeId > 0)
						relRow.CatalogNodeId = this.ParentCatalogNodeId;

					relRow.CatalogEntryId = currentCatalogEntryId;

					// Attach if it is a new row
					if (relRow.RowState == DataRowState.Detached)
					{
						relRow.SortOrder = 0;
						relation.NodeEntryRelation.Rows.Add(relRow);
					}
				}

				// Update newly added entry relationships with a parent catalog entry id
				if (relation.CatalogEntryRelation.Rows.Count > 0)
				{
					foreach (CatalogRelationDto.CatalogEntryRelationRow row in relation.CatalogEntryRelation.Rows)
					{
						if (row.RowState == DataRowState.Added && row.ParentEntryId <= 0)
							row.ParentEntryId = currentCatalogEntryId;
					}
				}

				if (relation.HasChanges())
					CatalogContext.Current.SaveCatalogRelationDto(relation);

				// Update newly added entry relationships with a parent catalog entry id
				if (association.CatalogAssociation.Rows.Count > 0)
				{
					foreach (CatalogAssociationDto.CatalogAssociationRow row in association.CatalogAssociation.Rows)
					{
						if (row.RowState == DataRowState.Added && row.CatalogEntryId <= 0)
							row.CatalogEntryId = currentCatalogEntryId;
					}
				}

                // Save association modifications
				if (association.HasChanges())
					CatalogContext.Current.SaveCatalogAssociation(association);

				// Call commit changes
				ViewControl.CommitChanges(dic);

                // Save modifications
                if (dto.HasChanges())
                    CatalogContext.Current.SaveCatalogEntry(dto);

                // Save relation modifications
                if (relation.HasChanges())
                    CatalogContext.Current.SaveCatalogRelationDto(relation);

                // Save association modifications
                if (association.HasChanges())
                    CatalogContext.Current.SaveCatalogAssociation(association);

				// Complete transaction
				scope.Complete();

				// we don't need to store Dto in session any more
				Session.Remove(_CatalogEntryDtoString);
				Session.Remove(_CatalogRelationDtoString);
				Session.Remove(_CatalogAssociationDtoString);
			}
        }
    }
}