<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="BookSubSearch.ascx.cs" Inherits="Mediachase.Cms.Website.Templates.NWTD.Modules.BookSubSearch" %>
<%@ Register Src="~/Structure/User/NWTDControls/Controls/Catalog/SearchResults.ascx" TagName="SearchResults" TagPrefix="NWTD" %>
<%@ Register src="~/Structure/User/NWTDControls/Controls/Misc/UpdateMessage.ascx" tagname="UpdateMessage" tagprefix="NWTD" %>
<%@ Register Assembly="OakTree.Web.UI" Namespace="OakTree.Web.UI.WebControls" TagPrefix="OakTree" %>
<OakTree:JavaScriptInclude runat="server" ID="jsBookSubSearch" >
$(document).ready(function(){
	Sys.WebForms.PageRequestManager.getInstance().add_pageLoaded(function(sender, args) {
		var updatedPanels = $.map(args.get_panelsUpdated(), function(el, i) { return el.id });
		var update = false;
		$.each(OakTree.Web.UI.WebControls.SubSearch, function(i, SubSearch) {
			if ($.inArray(SubSearch.updatePanelID, updatedPanels) != -1) {
				update = true;
			}
		});
		if (update){ 
			OakTree.Web.UI.WebControls.SearchResults.update();
		}
	});
});
</OakTree:JavaScriptInclude>


<div class="nwtd-filterSearch">
    Filter results:
	<asp:DropDownList runat="server" ID="ddlGradeFilter" AutoPostBack="true"  DataTextField="Name" DataValueField="Key" onselectedindexchanged="filter_changed" />
	<asp:DropDownList runat="server" ID="ddlYearFilter" AutoPostBack="true"  DataTextField="Name" DataValueField="Key" onselectedindexchanged="filter_changed" />
	<asp:DropDownList runat="server" ID="ddlTypeFilter" AutoPostBack="true"  DataTextField="Name" DataValueField="Key" onselectedindexchanged="filter_changed"  />
</div><br style="clear:both;" />



<asp:UpdatePanel runat="server" ID="udpBookSearch" UpdateMode="Conditional" ChildrenAsTriggers="false">
	<Triggers>
		<asp:AsyncPostBackTrigger ControlID="ddlGradeFilter" />
		<asp:AsyncPostBackTrigger ControlID="ddlYearFilter" />
		<asp:AsyncPostBackTrigger ControlID="ddlTypeFilter" />
	</Triggers>
	<ContentTemplate>
		<asp:UpdateProgress runat="server" ID="progressBookSearch"  DynamicLayout="true" >
			<ProgressTemplate>
					<NWTD:UpdateMessage ID="UpdateMessage1" CssClass="nwtd-update-message" runat="server" Message="Updating Results..." ImageUrl="~/Structure/User/NWTDControls/Images/loading.gif" />
			</ProgressTemplate>
		</asp:UpdateProgress>

	    <div id="nwtd-related">
			<NWTD:SearchResults runat="server"  ID="srBookSearch" />
		</div>
	</ContentTemplate>
</asp:UpdatePanel>


