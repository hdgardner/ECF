<%@ Control Language="c#" Inherits="Mediachase.Commerce.Manager.Order.Payments.Tabs.PaymentMethodEditBaseTab" Codebehind="PaymentMethodEditBaseTab.ascx.cs" %>
<%@ Register Src="~/Apps/Core/Controls/BooleanEditControl.ascx" TagName="BooleanEditControl" TagPrefix="ecf" %>
<%@ Register TagPrefix="console" Namespace="Mediachase.Web.Console.Controls" Assembly="Mediachase.WebConsoleLib" %>
<div id="DataForm">
    <table class="DataForm">
        <tr>
            <td class="FormLabelCell">
                <asp:Label ID="lblPaymentMethodIdText" runat="server" Text="<%$ Resources:SharedStrings, Id %>"></asp:Label>:
            </td>
            <td class="FormFieldCell">
                <asp:Label runat="server" ID="lblPaymentMethodId" CssClass="text"></asp:Label>
            </td>
        </tr>
        <tr>
            <td colspan="2" class="FormSpacerCell"></td>
        </tr>
        <tr>
            <td class="FormLabelCell">
                <asp:Label ID="lblName" runat="server" Text="<%$ Resources:SharedStrings, Name %>"></asp:Label>:
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
                <asp:Label ID="lblDescription" runat="server" Text="<%$ Resources:SharedStrings, Description %>"></asp:Label>:
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
                <asp:Label ID="lblSystemKeyword" runat="server" Text="<%$ Resources:SharedStrings, System_Keyword %>"></asp:Label>:
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
                <asp:CustomValidator runat="server" ID="SystemNameCustomValidator" ControlToValidate="tbSystemName" OnServerValidate="PaymentMethodSystemNameCheck" Display="Dynamic" ErrorMessage="<%$ Resources:SharedStrings, Payment_Method_Exists %>" />
            </td>
        </tr>
        <tr>
            <td colspan="2" class="FormSpacerCell"></td>
        </tr>
        <tr>
            <td class="FormLabelCell" style="vertical-align: middle; width: 120px">
                <asp:Label ID="lblLanguage" runat="server" Text="<%$ Resources:SharedStrings, Language %>"></asp:Label>:
            </td>
            <td class="FormFieldCell">
                <asp:DropDownList runat="server" id="ddlLanguage">
                    <asp:ListItem Value="" Text="<%$ Resources:SharedStrings, select_language %>"></asp:ListItem>
                </asp:DropDownList>
                <asp:RequiredFieldValidator runat="server" ID="LanguageRequired" ControlToValidate="ddlLanguage" ErrorMessage="*" Display="Dynamic"></asp:RequiredFieldValidator>
            </td>
        </tr>
        <tr>
            <td colspan="2" class="FormSpacerCell"></td>
        </tr>
        <tr>
            <td class="FormLabelCell" style="vertical-align: middle; width: 120px">
                <asp:Label ID="Label2" runat="server" Text="<%$ Resources:SharedStrings, Class_Name %>"></asp:Label>:
            </td>
            <td class="FormFieldCell">
                <asp:DropDownList runat="server" id="ddlClassName">
                    <asp:ListItem Value="" Text="<%$ Resources:SharedStrings, select_class %>"></asp:ListItem>
                </asp:DropDownList>
                <asp:RequiredFieldValidator runat="server" ID="ClassNameRequired" ControlToValidate="ddlClassName" ErrorMessage="*" Display="Dynamic"></asp:RequiredFieldValidator>
            </td>
        </tr>
        <tr>
            <td colspan="2" class="FormSpacerCell"></td>
        </tr>
        <tr>  
            <td class="FormLabelCell"><asp:Label ID="Label1" runat="server" Text="<%$ Resources:SharedStrings, Sort_Order %>"></asp:Label>:</td> 
            <td class="FormFieldCell">
                <asp:TextBox runat="server" ID="tbSortOrder"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td colspan="2" class="FormSpacerCell"></td>
        </tr>
        <tr>
            <td class="FormLabelCell"><asp:Label ID="Label3" runat="server" Text="<%$ Resources:SharedStrings, IsActive %>"></asp:Label>:</td> 
            <td class="FormFieldCell">
                <ecf:BooleanEditControl id="IsActive" runat="server"></ecf:BooleanEditControl>
            </td>
        </tr>
        <tr>
            <td colspan="2" class="FormSpacerCell"></td>
        </tr>
        <tr>
            <td class="FormLabelCell"><asp:Label ID="Label4" runat="server" Text="<%$ Resources:SharedStrings, IsDefault %>"></asp:Label>:</td> 
            <td class="FormFieldCell">
                <ecf:BooleanEditControl id="IsDefault" runat="server"></ecf:BooleanEditControl>
            </td>
        </tr>
        <tr>
            <td colspan="2" class="FormSpacerCell"></td>
        </tr>
        <tr>
            <td class="FormLabelCell"><asp:Label ID="Label5" runat="server" Text="<%$ Resources:SharedStrings, Supports_Recurring %>"></asp:Label>:</td> 
            <td class="FormFieldCell">
                <ecf:BooleanEditControl id="SupportsRecurring" runat="server"></ecf:BooleanEditControl>
            </td>
        </tr>
        
        <tr>
            <td class="FormSectionCell" colspan="2">
                <asp:Label runat="server" ID="lblShippingMethods" Text="<%$ Resources:SharedStrings, Restricted_Shipping_Methods %>"></asp:Label>
            </td>
        </tr>
		<tr>
			<td class="FormFieldCell" colspan="2">
				<console:DualList id="ShippingMethodsList" runat="server" ListRows="6" EnableMoveAll="True" CssClass="text"
					LeftDataTextField="Name" LeftDataValueField="ShippingMethodId" RightDataTextField="Name" RightDataValueField="ShippingMethodId"
					ItemsName="Shipping Methods">
					<RightListStyle Font-Bold="True" Width="200px" Height="150px"></RightListStyle>
					<ButtonStyle Width="100px"></ButtonStyle>
					<LeftListStyle Width="200px" Height="150px"></LeftListStyle>
				</console:DualList>
			</td>
		</tr>
    </table>
</div>