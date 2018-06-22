<%@ Control 
	Language="C#" 
	AutoEventWireup="true" 
	CodeBehind="SideMenuControl.ascx.cs" 
	Inherits="Mediachase.Cms.Website.Templates.NWTD.Modules.SideMenuControl"
%>
<%@ Register TagPrefix="ComponentArt" Namespace="ComponentArt.Web.UI" Assembly="ComponentArt.Web.UI" %>
<%@ Import Namespace="Mediachase.Cms.WebUtility.Search" %>
<%@ Import Namespace="System.Collections.Generic" %>
<div class="nwtd-sideMenu">
    
    <asp:Repeater 
		runat="server" 
		ID="ActiveFilterList" 
		EnableViewState="false">
        <HeaderTemplate>
            <div class="current-filters">
                <h4>Currently Shopping by:</h4>
			    <ol class="nwtd-activeFilters">      
        </HeaderTemplate>
        <ItemTemplate>
			        <li>
				        <cms:ThemedHyperLink 
					        ID="ThemedHyperLink1" 
					        runat="server" 
					        ImageUrl="images/list_remove_btn.gif" 
					        AlternateText="Remove This Item" 
					        CssClass="remove" 
					        NavigateUrl=<%#  SearchFilterHelper.GetQueryStringNavigateUrl( (string)Eval("RemoveUrl"), "page", string.Empty, true) %> />
				        <span class="label"><%#Eval("Name")%>:</span> <%#Eval("ValueName")%>
			        </li>
        </ItemTemplate>
        <FooterTemplate>
			    </ol>
    
			    <div class="actions">
    				<a href="<%#SearchFilterHelper.GetQueryStringNavigateUrl( SearchHelper.GetCleanUrl(), "page", string.Empty, true) %>">Clear All</a>
			    </div>
            </div>			    
        </FooterTemplate>
    </asp:Repeater>
    
    <asp:Repeater 
		runat="server" 
		ID="FilterList" 
		EnableViewState="false">
        <HeaderTemplate>
            <h4 class="refine-search-title">Refine your search</h4>
			<dl class="nwtd-filterList">
        </HeaderTemplate>
        <ItemTemplate>
			<dt class="nwtd-searchFacetCategoryName"><%#Eval("Name")%></dt>
			<dd>
				<ol>
					<asp:Repeater 
						runat="server" 
						ID="ValueList" 
						DataSource='<%#Eval("Facets") %>'>
						<ItemTemplate>
							<li>
								<a href='<%# SearchFilterHelper.GetQueryStringNavigateUrl(SearchFilterHelper.GetQueryStringNavigateUrl((string)Eval("Group.FieldName"), (string)Eval("Key")), "page", string.Empty, true)%>'>
									<%#Eval("Name")%>
								</a> (<%#Eval("Count")%>) 
							</li>
						</ItemTemplate>
					</asp:Repeater>
				</ol>
			</dd>
        </ItemTemplate>
        <FooterTemplate>
				</dl> 
        </FooterTemplate>
    </asp:Repeater>
</div>