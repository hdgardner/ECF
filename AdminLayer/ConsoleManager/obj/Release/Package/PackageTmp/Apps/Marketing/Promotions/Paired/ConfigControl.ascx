<%@ Control Language="C#" AutoEventWireup="true"
    Inherits="Apps_Marketing_Promotions_Paired_ConfigControl" Codebehind="ConfigControl.ascx.cs" %>
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
    <tr>
        <td class="FormLabelCell">
            <asp:Label ID="Label1" runat="server" Text="<%$ Resources:MarketingStrings, Promotion_Quantity_X_needed_to_qualify %>"></asp:Label>:</td>
        <td class="FormFieldCell">
            <asp:TextBox runat="server" ID="SourceQuantity"></asp:TextBox>
            <asp:RequiredFieldValidator runat="server" ControlToValidate="SourceQuantity" Display="Dynamic" ErrorMessage="*"></asp:RequiredFieldValidator>
            <asp:RangeValidator runat="server" ControlToValidate="SourceQuantity" Display="Dynamic" ErrorMessage="*" Type="Currency" MinimumValue="0" MaximumValue="10000"></asp:RangeValidator>
        </td>
    </tr>
    <tr>
        <td colspan="2" class="FormSpacerCell">
        </td>
    </tr>    
    <tr>
        <td class="FormLabelCell">
            <asp:Label ID="Label4" runat="server" Text="<%$ Resources:MarketingStrings, Promotion_Quantity_Y_needed_to_qualify %>"></asp:Label>:</td>
        <td class="FormFieldCell">
            <asp:TextBox runat="server" ID="TargetQuantity"></asp:TextBox>
            <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="TargetQuantity" Display="Dynamic" ErrorMessage="*"></asp:RequiredFieldValidator>
            <asp:RangeValidator ID="RangeValidator1" runat="server" ControlToValidate="TargetQuantity" Display="Dynamic" ErrorMessage="*" Type="Currency" MinimumValue="0" MaximumValue="10000"></asp:RangeValidator>
        </td>
    </tr>    
        <tr>
            <td colspan="2" class="FormSpacerCell">
            </td>
        </tr>
        <tr>
            <td class="FormLabelCell">
                <asp:Label ID="Label9" runat="server" Text="<%$ Resources:SharedStrings, Amount %>"></asp:Label>:</td>
            <td class="FormFieldCell" valign="top">
                <asp:TextBox runat="server" ID="OfferAmount"></asp:TextBox>
                <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ControlToValidate="OfferAmount" Display="Dynamic" ErrorMessage="*"></asp:RequiredFieldValidator>
                <asp:RangeValidator ID="RangeValidator2" runat="server" ControlToValidate="OfferAmount" Display="Dynamic" ErrorMessage="*" Type="Currency" MinimumValue="-100000000" MaximumValue="100000000"></asp:RangeValidator>
                <asp:DropDownList runat="server" ID="OfferType">
                    <asp:ListItem Text="<%$ Resources:MarketingStrings, Promotion_Percentage_Based %>" Value="1"></asp:ListItem>
                    <asp:ListItem Text="<%$ Resources:MarketingStrings, Promotion_Value_Based %>" Value="2"></asp:ListItem>
                </asp:DropDownList>
            </td>
        </tr>           
</table>
