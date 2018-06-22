using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Mediachase.Web.Console.BaseClasses;
using Mediachase.Web.Console.Common;
using ComponentArt.Web.UI;

namespace Mediachase.Commerce.Manager.Core.Controls.GridTemplates
{
	public partial class DateTimeTemplate : BaseUserControl
	{
		#region Public Properties
		private string _Argument = null;

		/// <summary>
		/// Gets or sets the date argument.
		/// </summary>
		/// <value>The date argument.</value>
		public string DateArgument
		{
			get { return _Argument; }
			set { _Argument = value; }
		}

		private bool _ConvertFromServerTime = false;

		/// <summary>
		/// If true, DateTime value is supposed to be in server time, overwise in UTC..
		/// </summary>
		/// <value>The value.</value>
		public bool ConvertFromServerTime
		{
			get { return _ConvertFromServerTime; }
			set { _ConvertFromServerTime = value; }
		}
		#endregion

		protected void Page_Load(object sender, EventArgs e)
		{
			if (String.IsNullOrEmpty(DateArgument))
				return;

			object gItemValue = ((GridServerTemplateContainer)this.Parent).DataItem[DateArgument];

			DateTime? dtValue = null;

			if (gItemValue != null && gItemValue != DBNull.Value)
				dtValue = (DateTime)gItemValue;

			if (dtValue.HasValue)
				FieldText.Text = ManagementHelper.FormatDateTime(ConvertFromServerTime ? dtValue.Value.ToUniversalTime() : dtValue.Value);
		}
	}
}