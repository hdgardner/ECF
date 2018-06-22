<%@ Control 
	Language="C#" 
	AutoEventWireup="true" 
	CodeBehind="BookSearchControl.ascx.cs" 
	Inherits="Mediachase.Cms.Website.Templates.NWTD.Modules.BookSearchControl" %>
<%@ Register Src="~/Templates/NWTD/Modules/SideMenuControl.ascx" TagPrefix="NWTD" TagName="SideMenu" %>    
<%@ Register Src="~/Structure/User/NWTDControls/Controls/Catalog/SearchResults.ascx" TagName="SearchResults" TagPrefix="NWTD" %>

<%@ Register src="~/Structure/User/NWTDControls/Controls/Cart/CurrentShoppingCart.ascx" tagname="CurrentShoppingCart" tagprefix="NWTD" %>

<div id="sidemenu">
    <NWTD:SideMenu runat="server" ID="FiltersSideMenu" />
</div>
<div style="margin-left:200px;">
<asp:Panel runat="server" ID="pnlBrowseCatalog" Visible="false">
	<h1>Search our Catalog</h1>

<p>Welcome to the Northwest Textbook Depository Online Catalog!  From here you have access to information on over 100,000 titles from over 50 publishers.</p>

<p>You can search our catalog in two different ways - via an ISBN/Keyword search or via a Category search.</p>

<p><strong>ISBN / Keyword search</strong> - We recommend you use this method if you know the ISBN of the material you are looking for (or know the title of the book).  To perform an ISBN / Keyword search, simply enter the ISBN (or title) into the search box above and click on the "Search" button.  You can also perform searches by entering partial ISBNs or only certain keywords out of a title.</p>

<p><strong>Category search</strong> - We recommend you use a Category search if you want to see a number of different titles that meet certain criteria.  To perform a Category search, simply select the appropriate categories from the listing on the left side of the page under the heading "Refine your search".  For example, you might make the following selections: Type = textbook, Copyright = 2009, and Subject = Math:Algebra.  The results will be displayed below.   </p>

<p>After seeing the results of your Category search, you can easily modify you search by either adding or subtracting categories.  To add an additional category, select it from the listing on the left under the heading "Refine your search".  To remove a category from your search, find the category in the "Currently Shopping by" box in the upper left side of the page and click on the "X" next to that category.</p>

<p>If you are looking for a unique title or have any questions, please give us a call at 503-906-1100 or 800-676-6630.<br />We are open from 8:00am - 4:30pm Monday - Friday.</p>
	
</asp:Panel>
</div>
<div style="margin-left:200px;">
<asp:Panel runat="server" ID="pnlNoResults" Visible="false">
    <h1>No Results Found</h1>
	<p style="text-align:left;">
		The ISBN, Keyword, or Publisher you entered is not in our system.
		You may <asp:HyperLink runat="server" NavigateUrl="~/Contact/contact.aspx">contact us</asp:HyperLink> we will be happy to look into this further.
	</p>
	<asp:HyperLink runat="server" Text="Start New Search" NavigateUrl="~/catalog/searchresults.aspx" cssclass="new-search-btn buttons" ID="hlClearSearch" />

</asp:Panel>
</div>
<!--<asp:DropDownList 
	runat="server" 
	ID="ddlSortBy" 
	AutoPostBack="true" 
	EnableViewState="true"
	onselectedindexchanged="ddlSortBy_SelectedIndexChanged" 
	onprerender="ddlSortBy_PreRender">
	<asp:ListItem Text="Title" Value="DisplayName" />
	<asp:ListItem Text="Year" Value="Year" />
</asp:DropDownList>-->

<asp:Panel runat="server" ID="pnlSearchHead" Visible="true" CssClass="search-head" >
<NWTD:CurrentShoppingCart ID="CurrentShoppingCart1" runat="server" />
    <div class="search-heading">
		
		<h1 style="display:inline;">Search Results</h1>
		<asp:Literal runat="server" ID="litSearchString" />
		<strong><asp:Literal runat="server" ID="litPageNumber" /></strong><br />
		
		<span>Click on any item to view its detail and related items.</span>
    </div>
    <div class="nwtd-search-head">
		<ul runat="server" id="ulPager"></ul>
		
		<%--This is the pager (will be dynaically populated in codebehind)--%>
		<asp:BulletedList runat="server" ID="blPager" DisplayMode="HyperLink"></asp:BulletedList>
    </div>
    <br style="clear:right;" />
</asp:Panel>

<NWTD:SearchResults runat="server" ID="srBookSearch"  />

<%--This is the pager for the bottom of the page--%>
<asp:Panel runat="server" ID="pnlBottomPager">
	<div id="nwtd-bottomPager">
		<asp:BulletedList ID="blBottomPager" DisplayMode="HyperLink" runat="server" />
	</div>
</asp:Panel>

