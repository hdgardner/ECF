<%@ Control Language="C#" AutoEventWireup="true" ClassName="Mediachase.Ibn.Web.UI.MetaUI.Primitives.Html.View" Inherits="Mediachase.Ibn.Web.UI.Controls.Util.BaseType" %>
<script runat="server" language="C#">
	public string GetHtml()
	{
		if (DataItem == null || DataItem.Properties[FieldName].Value == null) return "null";
		string ret = DataItem.Properties[FieldName].Value.ToString();
		// OR: Для чего это делается - это же HTML?
//		ret = ret.Replace("<", "&lt;");
//		ret = ret.Replace(">", "&gt;");
		return ret;
	}
</script>
<table cellpadding="0" cellspacing="0" border="0" width="100%" class="ibn-propertysheet" style="table-layout:fixed">
	<tr>
		<td class="ibn-value">
		  <%# GetHtml()%>
		</td>
	</tr>
</table>