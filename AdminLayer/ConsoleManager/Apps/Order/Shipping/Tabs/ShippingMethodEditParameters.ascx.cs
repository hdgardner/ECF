using System;
using System.Collections;
using System.Reflection;
using System.Web;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using Mediachase.Commerce.Orders;
using Mediachase.Commerce.Orders.Dto;
using Mediachase.Commerce.Manager.Core;
using Mediachase.Commerce.Manager.Core.Controls;
using Mediachase.Web.Console.BaseClasses;
using Mediachase.Web.Console.Common;
using Mediachase.Web.Console.Interfaces;

namespace Mediachase.Commerce.Manager.Order.Shipping.Tabs
{
	/// <summary>
	///		Summary description for ShippingMethodEditParameters.
	/// </summary>
	public partial class ShippingMethodEditParameters : OrderBaseUserControl, IAdminTabControl, IAdminContextControl
	{
		private const string _ShippingGatewayConfigurationBasePath = "~/Apps/Order/Shipping/Plugins/";
		private const string _ShippingGatewayConfigurationFileName = "/ConfigureShippingMethod.ascx";

		private const string _ShippingMethodDtoString = "ShippingMethodDto";

		private ShippingMethodDto _ShippingMethodDto = null;

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
			BindShippingConfig();
		}

        /// <summary>
        /// Binds the shipping config.
        /// </summary>
		private void BindShippingConfig()
		{
			if (this.phAdditionalParameters.Controls.Count > 0)
				return;	

			if (_ShippingMethodDto != null && _ShippingMethodDto.ShippingMethod.Count > 0)
			{
				try
				{
					ShippingMethodDto.ShippingMethodRow shippingRow = _ShippingMethodDto.ShippingMethod[0];
					// Load dynamic configuration form
					System.Web.UI.Control ctrl = null;
                    String mainPath = string.Concat(_ShippingGatewayConfigurationBasePath, shippingRow.ShippingOptionRow.SystemKeyword, _ShippingGatewayConfigurationFileName);
					if (System.IO.File.Exists(Server.MapPath(mainPath)))
						ctrl = base.LoadControl(mainPath);
					else
						ctrl = base.LoadControl(string.Concat(_ShippingGatewayConfigurationBasePath, "Generic", _ShippingGatewayConfigurationFileName));

					if (ctrl != null)
					{
                        ctrl.ID = shippingRow.ShippingOptionRow.SystemKeyword;
						IGatewayControl tmpCtrl = (IGatewayControl)ctrl;
						tmpCtrl.LoadObject(_ShippingMethodDto);
						tmpCtrl.ValidationGroup = "vg" + shippingRow.Name;
						
						this.phAdditionalParameters.Controls.Add(ctrl);

						ctrl.DataBind();
					}
				}
				catch (Exception ex)
				{
					DisplayErrorMessage("Error during binding additional shipping method parameters: " + ex.Message);
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
			_ShippingMethodDto = (ShippingMethodDto)context[_ShippingMethodDtoString];
		}
		#endregion

		#region IAdminTabControl Members
        /// <summary>
        /// Saves the changes.
        /// </summary>
        /// <param name="context">The context.</param>
		public void SaveChanges(IDictionary context)
		{
			ShippingMethodDto dto = (ShippingMethodDto)context[_ShippingMethodDtoString];

			if (dto == null)
				// dto must be created in base shipping control that holds tabs
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