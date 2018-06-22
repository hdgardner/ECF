<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UpdateMessage.ascx.cs" Inherits="Mediachase.Cms.Website.Structure.User.NWTDControls.Controls.Misc.UpdateMessage" %>

<asp:Panel runat="server" ID="pnlUpdateMessage" CssClass="nwtd-update-message" >
	<asp:Image ID="MessageImage" runat="server" />
	<p><%= this.Message %></p>
</asp:Panel>