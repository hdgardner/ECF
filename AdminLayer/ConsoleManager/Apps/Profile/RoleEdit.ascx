<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="RoleEdit.ascx.cs" Inherits="Mediachase.Commerce.Manager.Profile.RoleEdit" %>
<%@ Register Src="../Core/SaveControl.ascx" TagName="SaveControl" TagPrefix="ecf" %>
<%@ Register Src="../Core/Controls/EditViewControl.ascx" TagName="EditViewControl" TagPrefix="ecf" %>
<div class="editDiv">
<ecf:EditViewControl AppId="Profile" ViewId="Role-Edit" id="ViewControl" runat="server"></ecf:EditViewControl>
<ecf:SaveControl id="EditSaveControl" ShowDeleteButton="false" CancelClientScript="CSManagementClient.ChangeView('Profile','Roles-List');" SavedClientScript="CSManagementClient.ChangeView('Profile','Roles-List');" runat="server"></ecf:SaveControl>
</div>