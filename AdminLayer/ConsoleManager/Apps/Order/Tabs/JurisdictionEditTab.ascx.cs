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
using Mediachase.Commerce.Orders;
using Mediachase.Commerce.Orders.Dto;
using Mediachase.Commerce.Orders.Managers;
using Mediachase.Web.Console.BaseClasses;
using Mediachase.Web.Console.Common;
using Mediachase.Web.Console.Interfaces;

namespace Mediachase.Commerce.Manager.Order.Tabs
{
	public partial class JurisdictionEditTab : OrderBaseUserControl, IAdminTabControl, IAdminContextControl
	{
		private const string _JurisdictionIdString = "jid";
		private const string _JurisdictionDtoString = "JurisdictionDto";

		private JurisdictionDto _JurisdictionDto = null;

		/// <summary>
		/// Gets the Jurisdiction id.
		/// </summary>
		/// <value>The Jurisdiction id.</value>
		public int JurisdictionId
		{
			get
			{
				return ManagementHelper.GetIntFromQueryString(_JurisdictionIdString);
			}
		}

		/// <summary>
		/// Gets the JurisdictionType.
		/// </summary>
		/// <value>The JurisdictionType.</value>
		public JurisdictionManager.JurisdictionType JurisdictionType
		{
			get
			{
				JurisdictionManager.JurisdictionType jType = JurisdictionManager.JurisdictionType.Shipping;

				string type = ManagementHelper.GetStringValue(Request.QueryString["type"], String.Empty);
				if (!String.IsNullOrEmpty(type))
				{
					try
					{
						jType = (JurisdictionManager.JurisdictionType)Enum.Parse(typeof(JurisdictionManager.JurisdictionType), type);
					}
					catch { }
				}

				return jType;
			}
		}

		protected void Page_Load(object sender, EventArgs e)
		{
			if (!this.IsPostBack)
				BindForm();
		}

		/// <summary>
		/// Binds the form.
		/// </summary>
		private void BindForm()
		{
			if (JurisdictionId > 0)
			{
				if (_JurisdictionDto.Jurisdiction.Count > 0)
				{
					this.DisplayName.Text = _JurisdictionDto.Jurisdiction[0].DisplayName;
					this.Code.Text = _JurisdictionDto.Jurisdiction[0].Code;
					this.CountryCode.Text = _JurisdictionDto.Jurisdiction[0].CountryCode;
					this.StateProvinceCode.Text = _JurisdictionDto.Jurisdiction[0].IsStateProvinceCodeNull() ? String.Empty : _JurisdictionDto.Jurisdiction[0].StateProvinceCode;
					this.ZipCodeStart.Text = _JurisdictionDto.Jurisdiction[0].IsZipPostalCodeStartNull() ? String.Empty : _JurisdictionDto.Jurisdiction[0].ZipPostalCodeStart;
					this.ZipCodeEnd.Text = _JurisdictionDto.Jurisdiction[0].IsZipPostalCodeEndNull() ? String.Empty : _JurisdictionDto.Jurisdiction[0].ZipPostalCodeEnd;
					this.City.Text = _JurisdictionDto.Jurisdiction[0].IsCityNull() ? String.Empty : _JurisdictionDto.Jurisdiction[0].City;
					this.District.Text = _JurisdictionDto.Jurisdiction[0].IsDistrictNull() ? String.Empty : _JurisdictionDto.Jurisdiction[0].District;
					this.County.Text = _JurisdictionDto.Jurisdiction[0].IsCountyNull() ? String.Empty : _JurisdictionDto.Jurisdiction[0].County;
					this.GeoCode.Text = _JurisdictionDto.Jurisdiction[0].IsGeoCodeNull() ? String.Empty : _JurisdictionDto.Jurisdiction[0].GeoCode;
				}
			}
			else
			{
			}
		}

		/// <summary>
		/// Checks if entered code is unique.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="args">The <see cref="System.Web.UI.WebControls.ServerValidateEventArgs"/> instance containing the event data.</param>
		public void CodeCheck(object sender, ServerValidateEventArgs args)
		{
			// load jurisdiction by code
			JurisdictionDto dto = JurisdictionManager.GetJurisdiction(args.Value);

			// check if jurisdiction with specified code is loaded
			if (dto != null && dto.Jurisdiction.Count > 0 &&
				dto.Jurisdiction[0].JurisdictionId != JurisdictionId &&
				String.Compare(dto.Jurisdiction[0].Code, args.Value, StringComparison.CurrentCultureIgnoreCase) == 0)
			{
				args.IsValid = false;
				return;
			}

			args.IsValid = true;
		}

		#region IAdminTabControl Members

		public void SaveChanges(IDictionary context)
		{
			JurisdictionDto dto = (JurisdictionDto)context[_JurisdictionDtoString];
			JurisdictionDto.JurisdictionRow row = null;

			if (dto.Jurisdiction.Count > 0)
			{
				row = dto.Jurisdiction[0];
			}
			else
			{
				row = dto.Jurisdiction.NewJurisdictionRow();
				row.JurisdictionType = JurisdictionType.GetHashCode();
			}

			row.ApplicationId = OrderConfiguration.Instance.ApplicationId;
			row.DisplayName = DisplayName.Text;
			row.Code = Code.Text;

            if (CountryCode.Text.Trim() == String.Empty)
                row.CountryCode = null;
            else
    			row.CountryCode = CountryCode.Text;

            if (StateProvinceCode.Text.Trim() == String.Empty)
                row.StateProvinceCode = null;
            else
    			row.StateProvinceCode = StateProvinceCode.Text;

            if (ZipCodeStart.Text.Trim() == String.Empty)
                row.ZipPostalCodeStart = null;
            else
    			row.ZipPostalCodeStart = ZipCodeStart.Text;

            row.ZipPostalCodeEnd = ZipCodeEnd.Text;

            if (City.Text.Trim() == String.Empty)
                row.City = null;
            else
                row.City = City.Text;

            if (District.Text.Trim() == String.Empty)
                row.District = null;
            else
    			row.District = District.Text;

            if (County.Text.Trim() == String.Empty)
                row.County = null;
            else
    			row.County = County.Text;

            if (GeoCode.Text.Trim() == String.Empty)
                row.GeoCode = null;
            else
    			row.GeoCode = GeoCode.Text;

			if (row.RowState == DataRowState.Detached)
				dto.Jurisdiction.Rows.Add(row);
		}

		#endregion

		#region IAdminContextControl Members

		public void LoadContext(IDictionary context)
		{
			_JurisdictionDto = (JurisdictionDto)context[_JurisdictionDtoString];
		}

		#endregion
	}
}