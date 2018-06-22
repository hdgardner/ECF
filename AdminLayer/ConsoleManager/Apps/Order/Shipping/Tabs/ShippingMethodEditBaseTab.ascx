<%@ Control Language="c#" Inherits="Mediachase.Commerce.Manager.Order.Shipping.Tabs.ShippingMethodEditBaseTab" Codebehind="ShippingMethodEditBaseTab.ascx.cs" %>
<%@ Register Src="~/Apps/Core/Controls/BooleanEditControl.ascx" TagName="BooleanEditControl" TagPrefix="ecf" %>
<div id="DataForm">
    <table class="DataForm">
        <tr>
            <td class="FormLabelCell">
                <asp:Label ID="lblShippingMethodIdText" runat="server" Text="<%$ Resources:SharedStrings, ID %>"></asp:Label>:
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
                <asp:Label ID="lblName" runat="server" Text="<%$ Resources:SharedStrings, Name %>"></asp:Label>:
            </td>
            <td class="FormFieldCell">
                <asp:TextBox runat="server" ID="tbName" CssClass="text" Width="200px"></asp:TextBox>
                <asp:RequiredFieldValidator ID="rfName" runat="server" ErrorMessage="<%$ Resources:SharedStrings, Name_Required %>"
                    Font-Size="9pt" Font-Names="verdana" Display="None" ControlToValidate="tbName" />
                <ajaxToolkit:ValidatorCalloutExtender runat="server" ID="vce" Width="220" TargetControlID="rfName"
                    HighlightCssClass="FormHighlight" />
                    <asp:RegularExpressionValidator runat="server" ControlToValidate="tbName" Display="None"
                    ValidationExpression="^[a-z,A-Z,0-9,\s]*$" ID="revName" ErrorMessage="<%$ Resources:SharedStrings, Latin_Symbols_Only %>" />
                <ajaxToolkit:ValidatorCalloutExtender runat="server" ID="vceRegularExpressionValidator1"
                    Width="220" TargetControlID="revName" HighlightCssClass="FormHighlight" />
                <asp:CustomValidator runat="server" ID="SystemNameCustomValidator" ControlToValidate="tbName" OnServerValidate="ShippingMethodNameCheck" Display="Dynamic" ErrorMessage="<%$ Resources:SharedStrings, Shipping_Method_Exists %>" />
            </td>
        </tr>
        <tr>
            <td colspan="2" class="FormSpacerCell"></td>
        </tr>
        <tr>
            <td class="FormLabelCell" style="vertical-align: middle; width: 120px">
                <asp:Label ID="lblFriendlyName" runat="server" Text="<%$ Resources:SharedStrings, Friendly_Name %>"></asp:Label>:
            </td>
            <td class="FormFieldCell">
                <asp:TextBox runat="server" ID="tbFriendlyName" CssClass="text" Width="200px"></asp:TextBox>
                <asp:RequiredFieldValidator ID="rfFriendlyName" runat="server" ErrorMessage="<%$ Resources:SharedStrings, Friendly_Name_Required %>"
                    Font-Size="9pt" Font-Names="verdana" Display="None" ControlToValidate="tbFriendlyName" />
                <ajaxToolkit:ValidatorCalloutExtender runat="server" ID="vcerfFriendlyName"
                    Width="220" TargetControlID="rfFriendlyName" HighlightCssClass="FormHighlight" />
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
                <asp:Label ID="Label2" runat="server" Text="<%$ Resources:SharedStrings, Provider %>"></asp:Label>:
            </td>
            <td class="FormFieldCell">
                <asp:UpdatePanel UpdateMode="Conditional" ID="UpdatePanel1" runat="server" RenderMode="Inline">
                    <ContentTemplate>
                        <asp:DropDownList runat="server" id="ddlShippingOption" DataValueField="ShippingOptionId" DataTextField="Name">
                            <asp:ListItem Value="" Text="<%$ Resources:SharedStrings, select_shipping_option %>"></asp:ListItem>
                        </asp:DropDownList>
                        <asp:RequiredFieldValidator runat="server" ID="ShippingOptionRequired" ControlToValidate="ddlShippingOption" ErrorMessage="*" Display="Dynamic"></asp:RequiredFieldValidator>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </td>
        </tr>
        <!-- BEGIN: shipping option parameters -->
        <tr>
            <td class="FormFieldCell" colspan="2">
                <asp:UpdatePanel UpdateMode="Conditional" ID="ShippingOptionParametersContentPanel" runat="server" RenderMode="Inline">
                    <ContentTemplate>
                        <asp:placeholder id="phShippingOptionPatameters" Runat="server"></asp:placeholder>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </td>
        </tr>
        <!-- END: shipping option parameters -->
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
            <td class="FormLabelCell"><asp:Label ID="Label1" runat="server" Text="<%$ Resources:SharedStrings, Base_Price %>"></asp:Label>:</td> 
            <td class="FormFieldCell">
                <asp:TextBox runat="server" ID="tbBasePrice"></asp:TextBox>
                <asp:RequiredFieldValidator ID="rfBasePrice" runat="server" ErrorMessage="<%$ Resources:SharedStrings, Base_Price_Required %>"
                    Font-Size="9pt" Font-Names="verdana" Display="None" ControlToValidate="tbBasePrice" />
                <ajaxToolkit:ValidatorCalloutExtender runat="server" ID="vceBasePrice" Width="220" TargetControlID="rfBasePrice" HighlightCssClass="FormHighlight" />
                <asp:RangeValidator runat="server" ID="rvBasePrice" Display="None" ControlToValidate="tbBasePrice" MinimumValue="0" MaximumValue="1000000000" Type="Currency" ErrorMessage="<%$ Resources:SharedStrings, Base_Price_Invalid %>"></asp:RangeValidator>
                <ajaxToolkit:ValidatorCalloutExtender runat="server" ID="vseBasePriceRange" Width="220" TargetControlID="rfBasePrice" HighlightCssClass="FormHighlight" />
            </td>
        </tr>
        <tr>
            <td colspan="2" class="FormSpacerCell"></td> 
        </tr>
        <tr>  
            <td class="FormLabelCell"><asp:Label ID="Label7" runat="server" Text="<%$ Resources:SharedStrings, Currency %>"></asp:Label>:</td> 
            <td class="FormFieldCell">
                <asp:DropDownList runat="server" id="ddlCurrency">
                    <asp:ListItem Value="" Text="<%$ Resources:SharedStrings, select_currency %>"></asp:ListItem>
                </asp:DropDownList>
                <asp:RequiredFieldValidator runat="server" ID="RequiredFieldValidator1" ControlToValidate="ddlCurrency" ErrorMessage="*" Display="Dynamic"></asp:RequiredFieldValidator>
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
            <td class="FormLabelCell"><asp:Label ID="Label5" runat="server" Text="<%$ Resources:SharedStrings, IsDefault %>"></asp:Label>:</td> 
            <td class="FormFieldCell">
                <ecf:BooleanEditControl id="IsDefault" runat="server"></ecf:BooleanEditControl>
            </td>
        </tr>
        <tr>
            <td colspan="2" class="FormSpacerCell"></td>
        </tr>
        <tr>  
            <td class="FormLabelCell"><asp:Label ID="Label4" runat="server" Text="<%$ Resources:SharedStrings, Sort_Order %>"></asp:Label>:</td> 
            <td class="FormFieldCell">
                <asp:TextBox runat="server" ID="tbSortOrder"></asp:TextBox>
                <asp:RangeValidator runat="server" ID="RangeValidator1" Display="None" ControlToValidate="tbSortOrder" MinimumValue="0" MaximumValue="1000000000" Type="Integer" ErrorMessage="<%$ Resources:SharedStrings, Sort_Order_Invalid %>"></asp:RangeValidator>
                <ajaxToolkit:ValidatorCalloutExtender runat="server" ID="ValidatorCalloutExtender1" Width="220" TargetControlID="rfBasePrice" HighlightCssClass="FormHighlight" />
            </td>
        </tr>
        <tr>
            <td colspan="2" class="FormSpacerCell"></td>
        </tr>
    </table>
</div>