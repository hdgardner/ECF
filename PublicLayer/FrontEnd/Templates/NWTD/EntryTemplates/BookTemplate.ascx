<%@ Control 
	Language="C#"
	AutoEventWireup="true" 
	CodeBehind="BookTemplate.ascx.cs" 
	Inherits="Mediachase.Cms.Website.Templates.NWTD.EntryTemplates.BookTemplate" %>
<%@ Register Src="~/Templates/Everything/EntryTemplates/Modules/ProductDisplayModule.ascx" TagName="ProductDisplayModule" TagPrefix="catalog" %>
<%@ Register Src="~/Templates/Everything/EntryTemplates/Modules/RecentlyViewedModule.ascx" TagName="RecentlyViewedModule" TagPrefix="catalog" %>
<%@ Register Src="~/Templates/Everything/EntryTemplates/Modules/CompareProductModule.ascx" TagName="CompareProductModule" TagPrefix="catalog" %>
<%@ Register Src="~/Templates/Everything/EntryTemplates/Modules/BuyModule.ascx" TagName="BuyModule" TagPrefix="catalog" %>
<%@ Register Src="~/Templates/Everything/EntryTemplates/Modules/OverviewModule.ascx" TagName="OverviewModule" TagPrefix="catalog" %>
<%@ Register Src="~/Templates/Everything/EntryTemplates/Modules/YouMayAlsoLikeModule.ascx" TagName="YouMayAlsoLikeModule" TagPrefix="catalog" %>

<%@ Register Src="~/Templates/NWTD/Modules/BookSubSearch.ascx" TagName="BookSubSearch" TagPrefix="NWTD" %>
<%@ Register Src="~/Structure/User/NWTDControls/Controls/Catalog/EntryPrice.ascx" TagName="YourPrice" TagPrefix="NWTD" %>
<%@ Register Src="~/Structure/User/NWTDControls/Controls/Catalog/AddToCartButton.ascx" TagName="AddToCart" TagPrefix="NWTD" %>
<%@ Register Assembly="OakTree.Web.UI" Namespace="OakTree.Web.UI.WebControls" TagPrefix="OakTree" %>

<OakTree:JavaScriptInclude runat="server" ID="jsBookSubSearchShowHide" >

jQuery(document).ready(function($) {
	var updateScroll = function(){

		
		subSearchControls = OakTree.Web.UI.WebControls.SubSearch || [];
		
		$.each(subSearchControls, function() {
			var updatePanel = $('#' + this.updatePanelID);
			var overflowWrapper = updatePanel.find('.results-overflow');
			var wrapperHeight = overflowWrapper.height();
			var gridHeight = updatePanel.find('.nwtd-results-grid').height();


			if (gridHeight > wrapperHeight) {
				var selectedItem = updatePanel.find('.nwtd-table-row-selected');

				//make sure we're scrolled all the way up so we get an accurate calculation
				overflowWrapper.scrollTop(0);
				
				//make the calculation. Get it exactly in the middle. Do it.
				var scrollTo =  selectedItem.position().top - overflowWrapper.position().top - (wrapperHeight / 2) + (selectedItem.height() / 2);;
						
				//do the scrolling
				overflowWrapper.scrollTop(scrollTo);
			}
			else {
				overflowWrapper.css({
					height: 'auto',
					oveflow: 'auto'
				});
			}
		});
	};


	var showLink = $('.nwtd-show-related');
	var relatedItems = $('.nwtd-related').hide();
	var toggleRelated = $('.nwtd-toggle-related');

	showLink.click(function(){
		if(relatedItems.hasClass('shown')){
			relatedItems
				.hide()
				.removeClass('shown')
				.addClass('hidden');
			toggleRelated.text('Show Related Items');
		}
		
		else{
			relatedItems
				.show(0,updateScroll)
				.removeClass('hidden')
				.addClass('shown');
			toggleRelated.text('Hide Related Items');
		}
		return false;
	});
	
	//updateScroll();
});
</OakTree:JavaScriptInclude>

<div class="nwtd-bookDetails">
    <div class="detail-padding">
	    <h1><%#StoreHelper.GetEntryDisplayName(Entry)%> </h1>
	    
	    <ul style="clear: left;float:left;width:65%;">
			<li style="width:45%; font-size:14px;"><strong>Series:</strong> <%=Entry.ItemAttributes["Series"] %></li>
			<li style="width:45%; font-size:14px;"><strong>Publisher:</strong> <%= Entry.ItemAttributes["Publisher"] %> </li>
		</ul>
	    
	    <asp:HyperLink runat="server" ID="hlPreviousPage" Text="Back to Results" CssClass="back-to-results-btn buttons" NavigateUrl="javascript:history.go(-1);" />
	    <asp:HyperLink runat="server" Text="Start New Search" NavigateUrl="~/catalog/searchresults.aspx" cssclass="new-search-btn buttons" ID="hlClearSearch" />
	    <table class="nwtd-searchResults" cellpadding="0" style="background-color:White;border-collapse:collapse;">
    		<tr class="nwtd-table-header-results">
			    <th class="title">Title</th>
			    <th>Grade</th>
			    <th>Type</th>
			    <th class="publisher">Publisher&nbsp;Number/ISBN</th>
			    <th class="year">Year</th>
			    <th>Net School Price</th>
			    <th>Contract Price</th>
			    <th class="add">&nbsp;</th>
		    </tr>
		    <tr>
    			<td><%#StoreHelper.GetEntryDisplayName(Entry)%></td>
			    <td><%#Entry.ItemAttributes["Grade"] %></td>
			    <td><%#Entry.ItemAttributes["Type"] %></td>
			    <td><%#Entry.ItemAttributes["ISBN10"] %><br />
			    <%#Entry.ItemAttributes["ISBN13"] %></td>
			    <td><%# this.ParseYear(Entry.ItemAttributes["Year"])%></td>
			    <td><NWTD:YourPrice runat="server" ID="NetSchoolPrice" Entry="<%#Entry%>" PriceType="NetSchool" /></td>
			    <td><NWTD:YourPrice runat="server" ID="YourPrice" Entry="<%#Entry%>" PriceType="Discount" /></td>
				<td><NWTD:AddToCart runat="server" ID="addToCart" Code="<%#Entry.ID%>" /></td>
		    </tr>
	    </table>
	    <hr />    
    </div>
</div>   
<asp:Image ID="Image1" runat="server" ImageUrl="~/App_Themes/NWTD/images-template/curve-details.png" cssclass="curves" />
<div id="nwtd-related-items">
    <div class="detail-padding">
	    <h2><a href="#" class="nwtd-show-related">Related Items</a></h2>
	    <div class="close-link"><a class="nwtd-show-related nwtd-toggle-related" href="#">Show Related Items</a></div>
		<div class="nwtd-related" style="display:none;" >

			<NWTD:BookSubSearch AllowPaging="false"  PageSize="10" runat="server" ID="bssCurrentEntry" SelectedEntryId="<%#Entry.ID%>"  Publisher='<%#Entry.ItemAttributes["Publisher"] %>' Series='<%#Entry.ItemAttributes["Series"] %>' />
		</div>
    </div>	   
</div>



