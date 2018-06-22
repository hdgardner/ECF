<%@ Control Language="C#" Inherits="Mediachase.Ibn.Web.UI.Controls.Util.BaseType" AutoEventWireup="true" ClassName="Mediachase.Ibn.Web.UI.MetaUI.Primitives.Enum.View" %>
<%@ Import Namespace="Mediachase.Ibn.Data.Meta.Management" %>
<table cellpadding="0" cellspacing="0" border="0" width="100%" class="ibn-propertysheet" style="table-layout:fixed">
	<tr>
	 	<td class="ibn-value">
		  <%# (DataItem == null || DataItem.Properties[FieldName].Value == null) ? "" : MetaEnum.GetFriendlyName(DataItem.Properties[FieldName].GetMetaType().GetMetaType(), (int)DataItem.Properties[FieldName].Value)%>
		</td>
	</tr>
</table>