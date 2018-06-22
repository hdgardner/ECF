<%@ Page Language="C#" AutoEventWireup="true" Inherits="Mediachase.Ibn.Web.UI.MetaDataBase.Pages.Admin.EnumView" MasterPageFile="~/Apps/MetaDataBase/MasterPages/ExtGrid.master" Codebehind="EnumView.aspx.cs" %>
<%@ Register TagPrefix="ibn" TagName="EnumViewControl" Src="~/Apps/MetaDataBase/Modules/ManageControls/EnumViewControl.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" Runat="Server">
	<ibn:EnumViewControl runat="server" ID="ucEnumViewControl" />
</asp:Content>
