using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Mediachase.Web.Console.BaseClasses;
using Mediachase.Web.Console.Interfaces;
using Mediachase.Commerce.Orders.Dto;
using Mediachase.Web.Console.Common;
using Mediachase.Commerce.Orders;
using System.Globalization;
using Mediachase.Commerce.Orders.Managers;

namespace Mediachase.Commerce.Manager.Order.Shipping.Tabs
{
	public partial class PackageEditBaseTab : OrderBaseUserControl, IAdminTabControl, IAdminContextControl
	{
		private const string _PackageIdString = "PackageId";
		private const string _ShippingMethodDtoString = "ShippingMethodDto";

		private ShippingMethodDto _ShippingMethodDto = null;

		NumberFormatInfo nfi = CultureInfo.InvariantCulture.NumberFormat;

		/// <summary>
		/// Gets the PackageId.
		/// </summary>
		/// <value>The PackageId.</value>
		public int PackageId
		{
			get
			{
				return ManagementHelper.GetIntFromQueryString(_PackageIdString);
			}
		}

		protected void Page_Load(object sender, EventArgs e)
		{
			if (!this.IsPostBack)
				BindForm();

			string scriptString = "ToggleDimensionsTable();";
			Page.ClientScript.RegisterStartupScript(this.GetType(), "toggleDimensions", scriptString, true);
		}

		private void BindForm()
		{
			if (_ShippingMethodDto != null && _ShippingMethodDto.Package.Count > 0)
			{
				try
				{
					ShippingMethodDto.PackageRow packageRow = _ShippingMethodDto.Package[0];

					this.tbName.Text = packageRow.Name;
					try
					{
						this.tbDescription.Text = packageRow.Description;
					}
					catch
					{
						this.tbDescription.Text = "";
					}

					if (packageRow.Width > 0)
						this.tbWidth.Text = packageRow.Width.ToString("F2", nfi);
					else
						this.tbWidth.Enabled = false;

					if (packageRow.Height > 0)
						this.tbHeight.Text = packageRow.Height.ToString("F2", nfi);
					else
						this.tbHeight.Enabled = false;

					if (packageRow.Length > 0)
						this.tbLength.Text = packageRow.Length.ToString("F2", nfi);
					else
						this.tbLength.Enabled = false;

					chbDimensions.Checked = packageRow.Width > 0 && packageRow.Length > 0 && packageRow.Height > 0;
				}
				catch (Exception ex)
				{
					DisplayErrorMessage("Error during binding form: " + ex.Message);
				}
			}
		}

		/// <summary>
		/// Checks if entered shipping package name is unique.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="args">The <see cref="System.Web.UI.WebControls.ServerValidateEventArgs"/> instance containing the event data.</param>
		public void ShippingPackageNameCheck(object sender, ServerValidateEventArgs args)
		{
			// load shipping package
			ShippingMethodDto dto = ShippingManager.GetShippingPackage(args.Value);

			if (dto != null && dto.Package.Count > 0)
			{
				if (dto.Package[0].PackageId != PackageId)
				{
					args.IsValid = false;
					return;
				}
			}

			args.IsValid = true;
		}

		protected void WidthValidate(object source, ServerValidateEventArgs args)
		{
			if (!chbDimensions.Checked)
			{
				args.IsValid = true;
				return;
			}

			double width;
			try
			{
				width = double.Parse(tbWidth.Text, nfi);
			}
			catch
			{
				args.IsValid = false;
				return;
			}
			args.IsValid = width > 0;
		}

		protected void LengthValidate(object source, ServerValidateEventArgs args)
		{
			if (!chbDimensions.Checked)
			{
				args.IsValid = true;
				return;
			}

			double length;
			try
			{
				length = double.Parse(tbLength.Text, nfi);
			}
			catch
			{
				args.IsValid = false;
				return;
			}

			args.IsValid = length > 0;
		}

		protected void HeightValidate(object source, ServerValidateEventArgs args)
		{
			if (!chbDimensions.Checked)
			{
				args.IsValid = true;
				return;
			}

			double height;
			try
			{
				height = double.Parse(tbLength.Text, nfi);
			}
			catch
			{
				args.IsValid = false;
				return;
			}
			args.IsValid = height > 0;
		}
		
		#region IAdminTabControl Members

		public void SaveChanges(IDictionary context)
		{
			//WidthValidator.Validate();
			//LengthValidator.Validate();
			//HeightValidator.Validate();

			if (!chbDimensions.Checked || (chbDimensions.Checked && Page.IsValid))
			{
				ShippingMethodDto dto = (ShippingMethodDto)context[_ShippingMethodDtoString];
				ShippingMethodDto.PackageRow row = null;

				if (dto == null)
					// dto must be created in base Package edit control that holds tabs
					return;

				// create the row if it doesn't exist; or update its modified date if it exists
				if (dto.Package.Count > 0)
				{
					row = dto.Package[0];
				}
				else
				{
					row = dto.Package.NewPackageRow();
					row.ApplicationId = OrderConfiguration.Instance.ApplicationId;
				}

				// fill the row with values
				row.Name = tbName.Text;
				row.Description = tbDescription.Text;

				if (chbDimensions.Checked)
				{
					row.Width = double.Parse(tbWidth.Text);
					row.Length = double.Parse(tbLength.Text);
					row.Height = double.Parse(tbHeight.Text);
				}
				else
				{
					row.Width = 0;
					row.Height = 0;
					row.Length = 0;
				}

				// add the row to the dto
				if (row.RowState == DataRowState.Detached)
					dto.Package.Rows.Add(row);
			}
			else
			{
				chbDimensions.Checked = false;
			}
		}

		#endregion

		#region IAdminContextControl Members

		public void LoadContext(IDictionary context)
		{
			_ShippingMethodDto = (ShippingMethodDto)context[_ShippingMethodDtoString];
		}

		#endregion
	}
}