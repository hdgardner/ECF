<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="OrderDetails.ascx.cs" Inherits="Mediachase.Cms.Website.Structure.User.NWTDControls.Controls.CSS.OrderDetails" %>

<h1>Order <%=this.Order.Id %></h1>
<asp:Panel runat="server" ID="pnlOrderDetails">
	<h2 style="display:inline;"><%=this.Organization.Name %></h2> [<a href="javascript:history.back();">back to results</a>]<br /><br />
	<dl id="pnlOrderDetailsDef">
		<dt>Order Number:</dt>
		<dd><%=this.Order.Id %></dd> 
		<dt>Customer PO:</dt>
		<dd><%=this.Order.Header.CustomerReferenceNumber %></dd>
		<dt>Remarks:</dt>
		<dd><%=this.Order.Header.Comment.Replace(Environment.NewLine, "<br />") %></dd> 
	</dl>
	<div style="width:900px;">
	<asp:DropDownList runat="server" ID="ddlSort" AutoPostBack="true" onselectedindexchanged="ddlSort_SelectedIndexChanged" CssClass="rightSelect">
		<asp:ListItem Text="Sort By" />
		<asp:ListItem Text="Ship Date" Value="ShipDate" />
		<asp:ListItem Text="ISBN" Value="ISBN" />
		<asp:ListItem Text="Order Line Number" Value="LineNumber" />
	</asp:DropDownList>
	</div>
</asp:Panel>

<asp:Panel runat=server ID="pnlAccessDenied" Visible="false">
	<h3>Access Denied</h3>
	<p>You are not authorized to view information on this order.</p>
</asp:Panel>

<asp:GridView runat="server" ID="gvOderItems" AutoGenerateColumns="false">
	<EmptyDataTemplate>
		there are no items in this order
	</EmptyDataTemplate>
	<Columns>
		<asp:BoundField  HeaderText="#" DataField="Reserved1" />
		<asp:BoundField HeaderText="QTY" DataField="Quantity" />
		<asp:BoundField HeaderText="Price" DataField="Price" DataFormatString="{0:C}" />
		<asp:BoundField HeaderText="ISBN-13" DataField="ItemCode" />
		<asp:BoundField HeaderText="Item Description" DataField="ItemName" />
		<asp:TemplateField HeaderText="Invoice Number">
			<ItemTemplate>
				<%# ((NWTD.InfoManager.OrderLine)Container.DataItem).Invoice != null ? ((NWTD.InfoManager.OrderLine)Container.DataItem).Invoice.Id : string.Empty %>
			</ItemTemplate>
		</asp:TemplateField>
		<asp:TemplateField HeaderText="Ship Date" >
			<ItemTemplate>
				<%#  ((NWTD.InfoManager.OrderLine)Container.DataItem).Invoice != null && ((NWTD.InfoManager.OrderLine)Container.DataItem).Invoice.Header.ShipDate.HasValue ? ((NWTD.InfoManager.OrderLine)Container.DataItem).Invoice.Header.ShipDate.Value.ToString("d") : string.Empty%>
			</ItemTemplate>
		</asp:TemplateField>
		<asp:TemplateField>
			<ItemTemplate>	
				
				
				<asp:HyperLink 
					runat="server" 
					NavigateUrl='<%# string.Format("~/CSS/ShippingDetails.aspx?InvoiceId={0}",  ((NWTD.InfoManager.OrderLine)Container.DataItem).Invoice != null ? ((NWTD.InfoManager.OrderLine)Container.DataItem).Invoice.Id: string.Empty ) %>' 
					Text="Details" 
					Visible='<%#  ((NWTD.InfoManager.OrderLine)Container.DataItem).Invoice != null %>' />
			</ItemTemplate>
		</asp:TemplateField> 
	</Columns>
</asp:GridView>