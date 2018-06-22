<%@ Control Language="c#" Inherits="Mediachase.Commerce.Manager.Order.Shipping.Tabs.ShippingOptionEditBaseTab" Codebehind="ShippingOptionEditBaseTab.ascx.cs" %>
<%@ Register Src="~/Apps/Core/Controls/BooleanEditControl.ascx" TagName="BooleanEditControl" TagPrefix="ecf" %>
<div id="DataForm">
    <table class="DataForm">
        <tr>
            <td class="FormLabelCell">
                <asp:Label ID="lblShippingMethodIdText" runat="server" text="<%$ Resources:SharedStrings, ID %>"></asp:Label>:
            </td>
            <td class="FormFieldCell">
                <asp:Label runat="server" ID="lblShippingMethodId" CssClass="text"></asp:Label>
            </td>
        </tr>
        <tr>
            <td colspan="2" class="FormSpacerCell"></td>
        </tr>
        <tr>
            <td class="FormLabelCell">
                <asp:Label ID="lblName" runat="server" text="<%$ Resources:SharedStrings, Name %>"></asp:Label>:
            </td>
            <td class="FormFieldCell">
                <asp:TextBox runat="server" ID="tbName" CssClass="text" Width="200px"></asp:TextBox>
                <asp:RequiredFieldValidator ID="rfName" runat="server" ErrorMessage="<%$ Resources:SharedStrings, Name_Required %>"
                    Font-Size="9pt" Font-Names="verdana" Display="None" ControlToValidate="tbName" />
                <ajaxToolkit:ValidatorCalloutExtender runat="server" ID="vce" Width="220" TargetControlID="rfName"
                    HighlightCssClass="FormHighlight" />
            </td>
        </tr>
        <tr>
            <td colspan="2" class="FormSpacerCell"></td>
        </tr>
        <tr>
            <td class="FormLabelCell">
                <asp:Label ID="lblDescription" runat="server" text="<%$ Resources:SharedStrings, Description %>"></asp:Label>:
            </td>
            <td class="FormFieldCell">
                <asp:TextBox runat="server" ID="tbDescription" Width="350px" Rows="4" TextMode="MultiLine"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td colspan="2" class="FormSpacerCell"></td>
        </tr>
        <tr>
            <td class="FormLabelCell" style="vertical-align: middle; width: 120px">
                <asp:Label ID="lblSystemKeyword" runat="server" text="<%$ Resources:SharedStrings, System_Keyword %>"></asp:Label>:
            </td>
            <td class="FormFieldCell">
                <asp:TextBox runat="server" ID="tbSystemName" CssClass="text" Width="200px"></asp:TextBox>
                <asp:RequiredFieldValidator ID="rfSystemName" runat="server" ErrorMessage="<%$ Resources:SharedStrings, System_Name_Required %>"
                    Font-Size="9pt" Font-Names="verdana" Display="None" ControlToValidate="tbSystemName" />
                <ajaxToolkit:ValidatorCalloutExtender runat="server" ID="vcerfSystemName"
                    Width="220" TargetControlID="rfSystemName" HighlightCssClass="FormHighlight" />
                <asp:RegularExpressionValidator runat="server" ControlToValidate="tbSystemName" Display="None"
                    ValidationExpression="^[a-z,A-Z]\S*$" ID="revSystemName" ErrorMessage="<%$ Resources:SharedStrings, Latin_Symbols_Only %>" />
                <ajaxToolkit:ValidatorCalloutExtender runat="server" ID="vceRegularExpressionValidator1"
                    Width="220" TargetControlID="revSystemName" HighlightCssClass="FormHighlight" />
               <asp:CustomValidator runat="server" ID="SystemNameCustomValidator" ControlToValidate="tbSystemName" OnServerValidate="ShippingOptionSystemNameCheck" Display="Dynamic" ErrorMessage="<%$ Resources:SharedStrings, Shipping_Option_Name_Exists %>" />
            </td>
        </tr>
        <tr>
            <td colspan="2" class="FormSpacerCell"></td>
        </tr>
        <tr>
            <td class="FormLabelCell" style="vertical-align: middle; width: 120px">
                <asp:Label ID="Label2" runat="server" text="<%$ Resources:SharedStrings, Class_Name %>"></asp:Label>:
            </td>
            <td class="FormFieldCell">
                <asp:DropDownList runat="server" id="ddlClassName">
                    <asp:ListItem Value="" text="<%$ Resources:SharedStrings, select_class %>"></asp:ListItem>
                </asp:DropDownList>
                <asp:RequiredFieldValidator runat="server" ID="RequiredFieldValidator1" ControlToValidate="ddlClassName" ErrorMessage="*" Display="Dynamic"></asp:RequiredFieldValidator>
            </td>
        </tr>
        <tr>
            <td colspan="2" class="FormSpacerCell"></td>
        </tr>
    </table>
</div>