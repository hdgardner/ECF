<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="RuleEditDialog.ascx.cs"
    Inherits="Mediachase.Commerce.Manager.Apps.Marketing.Dialogs.RuleEditDialog" %>
<%@ Register TagPrefix="ibn" Assembly="Mediachase.Ibn.Web.UI.WebControls" Namespace="Mediachase.Ibn.Web.UI.WebControls" %>
<div id="DataForm">
    <table class="DataForm">
        <tr>
            <td class="FormLabelCell">
                <asp:Label ID="Label1" runat="server" Text="<%$ Resources:MarketingStrings, Expression_Name %>"></asp:Label>:
            </td>
            <td class="FormFieldCell">
                <asp:TextBox runat="server" Width="250" ID="ExpressionName"></asp:TextBox><br />
                <asp:Label ID="Label2" CssClass="FormFieldDescription" runat="server" Text="<%$ Resources:MarketingStrings, Expression_Name_Enter %>"></asp:Label>
                <asp:RequiredFieldValidator runat="server" ID="ExpressionNameRequired" ControlToValidate="ExpressionName"
                    Display="None" ErrorMessage="<%$ Resources:MarketingStrings, Expression_Name_Required %>" />
                <ajaxToolkit:ValidatorCalloutExtender runat="Server" ID="ExpressionNameRequiredE"
                    Width="220" TargetControlID="ExpressionNameRequired" HighlightCssClass="FormHighlight" />
            </td>
        </tr>
        <tr>
            <td colspan="2" class="FormSpacerCell">
            </td>
        </tr>
        <tr>
            <td class="FormLabelCell">
                <asp:Label ID="Label3" runat="server" Text="<%$ Resources:MarketingStrings, Expression_Xml %>"></asp:Label>:
            </td>
            <td class="FormFieldCell">
                <div style="height:415px; width: 490px; overflow: auto;">
                <asp:UpdatePanel runat="server" ID="FilterPanel" ChildrenAsTriggers="true" UpdateMode="Conditional">
                    <ContentTemplate>
                        <ibn:FilterExpressionBuilder runat="server" ID="ExprFilter" />
                    </ContentTemplate>
                </asp:UpdatePanel>
                </div>
            </td>
        </tr>
    </table>
    <table cellpadding="0" cellspacing="0" width="100%">
    <tr>
        <td colspan="2" style="background-image: url(../../../App_Themes/Main/images/dialog/bottom_content.gif);
            height: 41px; padding-right: 10px;" align="right">
            <asp:Button runat="server" ID="SaveButton" OnClick="SaveButton_Click" Text="<%$ Resources:SharedStrings, OK %>"
                Width="80px" />
            &nbsp;&nbsp;&nbsp;
            <asp:Button runat="server" ID="CancelButton" CausesValidation="false" Text="<%$ Resources:SharedStrings, Close %>"
                Width="80px" />
        </td>
    </tr>
</table>
</div>

