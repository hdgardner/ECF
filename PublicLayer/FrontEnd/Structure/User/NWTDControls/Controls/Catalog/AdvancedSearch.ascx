<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AdvancedSearch.ascx.cs" Inherits="Mediachase.Cms.Website.Structure.User.NWTDControls.Controls.Catalog.AdvancedSearch" %>
<div class="nwtd-advanced-search" >
<h3>Search Our Catalog</h3>
	<div  class="nwtd-advanced-search-top">
		<asp:TextBox runat="server" ID="tbSearchTerms" Text="enter search terms" CssClass="nwtd-search-keyword" />
	</div>
	<div class="nwtd-advanced-search-left">
		<asp:DropDownList runat="server" ID="ddlGrades" DataTextField="Name" DataValueField="Key"/>
		<asp:DropDownList runat="server" ID="ddlPublisher" DataTextField="Name" DataValueField="Key"/>
	</div>
	<div  class="nwtd-advanced-search-right">
		<asp:DropDownList runat="server" ID="ddlYear" DataTextField="Name" DataValueField="Key" />
		<asp:DropDownList runat="server" ID="ddlSubject" DataTextField="Name" DataValueField="Key" />
		<asp:DropDownList runat="server" ID="ddlType" DataTextField="Name"  DataValueField="Key"/>
	</div>
	<div  class="nwtd-advanced-search-bottom">
		<input class="nwtd-button-blue" type="reset" value="Clear" />
		<asp:Button CssClass="nwtd-button-blue" runat="server" ID="btnSumbitSearch" Text="Search" onclick="btnSumbitSearch_Click" />
	</div>
</div>
