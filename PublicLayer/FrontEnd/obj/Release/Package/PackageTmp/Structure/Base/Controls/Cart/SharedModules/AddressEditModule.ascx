<%@ Control Language="c#" Inherits="Mediachase.eCF.PublicStore.SharedModules.AddressEditModule" Codebehind="AddressEditModule.ascx.cs" %>
<asp:UpdatePanel runat="server" ID="upAddress" UpdateMode="Conditional">
    <ContentTemplate>
<asp:TextBox ID="tbAddressID" Visible="false" runat="server"></asp:TextBox>
<table id="Table6" border="0">
	<tbody>
		<tr runat="server" id="EmailTr" visible="false">
			<td align="right">
				*<b><%=RM.GetString("ACCOUNT_NEW_Email_LABEL")%>:</b>
			</td>
			<td>
			    <asp:TextBox runat="server" MaxLength="255" AutoCompleteType="Email" Width="230" ID="Email"></asp:TextBox>
			    <asp:requiredfieldvalidator id="EmailRequired" runat="server" ControlToValidate="Email" Display="dynamic" ErrorMessage="<%$resources:StoreResources,ERROR_BLANK_Email %>"></asp:requiredfieldvalidator>
				<asp:regularexpressionvalidator id="EmailValid" runat="server" ControlToValidate="Email" Display="Dynamic" ErrorMessage="<%$resources:StoreResources,ERROR_INVALID_Email%>" ValidationExpression="[\w\.-]+(\+[\w-]*)?@([\w-]+\.)+[\w-]+"></asp:regularexpressionvalidator>
			</td>
		</tr>	
		<tr>
			<td align="right">
				*<b>
					<%=RM.GetString("ADDRESS_FIRSTNAME")%>:
				</b> 
			</td>
			<td align="left">
				<asp:textbox id="FirstName" Runat="server" AutoCompleteType="FirstName" MaxLength="100" Width="230"></asp:textbox>
				<asp:requiredfieldvalidator id="NameValidator" Runat="server" ControlToValidate="FirstName" Display="Dynamic">*</asp:requiredfieldvalidator>
			</td>
		</tr>
		<tr>
			<td align="right">
				*<b>
					<%=RM.GetString("ADDRESS_LASTNAME")%>:
				</b> 
			</td>
			<td align="left">
				<asp:textbox id="LastName" Runat="server" AutoCompleteType="LastName" MaxLength="100" Width="230"></asp:textbox>
				<asp:requiredfieldvalidator id="LastNameValidator" Runat="server" ControlToValidate="LastName" Display="Dynamic">*</asp:requiredfieldvalidator>
			</td>
		</tr>
		<tr>
			<td align="right">
				<b>
					<%=RM.GetString("ADDRESS_COMPANY")%>:
				</b>
			</td>
			<td align="left">
				<asp:textbox id="Company" Runat="server" MaxLength="100" Width="230" AutoCompleteType="Company"></asp:textbox>
			</td>
		</tr>
		<tr>
			<td align="right">
				*<b>
					<%=RM.GetString("ADDRESS_STREET")%>:
				</b> 
			</td>
			<td align="left">
				<asp:textbox id="Address1" Runat="server" MaxLength="200" Width="230" AutoCompleteType="HomeStreetAddress"></asp:textbox>
				<asp:requiredfieldvalidator id="AddressValidator" Runat="server" ControlToValidate="Address1" Display="Dynamic">*</asp:requiredfieldvalidator>
			</td>
		</tr>
		<tr>
			<td></td>
			<td align="left">
				<asp:textbox id="Address2" Runat="server" MaxLength="200" Width="230"></asp:textbox>
			</td>
		</tr>
		<tr>
			<td align="right">
				*<b>
					<%=RM.GetString("ADDRESS_CITY")%>:
				</b> 
			</td>
			<td align="left">
				<asp:textbox id="City" Runat="server" MaxLength="100" Width="230" AutoCompleteType="HomeCity"></asp:textbox>
				<asp:requiredfieldvalidator id="CityValidator" Runat="server" ControlToValidate="City" Display="Dynamic">*</asp:requiredfieldvalidator>
			</td>
		</tr>
		<tr>
			<td align="right">
				<b>
					<%=RM.GetString("ADDRESS_COUNTRY")%>:
				</b> 
			</td>
			<td align="left">
				<asp:dropdownlist AutoPostBack="True" id="Country" Runat="server" DataTextField="Name" DataMember="Country" DataValueField="Code" onselectedindexchanged="Country_SelectedIndexChanged"></asp:dropdownlist>
			</td>   
		</tr>	
		<tr>
			<td align="right">
				*<b>
					<%=RM.GetString("ADDRESS_STATE")%>:
				</b> 
			</td>
			<td align="left" style="white-space: nowrap">
				<asp:DropDownList ID="State" Runat="server" DataTextField="Name" DataValueField="Name"></asp:DropDownList><asp:TextBox Runat="server" Width="102" ID="StateTxt" Visible="False"></asp:TextBox>
				<asp:requiredfieldvalidator id="StateValidator" Runat="server" ControlToValidate="StateTxt" Enabled="false" Display="Dynamic">*</asp:requiredfieldvalidator>
				&nbsp;*<b><%=RM.GetString("ADDRESS_ZIP")%>:
					</b>
				<asp:textbox id="Zip" Runat="server" MaxLength="15" Width="76px" AutoCompleteType="HomeZipCode"></asp:textbox>
				<asp:requiredfieldvalidator id="ZipValidator" Runat="server" ControlToValidate="Zip" Display="Dynamic">*</asp:requiredfieldvalidator></td>
		</tr>
		<tr>
			<td align="right">
				<b>
					Day <%=RM.GetString("ADDRESS_PHONE")%>:
				</b> 
			</td>
			<td align="left">
				<asp:textbox id="Phone" Runat="server" MaxLength="100" Width="230" AutoCompleteType="HomePhone"></asp:textbox>
			</td>
		</tr>		
		<tr>
			<td align="right">
				<b>
					Evening Phone:
				</b> 
			</td>
			<td align="left">
				<asp:textbox id="EveningPhone" Runat="server" MaxLength="100" Width="230"></asp:textbox>
			</td>
		</tr>		
		<tr>
			<td align="right">
				<b>
					<%=RM.GetString("ADDRESS_FAX")%>:
				</b> 
			</td>
			<td align="left">
				<asp:textbox id="Fax" Width="230" MaxLength="100" Runat="server" AutoCompleteType="HomeFax"></asp:textbox>
			</td>
		</tr>
	</tbody>
</table>
    </ContentTemplate>
</asp:UpdatePanel>