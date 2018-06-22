<%@ Control Language="C#" AutoEventWireup="true" Inherits="Templates_Everything_Entry_Modules_ImagesModule" Codebehind="ImagesModule.ascx.cs" %>
<div align="left">
<asp:Repeater id="AltList" Runat="server">
	<HeaderTemplate><h2><%=RM.GetString("ALTERNATIVE_PHOTOS_LABEL")%></h2></HeaderTemplate>
	<ItemTemplate>
	    <asp:HyperLink ID="HyperLink1" runat="server" NavigateUrl='<%# GetUrl((Mediachase.Ibn.Library.FolderElement)Container.DataItem)%>' Target="_blank">
		<asp:Image runat="server" ID="Image1" BorderWidth="0" Width="80" 
		    ImageUrl='<%# GetUrl((Mediachase.Ibn.Library.FolderElement)Container.DataItem)%>' 
		    />
		    </asp:HyperLink>
	</ItemTemplate>
</asp:Repeater>
</div>