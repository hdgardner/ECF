<%@ Control Language="C#" Inherits="Mediachase.Ibn.Web.UI.Controls.Util.BaseType" AutoEventWireup="true" ClassName="Mediachase.Ibn.Web.UI.MetaUI.Primitives.Text.View" %>
<table cellpadding="0" cellspacing="0" border="0" width="100%" class="ibn-propertysheet" style="table-layout:fixed">
	<tr>
	 	<td class="ibn-value">
		  <%# (DataItem == null || DataItem.Properties[FieldName].Value == null) ? "" : DataItem.Properties[FieldName].Value.ToString()%>
		</td>
	</tr>
</table>
