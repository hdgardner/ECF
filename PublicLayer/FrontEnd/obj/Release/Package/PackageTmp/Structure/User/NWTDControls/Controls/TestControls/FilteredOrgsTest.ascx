<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="FilteredOrgsTest.ascx.cs" Inherits="Mediachase.Cms.Website.Structure.User.NWTDControls.Controls.TestControls.FilteredOrgsTest" %>

<asp:Label runat="server" ID="lblState" AssociatedControlID="tbState" Text="State" />
<asp:TextBox runat="server" ID="tbState" ></asp:TextBox>
<asp:Button runat="server" ID="btnSubmit" Text="Get Organizations" onclick="btnSubmit_Click" />
<asp:Repeater runat="server" ID="rpFilteredOrgs">
	<ItemTemplate>
		<%#Eval("Name") %>
	</ItemTemplate>
</asp:Repeater>