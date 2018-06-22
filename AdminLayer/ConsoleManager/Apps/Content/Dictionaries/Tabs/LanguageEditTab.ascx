<%@ Control Language="C#" AutoEventWireup="True" CodeBehind="LanguageEditTab.ascx.cs" Inherits="Mediachase.Commerce.Manager.Content.Dictionaries.Tabs.LanguageEditTab" %>
<%@ Register Src="~/Apps/Core/Controls/BooleanEditControl.ascx" TagName="BooleanEditControl" TagPrefix="ecf" %>
<div id="DataForm">
    <table class="DataForm">
        <tr>
            <td class="FormLabelCell">
                <asp:Label ID="Label1" runat="server" Text="<%$ Resources:ContentStrings, Language_Code %>"></asp:Label>:
            </td>
            <td class="FormFieldCell">
                <asp:TextBox runat="server" ID="tbLanguageName" Width="100px" MaxLength="50" autocomplete="off"></asp:TextBox>
                <asp:RequiredFieldValidator runat="server" ID="rfvLangageName" ControlToValidate="tbLanguageName" Display="Dynamic" ErrorMessage="<%$ Resources:ContentStrings, Language_Code_Required %>"></asp:RequiredFieldValidator>
                <asp:CustomValidator runat="server" ID="CodeCheckCustomValidator" ControlToValidate="tbLanguageName" OnServerValidate="LanguageCodeCheck" Display="Dynamic" ErrorMessage="<%$ Resources:ContentStrings, Language_Code_Exists %>" />
                <br /><asp:Label ID="Label2" CssClass="FormFieldDescription" runat="server"><asp:Literal ID="Literal1" runat="server" Text="<%$ Resources:ContentStrings, Language_Code_Description %>"/></asp:Label>
            </td>
        </tr>
        <tr>
            <td colspan="2" class="FormSpacerCell">
            </td>
        </tr>
        <tr>
            <td class="FormLabelCell">
                <asp:Label ID="Label9" runat="server" Text="<%$ Resources:ContentStrings, Language_Friendly_Name %>"></asp:Label>:
            </td>
            <td class="FormFieldCell">
                <asp:TextBox runat="server" Width="300" ID="FriendlyNameText" MaxLength="50"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td colspan="2" class="FormSpacerCell">
            </td>
        </tr>
        <tr>
            <td class="FormLabelCell">
                <asp:Label ID="Label5" runat="server" Text="<%$ Resources:ContentStrings, Language_Is_Default %>"></asp:Label>:
            </td>
            <td class="FormFieldCell">
                <ecf:BooleanEditControl ID="IsDefault" runat="server"></ecf:BooleanEditControl>
            </td>
        </tr>
    </table>
</div>