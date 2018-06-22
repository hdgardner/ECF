<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="IndexTab.ascx.cs" Inherits="Mediachase.Commerce.Manager.Apps.Core.Search.Tabs.IndexTab" %>
<%@ Register src="~/Apps/Core/Controls/ProgressControl.ascx" tagname="ProgressControl" tagprefix="core" %>
<div id="DataForm">
    <table>
        <tr>
            <td class="FormLabelCell">
                <asp:UpdatePanel ID="MainPanel" runat="server" ChildrenAsTriggers="true" UpdateMode="Conditional">
                    <ContentTemplate>
                        <asp:Button runat="server" ID="RebuildIndexButton" OnClick="RebuildIndex" Text="<%$ Resources:CoreStrings, Search_Rebuild_Index %>" />
                    </ContentTemplate>
                </asp:UpdatePanel>
            </td>
            <td class="FormLabelCell">
                <asp:Literal ID="Literal1" runat="server" Text="<%$ Resources:CoreStrings, Search_Rebuild_Instructions %>"/>
            </td>
        </tr>
        <tr>
            <td colspan="2" class="FormSpacerCell">
            </td>
        </tr>
        
        <tr>
            <td class="FormLabelCell">
                <asp:UpdatePanel ID="UpdatePanel1" runat="server" ChildrenAsTriggers="true" UpdateMode="Conditional">
                    <ContentTemplate>
                        <asp:Button runat="server" ID="BuildIndexButton" OnClick="BuildIndex" Text="<%$ Resources:CoreStrings, Search_Build_Index %>" />
                    </ContentTemplate>
                </asp:UpdatePanel>
            </td>
            <td class="FormLabelCell">
                <asp:Literal ID="Literal2" runat="server" Text="<%$ Resources:CoreStrings, Search_Build_Instructions %>"/>
            </td>
        </tr>
        <tr>
            <td colspan="2" class="FormSpacerCell">
            </td>
        </tr>        
        <tr>
            <td colspan="2" class="FormLabelCell">
                <core:ProgressControl Title="Indexing Data" ID="ProgressControl1" runat="server" />
            </td>
        </tr>
    </table>
</div>


