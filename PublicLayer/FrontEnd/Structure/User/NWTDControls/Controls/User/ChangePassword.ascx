<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ChangePassword.ascx.cs" Inherits="Mediachase.Cms.Website.Structure.User.NWTDControls.Controls.User.ChangePassword" %>
<div runat="server" id="divViewer">
</div>
<h2>Change Password</h2>
<asp:UpdatePanel ID="UpdatePanel1" runat="server"  >
	<ContentTemplate>
		<div class="nwtd-change-user-info">

	

			<asp:ChangePassword  Width="100%" ID="ChangePwd" runat="server"  SuccessTitleText=""  ChangePasswordTitleText="" SuccessText="Your password has been changed">
				<ChangePasswordTemplate>
					<div class="nwtd-form-field">
						<asp:Label ID="CurrentPasswordLabel" runat="server" AssociatedControlID="CurrentPassword"><%=RM.GetString("GENERAL_PASSWORD_LABEL")%>:</asp:Label>
						<asp:TextBox ID="CurrentPassword" Width="250" runat="server" TextMode="Password"></asp:TextBox>
						<asp:RequiredFieldValidator ID="CurrentPasswordRequired" runat="server" ErrorMessage="Required" ControlToValidate="CurrentPassword" Display="Dynamic" ValidationGroup="ctl00$ChangePwd"/>
					</div>
					<div class="nwtd-form-field">
						<asp:Label ID="NewPasswordLabel" runat="server" AssociatedControlID="NewPassword"><%=RM.GetString("ACCOUNT_REGISTRATION_NEW_PASSWORD_LABEL")%>:</asp:Label>
						<asp:TextBox ID="NewPassword" Width="250" runat="server" TextMode="Password"></asp:TextBox>
						<asp:RequiredFieldValidator ID="NewPasswordRequired" runat="server" ControlToValidate="NewPassword"
							ErrorMessage="Required" Display="Dynamic" ToolTip='<%#RM.GetString("ACCOUNT_NEW_PASSWORD_REQUIRED") %>'
							ValidationGroup="ctl00$ChangePwd"></asp:RequiredFieldValidator>
					</div>
					<div class="nwtd-form-field">
						<asp:Label ID="ConfirmNewPasswordLabel" runat="server" AssociatedControlID="ConfirmNewPassword"><%=RM.GetString("ACCOUNT_REGISTRATION_CONFIRM_PASSWORD_LABEL")%>:</asp:Label>
						<asp:TextBox ID="ConfirmNewPassword" Width="250" runat="server" TextMode="Password"></asp:TextBox>
						<asp:RequiredFieldValidator ID="ConfirmNewPasswordRequired" runat="server" ControlToValidate="ConfirmNewPassword"
							ErrorMessage="Required" Display="Dynamic" ToolTip='<%#RM.GetString("ACCOUNT_CONFIRM_NEW_PASSWORD_REQUIRED") %>'
							ValidationGroup="ctl00$ChangePwd"></asp:RequiredFieldValidator>
						<asp:CompareValidator ID="NewPasswordCompare" runat="server" ControlToCompare="NewPassword"
							ControlToValidate="ConfirmNewPassword" Display="Dynamic" ErrorMessage="New Password and Confirm Password does not match.  Please renter and confirm your new password."
							ValidationGroup="ctl00$ChangePwd"></asp:CompareValidator>
						<asp:Literal ID="FailureText" runat="server" EnableViewState="False"></asp:Literal>
					</div>
					<div class="nwtd-form-field">
						<asp:Button ID="ChangePasswordPushButton" runat="server" CommandName="ChangePassword"
							Text="Change Password"  CssClass="save-changes-blue-btn buttons password" ValidationGroup="ctl00$ChangePwd" />
						<asp:Button ID="CancelPushButton" Visible="false" runat="server" CausesValidation="False"
							CommandName="Cancel" Text="Cancel" />
					</div>
				</ChangePasswordTemplate>
				<SuccessTemplate>
					<span class="notification">Your password has been changed.</span>
				</SuccessTemplate>
			</asp:ChangePassword>
			<asp:Label ID="ErrorLabel" CssClass="ErrorText" EnableViewState="false" runat="server" />
		</div>

	</ContentTemplate>
</asp:UpdatePanel>