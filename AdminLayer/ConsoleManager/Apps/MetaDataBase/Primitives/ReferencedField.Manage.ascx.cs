using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

using Mediachase.Ibn.Core;
using Mediachase.Ibn.Data;
using Mediachase.Ibn.Data.Meta.Management;
using Mediachase.Ibn.Web.UI.Controls.Util;

namespace Mediachase.Ibn.Web.UI.MetaDataBase.Primitives
{
	public partial class ReferencedField_Manage : System.Web.UI.UserControl, IManageControl
	{
        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		protected void Page_Load(object sender, EventArgs e)
		{

		}

		#region IManageControl Members
        /// <summary>
        /// Gets the default value.
        /// </summary>
        /// <param name="AllowNulls">if set to <c>true</c> [allow nulls].</param>
        /// <returns></returns>
		public string GetDefaultValue(bool AllowNulls)
		{
			return String.Format("{0},{1}", ddlClass.SelectedValue, ddlField.SelectedValue);
		}

        /// <summary>
        /// Gets the field attributes.
        /// </summary>
        /// <value>The field attributes.</value>
		public Mediachase.Ibn.Data.Meta.Management.AttributeCollection FieldAttributes
		{
			get
			{
				string[] str = ddlClass.SelectedValue.Split(',');
				Mediachase.Ibn.Data.Meta.Management.AttributeCollection Attr = new Mediachase.Ibn.Data.Meta.Management.AttributeCollection();
				Attr.Add(McDataTypeAttribute.ReferencedFieldMetaClassName, str[0]);
				Attr.Add(McDataTypeAttribute.ReferencedFieldMetaFieldName, ddlField.SelectedValue);
				Attr.Add(McDataTypeAttribute.ReferencedFieldReferenceName, str[1]);
				return Attr;
			}
		}

        /// <summary>
        /// Binds the data.
        /// </summary>
        /// <param name="mc">The mc.</param>
        /// <param name="FieldType">Type of the field.</param>
		public void BindData(MetaClass mc, string FieldType)
		{
			if (ddlClass.Items.Count == 0)
			{
				foreach (MetaField field in mc.GetReferences())
				{
					string RefClassName = field.Attributes[McDataTypeAttribute.ReferenceMetaClassName].ToString();
					string RefFieldName = field.Name;

					ddlClass.Items.Add(
						new ListItem(
							String.Format("{0} ({1})", RefClassName, RefFieldName),
							String.Format("{0},{1}", RefClassName, RefFieldName))
						);
				}
				BindFields();
			}
		}

        /// <summary>
        /// Binds the data.
        /// </summary>
        /// <param name="mf">The mf.</param>
		public void BindData(MetaField mf)
		{
			string RefClassName = mf.Attributes[McDataTypeAttribute.ReferenceMetaClassName].ToString();
			string ReferenceName = mf.Attributes[McDataTypeAttribute.ReferencedFieldReferenceName].ToString();
			string RefFieldName = mf.Attributes[McDataTypeAttribute.ReferencedFieldMetaFieldName].ToString();

			ddlClass.Items.Add(
			  new ListItem(
				  String.Format("{0} ({1})", RefClassName, ReferenceName),
				  String.Format("{0},{1}", RefClassName, ReferenceName))
			  );
			ddlClass.Enabled = false;

			ddlField.Items.Add(new ListItem(RefFieldName));
			ddlField.Enabled = false;
		}
		#endregion

		#region ddlClass_SelectedIndexChanged
        /// <summary>
        /// Handles the SelectedIndexChanged event of the ddlClass control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		protected void ddlClass_SelectedIndexChanged(object sender, EventArgs e)
		{
			BindFields();
		}
		#endregion

		#region BindFields
        /// <summary>
        /// Binds the fields.
        /// </summary>
		private void BindFields()
		{
			ddlField.Items.Clear();

			string[] class_field = ddlClass.SelectedValue.Split(',');
			MetaClass mc = MetaDataWrapper.GetMetaClassByName(class_field[0]);
			foreach (MetaField field in mc.Fields)
			{
				McDataType type = field.GetMetaType().McDataType;
				McDataType originalType = field.GetOriginalMetaType().McDataType;
				// forbid reference and referenced fields
				if (type != McDataType.Reference && type != McDataType.ReferencedField && originalType != McDataType.ReferencedField)
				{
					ddlField.Items.Add(new ListItem(String.Format("{0} ({1})", CHelper.GetResFileString(field.FriendlyName), field.Name), field.Name));
				}
			}
		}
		#endregion
	}
}