<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SearchTest.ascx.cs" Inherits="Mediachase.Cms.Website.Structure.User.NWTDControls.Controls.TestControls.SearchTest" %>
<h2>Direct SQL Query</h2>
<asp:GridView runat="server" ID="gvQueryResults" AutoGenerateColumns="false">
	<Columns>
		<asp:BoundField HeaderText="CatalogEntryID"  DataField="CatalogEntryID" />
		<asp:BoundField HeaderText="Name" DataField="Name" />
		<asp:BoundField HeaderText="TypeSort" DataField="TypeSort" />
	</Columns>
</asp:GridView>
<h2>Directly with Lucene API</h2>
<asp:GridView runat="server" ID="gvLuceneSearchResults" ></asp:GridView>
<h2>Using ECF's Search API</h2>
<asp:GridView runat="server" ID="gvSearchResults" AutoGenerateColumns="false">
	<Columns>
		<asp:BoundField HeaderText="CatalogEntryID" DataField="CatalogEntryID" />
		<asp:BoundField HeaderText="Name" DataField="Name" />
		<asp:TemplateField HeaderText="TypeSort">
			<ItemTemplate>
				<%# ((Mediachase.Commerce.Catalog.Objects.ItemAttributes)Eval("ItemAttributes"))["TypeSort"]%>
			</ItemTemplate>
		</asp:TemplateField>
	</Columns>
</asp:GridView>