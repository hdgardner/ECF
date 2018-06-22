using System;
using System.Globalization;
using System.Web.UI;
using Mediachase.Web.Console.BaseClasses;
using System.Text.RegularExpressions;

namespace Mediachase.Commerce.Manager.Core.Controls
{
    [ValidationProperty("Value")]
    public partial class CalendarDatePicker : CoreBaseUserControl
	{
		#region Private Properties
		private bool _use24HourFormat = CultureInfo.CurrentUICulture.DateTimeFormat.ShortTimePattern.Contains("H");
		#endregion

		#region ValidationGroup Property
		string _validationGroup = "";

		public string ValidationGroup
		{
			get
			{
				return _validationGroup;
			}
			set
			{
				_validationGroup = value;
				rfvDate.ValidationGroup = value;
				cvDate.ValidationGroup = value;
				rvDate.ValidationGroup = value;
				timeMaskedValidator.ValidationGroup = value;
			}
		}
		#endregion

		#region ValidationEnabled Property
		bool _validationEnabled = true;

		/// <summary>
		/// If false, validation will be disabled. Default is true.
		/// </summary>
		public bool ValidationEnabled
		{
			get
			{
				return _validationEnabled;
			}
			set
			{
				_validationEnabled = value;
				rfvDate.Enabled = value;
				cvDate.Enabled = value;
				rvDate.Enabled = value;
				timeMaskedValidator.Enabled = value;
			}
		}
		#endregion

		/// <summary>
		/// Gets or sets the value.
		/// </summary>
		/// <value>The value.</value>
		public DateTime Value
		{
			get
			{
				DateTime val = DateTime.MinValue;
				bool parsed = DateTime.TryParse(Date.Text, out val);
				if (parsed)
				{
					DateTime time = new DateTime(val.Year, val.Month, val.Day, 0, 0, 0);
					DateTime.TryParse(tbTime.Text, CultureInfo.CurrentUICulture.DateTimeFormat, DateTimeStyles.None, out time);
					val = val.Add(time.TimeOfDay);
				}
				return val;
			}
			set
			{
				Date.Text = value.ToString("d", DateTimeFormatInfo.CurrentInfo);
				tbTime.Text = value.ToShortTimeString();
			}
		}

        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Page_Load(object sender, EventArgs e)
        {
			if (!IsPostBack)
			{
				// set extender properties
				if (CultureInfo.CurrentUICulture.DateTimeFormat.ShortTimePattern.Contains("S") ||
					CultureInfo.CurrentUICulture.DateTimeFormat.ShortTimePattern.Contains("s"))
					timeMaskedExtender.Mask = "99:99:99";
				else
					timeMaskedExtender.Mask = "99:99";
				timeMaskedExtender.AcceptAMPM = !_use24HourFormat;

				// set validation properties
				if (!String.IsNullOrEmpty(ValidationGroup))
				{
					rfvDate.ValidationGroup = ValidationGroup;
					cvDate.ValidationGroup = ValidationGroup;
					rvDate.ValidationGroup = ValidationGroup;
					timeMaskedValidator.ValidationGroup = ValidationGroup;
				}

				rfvDate.Enabled = _validationEnabled;
				cvDate.Enabled = _validationEnabled;
				rvDate.Enabled = _validationEnabled;
				timeMaskedValidator.Enabled = _validationEnabled;
			}

            if (Date.Text.Equals(String.Empty) && rfvDate.Enabled)
                Value = DateTime.Today;
        }
    }
}