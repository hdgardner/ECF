<%@ Control Language="C#" AutoEventWireup="true"
    Inherits="Apps_Marketing_Promotions_BuyXGetYFree_ConfigControl" Codebehind="ConfigControl.ascx.cs" %>
<%@ Register Src="../../Modules/EntryFilter.ascx" TagName="EntryFilter" TagPrefix="uc1" %>
<table class="DataForm">
    <tr>
        <td colspan="2" class="FormFieldCell">
            <asp:Label runat="server" ID="Description"></asp:Label>
        </td>
    </tr>
    <tr>
        <td colspan="2" class="FormSpacerCell">
        </td>
    </tr>
    <tr>
        <td class="FormLabelCell">
            <asp:Label ID="Label3" runat="server" Text="<%$ Resources:MarketingStrings, Promotion_Select_Catalog_Entry_X %>"></asp:Label>:</td>
        <td class="FormFieldCell">
            <uc1:EntryFilter ID="EntryXFilter" runat="server" />
        </td>
    </tr>
    <tr>
        <td colspan="2" class="FormSpacerCell">
        </td>
    </tr>
    <tr>
        <td class="FormLabelCell">
            <asp:Label ID="Label2" runat="server" Text="<%$ Resources:MarketingStrings, Promotion_Select_Catalog_Entry_Y %>"></asp:Label>:</td>
        <td class="FormFieldCell">
            <uc1:EntryFilter ID="EntryYFilter" runat="server" />
        </td>
    </tr>
    <tr>
        <td colspan="2" class="FormSpacerCell">
        </td>
    </tr>    
</table>
