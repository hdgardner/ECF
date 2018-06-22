using System;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Web;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using Mediachase.Commerce.Manager.Core.Controls;
using Mediachase.Commerce.Orders;
using Mediachase.Commerce.Orders.Dto;
using Mediachase.Commerce.Orders.Managers;
using Mediachase.Web.Console;
using Mediachase.Web.Console.BaseClasses;
using Mediachase.Web.Console.Common;
using Mediachase.Web.Console.Interfaces;

namespace Mediachase.Commerce.Manager.Order.Shipping.Plugins.WeightJurisdiction
{
	/// <summary>
	///		Summary description for ConfigureShippingMethod.
	/// </summary>
	public partial class ConfigureShippingMethod : OrderBaseUserControl, IGatewayControl
	{
		string _ValidationGroup = String.Empty;

		private ShippingMethodDto _ShippingMethodDto = null;
		NumberFormatInfo _NumberFormat = ManagementContext.Current.ConsoleUICulture.NumberFormat;

        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		protected void Page_Load(object sender, System.EventArgs e)
		{
			if (!IsPostBack)
				BindData();
		}

		override protected void OnInit(EventArgs e)
		{
			this.ItemsGrid.CancelCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.ItemsGrid_Cancel);
			this.ItemsGrid.EditCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.ItemsGrid_EditCommand);
			this.ItemsGrid.UpdateCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.ItemsGrid_Update);
			this.ItemsGrid.DeleteCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.ItemsGrid_Delete);
			JurisdictionGroupList.SelectedIndexChanged += new EventHandler(JurisdictionGroupList_SelectedIndexChanged);
			base.OnInit(e);
		}

		void JurisdictionGroupList_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (_ShippingMethodDto != null && !String.IsNullOrEmpty(JurisdictionGroupList.SelectedValue))
			{
				ShippingMethodDto.ShippingMethodCaseRow[] rows = (ShippingMethodDto.ShippingMethodCaseRow[])_ShippingMethodDto.ShippingMethodCase.Select(String.Format("JurisdictionGroupId={0}", JurisdictionGroupList.SelectedValue));
				BindItemsGrid();
			}
		}

		protected void AddWeightButton_Click(object sender, System.EventArgs e)
		{
			if (WeightRange.IsValid && WeightRequired.IsValid && PriceRange.IsValid && PriceRequired.IsValid)
			{
				if (_ShippingMethodDto != null && _ShippingMethodDto.ShippingMethod.Count > 0)
				{
					ShippingMethodDto.ShippingMethodCaseRow row = _ShippingMethodDto.ShippingMethodCase.NewShippingMethodCaseRow();
					row.Total = Double.Parse(Weight.Text, _NumberFormat);
					row.Charge = Double.Parse(Price.Text, _NumberFormat);
					row.StartDate = StartDate.Value;
					row.EndDate = EndDate.Value;
					row.ShippingMethodId = _ShippingMethodDto.ShippingMethod[0].ShippingMethodId;
					row.JurisdictionGroupId = Int32.Parse(JurisdictionGroupList.SelectedValue);
					// add the row to the dto
					if (row.RowState == DataRowState.Detached)
						_ShippingMethodDto.ShippingMethodCase.Rows.Add(row);
				}
			}
			BindItemsGrid();
		}

        /// <summary>
        /// Binds a data source to the invoked server control and all its child controls.
        /// </summary>
		public override void DataBind()
		{
            if (!Page.IsPostBack)
            {
                BindData();

                base.DataBind();
            }
		}

        /// <summary>
        /// Binds the data.
        /// </summary>
		public void BindData()
		{
			if (_ShippingMethodDto != null && _ShippingMethodDto.ShippingMethod.Count > 0)
			{
				// bind jurisdiction groups dropdown
				JurisdictionDto jDto = JurisdictionManager.GetJurisdictionGroups(Mediachase.Commerce.Orders.Managers.JurisdictionManager.JurisdictionType.Shipping);
				JurisdictionGroupList.DataSource = jDto.JurisdictionGroup;
				JurisdictionGroupList.DataBind();

                EndDate.Value = DateTime.UtcNow.AddYears(1);

				// bind shipping method cases
				BindItemsGrid();
			}
			else
				this.Visible = false;
		}

		private void BindItemsGrid()
		{
			if (!String.IsNullOrEmpty(JurisdictionGroupList.SelectedValue))
			{
				ShippingMethodDto.ShippingMethodCaseRow[] rows = (ShippingMethodDto.ShippingMethodCaseRow[])_ShippingMethodDto.ShippingMethodCase.Select(String.Format("JurisdictionGroupId={0}", JurisdictionGroupList.SelectedValue));
				if (rows != null)
				{
					ItemsGrid.DataSource = rows;
					ItemsGrid.DataBind();
					if (ItemsGrid.Items.Count != 0)
					{
						//string that hold the javascript confirm message 
						string confirm = "return confirm('Are you sure you want to delete?')";
						foreach (DataGridItem item in ItemsGrid.Items)
						{
							LinkButton deleteBtn = (LinkButton)item.Cells[6].Controls[0];
							deleteBtn.Attributes.Add("onclick", confirm);
						}
					}
				}
			}

			if (IsPostBack)
				UpdatePanel1.Update();
				//JurisdictionGroupWeightPanel.Update();
		}

		#region Standard DataGrid Functions
		private void ItemsGrid_Delete(object sender, DataGridCommandEventArgs e)
		{
			if (_ShippingMethodDto != null)
			{
				int id = Int32.Parse(((DataGrid)sender).DataKeys[e.Item.ItemIndex].ToString());
				ShippingMethodDto.ShippingMethodCaseRow row = _ShippingMethodDto.ShippingMethodCase.FindByShippingMethodCaseId(id);
				if (row != null)
					row.Delete();
			}

			BindItemsGrid();
		}

		private void ItemsGrid_Cancel(object sender, DataGridCommandEventArgs e)
		{
			ItemsGrid.EditItemIndex = -1;
			BindItemsGrid();
		}

		private void ItemsGrid_Update(object sender, DataGridCommandEventArgs e)
		{
			string selectedJGroup = String.Empty;

			if (_ShippingMethodDto != null)
			{
				int id = Int32.Parse(((DataGrid)sender).DataKeys[e.Item.ItemIndex].ToString());
				ShippingMethodDto.ShippingMethodCaseRow row = _ShippingMethodDto.ShippingMethodCase.FindByShippingMethodCaseId(id);
				if (row != null)
				{
					row.Total = Double.Parse(((TextBox)e.Item.Cells[0].FindControl("RowTotal")).Text, _NumberFormat);
					row.Charge = Double.Parse(((TextBox)e.Item.Cells[1].FindControl("RowPrice")).Text, _NumberFormat);
                    row.StartDate = ((CalendarDatePicker)e.Item.Cells[2].FindControl("StartDate")).Value;
                    row.EndDate = ((CalendarDatePicker)e.Item.Cells[3].FindControl("EndDate")).Value;
				}
			}

			ItemsGrid.EditItemIndex = -1;
			BindItemsGrid();
		}

		private void ItemsGrid_EditCommand(object sender, DataGridCommandEventArgs e)
		{
			ItemsGrid.EditItemIndex = e.Item.ItemIndex;
			BindItemsGrid();
		}

		#endregion

		#region IGatewayControl Members
        /// <summary>
        /// Saves the object changes.
        /// </summary>
        /// <param name="dto">The dto.</param>
		public void SaveChanges(object dto)
		{
			_ShippingMethodDto = dto as ShippingMethodDto;
		}

        /// <summary>
        /// Loads the object.
        /// </summary>
        /// <param name="dto">The dto.</param>
		public void LoadObject(object dto)
		{
			_ShippingMethodDto = dto as ShippingMethodDto;
		}

        /// <summary>
        /// Gets or sets the validation group.
        /// </summary>
        /// <value>The validation group.</value>
		public string ValidationGroup
		{
			get
			{
				return _ValidationGroup;
			}
			set
			{
				_ValidationGroup = value;
			}
		}
		#endregion
	}
}