<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DistrictSelector.ascx.cs" Inherits="Mediachase.Cms.Website.Structure.User.NWTDControls.Controls.User.DistrictSelector" %>
<%@ Register assembly="Mediachase.Commerce" namespace="Mediachase.Commerce.Profile.DataSources" tagprefix="profile" %>
<%@ Register src="~/Structure/User/NWTDControls/Controls/Misc/UpdateMessage.ascx" tagname="UpdateMessage" tagprefix="NWTD" %>

	<asp:Label runat="server" ID="lblState" AssociatedControlID="ddlState" CssClass="select-a-state-lbl" Text="State:" />
	<div class="select-a-state">
		<asp:DropDownList 
			ID="ddlState" 
			runat="server" 
			DataTextField="Value" 
			DataValueField="Key" 
			AutoPostBack="true" 
			TabIndex="1" AppendDataBoundItems="true">
			<asp:ListItem Text="Select State" Value="" />
		</asp:DropDownList>

		<%--<asp:SqlDataSource 
			ID="sqlDsStates" 
			runat="server" 
			ConnectionString="<%$ ConnectionStrings:EcfSqlConnection%>" 
			SelectCommand="SELECT DISTINCT [BusinessPartnerState] FROM [Principal_Organization] WHERE BusinessPartnerState<>'' ORDER BY BusinessPartnerState"></asp:SqlDataSource>
			
--%>	
        
    </div>

	<asp:UpdatePanel runat="server" ID="udpDistricts" ChildrenAsTriggers="false" UpdateMode="Conditional">
		<Triggers>
			<asp:AsyncPostBackTrigger ControlID="ddlState" />
		</Triggers>
		<ContentTemplate>
			<asp:UpdateProgress  runat="server" ID="progressGetDistricts"  DynamicLayout="true" >
				<ProgressTemplate>
					<NWTD:UpdateMessage CssClass="nwtd-update-message" runat="server" Message="Loading Districts..." ImageUrl="~/Structure/User/NWTDControls/Images/loading.gif" />
				</ProgressTemplate>
			</asp:UpdateProgress>
			<div class="select-public-school">
				<asp:Label runat="server" ID="lblDistricts"  AssociatedControlID="ddlDistricts" Text="Public Schools:" />
				<asp:DropDownList 
					runat="server" 
					ID="ddlDistricts" 
					DataSourceID="dsDistricts" 
					DataTextField="DisplayName" 
					DataValueField="ObjectId" 
					CssClass="nwtd-districtSelector"
					TabIndex="2" 
					ondatabound="ddlDistricts_DataBound" >
				</asp:DropDownList>
				<asp:SqlDataSource 
					ID="dsDistricts" runat="server" 
					ConnectionString="<%$ ConnectionStrings:EcfSqlConnection %>" 
					SelectCommand="SELECT o.BusinessPartnerID, o.DisplayName, o.ObjectId FROM Principal_Organization o LEFT JOIN Principal p ON p.id = o.ObjectId WHERE (o.BusinessPartnerState = @BusinessPartnerState) AND o.BPType = 'SD' AND p.state <> 3 ORDER BY DisplayName">
					<SelectParameters>
						<asp:ControlParameter ControlID="ddlState" DefaultValue="" Name="BusinessPartnerState" PropertyName="SelectedValue" Type="String" />
					</SelectParameters>
				</asp:SqlDataSource>
			</div>
            <div class="or-seperator">OR</div>
			<div class="select-other-than">
				<asp:Label runat="server" ID="lblNonDistricts" AssociatedControlID="ddlNonDistricts" Text="Other than Public Schools:" />
				<asp:DropDownList 
					runat="server"
					CausesValidation="true" 
					ID="ddlNonDistricts" 
					DataSourceID="dsNonDistricts" 
					DataTextField="DisplayName" 
					DataValueField="ObjectId" 
					ondatabound="ddlNonDistricts_DataBound" 
					CssClass="nwtd-districtSelector" 
					TabIndex="3">
				</asp:DropDownList>
				<asp:SqlDataSource
					ID="dsNonDistricts"
					runat="server"
					ConnectionString="<%$ ConnectionStrings:EcfSqlConnection %>" 
					SelectCommand="SELECT o.BusinessPartnerID, o.DisplayName, o.ObjectId FROM Principal_Organization o LEFT JOIN Principal p ON p.id = o.ObjectId WHERE (o.BusinessPartnerState = @BusinessPartnerState AND o.BPType <> 'SD' AND o.BPType <> 'PUB') AND p.state <> 3  ORDER BY DisplayName">
					<SelectParameters>
						<asp:ControlParameter ControlID="ddlState" DefaultValue="" Name="BusinessPartnerState" PropertyName="SelectedValue" Type="String" />
					</SelectParameters>
				</asp:SqlDataSource>
			</div>

			
		</ContentTemplate>
	</asp:UpdatePanel>





