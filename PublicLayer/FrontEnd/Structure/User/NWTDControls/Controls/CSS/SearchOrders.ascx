<%@ Control 
	Language="C#" 
	AutoEventWireup="true" 
	CodeBehind="SearchOrders.ascx.cs" 
	Inherits="Mediachase.Cms.Website.Structure.User.NWTDControls.Controls.CSS.SearchOrders" %> 

<script type="text/javascript" >
	jQuery(document).ready(function($) {
		$.each(OakTree.Web.UI.WebControls.OrderSearchCriteria, function(i, ctrl) {
			var criteria = $('#' + ctrl.controlID);
			var criteriaselect = criteria.find('.ntwd-searchOrdersCriteria');
			var textbox = criteria.find('.nwtd-searchOrdersKeyword');
			var addressselect = criteria.find('.nwtd-searchOrdersAddresses');

			var showCorrectSearchBox = function() {
				if (criteriaselect.val() == 'ShipTo') {
					textbox.hide();
					addressselect.show();
				}
				else {
					textbox.show();
					addressselect.hide();
				}
			}

			criteriaselect.change(function() {
				showCorrectSearchBox();
			});
			showCorrectSearchBox();
		});
		
	});
</script>
<asp:Panel runat="server" ID="pnlSearchCriteria" CssClass="centerPnl" DefaultButton="btnCatalogSearch">
	<fieldset>
		<asp:Label runat="server" ID="lblSearchCriteria" Text="Search " AssociatedControlID="ddlSearchCriteria" />
		<asp:DropDownList runat="server" ID="ddlSearchCriteria" CssClass="ntwd-searchOrdersCriteria" >
			<asp:ListItem Text="PO #" Value="PO" />
			<asp:ListItem Text="Invoice #" Value="Invoice" />
			<asp:ListItem Text="Order #" Value="Order" />
			<asp:ListItem Text="Web Confirmation #" Value="WebConfirmation" />
			<asp:ListItem Text="Contains ISBN" Value="ISBN" />
			<asp:ListItem Text="Ship to Address" Value="ShipTo" />
		</asp:DropDownList>

		<asp:TextBox runat="server" ID="tbSearchBox" CssClass="nwtd-searchOrdersKeyword"></asp:TextBox>
		<asp:DropDownList runat="server" ID="ddlAddress" DataTextField="FirstName" DataValueField="Name" CssClass="nwtd-searchOrdersAddresses">
			<asp:ListItem Text="Select Address" Value="" />
		</asp:DropDownList>
	</fieldset>
	<fieldset>
		<asp:Label runat="server" ID="lblDateRange" Text="Date Range" />
		<asp:DropDownList runat="server" ID="ddlDateRange">
			<asp:ListItem Text="last 90 days" Value="90" />
			<asp:ListItem Text="past year" Value="365" />
		</asp:DropDownList>	
	</fieldset>
	<fieldset class="cssSubmit">
		<asp:Button runat="server" ID="btnCatalogSearch" Text="Search" onclick="btnCatalogSearch_Click" CssClass="css-search-btn buttons" />
	</fieldset>
    <fieldset>
		<asp:Label runat="server" ID="lblSortBy" Text="Sort By" />
		<asp:DropDownList runat="server" ID="ddlSortBy" OnSelectedIndexChanged="HandleSortByIndexChanged" AutoPostBack="True" CssClass="ntwd-searchOrdersCriteria">
			<asp:ListItem Text="Create Date" Value="0" />
			<asp:ListItem Text="Order Number" Value="1" />
            <asp:ListItem Text="Purchase Order" Value="2" />
			<asp:ListItem Text="Ship To Address" Value="3" />
            <asp:ListItem Text="Shipped Status" Value="4" />
			<asp:ListItem Text="Confirmation Number" Value="5" />
		</asp:DropDownList>	
	</fieldset>
</asp:Panel>
<asp:GridView runat="server" ID="gvSearchResults" AutoGenerateColumns="false" >
	<EmptyDataTemplate>
		Your search yielded no results
	</EmptyDataTemplate>
	<Columns>
		<asp:BoundField HeaderText="Purchase Order" DataField="CustomerReferenceNumber" />
		<asp:BoundField HeaderText="Date" DataField="CreatedTime" DataFormatString="{0:d}" />
        <asp:BoundField HeaderText="Ship To Address" DataField="ShipToName"  />
        <asp:BoundField HeaderText="Confirmation Number" DataField="WebConfirmNum"  />
		<asp:BoundField HeaderText="Order Number" DataField="Id" />
        <asp:BoundField HeaderText="Shipped?" DataField="ShippedStatus"  />
		<asp:HyperLinkField Text="Details"  DataNavigateUrlFormatString="~/CSS/OrderDetails.aspx?OrderId={0}" DataNavigateUrlFields="Id" />
	</Columns>
</asp:GridView>