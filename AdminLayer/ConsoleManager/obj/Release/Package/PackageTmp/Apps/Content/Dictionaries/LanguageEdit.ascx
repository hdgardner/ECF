<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="LanguageEdit.ascx.cs" Inherits="Mediachase.Commerce.Manager.Content.Dictionaries.LanguageEdit" %>
<%@ Register Src="~/Apps/Core/SaveControl.ascx" TagName="SaveControl" TagPrefix="ecf" %>
<%@ Register Src="~/Apps/Core/Controls/EditViewControl.ascx" TagName="EditViewControl" TagPrefix="ecf" %>
<div class="editDiv">
    <ecf:EditViewControl AppId="Content" ViewId="Language-Edit" ID="ViewControl" runat="server"></ecf:EditViewControl>
    <ecf:SaveControl ID="EditSaveControl" CancelClientScript="CSContentClient.LanguageSaveRedirect();" SavedClientScript="CSContentClient.LanguageSaveRedirect();" runat="server"></ecf:SaveControl>
</div>