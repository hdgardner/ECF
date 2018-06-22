<%@ Control Language="C#" AutoEventWireup="true" Inherits="Mediachase.Commerce.Manager.Catalog.CatalogNodeEdit" Codebehind="CatalogNodeEdit.ascx.cs" %>
<%@ Register Src="~/Apps/Core/SaveControl.ascx" TagName="SaveControl" TagPrefix="ecf" %>
<%@ Register Src="~/Apps/Core/Controls/EditViewControl.ascx" TagName="EditViewControl" TagPrefix="ecf" %>
<div class="editDiv">
    <ecf:EditViewControl AppId="Catalog" ViewId="Node-Edit" id="ViewControl" runat="server" MDContext="<%# Mediachase.Commerce.Catalog.CatalogContext.MetaDataContext %>"></ecf:EditViewControl>
    <ecf:SaveControl id="EditSaveControl" CancelMessage="Catalog Node changes has been discarded" SavedMessage="Catalog Node has been updated successfully." CancelClientScript="CSCatalogClient.CatalogSaveRedirect();" SavedClientScript="CSCatalogClient.CatalogSaveRedirect();" runat="server"></ecf:SaveControl>
</div>