<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ShippingDetails.ascx.cs" Inherits="Mediachase.Cms.Website.Structure.User.NWTDControls.Controls.CSS.ShippingDetails" %>

<%--On 09/16/13 Heath Gardner added this pnlInvoiceDetails panel to control visibility of Invoice info (e.g. validate invoice belongs to customer before displaying) --%>
<asp:Panel runat="server" ID="pnlInvoiceDetails">
<h1 style="display:inline;"> Shipping Details</h1> <a href="javascript:history.back();">[back to order]</a>
<div class="buffer"></div>
<dl class="css">
<%--	<dt>Shipment Number</dt>
	<dd><%=this.Invoice.Header.ShipmentNumber %></dd>--%>
	<dt>Order Number</dt>
	<dd><%=(this.Invoice.Lines.Length > 0)? this.Invoice.Lines[0].OrderId : string.Empty %></dd>
	<dt>Customer PO Number</dt>
	<dd><%=this.Invoice.Header.CustomerReferenceNumber %></dd>
	<dt>Invoice Number</dt>
	<dd><%=this.InvoiceId %></dd>
	<dt>Ship To:</dt>
	<dd>
		<%=this.Invoice.Header.Address.Line1 %><br />
		<%=this.Invoice.Header.Address.Line2 %>
	</dd>
	<dt>Shipped Date</dt>
	<dd><%= String.Format("{0:M/d/yyyy}", this.Invoice.Header.ShipDate)%></dd>
    <dt>Total Shipment Weight</dt>
	<dd><%=this.Invoice.Header.ShipmentWeight %></dd>
    <dt>Total Shipment Cartons</dt>
	<dd><%=this.Invoice.Header.CartonCount %></dd>
	<dt>Shipped By</dt>
	<dd><%=this.Invoice.Header.ShippedBy %></dd>
	<dt>Tracking Number(s)</dt>
<%-- <dd><%=this.Invoice.Header.TrackingNums.Replace(","," ") %></dd> --%>	
<%--On 09/16/13, Heath Gardner replaced the above line with below Track Num Repeaters & HyperLinks for tracking functionality--%>
   <dd><asp:Repeater ID="UPSTrackNumsRepeater" runat="server" visible="false" >
        <ItemTemplate>
            <%--Added "<br />" to Container.DataItem from Code Behind so user can copy and paste of multiple tracking #s to UPS.com (e.g. tracknums must be seperated by line break) Heath Gardner 09/16/13--%>
            <asp:HyperLink ID="UPSHyperLink" runat="server" Text='<%# Container.DataItem %>' ToolTip='<%#"Click to view UPS tracking details for " + RemoveSpecialChar(Container.DataItem, "LineBreak") %>' NavigateUrl='<%# "http://wwwapps.ups.com/WebTracking/track?track=yes&trackNums=" + RemoveSpecialChar(Container.DataItem, "LineBreak") %>' Target="_blank" /> 
        </ItemTemplate>
    </asp:Repeater></dd>
    <dd><asp:Repeater ID="MailTrackNumsRepeater" runat="server" visible="false" >
        <ItemTemplate>
            <%--Added "," to Container.DataItem from Code Behind so user can copy and paste multiple track #s to USPS.com (e.g. tracknums must be seperated by comma) Heath Gardner 09/16/13--%>
           <asp:HyperLink ID="MailHyperLink" runat="server" Text='<%# Container.DataItem %>' ToolTip='<%#"Click to view USPS tracking details for " + RemoveSpecialChar(Container.DataItem, "Comma") %>' NavigateUrl='<%# "https://tools.usps.com/go/TrackConfirmAction.action?tLabels=" + RemoveSpecialChar(Container.DataItem, "Comma") %>' Target="_blank" />
        </ItemTemplate>
    </asp:Repeater></dd>
    <dd><asp:Repeater ID="GenericTrackNumsRepeater" runat="server" visible="false" >
        <ItemTemplate>
            <%# Container.DataItem %> 
        </ItemTemplate>
    </asp:Repeater></dd>  
    <%--On 09/16/13, Heath Gardner added the two following labels for indicating some track nums are missing --%> 
    <dt><asp:Label runat=server ID=lblPartialTrackTitle Visible="false" Text=&nbsp;></asp:Label></dt>
    <dd><asp:Label runat=server ID="lblPartialTrackMessage" Visible="false" Width=300px Text="Call depository for additional tracking numbers."> </asp:Label></dd>
<%--End of Hyperlink functionality (hg) --%>  
</dl>
</asp:Panel>
<%--On 09/16/13, Heath Gardner added this pnlAccessIsDenied panel to display if shipment doesn't match user (e.g. manually type invoice into url)--%>
<asp:Panel runat=server ID="pnlAccessIsDenied" Visible="false">
	<h1>Shipping Details</h1>
    <h3>Access Denied</h3>
	<p>You are not authorized to view information for this shipment.</p>
</asp:Panel>
<div class="buffer"></div>
<asp:GridView runat="server" ID="gvInvoice" AutoGenerateColumns="false">
	<EmptyDataTemplate>
		This Invoice has no line items.
	</EmptyDataTemplate>
	<Columns>
		<asp:BoundField HeaderText="QTY" DataField="Quantity" />
		<asp:BoundField HeaderText="Price" DataField="Price" DataFormatString="{0:C}"  />
		<asp:BoundField HeaderText="ISBN-13" DataField="ItemCode" />
		<asp:BoundField HeaderText="Item Description" DataField="ItemName" />
	</Columns>
</asp:GridView>