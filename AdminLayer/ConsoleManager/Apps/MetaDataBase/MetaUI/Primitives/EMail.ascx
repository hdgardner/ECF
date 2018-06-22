<%@ Control Language="C#" AutoEventWireup="true"  Inherits="Mediachase.Ibn.Web.UI.Controls.Util.BaseType" ClassName="Mediachase.Ibn.Web.UI.MetaUI.Primitives.EMailControl" %>
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
<%# GetValue(DataItem, FieldName) %>