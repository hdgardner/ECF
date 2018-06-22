using System;
using System.Collections;
using System.Data;
using System.Globalization;
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
using Mediachase.Commerce.Orders.Managers;

namespace Mediachase.Commerce.Manager.Order.Shipping.Tabs
{
	/// <summary>
	///		Summary description for ShippingOptionEditBaseTab.
	/// </summary>
	public partial class ShippingOptionEditBaseTab : OrderBaseUserControl, IAdminTabControl, IAdminContextControl
	{
		private const string _ShippingMethodDtoString = "ShippingMethodDto";

		private ShippingMethodDto _ShippingMethodDto = null;

		/// <summary>
		/// Gets the shipping option id.
		/// </summary>
		/// <value>The shipping option id.</value>
		public Guid ShippingOptionId
		{
			get
			{
				return ManagementHelper.GetGuidFromQueryString("soid");
			}
		}

        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		protected void Page_Load(object sender, System.EventArgs e)
		{
			if (!this.IsPostBack)
				BindForm();
		}

		/// <summary>
		/// Checks if entered shipping option name is unique.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="args">The <see cref="System.Web.UI.WebControls.ServerValidateEventArgs"/> instance containing the event data.</param>
		public void ShippingOptionSystemNameCheck(object sender, ServerValidateEventArgs args)
		{
			// load shipping option
			ShippingMethodDto dto = ShippingManager.GetShippingMethods(null, true);

			if (dto != null && dto.ShippingOption.Count > 0)
			{
				ShippingMethodDto.ShippingOptionRow[] shippingRows = (ShippingMethodDto.ShippingOptionRow[])dto.ShippingOption.Select(String.Format("SystemKeyword='{0}' and ShippingOptionId<>'{1}'", args.Value, ShippingOptionId));
				if (shippingRows != null && shippingRows.Length > 0)
				{
					args.IsValid = false;
					return;
				}
			}

			args.IsValid = true;
		}

        /// <summary>
        /// Binds the form.
        /// </summary>
		private void BindForm()
		{
			// bind available Shipping gateway classes
			BindClassNames();

			if (_ShippingMethodDto != null && _ShippingMethodDto.ShippingOption.Count > 0)
			{
				try
				{
					ShippingMethodDto.ShippingOptionRow shippingRow = _ShippingMethodDto.ShippingOption[0];

					this.lblShippingMethodId.Text = shippingRow.ShippingOptionId.ToString();
					this.tbName.Text = shippingRow.Name;
					this.tbDescription.Text = shippingRow.Description;
					this.tbSystemName.Text = shippingRow.SystemKeyword;

					ManagementHelper.SelectListItem(ddlClassName, shippingRow.ClassName);

					// do not allow to change system name
					this.tbSystemName.Enabled = false;
				}
				catch(Exception ex)
				{
					DisplayErrorMessage("Error during binding form: " + ex.Message); 
				}
			}
			else
			{
				// set default form values
				this.tbSystemName.Enabled = true;
			}
		}

        /// <summary>
        /// Binds the class names.
        /// </summary>
		private void BindClassNames()
		{
			this.ddlClassName.Items.Clear();
			ddlClassName.Items.Add(new ListItem(RM.GetString("GENERAL_NONE"), ""));

			foreach (string cls in ReflectionHelper.GetClassesBasedOnTypeInSiteDir(typeof(IShippingGateway)))
			{
				string className = cls.Split(new char[] { ',' })[0];
				ddlClassName.Items.Add(new ListItem(className, className + "," + cls.Split(new char[] { ',' })[1]));
			}

			ddlClassName.DataBind();
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
			ShippingMethodDto.ShippingOptionRow row = null;

			if (dto == null)
				// dto must be created in base Shipping control that holds tabs
				return;

			// create the row if it doesn't exist; or update its modified date if it exists
			if (dto.ShippingOption.Count > 0)
			{
				row = dto.ShippingOption[0];
			}
			else
			{
				row = dto.ShippingOption.NewShippingOptionRow();
				row.ApplicationId = OrderConfiguration.Instance.ApplicationId;
				row.ShippingOptionId = Guid.NewGuid();
                row.Created = DateTime.UtcNow;
				row.SystemKeyword = this.tbSystemName.Text;
			}

			// fill the row with values
            row.Modified = DateTime.UtcNow;
			row.Name = tbName.Text;
			row.ClassName = ddlClassName.SelectedValue;
			row.Description = tbDescription.Text;

			// add the row to the dto
			if (row.RowState == DataRowState.Detached)
				dto.ShippingOption.Rows.Add(row);
		}
		#endregion
	}
}
