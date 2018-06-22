<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SchoolSelector.ascx.cs" Inherits="Mediachase.Cms.Website.Structure.User.NWTDControls.Controls.User.SchoolSelector" %>
<div class="nwtd-form-field">
	<asp:Label runat="server" AssociatedControlID="ddlState" ID="lblState" Text="State:" />
	<asp:DropDownList 
		ID="ddlState" 
		runat="server" 
		AutoPostBack="true"
		DataSourceID="dsStates" />
</div>
<asp:ObjectDataSource 
	ID="dsStates" 
	runat="server" 
	SelectMethod="GetStates" 
	TypeName="Mediachase.Cms.Website.Services.States" />
	
<div class="nwtd-form-field">
	<asp:Label runat="server" AssociatedControlID="ddlDistricts" ID="sblDistrict" Text="District:" />
	<asp:UpdatePanel 
		ID="udpDistricts" 
		runat="server" 
		ChildrenAsTriggers="false" 
		UpdateMode="Conditional">
		<Triggers>
			<asp:AsyncPostBackTrigger ControlID="ddlState" />
		</Triggers>
		<ContentTemplate>
			<asp:DropDownList 
				ID="ddlDistricts"
				runat="server" 
				DataSourceID="dsDistricts" 
				DataTextField="value" 
				DataValueField="key" 
				ondatabound="ddlDistricts_DataBound" />
		</ContentTemplate>
</asp:UpdatePanel>
</div>

<asp:ObjectDataSource 
	ID="dsDistricts" 
	runat="server" 
	SelectMethod="GetDistrictsByState" 
	TypeName="Mediachase.Cms.Website.Services.States">
	<SelectParameters>
		<asp:ControlParameter ControlID="ddlState" Name="state" PropertyName="SelectedValue" Type="String" />
	</SelectParameters>
</asp:ObjectDataSource>

<div class="nwtd-form-field">
	<asp:Label runat="server" AssociatedControlID="ddlSchools" ID="lblSchool" Text="School:" />
	<asp:UpdatePanel 
		ID="udpSchools" 
		runat="server" 
		ChildrenAsTriggers="false" 
		UpdateMode="Conditional">
		<Triggers>
			<asp:AsyncPostBackTrigger ControlID="ddlDistricts" />
		</Triggers>
			<ContentTemplate>
			<asp:DropDownList 
				ID="ddlSchools" 
				AutoPostBack="true"
				runat="server" EnableViewState="false"
				DataSourceID="dsSchools" />
		 </ContentTemplate>
	</asp:UpdatePanel>
</div>

<asp:ObjectDataSource 
	ID="dsSchools" 
	runat="server" 
	SelectMethod="GetSchoolsByDistrict" 
	TypeName="Mediachase.Cms.Website.Services.States">
	<SelectParameters>
		<asp:ControlParameter ControlID="ddlDistricts" Name="district" PropertyName="SelectedValue" Type="String" />
	</SelectParameters>
</asp:ObjectDataSource>
