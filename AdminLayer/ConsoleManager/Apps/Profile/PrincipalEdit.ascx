<%@ Control Language="C#" AutoEventWireup="true" Inherits="Mediachase.Commerce.Manager.Profile.PrincipalEdit" Codebehind="PrincipalEdit.ascx.cs" %>
<%@ Register Src="../Core/SaveControl.ascx" TagName="SaveControl" TagPrefix="ecf" %>
<%@ Register Src="../Core/Controls/EditViewControl.ascx" TagName="EditViewControl" TagPrefix="ecf" %>
<div class="editDiv">
<ecf:EditViewControl AppId="Profile" ViewId="Account-Edit" id="ViewControl" runat="server"></ecf:EditViewControl>
<ecf:SaveControl id="EditSaveControl" runat="server"></ecf:SaveControl>
</div>