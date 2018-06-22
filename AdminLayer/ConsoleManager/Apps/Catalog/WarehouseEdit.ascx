<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="WarehouseEdit.ascx.cs" Inherits="Mediachase.Commerce.Manager.Catalog.WarehouseEdit" %>
<%@ Register Src="~/Apps/Core/SaveControl.ascx" TagName="SaveControl" TagPrefix="ecf" %>
<%@ Register Src="~/Apps/Core/Controls/EditViewControl.ascx" TagName="EditViewControl" TagPrefix="ecf" %>
<div class="editDiv">
<ecf:EditViewControl AppId="Catalog" ViewId="Warehouse-Edit" id="ViewControl" runat="server"></ecf:EditViewControl>
<ecf:SaveControl id="EditSaveControl" CancelClientScript="CSManagementClient.ChangeView('Catalog','Warehouse-List');" SavedClientScript="CSManagementClient.ChangeView('Catalog', 'Warehouse-List');" runat="server"></ecf:SaveControl>
</div>