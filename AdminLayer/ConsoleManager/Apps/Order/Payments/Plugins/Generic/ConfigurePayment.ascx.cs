using System;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using Mediachase.Commerce.Orders;
using Mediachase.Commerce.Orders.Dto;
using Mediachase.Web.Console.BaseClasses;
using Mediachase.Web.Console.Interfaces;

namespace Mediachase.Commerce.Manager.Order.Payments.Plugins.Generic
{
	/// <summary>
	///		Summary description for ConfigurePayment.
	/// </summary>
	public partial class ConfigurePayment : OrderBaseUserControl, IGatewayControl
	{
		string _ValidationGroup = String.Empty;

		private PaymentMethodDto _PaymentMethodDto = null;

        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		protected void Page_Load(object sender, System.EventArgs e)
		{
		}

        /// <summary>
        /// Binds a data source to the invoked server control and all its child controls.
        /// </summary>
		public override void DataBind()
		{
			BindData();

			base.DataBind();
		}

        /// <summary>
        /// Binds the data.
        /// </summary>
		public void BindData()
		{
			if (_PaymentMethodDto != null && _PaymentMethodDto.PaymentMethodParameter != null)
			{
				//foreach (PaymentMethodDto.PaymentMethodParameterRow parameterRow in _PaymentMethodDto.PaymentMethodParameter.Rows)
				//{
				//    HtmlTableRow row = new HtmlTableRow();
				//    HtmlTableCell cell1 = new HtmlTableCell();
				//    HtmlTableCell cell2 = new HtmlTableCell();
				//    cell1.Attributes.Add("class", "FormLabelCell");
				//    cell2.Attributes.Add("class", "FormFieldCell");
					
				//    // Create label cell
				//    Label label = new Label();
				//    label.Text = parameterRow.Parameter;
				//    cell1.Controls.Add(label);

				//    // Create value field
				//    TextBox box = new TextBox();
				//    box.Text = parameterRow.Value;
				//    //box.Width = Unit.Parse("330");
				//    box.ID = parameterRow.ParameterId.ToString();
				//    cell2.Controls.Add(box);

				//    row.Cells.Add(cell1);
				//    row.Cells.Add(cell2);
				//    GenericTable.Rows.Add(row);
				//}
			}
			else
				this.Visible = false;
		}

		#region IGatewayControl Members
        /// <summary>
        /// Saves the object changes.
        /// </summary>
        /// <param name="dto">The dto.</param>
		public void SaveChanges(object dto)
		{
			_PaymentMethodDto = dto as PaymentMethodDto;
		}

        /// <summary>
        /// Loads the object.
        /// </summary>
        /// <param name="dto">The dto.</param>
		public void LoadObject(object dto)
		{
			_PaymentMethodDto = dto as PaymentMethodDto;
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
