<%@ Control Language="c#" Inherits="Mediachase.eCF.PublicStore.SharedModules.CustomerAddressViewModule" Codebehind="CustomerAddressViewModule.ascx.cs" %>
<asp:TextBox ID="tbAddressID" Visible="false" runat="server"></asp:TextBox>
<div id="AddressView">
            <asp:Label ID="FirstName" runat="server"></asp:Label> <asp:Label ID="LastName" runat="server"></asp:Label><br />
            <asp:Label ID="Company" runat="server"></asp:Label>
            <asp:Label ID="Address1" runat="server"></asp:Label><br />
            <asp:Label ID="Address2" runat="server"></asp:Label>
            <asp:Label ID="City" runat="server"></asp:Label>, <asp:Label ID="State" runat="server"></asp:Label> <asp:Label ID="Zip" runat="server"></asp:Label><br />
            <asp:Label ID="Country" runat="server"></asp:Label><br />
            <!--
            <asp:Label ID="Email" runat="server"></asp:Label><br />
            -->
            <asp:Label ID="Phone" runat="server"></asp:Label><br />
            <asp:Label ID="EveningPhone" runat="server"></asp:Label><br />
            <asp:Label ID="Fax" runat="server"></asp:Label>
</div>