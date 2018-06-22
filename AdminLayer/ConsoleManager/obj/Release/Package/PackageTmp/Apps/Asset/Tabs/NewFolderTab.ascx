<%@ Control Language="C#" AutoEventWireup="true" Inherits="Mediachase.Commerce.Manager.Asset.Tabs.NewFolderTab" Codebehind="NewFolderTab.ascx.cs" %>
<%@ Register Assembly="Mediachase.FileUploader" Namespace="Mediachase.FileUploader.Web.UI" TagPrefix="mcf" %>
<asp:Panel runat="server" DefaultButton="btnCreate">
<table cellspacing="8">
    <tr>
        <td>
            <table width="100%" cellspacing="8" border="0">
                <tr>
                    <td><asp:Literal runat="server" Text="<%$ Resources:SharedStrings, Name %>"/></td>
                    <td><asp:TextBox runat="server" ID="FolderName" Width="280"></asp:TextBox><asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="FolderName" Display="Static" ErrorMessage="*"></asp:RequiredFieldValidator></td>
                    <td><asp:Button ID="btnCreate" CausesValidation="true" runat="server" Text="<%$ Resources:SharedStrings, Create %>" OnClick="btnCreate_ServerClick" /></td>
                </tr>
                <tr>
                    <td></td>
                    <td><asp:CustomValidator runat="server" ID="FolderNameCheckCustomValidator" ControlToValidate="FolderName" OnServerValidate="FolderNameCheck" Display="Dynamic" ErrorMessage="<%$ Resources:AssetStrings, Folder_With_Name_Exists %>" /></td>
                </tr>
            </table>
        </td>
    </tr>
</table>
</asp:Panel>
