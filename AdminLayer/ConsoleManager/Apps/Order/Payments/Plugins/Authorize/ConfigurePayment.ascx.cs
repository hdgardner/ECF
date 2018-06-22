using System;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using Mediachase.Commerce.Orders.Dto;
using Mediachase.Commerce.Orders.Managers;
using Mediachase.Commerce.Plugins.Payment.Authorize;
using Mediachase.Web.Console.Common;
using Mediachase.Web.Console.Interfaces;

namespace Mediachase.Commerce.Manager.Order.Payments.Plugins.Authorize
{
	/// <summary>
	///		Summary description for ConfigurePayment.
	/// </summary>
	public partial class ConfigurePayment : System.Web.UI.UserControl, IGatewayControl
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
			BindData();

			RecurringProcessingUrlCustomValidator.ServerValidate += new ServerValidateEventHandler(RecurringProcessingUrlCustomValidator_ServerValidate);
		}

		protected void RecurringProcessingUrlCustomValidator_ServerValidate(object source, ServerValidateEventArgs args)
		{
			bool valid = true;
			if (String.Compare(ddlRecurringMethod.SelectedValue, "authorize", StringComparison.OrdinalIgnoreCase) == 0)
			{
				if (String.IsNullOrEmpty(args.Value))
					valid = false;
			}

			args.IsValid = valid;
		}

        /// <summary>
        /// Binds a data source to the invoked server control and all its child controls.
        /// </summary>
		public override void DataBind()
		{
			//BindData();

			base.DataBind();
		}

        /// <summary>
        /// Binds the data.
        /// </summary>
        public void BindData()
        {
			BindCancelStatusesDropDown();

			// fill in the form fields
			if (_PaymentMethodDto != null && _PaymentMethodDto.PaymentMethodParameter != null)
			{
				PaymentMethodDto.PaymentMethodParameterRow param = null;

				param = GetParameterByName(AuthorizePaymentGateway._UserParameterName);
				if (param != null)
					User.Text = param.Value;

				param = GetParameterByName(AuthorizePaymentGateway._TransactionKeyParameterName);
				if (param != null)
					Password.Text = param.Value;
				
				param = GetParameterByName(AuthorizePaymentGateway._ProcessUrlParameterName);
				if (param != null)
					ProcessingUrl.Text = param.Value;

				param = GetParameterByName(AuthorizePaymentGateway._PaymentOptionParameterName);
				if (param != null)
				{
					if (String.Compare(param.Value, "A", true) == 0)
						RadioButtonListOptions.SelectedIndex = 0;
					else if (String.Compare(param.Value, "S", true) == 0)
						RadioButtonListOptions.SelectedIndex = 1;
				}

				param = GetParameterByName(AuthorizePaymentGateway._RecurringProcessUrlParameterName);
				if (param != null)
					RecurringProcessingUrl.Text = param.Value;

				param = GetParameterByName(AuthorizePaymentGateway._RecurringMethodParameterName);
				if (param != null)
					ManagementHelper.SelectListItem(ddlRecurringMethod, param.Value);

				param = GetParameterByName(AuthorizePaymentGateway._CancelStatusParameterName);
				if (param != null)
					ManagementHelper.SelectListItem(ddlCancelStatus, param.Value);
			}
			else
				this.Visible = false;
        }

		private void BindCancelStatusesDropDown()
		{
			ddlCancelStatus.DataSource = OrderStatusManager.GetOrderStatus();
            ddlCancelStatus.DataBind();

			if(ddlCancelStatus.Items.Count == 0)
			{
				ddlCancelStatus.Items.Clear();
				ddlCancelStatus.Items.Add(new ListItem(Mediachase.Ibn.Web.UI.WebControls.UtilHelper.GetResFileString("{OrderStrings:RecurringPayment_Select_CancelStatus}"), ""));
			}
		}

		#region IGatewayControl Members
        /// <summary>
        /// Saves the object changes.
        /// </summary>
        /// <param name="dto">The dto.</param>
		public void SaveChanges(object dto)
		{
			if (this.Visible)
			{
				_PaymentMethodDto = dto as PaymentMethodDto;

				if (_PaymentMethodDto != null && _PaymentMethodDto.PaymentMethodParameter != null)
				{
					Guid paymentMethodId = Guid.Empty;
					if (_PaymentMethodDto.PaymentMethod.Count > 0)
						paymentMethodId = _PaymentMethodDto.PaymentMethod[0].PaymentMethodId;

					PaymentMethodDto.PaymentMethodParameterRow param = null;

					param = GetParameterByName(AuthorizePaymentGateway._UserParameterName);
					if (param != null)
						param.Value = User.Text;
					else
						CreateParameter(_PaymentMethodDto, AuthorizePaymentGateway._UserParameterName, User.Text, paymentMethodId);

					param = GetParameterByName(AuthorizePaymentGateway._TransactionKeyParameterName);
					if (param != null)
						param.Value = Password.Text;
					else
						CreateParameter(_PaymentMethodDto, AuthorizePaymentGateway._TransactionKeyParameterName, Password.Text, paymentMethodId);

					#region Regular Transaction Parameters
					param = GetParameterByName(AuthorizePaymentGateway._ProcessUrlParameterName);
					if (param != null)
						param.Value = ProcessingUrl.Text;
					else
						CreateParameter(_PaymentMethodDto, AuthorizePaymentGateway._ProcessUrlParameterName, ProcessingUrl.Text, paymentMethodId);

					string poValue = "";
					if (RadioButtonListOptions.SelectedIndex == 0)
						poValue = "A";
					else if (RadioButtonListOptions.SelectedIndex == 1)
						poValue = "S";

					param = GetParameterByName(AuthorizePaymentGateway._PaymentOptionParameterName);
					if (param != null)
						param.Value = poValue;
					else
						CreateParameter(_PaymentMethodDto, AuthorizePaymentGateway._PaymentOptionParameterName, poValue, paymentMethodId);
					#endregion

					#region Recurring Transaction Parameters
					param = GetParameterByName(AuthorizePaymentGateway._RecurringProcessUrlParameterName);
					if (param != null)
						param.Value = RecurringProcessingUrl.Text;
					else
						CreateParameter(_PaymentMethodDto, AuthorizePaymentGateway._RecurringProcessUrlParameterName, RecurringProcessingUrl.Text, paymentMethodId);

					param = GetParameterByName(AuthorizePaymentGateway._RecurringMethodParameterName);
					if (param != null)
						param.Value = ddlRecurringMethod.SelectedValue;
					else
						CreateParameter(_PaymentMethodDto, AuthorizePaymentGateway._RecurringMethodParameterName, ddlRecurringMethod.SelectedValue, paymentMethodId);

					param = GetParameterByName(AuthorizePaymentGateway._CancelStatusParameterName);
					if (param != null)
						param.Value = ddlCancelStatus.SelectedValue;
					else
						CreateParameter(_PaymentMethodDto, AuthorizePaymentGateway._CancelStatusParameterName, ddlCancelStatus.SelectedValue, paymentMethodId);
					#endregion
				}
			}
		}

        /// <summary>
        /// Gets the name of the parameter by.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
		private PaymentMethodDto.PaymentMethodParameterRow GetParameterByName(string name)
		{
			PaymentMethodDto.PaymentMethodParameterRow[] rows = (PaymentMethodDto.PaymentMethodParameterRow[])_PaymentMethodDto.PaymentMethodParameter.Select(String.Format("Parameter = '{0}'", name));
			if (rows != null && rows.Length > 0)
				return rows[0];
			else
				return null;
		}

        /// <summary>
        /// Creates the parameter.
        /// </summary>
        /// <param name="dto">The dto.</param>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        /// <param name="paymentMethodId">The payment method id.</param>
		private void CreateParameter(PaymentMethodDto dto, string name, string value, Guid paymentMethodId)
		{
			PaymentMethodDto.PaymentMethodParameterRow newRow = dto.PaymentMethodParameter.NewPaymentMethodParameterRow();
			newRow.PaymentMethodId = paymentMethodId;
			newRow.Parameter = name;
			newRow.Value = value;

			// add the row to the dto
			if (newRow.RowState == DataRowState.Detached)
				dto.PaymentMethodParameter.Rows.Add(newRow);
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
