<%@ Control Language="C#" AutoEventWireup="true" ClassName="Mediachase.Ibn.Web.UI.MetaUI.Primitives.Html.Grid" Inherits="Mediachase.Ibn.Web.UI.Controls.Util.BaseType" %>
<script runat="server" language="C#">
	public string GetHtml()
	{
		if (DataItem == null || DataItem.Properties[FieldName].Value == null) return "null";
		string ret = DataItem.Properties[FieldName].Value.ToString();
		ret = ret.Replace("<", "&lt;");
		ret = ret.Replace(">", "&gt;");
		return ret;
	}
</script>
<%# GetHtml()%>