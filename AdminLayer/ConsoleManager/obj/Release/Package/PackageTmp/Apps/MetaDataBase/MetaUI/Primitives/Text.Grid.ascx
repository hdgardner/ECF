<%@ Control Language="C#" AutoEventWireup="true" ClassName="Mediachase.Ibn.Web.UI.MetaUI.Primitives.Text.GridControl" Inherits="Mediachase.Ibn.Web.UI.Controls.Util.BaseType" %>
<%@ Import Namespace="Mediachase.Ibn.Data.Meta" %>
<%@ Import Namespace="Mediachase.Ibn.Data" %>
<%@ Import Namespace="Mediachase.Ibn.Data.Meta.Management" %>
<script language="c#" runat="server">
    protected string GetValue(MetaObject DataItem, string FieldName)
    {
        string retVal = "";
        
        if (DataItem != null)
        {
			if (DataItem.GetMetaType().TitleFieldName == FieldName)
			{
				string metaClassName = DataItem.GetMetaType().Name;
				if (DataItem.GetCardMetaType() != null)
					metaClassName = DataItem.GetCardMetaType().Name;
				
				string metaViewName = CHelper.GetFromContext("MetaViewName").ToString();
				string titleText = String.Empty;
				if (DataItem.Properties[FieldName].Value != null)
					titleText = DataItem.Properties[FieldName].Value.ToString();

				int id = DataItem.PrimaryKeyId ?? -1;

				string sUrl = String.Empty;// = CHelper.GetLinkObjectView_Edit(metaClassName, id.ToString());
				sUrl += String.Format("&ViewName={0}", metaViewName);
				sUrl = ResolveClientUrl(sUrl);
				
				retVal = String.Format("<a href='{0}'>{1}</a>", sUrl, titleText);
			}
			else
			{
				if (DataItem.Properties[FieldName].Value != null)
					retVal = DataItem.Properties[FieldName].Value.ToString();
			}
        }
        
        return retVal;
    }
</script>
<%# GetValue(DataItem, FieldName) %>