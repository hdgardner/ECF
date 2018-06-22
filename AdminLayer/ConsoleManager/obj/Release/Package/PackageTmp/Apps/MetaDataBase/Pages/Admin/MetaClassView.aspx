<%@ Page Language="C#" AutoEventWireup="true" Inherits="Mediachase.Ibn.Web.UI.MetaDataBase.Pages.Admin.MetaClassView" MasterPageFile="~/Apps/MetaDataBase/MasterPages/ExtGrid.master" Codebehind="MetaClassView.aspx.cs" %>
<%@ Register TagPrefix="mc" TagName="classView" Src="~/Apps/MetaDataBase/Modules/ManageControls/MetaClassView.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" Runat="Server">
	<mc:classView ID="ucClassView" runat="server" />
</asp:Content>

