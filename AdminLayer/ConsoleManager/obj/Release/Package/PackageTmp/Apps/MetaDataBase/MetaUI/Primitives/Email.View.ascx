<%@ Control Language="C#" AutoEventWireup="true" ClassName="Mediachase.Ibn.Web.UI.MetaUI.Primitives.EMail.View" Inherits="Mediachase.Ibn.Web.UI.Controls.Util.BaseType"%>
<%@ Import Namespace="Mediachase.Ibn.Data.Meta" %>
<%@ Import Namespace="Mediachase.Ibn.Data" %>
<%@ Import Namespace="Mediachase.Ibn.Data.Meta.Management" %>
<script language="c#" runat="server">
    protected string GetValue(MetaObject DataItem, string FieldName)
    {
        string retVal = "";
		
		if (DataItem != null && 
			DataItem.Properties[FieldName]!=null &&
			DataItem.Properties[FieldName].Value!=null)
        {
			string str = CHelper.ParseText(DataItem.Properties[FieldName].Value.ToString());
			retVal = String.Format("<a href='mailto:{0}'>{0}</a>", str);
        }
        return retVal;
    }
</script>
<table cellpadding="0" cellspacing="0" border="0" width="100%" class="ibn-propertysheet" style="table-layout:fixed">
	<tr>
	 	<td class="ibn-value">
		  <%# GetValue(DataItem, FieldName) %>
		</td>
	</tr>
</table>