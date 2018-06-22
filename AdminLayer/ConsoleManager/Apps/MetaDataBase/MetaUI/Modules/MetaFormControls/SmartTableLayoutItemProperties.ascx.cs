using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using Mediachase.Ibn.Data.Meta.Management;
using Mediachase.Ibn.Core;

namespace Mediachase.Ibn.Web.UI.MetaUI
{
	public partial class SmartTableLayoutItemProperties : System.Web.UI.UserControl
	{
		#region MetaClassName
		public string MetaClassName
		{
			get
			{
				if (ViewState[this.ID + "_MetaClassName"] == null)
					return null;
				return (string)ViewState[this.ID + "_MetaClassName"];
			}
			set
			{
				ViewState[this.ID + "_MetaClassName"] = value;
				lblSourceName.Visible = false;
				MetaClass mc = MetaDataWrapper.GetMetaClassByName(value);
				foreach (MetaField mf in mc.Fields)
					ddField.Items.Add(new ListItem(CHelper.GetMetaFieldName(mf), mf.Name));
				if (mc.CardOwner != null)
				{
					MetaClass card = mc.CardOwner;
					foreach (MetaField mf in card.Fields)
						ddField.Items.Add(new ListItem(CHelper.GetMetaFieldName(mf), mf.Name));
				}
			}
		}
		#endregion

		#region Source
		public string Source
		{
			get
			{
				if (ddField.Visible && ddField.SelectedItem != null)
					return ddField.SelectedValue;
				return lblSourceName.Text;
			}
			set
			{
				ddField.Visible = false;
				lblSourceName.Text = value;
			}
		}
		#endregion

		#region ReadOnly
		public bool ReadOnly
		{
			get
			{
				return cbReadOnly.Checked;
			}
			set
			{
				cbReadOnly.Checked = value;
			}
		}
		#endregion

		protected void Page_Load(object sender, EventArgs e)
		{
			cbReadOnly.Text = " " + GetGlobalResourceObject("MetaForm", "FieldIsReadOnly").ToString();
		}
	}
}