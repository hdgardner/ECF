﻿using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

using Mediachase.Ibn.Data.Meta.Management;
using Mediachase.Ibn.Web.UI.Controls.Util;

namespace Mediachase.Ibn.Web.UI.MetaUI.Primitives
{
	public partial class Date_Edit : System.Web.UI.UserControl, IEditControl
	{
		protected void Page_Load(object sender, EventArgs e)
		{

		}

		#region IEditControl Members

		#region Value
		public object Value
		{
			set
			{
				if (value != null)
					dtcValue.SelectedDate = (System.DateTime)value;
			}
			get
			{
				if (AllowNulls && dtcValue.SelectedDate == System.DateTime.MinValue)
					return null;
				else
					return dtcValue.SelectedDate;
			}
		}
		#endregion

		#region ShowLabel
		public bool ShowLabel
		{
			set { }
			get { return true; }
		}
		#endregion

		#region Label
		public string Label
		{
			set { }
			get { return ""; }
		}
		#endregion

		#region AllowNulls
		public bool AllowNulls
		{
			set
			{
				dtcValue.DateIsRequired = !value;
			}
			get
			{
				return !dtcValue.DateIsRequired;
			}
		}
		#endregion

		#region RowCount
		public int RowCount
		{
			set { }
			get { return 1; }
		}
		#endregion

		#region ReadOnly
		public bool ReadOnly
		{
			set { dtcValue.Enabled = !value; }
			get { return !dtcValue.Enabled; }
		}
		#endregion

		#region LabelWidth
		public string LabelWidth
		{
			set { }
			get { return ""; }
		}
		#endregion

		#region BindData
		public void BindData(MetaField field)
		{
		}
		#endregion

		#region FieldName
		public string FieldName
		{
			get
			{
				if (ViewState["FieldName"] != null)
					return ViewState["FieldName"].ToString();
				else
					return "";
			}
			set
			{
				ViewState["FieldName"] = value;
			}
		}
		#endregion
		#endregion
	}
}