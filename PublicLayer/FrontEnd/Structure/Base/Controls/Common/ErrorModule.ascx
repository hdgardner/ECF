<%@ Control Language="C#" AutoEventWireup="true" EnableViewState="false" Inherits="Mediachase.Cms.Controls.Common.ErrorModule" Codebehind="ErrorModule.ascx.cs" %>
<asp:Repeater runat="server" ID="ErrorMessages">
    <HeaderTemplate>
        <table cellpadding="2" cellspacing="2" style="width: 100%">
            <tr>
                <td>
    </HeaderTemplate>
    <FooterTemplate>
        </td> </tr> </table>
    </FooterTemplate>
    <SeparatorTemplate>
        <table cellpadding="2" style="width: 100%">
            <tr>
                <td colspan="2">
                </td>
            </tr>
        </table>
    </SeparatorTemplate>
    <ItemTemplate>
        <table cellpadding="2" style="width: 100%" class="ecf-errorbox" cellspacing="2">
            <tr>
                <td style="width: 100%" class="ecf-errorline-moderate"><asp:Label EnableViewState="False" runat="server" ID="ErrorMessageLabel"><%# DataBinder.Eval(Container, "DataItem")%></asp:Label></td>
            </tr>
        </table>
    </ItemTemplate>
</asp:Repeater>
<div id="ajax_feedback" class="feedback"></div>