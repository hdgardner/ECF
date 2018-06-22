<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ConfigControl.ascx.cs"
    Inherits="Apps_Marketing_Promotions_BuyXGetOffDiscount" %>
<%@ Register Src="../../Modules/EntryFilter.ascx" TagName="EntryFilter" TagPrefix="uc1" %>
<div id="DataForm">
    <asp:HiddenField runat="server" ID="SelectedEntries" />
    <table class="DataForm">
        <tr>
            <td colspan="2" class="FormFieldCell">
                <asp:Label runat="server" ID="Description"></asp:Label>
            </td>
        </tr>
        <tr>
            <td class="FormLabelCell">
                <asp:Label ID="Label11" runat="server" Text="<%$ Resources:SharedStrings, Minimum_Quantity %>"></asp:Label>:
            </td>
            <td class="FormFieldCell">
                <asp:TextBox runat="server" ID="MinQuantity"></asp:TextBox>
                <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ControlToValidate="MinQuantity"
                    Display="Dynamic" ErrorMessage="*"></asp:RequiredFieldValidator>
                <asp:RangeValidator ID="RangeValidator2" runat="server" ControlToValidate="MinQuantity"
                    Display="Dynamic" ErrorMessage="*" Type="Currency" MinimumValue="0" MaximumValue="100000000"></asp:RangeValidator>
            </td>
        </tr>
        <tr>
            <td colspan="2" class="FormSpacerCell">
            </td>
        </tr>
        <tr>
            <td class="FormLabelCell">
                <asp:Label ID="Label12" runat="server" Text="<%$ Resources:SharedStrings, Amount %>"></asp:Label>:
            </td>
            <td class="FormFieldCell" valign="top">
                <asp:TextBox runat="server" ID="OfferAmount"></asp:TextBox>
                <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" ControlToValidate="OfferAmount"
                    Display="Dynamic" ErrorMessage="*"></asp:RequiredFieldValidator>
                <asp:RangeValidator ID="RangeValidator3" runat="server" ControlToValidate="OfferAmount"
                    Display="Dynamic" ErrorMessage="*" Type="Currency" MinimumValue="-100000000"
                    MaximumValue="100000000"></asp:RangeValidator>
                <asp:DropDownList runat="server" ID="OfferType">
                    <asp:ListItem Text="<%$ Resources:MarketingStrings, Promotion_Percentage_Based %>" Value="1"></asp:ListItem>
                    <asp:ListItem Text="<%$ Resources:MarketingStrings, Promotion_Value_Based %>" Value="2"></asp:ListItem>
                </asp:DropDownList>                    
            </td>
        </tr>
        <tr>
            <td colspan="2" class="FormSpacerCell">
            </td>
        </tr>
        <tr>
            <td class="FormLabelCell">
                <asp:Label ID="Label13" runat="server" Text="<%$ Resources:SharedStrings, Variations %>"></asp:Label>:
            </td>
            <td class="FormFieldCell">
                <uc1:EntryFilter ID="SkuEntryFilter" IsFieldRequired="false" runat="server" />
                <asp:LinkButton runat="server" ID="AddEntry" Text="<%$ Resources:MarketingStrings, Promotion_Add_Variation %>" CausesValidation="false"></asp:LinkButton>
                <br />
                <asp:DataList runat="server" ID="EntryList">
                    <ItemTemplate>
                        <asp:ImageButton CausesValidation="false" runat="server" ID="DeleteButton" OnCommand="DeleteButton_Command"
                            ImageUrl="../../images/delete.png" CommandArgument='<%#Container.DataItem%>' />
                        <%#GetCatalogEntryName(Container.DataItem)%>
                    </ItemTemplate>
                </asp:DataList>
            </td>
        </tr>
    </table>
</div>
