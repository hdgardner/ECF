<%@ Control 
	Language="C#" 
	AutoEventWireup="true" 
	CodeBehind="SearchResults.ascx.cs"
	Inherits="Mediachase.Cms.Website.Structure.User.NWTDControls.Controls.Catalog.SearchResults" %>
<%@ Register Src="~/Structure/User/NWTDControls/Controls/Catalog/EntryPrice.ascx" TagName="YourPrice" TagPrefix="NWTD" %>
<%@ Import Namespace="Mediachase.Commerce.Catalog.Objects" %>
<%@ Import Namespace="Mediachase.Cms.WebUtility.Search" %>
<%@ Import Namespace="Mediachase.Search" %>
<%@ Import Namespace="Mediachase.Search.Extensions" %>
<%@ Import Namespace="Mediachase.Commerce" %>
<%@ Import Namespace="Mediachase.Commerce.Catalog" %>
<%@ Import Namespace="Mediachase.Commerce.Catalog.Objects" %>
<%@ Import Namespace="Mediachase.Commerce.Catalog.Managers" %>
<%@ Import Namespace="Mediachase.Commerce.Catalog.Dto" %>
<%@ Register Src="~/Structure/User/NWTDControls/Controls/Catalog/ISBN.ascx" TagName="ISBN" TagPrefix="NWTD" %>

<catalog:CatalogIndexSearchDataSource ID="dsSearchResults" runat="server">
</catalog:CatalogIndexSearchDataSource>
<asp:Panel runat="server" ID="pnlResultsWrapper" CssClass="results-wrapper">

	<asp:Panel runat="server" ID="pnlSearchResults" CssClass="nwtd-searchResultsContainer">
		 <div style="float:right; margin-bottom:5px;">
			<asp:Button runat="server" ID="btnAddSelectedTop" CssClass="nwtd-addSelectedToCart buttons" Text="Add selected to current Wish List" />	
		</div>
		<div class="results-heading">
			<table cellpadding="0" cellspacing="0">
				<tr class="nwtd-table-header-fixed">
					<th class="col1"></th>
					<th class="col2">Title</th>
					<th class="col3">Publisher Number / ISBN13</th>
					<th class="col4">Grade</th>
					<th class="col5">Year</th>
					<th class="col6">Type</th>
					<th class="col7">Net School Price</th>
					<th class="col8">Contract Price</th>
					<th class="nevada-adopting">Nevada Adopting District</th>
				</tr>
			</table>
		</div>
		<div class="results-overflow">
			<asp:GridView 
				runat="server"  BorderWidth="0px"   
				ID="gvSearchResults" 
				AutoGenerateColumns="False" 
				DataKeyNames="ID" EnableViewState="false"
				DataSourceID="dsSearchResults"
				SkinID="SearchResults" onrowdatabound="gvSearchResults_RowDataBound">
				
				<EmptyDataTemplate>
					<p>Your search returned no results.</p>
				</EmptyDataTemplate>
				<Columns>
					<asp:TemplateField ItemStyle-CssClass="nwtd-selectEntry" HeaderText="Add to Wish List" HeaderStyle-HorizontalAlign="Center">
						<ItemTemplate>
							<asp:CheckBox runat="server" ID="cbSelectEntry" />
						</ItemTemplate>
						<ItemStyle CssClass="nwtd-selectEntry" />
					</asp:TemplateField>
		<%--			<asp:TemplateField HeaderText="Qty">
						<ItemTemplate>
							<asp:TextBox runat="server" ID="tbQuantity" Text="1" Columns="1" MaxLength="3" />
						</ItemTemplate>
					</asp:TemplateField>--%>
					<asp:TemplateField HeaderText="Title" SortExpression="DisplayName" ItemStyle-CssClass="col2">
						<ItemTemplate>
							<asp:HyperLink ID="linkEntryLink" runat="server" NavigateUrl='<%# StoreHelper.GetEntryUrl((Entry)Container.DataItem)%>'><%# StoreHelper.GetEntryDisplayName((Entry)Container.DataItem)%></asp:HyperLink> 
							
							<%--Only Show Status notes for the depository we're currently in--%>
                            <%-- Heath Gardner - 03/06/13 Below statement was erroneously returning "NWTD" for MSSD users, so I changed it to calculate Depository like Catalog.cs does --%>
							    <%-- <asp:Label CssClass="status-note nwtd-status-note" runat="server" Visible='<%# this.Depository.Equals(NWTD.InfoManager.Depository.NWTD) %>'> --%>
                            <asp:Label CssClass="status-note nwtd-status-note" runat="server" Visible='<%# NWTD.Profile.CustomerDepository.Equals(NWTD.Depository.NWTD) %>'>
								<%# ((Mediachase.Commerce.Catalog.Objects.ItemAttributes)Eval("ItemAttributes"))["Status_PDX"]%> 
							</asp:Label>
                             <%-- Heath Gardner - 03/06/13 Below statement was erroneously returning "NWTD" for MSSD users, so I changed it to calculate Depository like Catalog.cs does --%>
							    <%--<asp:Label  CssClass="status-note mssd-status-note" ID="Label1" runat="server" Visible='<%# this.Depository.Equals(NWTD.InfoManager.Depository.MSSD) %>'> --%>
                            <asp:Label  CssClass="status-note mssd-status-note" runat="server" Visible='<%# NWTD.Profile.CustomerDepository.Equals(NWTD.Depository.MSSD) %>'>
								<%# ((Mediachase.Commerce.Catalog.Objects.ItemAttributes)Eval("ItemAttributes"))["Status_SLC"]%>
							</asp:Label>
						</ItemTemplate>
					</asp:TemplateField>
					<asp:TemplateField HeaderText="Publisher Number / ISBN13" ItemStyle-CssClass="col3">
						<ItemTemplate>
							<asp:HiddenField runat="server" ID="hfCode" Value='<%#Eval("ID") %>' />	
							<NWTD:ISBN runat="server" Entry='<%#Container.DataItem %>' />
						</ItemTemplate>
					</asp:TemplateField>
					<asp:TemplateField HeaderText="Grade" ItemStyle-CssClass="col4" SortExpression="GradeNumber">
						<ItemTemplate>
							<%# ((Mediachase.Commerce.Catalog.Objects.ItemAttributes)Eval("ItemAttributes"))["Grade"]%>
						</ItemTemplate>
					</asp:TemplateField>
					<asp:TemplateField HeaderText="Year" SortExpression="Year" ItemStyle-CssClass="col5">
						<ItemTemplate>
							<asp:Literal runat="server" ID="litYear" />
							<%--<%# float.Parse(((Mediachase.Commerce.Catalog.Objects.ItemAttributes)Eval("ItemAttributes"))["Year"].ToString()).ToString("#")%>--%>
						</ItemTemplate>
					</asp:TemplateField>
					<asp:TemplateField SortExpression="TypeSort" HeaderText="Type" ItemStyle-CssClass="col6">
						<ItemTemplate>
							<%# ((Mediachase.Commerce.Catalog.Objects.ItemAttributes)Eval("ItemAttributes"))["Type"]%>
						</ItemTemplate>
					</asp:TemplateField>
					<asp:TemplateField HeaderText="Net School Price" ItemStyle-CssClass="col7">
						<ItemTemplate>
							<NWTD:YourPrice runat="server" ID="NetSchoolPrice" Entry="<%#(Entry)Container.DataItem %>" PriceType="NetSchool"  />
						</ItemTemplate>
					</asp:TemplateField>
					<asp:TemplateField HeaderText="Contract Price" ItemStyle-CssClass="col8">
						<ItemTemplate>
							<NWTD:YourPrice runat="server" ID="YourPrice" Entry="<%#(Entry)Container.DataItem %>" PriceType="Discount"  />
						</ItemTemplate>
					</asp:TemplateField>
					<%--This column will get set to be visible in the codebehind, only if the current user is from Nevada--%>
					<asp:TemplateField Visible="false" ItemStyle-CssClass="nevada-adopting" HeaderText="Nevada Adopting District">
						<ItemTemplate>
							<%# ((Mediachase.Commerce.Catalog.Objects.ItemAttributes)Eval("ItemAttributes"))["AdoptDist_NV"]%>
						</ItemTemplate>
					</asp:TemplateField>
				</Columns>
			</asp:GridView>
		</div>
		<div class="nwtd-search-buttons">
			 <asp:Button ID="btnClearSelections" runat="server" Text="Clear Selections" CssClass="nwtd-clear-selections buttons" />
			 <div style="float:right;">
				<asp:Button runat="server" ID="btnAddToCurrentOrder" CssClass="nwtd-addSelectedToCart buttons" Text="Add selected to current Wish List" />	
				<asp:HyperLink runat="server" Text="Start New Search" NavigateUrl="~/catalog/searchresults.aspx" cssclass="new-search-btn buttons" ID="hlClearSearch" />
			 </div>
		</div>
	</asp:Panel>

</asp:Panel>

<div class="nwtd-addToCartSuccess nwtd-dialogBody" style="display:none" >
	<p class="nwtd-DialogMessage">These items have been added to your Wish List:</p>
	<ul class="nwtd-dialogItemsAdded"></ul>
	<ul class="nwtd-dialogErrors"></ul>
	<div class="option-btns">
	    <a href="#"><img src="~/App_Themes/NWTD/images-template/continue-search-btn.png" runat="server" /></a>&nbsp;&nbsp;<a href="~/Cart/view.aspx" runat="server"><img src="~/App_Themes/NWTD/images-template/view-cart-btn.png" runat="server" /></a>
    </div>	   
</div>


<asp:Panel runat="server" ID="pnlSelectCart" Visible="false">
	<div class="selectCartDialog nwtd-dialogBody">
		<p>
			Please select the Wish List that should receive your items. 
			The Wish List you select will become your active Wish List.
		</p>
		
		<asp:DropDownList runat="server" ID="ddlCarts" DataTextField="Name" DataValueField="Name" AppendDataBoundItems="true" CssClass="nwtd-selectCartList-sr"  >
			<asp:ListItem Text="[Create a new Wish List]" Value="" />
		</asp:DropDownList>	
		<div class="nwtd-selectNewCart-sr">
			<asp:Label ID="Label2" runat="server"  AssociatedControlID="SelectNewCartName" >Wish List Name:</asp:Label>
			<asp:TextBox runat="server" ID="SelectNewCartName" CssClass="nwtd-selectNewCartName" />
		</div>
		<input type="submit" class="nwtd-selectCart buttons" value="Set as Active Wish List" />
	</div>
</asp:Panel>