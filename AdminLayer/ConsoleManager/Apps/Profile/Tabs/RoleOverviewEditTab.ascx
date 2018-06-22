<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="RoleOverviewEditTab.ascx.cs"
    Inherits="Mediachase.Commerce.Manager.Profile.Tabs.RoleOverviewEditTab" %>
<asp:HiddenField ID="SelectedPermissions" runat="server" />
<div id="DataForm">
    <table class="DataForm">
        <tr>
            <td class="FormLabelCell">
                <asp:Label ID="Label1" runat="server" Text="<%$ Resources:SharedStrings, Role_Name %>"></asp:Label>:
            </td>
            <td class="FormFieldCell">
                <asp:TextBox runat="server" ID="tbRoleName" Width="300px"></asp:TextBox>
                <asp:RequiredFieldValidator runat="server" ID="rfvRoleName" ControlToValidate="tbRoleName"
                    Display="Dynamic" Text="*"></asp:RequiredFieldValidator>
                <asp:CustomValidator runat="server" ID="RoleNameCustomValidator" ControlToValidate="tbRoleName"
                    OnServerValidate="RoleNameCheck" Display="Dynamic" ErrorMessage="<%$ Resources:SharedStrings, Role_Exists %>" />
            </td>
        </tr>
        <tr>
            <td class="FormLabelCell">
                <asp:Label ID="Label3" runat="server" Text="<%$ Resources:SharedStrings, Permissions %>"></asp:Label>:
            </td>
            <td class="FormFieldCell">
                <%--<div id="tree-div" style="overflow:auto; height:100% !important;width:400px;border:0px !important"></div>--%>
                <asp:UpdatePanel runat="server" ID="TreeUpdatePanel" ChildrenAsTriggers="true" UpdateMode="Conditional"
                    RenderMode="Block">
                    <ContentTemplate>
                        <div style="overflow-x: auto; overflow-y: auto; height: 500px; width: 500px; border: 1px solid;">
                            <asp:TreeView OnClick="OnTreeClick(event)" runat="server" CssClass="ProfileTreeView"
                                ID="PermissionsTree" ShowCheckBoxes="All" ShowLines="false">
                                <SelectedNodeStyle CssClass="ProfileSelectedTreeNode" Font-Bold="true" />
                            </asp:TreeView>
                        </div>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </td>
        </tr>
    </table>
</div>
