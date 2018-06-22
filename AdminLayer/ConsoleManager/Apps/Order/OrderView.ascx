<%@ Control Language="C#" AutoEventWireup="true" Inherits="Mediachase.Commerce.Manager.Order.OrderView"
    CodeBehind="OrderView.ascx.cs" %>
<%@ Register Src="Tabs/OrderGroupEditTab.ascx" TagName="OrderGroupEditTab" TagPrefix="uc1" %>
<%@ Register TagPrefix="mc" TagName="MetaToolbar" Src="~/Apps/Core/Controls/MetaToolbar.ascx" %>
<%@ Register Src="~/Apps/Core/SaveControl.ascx" TagName="SaveControl" TagPrefix="ecf" %>
<%@ Register Src="~/Apps/Core/Controls/EditViewControl.ascx" TagName="EditViewControl"
    TagPrefix="ecf" %>
<IbnWebControls:McDock ID="DockTop" runat="server" Anchor="top" EnableSplitter="False"
    DefaultSize="26">
    <DockItems>
        <asp:UpdatePanel runat="server" ID="panelToolbar" UpdateMode="Conditional">
            <ContentTemplate>
                <table runat="server" id="topTable" cellspacing="0" cellpadding="0" border="0" width="100%">
                    <tr>
                        <td style="padding-left: 0px; padding-right: 0px;">
                            <mc:MetaToolbar runat="server" ID="MetaToolbar1" />
                        </td>
                    </tr>
                </table>
            </ContentTemplate>
        </asp:UpdatePanel>
    </DockItems>
</IbnWebControls:McDock>
        <table class="fill">
            <tr>
                <td valign="top">
                    <asp:ValidationSummary runat="server" />
                    <uc1:OrderGroupEditTab ID="OrderGroupEdit" runat="server"></uc1:OrderGroupEditTab>
                </td>
            </tr>
            <tr>
                <td style="height: 400px; vertical-align: top;">
                    <ecf:EditViewControl ID="ViewControl" runat="server"></ecf:EditViewControl>
                    <ecf:SaveControl ID="EditSaveControl" runat="server"></ecf:SaveControl>
                </td>
            </tr>
        </table>
