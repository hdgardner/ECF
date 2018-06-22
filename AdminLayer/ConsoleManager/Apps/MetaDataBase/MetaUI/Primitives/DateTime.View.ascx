<%@ Control Language="C#" AutoEventWireup="true" Inherits="Mediachase.Ibn.Web.UI.Controls.Util.BaseType" ClassName="Mediachase.Ibn.Web.UI.MetaUI.Primitives.DateTime.View" %>
<table cellpadding="0" cellspacing="0" border="0" width="100%" class="ibn-propertysheet" style="table-layout:fixed">
	<tr>
	 	<td class="ibn-value">
		  <%# (DataItem == null || DataItem.Properties[FieldName].Value == null) ? "" : String.Format("{0} {1}", ((System.DateTime)DataItem.Properties[FieldName].Value).ToShortDateString(), ((System.DateTime)DataItem.Properties[FieldName].Value).ToShortTimeString())%>
		</td>
	</tr>
</table>