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
using System.Web.UI.HtmlControls;
using Mediachase.MetaDataPlus;
using Mediachase.Commerce.Catalog;
using Mediachase.Commerce.Catalog.Dto;
using Mediachase.Commerce.Catalog.Managers;
using Mediachase.Web.Console.BaseClasses;
using Mediachase.Web.Console.Common;
using Mediachase.Web.Console.Interfaces;

namespace Mediachase.Commerce.Manager.Catalog.Tabs
{
	public partial class EntryAssociationEditPopup : CatalogBaseUserControl, IAdminContextControl
	{
		private const string _CatalogAssociationDtoString = "CatalogAssociationDto";
		private const string _CatalogEntryDtoString = "CatalogEntryDto";

		int _AssociationId;
		CatalogAssociationDto _CatalogAssociationDto = new CatalogAssociationDto();
		CatalogEntryDto _CatalogEntryDto = new CatalogEntryDto();

        /// <summary>
        /// Gets or sets the association id.
        /// </summary>
        /// <value>The association id.</value>
		public int AssociationId
		{
			get
			{
				if (_AssociationId == 0)
				{
					if (!String.IsNullOrEmpty(this.DialogTrigger.Value))
					{
						int index = this.DialogTrigger.Value.IndexOf('|');
						string idString = this.DialogTrigger.Value;

						if (index > 0)
							// cut off CatalogEntryId
							idString = this.DialogTrigger.Value.Substring(0, index);

						_AssociationId = Int32.Parse(idString);
					}
				}

				return _AssociationId;
			}
			set
			{
				_AssociationId = value;
			}
		}

        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		protected void Page_Load(object sender, EventArgs e)
		{
			BindForm(false);
			DialogTrigger.ValueChanged += new EventHandler(DialogTrigger_ValueChanged);
		}

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Init"/> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> object that contains the event data.</param>
		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);
		}

        /// <summary>
        /// Handles the ValueChanged event of the DialogTrigger control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		void DialogTrigger_ValueChanged(object sender, EventArgs e)
		{
			BindForm(true);
		}

        /// <summary>
        /// Handles the Click event of the SaveChangesButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		protected void SaveChangesButton_Click(object sender, EventArgs e)
		{
			CatalogAssociationDto.CatalogAssociationRow row = null;

			if (AssociationId != 0) // find existing
				row = _CatalogAssociationDto.CatalogAssociation.FindByCatalogAssociationId(AssociationId);

			// if not found, create new
			if (row == null)
			{
				row = _CatalogAssociationDto.CatalogAssociation.NewCatalogAssociationRow();
				if (_CatalogEntryDto != null && _CatalogEntryDto.CatalogEntry.Rows.Count > 0)
					row.CatalogEntryId = ((CatalogEntryDto.CatalogEntryRow)_CatalogEntryDto.CatalogEntry.Rows[0]).CatalogEntryId;
				else
					row.CatalogEntryId = 0;
			}

			// update Association fields
			if (row != null)
			{
				row.AssociationName = tbAssociationName.Text;
				row.AssociationDescription = tbDescription.Text;
				row.SortOrder = Int32.Parse(tbSortOrder.Text);

				if (row.RowState == DataRowState.Detached)
					_CatalogAssociationDto.CatalogAssociation.Rows.Add(row);
			}

			System.Text.StringBuilder closeDialog = new System.Text.StringBuilder("EntryAssociationEdit_CloseDialog");
			if (row != null)
				closeDialog.AppendFormat("({0}, '{1}');", row.CatalogAssociationId, row.AssociationName);
			else
				closeDialog.Append("(0, '');");
			ScriptManager.RegisterStartupScript(MetaDataTab, typeof(EntryAssociationEditPopup), "DialogClose", closeDialog.ToString(), true);
		}

        /// <summary>
        /// Binds the form.
        /// </summary>
        /// <param name="reset">if set to <c>true</c> [reset].</param>
		private void BindForm(bool reset)
		{
			CatalogAssociationDto.CatalogAssociationRow selectedAssociationRow = null;
			if (AssociationId != 0)
				selectedAssociationRow = _CatalogAssociationDto.CatalogAssociation.FindByCatalogAssociationId(AssociationId);

			if (selectedAssociationRow != null)
			{
				if (reset)
					SetFormFieldsValues(selectedAssociationRow.AssociationName,
							selectedAssociationRow.AssociationDescription,
							selectedAssociationRow.SortOrder);
			}
			else if (reset)
			{
				SetFormFieldsValues("", "", 1);
				/*if (_CatalogAssociationDto != null)
				{
				}
				else
					SetFormFieldsValues("", "", 1);*/
			}
		}

        /// <summary>
        /// Sets the form fields values.
        /// </summary>
        /// <param name="associationName">Name of the association.</param>
        /// <param name="associationDescription">The association description.</param>
        /// <param name="sortOrder">The sort order.</param>
		private void SetFormFieldsValues(string associationName, string associationDescription, int sortOrder)
		{
			tbAssociationName.Text = associationName;
			tbDescription.Text = associationDescription;
			tbSortOrder.Text = sortOrder.ToString();
		}

		#region IAdminContextControl Members
        /// <summary>
        /// Loads the context.
        /// </summary>
        /// <param name="context">The context.</param>
		public void LoadContext(IDictionary context)
		{
			_CatalogAssociationDto = (CatalogAssociationDto)context[_CatalogAssociationDtoString];
			_CatalogEntryDto = (CatalogEntryDto)context[_CatalogEntryDtoString];
		}
		#endregion
	}
}