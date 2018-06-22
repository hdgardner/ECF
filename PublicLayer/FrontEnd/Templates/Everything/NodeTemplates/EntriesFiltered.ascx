<%@ Control Language="C#" ClassName="EntriesFiltered" AutoEventWireup="true" Inherits="Mediachase.Cms.WebUtility.BaseControls.BaseNodeTemplate, Mediachase.Cms.WebUtility" %>
<%@ Register src="../Modules/SearchControl.ascx" tagname="SearchControl" tagprefix="ecf" %>
<script runat="server">
    public override void LoadContext(IDictionary context)
    {
        base.LoadContext(context);
        SearchControl1.LoadContext(context);
    }
</script>
<ecf:SearchControl ID="SearchControl1" runat="server" />