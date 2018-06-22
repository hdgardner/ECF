<%@ Control Language="C#" ClassName="NodeInfo" AutoEventWireup="true" Inherits="Mediachase.Cms.WebUtility.BaseStoreUserControl, Mediachase.Cms.WebUtility" %>
<%@ Register src="../Modules/SearchControl.ascx" tagname="SearchControl" tagprefix="ecf" %>
<script runat="server">
    public override void LoadContext(IDictionary context)
    {
        SearchControl1.LoadContext(context);
    }
</script>
<ecf:SearchControl ID="SearchControl1" runat="server" />
