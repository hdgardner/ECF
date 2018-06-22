<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ChangeUserInfo.ascx.cs" Inherits="Mediachase.Cms.Website.Structure.User.NWTDControls.Controls.User.ChangeUserInfo" %>


<h2 class="profile">Change Personal Info</h2>

<asp:UpdatePanel runat="server">
	<ContentTemplate>
	
	<div class="nwtd-change-user-info">
		<div class="nwtd-form-field">
			<label>User Name</label><asp:Literal runat="server" ID="litUserName" />
		</div>
		<div class="nwtd-form-field">
			<label>State</label><asp:Literal runat="server" ID="litUserState" />
		</div>
		<div class="nwtd-form-field">
			<label>District/School</label><asp:Literal runat="server" ID="litUserDistrict" />
		</div>
		<div class="nwtd-form-field">
			<asp:Label  runat="server" AssociatedControlID="tbEmail">Email <span class="nwtd-required-field">*</span></asp:Label>
			<asp:TextBox runat="server" ID="tbEmail" Width="150"></asp:TextBox>
			<asp:RegularExpressionValidator  ValidationExpression="^([a-z0-9_\.-]+)@([\da-z\.-]+)\.([a-z\.]{2,6})$" Display="Dynamic" runat="server" ValidationGroup="PersonalInfoGroup" ControlToValidate="tbEmail" ErrorMessage="You must enter valid Email address" />
			<asp:RequiredFieldValidator Display="Dynamic" runat="server" ValidationGroup="PersonalInfoGroup" ControlToValidate="tbEmail" ErrorMessage="You must enter an Email Address" />
		</div>
		<div class="nwtd-form-field">
			<asp:Label runat="server" AssociatedControlID="tbFirstName">First Name <span class="nwtd-required-field">*</span></asp:Label>
			<asp:TextBox runat="server" ID="tbFirstName" ></asp:TextBox>
			<asp:RequiredFieldValidator  Display="Dynamic" runat="server" ValidationGroup="PersonalInfoGroup" ControlToValidate="tbFirstName" ErrorMessage="You must enter a First Name" />
		</div>
		<div class="nwtd-form-field">
			<asp:Label runat="server" AssociatedControlID="tbLastName">Last Name <span class="nwtd-required-field">*</span></asp:Label>
			<asp:TextBox runat="server" ID="tbLastName"></asp:TextBox>
			<asp:RequiredFieldValidator Display="Dynamic" runat="server" ValidationGroup="PersonalInfoGroup" ControlToValidate="tbLastName" ErrorMessage="You must enter a Last Name" />
		</div>
		<div class="nwtd-form-field">
			<asp:Label runat="server" AssociatedControlID="tbPhone">Phone</asp:Label>
			<asp:TextBox runat="server" ID="tbPhone"></asp:TextBox>
		</div>
		<div class="nwtd-form-field">
			<asp:Button runat="server" ID="btnSaveChanges" Text="Save" ValidationGroup="PersonalInfoGroup" CssClass="save-changes-blue-btn buttons personal"  onclick="btnSaveChanges_Click" />
		</div>
	</div>
	<br /><br />
	<asp:Label CssClass="notification" runat="server" ID="lblMessage" />
	<hr />


	</ContentTemplate>
</asp:UpdatePanel>