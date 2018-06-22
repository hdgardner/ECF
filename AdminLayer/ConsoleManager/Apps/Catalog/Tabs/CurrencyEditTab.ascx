<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CurrencyEditTab.ascx.cs" Inherits="Mediachase.Commerce.Manager.Catalog.Tabs.CurrencyEditTab" %>
<%@ Register Src="~/Apps/Core/Controls/BooleanEditControl.ascx" TagName="BooleanEditControl" TagPrefix="ecf" %>
<div id="DataForm">
    <table class="DataForm">
        <tr>
            <td class="FormLabelCell">
                <asp:Label ID="Label1" runat="server" Text="<%$ Resources:CatalogStrings, Currency_Name%>"></asp:Label>:
            </td>
            <td class="FormFieldCell">
                <asp:TextBox runat="server" ID="tbCurrencyName" Width="300px" MaxLength="50"></asp:TextBox>
                <asp:RequiredFieldValidator runat="server" ID="rfvCurrencyName" ControlToValidate="tbCurrencyName" Display="Dynamic" Text="<%$ Resources:CatalogStrings, Currency_Name_Required %>"></asp:RequiredFieldValidator>
            </td>
        </tr>
        <tr>
            <td colspan="2" class="FormSpacerCell">
            </td>
        </tr>
        <tr>
            <td class="FormLabelCell">
                <asp:Label ID="Label9" runat="server" Text="<%$ Resources:SharedStrings, Code %>"></asp:Label>:
            </td>
            <td class="FormFieldCell">
                <asp:TextBox runat="server" Width="50" ID="CodeText" MaxLength="3"></asp:TextBox>
                <asp:RequiredFieldValidator ID="CodeRequiredValidator" runat="server" ControlToValidate="CodeText" ErrorMessage="<%$ Resources:SharedStrings, Code_Required %>" Display="Dynamic"></asp:RequiredFieldValidator>
                <asp:CustomValidator runat="server" ID="CodeCheckCustomValidator" ControlToValidate="CodeText" OnServerValidate="CurrencyCodeCheck" Display="Dynamic" ErrorMessage="<%$ Resources:CatalogStrings, Currency_With_Code_Exists %>" />
            </td>
        </tr>
        <tr>
            <td colspan="2" class="FormSpacerCell">
            </td>
        </tr>
        <tr>
            <td class="FormLabelCell">
                <asp:Label ID="Label11" runat="server" Text="<%$ Resources:SharedStrings, Modified %>"></asp:Label>:
            </td>
            <td class="FormLabelCell">
                <asp:Label ID="ModifiedDateLabel" runat="server"></asp:Label>
            </td>
        </tr>
    </table>
</div>