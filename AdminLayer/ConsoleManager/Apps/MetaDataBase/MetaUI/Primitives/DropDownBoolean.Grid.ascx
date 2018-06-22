<%@ Control Language="C#" AutoEventWireup="true" Inherits="Mediachase.Ibn.Web.UI.Controls.Util.BaseType" ClassName="Mediachase.Ibn.Web.UI.MetaUI.Primitives.DropDownBoolean.Grid" %>
<%@ Import Namespace="Mediachase.Ibn.Web.UI" %>
<script runat="server" language="c#">
	public string GetValue()
	{
		string ret = string.Empty;
		if (DataItem == null || DataItem.Properties[FieldName].Value == null) return "null";
		if (DataItem.Properties[FieldName].Value.GetType() == typeof(System.Boolean))
		{
			bool val = (bool)DataItem.Properties[FieldName].Value;
			Mediachase.Ibn.Data.Meta.Management.MetaField fld = DataItem.Properties[FieldName].GetMetaType();
			object fldBoolText = val ? Mediachase.Ibn.Data.Meta.Management.McDataTypeAttribute.BooleanTrueText : Mediachase.Ibn.Data.Meta.Management.McDataTypeAttribute.BooleanFalseText;
			if (fld.Attributes.ContainsKey(fldBoolText.ToString())) ret = fld.Attributes[fldBoolText.ToString()].ToString();
			else ret = val ? "true" : "false";
		}
		else ret = DataItem.Properties[FieldName].Value.ToString();
		return CHelper.GetResFileString(ret);
	}
</script>
	<%# GetValue() %>
