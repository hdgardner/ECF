using System;
using System.Collections;
using System.Reflection;
using System.Web;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using Mediachase.Cms;
using Mediachase.Commerce.Orders;
using Mediachase.Commerce.Orders.Dto;
using Mediachase.Commerce.Manager.Core;
using Mediachase.Commerce.Manager.Core.Controls;
using Mediachase.Web.Console.BaseClasses;
using Mediachase.Web.Console.Common;
using Mediachase.Web.Console.Interfaces;

namespace Mediachase.Commerce.Manager.Order.Payments.Tabs
{
	/// <summary>
	///		Summary description for PaymentMethodEditParameters.
	/// </summary>
	public partial class PaymentMethodEditParameters : OrderBaseUserControl, IAdminTabControl, IAdminContextControl
	{
		private const string _PaymentGatewayConfigurationBasePath = "~/Apps/Order/Payments/Plugins/";
		private const string _PaymentGatewayConfigurationFileName = "/ConfigurePayment.ascx";

		private const string _PaymentMethodDtoString = "PaymentMethodDto";

		private PaymentMethodDto _PaymentMethodDto = null;

        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		protected void Page_Load(object sender, System.EventArgs e)
		{
			DataBind();
		}

        /// <summary>
        /// Called by the ASP.NET page framework to notify server controls that use composition-based implementation to create any child controls they contain in preparation for posting back or rendering.
        /// </summary>
		protected override void CreateChildControls()
		{
			if (!this.ChildControlsCreated)
			{
				base.CreateChildControls();
				ChildControlsCreated = true;
			}
		}

        /// <summary>
        /// Binds a data source to the invoked server control and all its child controls.
        /// </summary>
		public override void DataBind()
		{
			base.DataBind();
			BindPaymentConfig();
		}

        /// <summary>
        /// Binds the payment config.
        /// </summary>
		private void BindPaymentConfig()
		{
			//this.phAdditionalParameters.EnableViewState = false;

			if (this.phAdditionalParameters.Controls.Count > 0)
				return;	

			if (_PaymentMethodDto != null && _PaymentMethodDto.PaymentMethod.Count > 0)
			{
				try
				{
					PaymentMethodDto.PaymentMethodRow paymentRow = _PaymentMethodDto.PaymentMethod[0];
					// Load dynamic configuration form
					System.Web.UI.Control ctrl = null;
					String mainPath = string.Concat(_PaymentGatewayConfigurationBasePath, paymentRow.SystemKeyword, _PaymentGatewayConfigurationFileName);
					if (System.IO.File.Exists(Server.MapPath(mainPath)))
						ctrl = base.LoadControl(mainPath);
					else
						ctrl = base.LoadControl(string.Concat(_PaymentGatewayConfigurationBasePath, "Generic", _PaymentGatewayConfigurationFileName));

					if (ctrl != null)
					{
						ctrl.ID = paymentRow.SystemKeyword;
						IGatewayControl tmpCtrl = (IGatewayControl)ctrl;
						tmpCtrl.LoadObject(_PaymentMethodDto);
						tmpCtrl.ValidationGroup = "vg" + paymentRow.SystemKeyword;
						
						this.phAdditionalParameters.Controls.Add(ctrl);

						ctrl.DataBind();
					}
				}
				catch (Exception ex)
				{
					DisplayErrorMessage("Error during binding additional gateway parameters: " + ex.Message);
					return;
				}
			}
		}

		#region IAdminContextControl Members
        /// <summary>
        /// Loads the context.
        /// </summary>
        /// <param name="context">The context.</param>
		public void LoadContext(IDictionary context)
		{
			_PaymentMethodDto = (PaymentMethodDto)context[_PaymentMethodDtoString];
		}
		#endregion

		#region IAdminTabControl Members
        /// <summary>
        /// Saves the changes.
        /// </summary>
        /// <param name="context">The context.</param>
		public void SaveChanges(IDictionary context)
		{
			PaymentMethodDto dto = (PaymentMethodDto)context[_PaymentMethodDtoString];

			if (dto == null)
				// dto must be created in base payment control that holds tabs
				return;

			foreach (System.Web.UI.Control ctrl in this.phAdditionalParameters.Controls)
			{
				IGatewayControl gateway = ctrl as IGatewayControl;
				if (gateway != null)
					gateway.SaveChanges(dto);
			}
		}
		#endregion
	}
}