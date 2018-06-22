<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="PublisherList.ascx.cs" Inherits="Mediachase.Cms.Website.Structure.User.NWTDControls.Controls.CSS.PublisherList" %>

<asp:GridView runat="server" ID="gvPublisherList" AutoGenerateColumns="false">
	<Columns>
		<asp:HyperLinkField DataTextField="Name" HeaderText="Name" DataNavigateUrlFormatString="~/Publishers/publisherrepresentatives.aspx?publisher={0}" DataNavigateUrlFields="code" />
<%--		<asp:HyperLinkField DataTextField="url" HeaderText="URL" DataNavigateUrlFormatString="{0}" DataNavigateUrlFields="url" />
--%></Columns>
</asp:GridView>