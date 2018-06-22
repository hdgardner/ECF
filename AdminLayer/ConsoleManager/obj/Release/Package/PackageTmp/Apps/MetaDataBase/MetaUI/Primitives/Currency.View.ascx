<%@ Control Language="C#" AutoEventWireup="true" Inherits="Mediachase.Ibn.Web.UI.Controls.Util.BaseType" ClassName="Mediachase.Ibn.Web.UI.MetaUI.Primitives.Currency.View" %>
<table cellpadding="0" cellspacing="0" border="0" width="100%" class="ibn-propertysheet" style="table-layout:fixed">
	<tr>
	 	<td class="ibn-value">
		  <%# (DataItem == null || DataItem.Properties[FieldName].Value == null) ? "" : Convert.ToDouble(DataItem.Properties[FieldName].Value).ToString("#.##") + "&nbsp;руб."%>
		</td>
	</tr>
</table>