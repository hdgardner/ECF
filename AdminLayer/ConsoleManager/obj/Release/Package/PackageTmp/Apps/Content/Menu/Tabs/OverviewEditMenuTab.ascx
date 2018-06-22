<%@ Control Language="C#" AutoEventWireup="true" Inherits="Mediachase.Commerce.Manager.Menu.Tabs.OverviewEditMenuTab" Codebehind="OverviewEditMenuTab.ascx.cs" %>
<%@ Register Src="~/Apps/Core/Controls/HtmlEditControl.ascx" TagName="HtmlEditControl" TagPrefix="ecf" %>
<%@ Register Src="~/Apps/Core/Controls/BooleanEditControl.ascx" TagName="BooleanEditControl" TagPrefix="ecf" %>
<div id="DataForm">
    <table class="DataForm" width="650"> 
        <tr>
            <td class="FormLabelCell"><asp:Label ID="Literal5" runat="server" Text='<%$ Resources:Admin, Text %>'></asp:Label> <asp:Label runat="server" ID="NameLanguageCode"></asp:Label>:</td>
            <td class="FormFieldCell">
                <asp:TextBox runat="server" Width="250" ID="Name"></asp:TextBox>
                <asp:RequiredFieldValidator runat="server" ID="NameValidation" ControlToValidate="Name" 
                    ErrorMessage="<%$ Resources:ContentStrings, Menu_Name_Required %>" Display="None"></asp:RequiredFieldValidator>
                <ajaxToolkit:ValidatorCalloutExtender runat="server" ID="vceNameRequired" Width="220" TargetControlID="NameValidation" HighlightCssClass="FormHighlight"/>
                <asp:CustomValidator runat="server" ID="NameCheckCustomValidator" ControlToValidate="Name" OnServerValidate="NameCheck" Display="Dynamic" ErrorMessage="<%$ Resources:ContentStrings, Menu_With_Name_Exists %>" />
                <br />
                <asp:Label ID="Label4" CssClass="FormFieldDescription" runat="server"
                    Text="<%$ Resources:ContentStrings, Menu_Name_Instructions %>"></asp:Label>
            </td>
        </tr>
    </table>
</div>