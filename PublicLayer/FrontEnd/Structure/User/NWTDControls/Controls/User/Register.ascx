<%@ Control 
	Language="C#"
	AutoEventWireup="true" 
	CodeBehind="Register.ascx.cs" 
	Inherits="Mediachase.Cms.Website.Structure.User.NWTDControls.Controls.User.Register" %>
<%@ Register Src="~/Structure/User/NWTDControls/Controls/User/DistrictSelector.ascx" TagName="DistrictSelector" TagPrefix="NWTD" %>

<asp:Panel runat="server" ID="pnlError" Visible="false" >
	<p style="color:#f00;" class="nwtd-register-form-errors"><asp:Literal runat="server" ID="litError" /></p>
</asp:Panel>
<asp:CreateUserWizard 
	ID="cuwRegister" 
	runat="server" 
	oncreateduser="cuwRegister_CreatedUser" 
	CreateUserButtonText="Register" 
	oncreateusererror="cuwRegister_CreateUserError" 
	oncreatinguser="cuwRegister_CreatingUser"
	LoginCreatedUser="true"> 
	<CreateUserButtonStyle CssClass="register-btn" />
	<WizardSteps>
		<asp:CreateUserWizardStep ID="CreateUserWizardStep1"  runat="server"> 
			<ContentTemplate>
			<div class="register-description">
				<%=this.DescriptionText %>
			</div>


			    <asp:ValidationSummary runat="server" ID="vsRegister" CssClass="nwtd-register-form-errors" ValidationGroup="cuwRegister" HeaderText="Please fix the following error(s):" />
				<div class="nwtd-register-form">			
					<fieldset class="nwtd-register-top">
						<NWTD:DistrictSelector runat="server" ID="dsDistricts" />
						<asp:RequiredFieldValidator Display="Dynamic" ID="rvDistricts" runat="server" Text="You must select a district" ErrorMessage="You must select a district or public school" ToolTip="You must select a district or public schoo" ControlToValidate="dsDistricts" ValidationGroup="cuwRegister" >*</asp:RequiredFieldValidator>
					</fieldset>
					<hr />
					<fieldset class="nwtd-register-left">		
						<div class="nwtd-form-field">
							<asp:Label runat="server" ID="lblFirstName" AssociatedControlID="FirstName" Text="First Name:" />
							<asp:TextBox runat="server" ID="FirstName" TabIndex="4" />
							<asp:RequiredFieldValidator  CssClass="nwtd-error" 
								ErrorMessage="First Name is Required" ID="rvFirstName" runat="server" Text="*" 
								ToolTip="First Name is required" ControlToValidate="FirstName" 
								ValidationGroup="cuwRegister" Display="Dynamic" />
						</div>
						<div class="nwtd-form-field">
							<asp:Label runat="server" ID="lblLastName" AssociatedControlID="LastName" Text="Last Name:" />
							<asp:TextBox runat="server" ID="LastName" TabIndex="5" />
							<asp:RequiredFieldValidator CssClass="nwtd-error" 
								ErrorMessage="Last Name is Required" ID="rvLastName" runat="server" Text="*" 
								ToolTip="Last Name is required" ControlToValidate="LastName" 
								ValidationGroup="cuwRegister" Display="Dynamic" />
						</div>
						<div class="nwtd-form-field">
							<asp:Label runat="server" ID="lblEmail" AssociatedControlID="Email" Text="Email:" />
							<asp:TextBox runat="server" ID="Email" TabIndex="6" />
							<asp:RequiredFieldValidator Display="Dynamic" CssClass="nwtd-error" ErrorMessage="Email is Required" ID="rvEmail" runat="server" Text="*" ToolTip="Email is required" ControlToValidate="Email" ValidationGroup="cuwRegister" /> 
							<asp:RegularExpressionValidator Text="*" CssClass="nwtd-error" Runat="server" ID="EmailValidator" Display="Dynamic" ValidationGroup="cuwRegister" ControlToValidate="Email" ErrorMessage='Invalid Email Address' ToolTip="You must supply a valid email address" ValidationExpression="[\w\.-]+(\+[\w-]*)?@([\w-]+\.)+[\w-]+" />
						</div>
						<div class="nwtd-form-field">
							<asp:Label runat="server" ID="lblConfirmEmail" AssociatedControlID="ConfirmEmail" Text="Confirm Email:" />
							<asp:TextBox runat="server" ID="ConfirmEmail" TabIndex="7" />
							<asp:RequiredFieldValidator ID="rvConfirmEmailRequired" Display="Dynamic" Text="*" runat="server" ToolTip="You must confirm your email address." ControlToValidate="ConfirmEmail" ValidationGroup="cuwRegister" CssClass="nwtd-error" />
							<asp:CompareValidator Text="*" runat="server" ID="cvEmail" Display="Dynamic" ValidationGroup="cuwRegister" ControlToCompare="Email" ControlToValidate="ConfirmEmail" ErrorMessage="Email Addresses Must Match"  />
						</div>
						<div class="nwtd-form-field">
						    <asp:Label runat="server" ID="lblUserName" AssociatedControlID="UserName" Text="User Name:" />
							<asp:TextBox runat="server" ID="UserName" TabIndex="8" />
							<asp:RequiredFieldValidator ID="rvUserName" CssClass="nwtd-error" 
								ErrorMessage="User Name is Required" runat="server" Text="*" 
								ToolTip="User Name is required" ControlToValidate="UserName" 
								ValidationGroup="cuwRegister" Display="Dynamic" />
						</div>
						<div class="nwtd-form-field">
						    <asp:Label runat="server" ID="lblPhone" AssociatedControlID="Phone" Text="Phone:" />
							<asp:TextBox runat="server" ID="Phone" TabIndex="9" />

						</div>
						<div class="nwtd-form-field">
						    <asp:Label runat="server" ID="lblPassword" AssociatedControlID="Password" Text="Password:" />
							<asp:TextBox runat="server" ID="Password" TextMode="Password" TabIndex="10" />
							<asp:RequiredFieldValidator  CssClass="nwtd-error" 
								ErrorMessage="Password is Required" ID="rvPassword" runat="server" Text="*" 
								ToolTip="Password is required" ControlToValidate="Password" 
								ValidationGroup="cuwRegister" Display="Dynamic" />
								&nbsp;Password must be at least four characters long.
						</div>
						<div class="nwtd-form-field">
							<asp:Label runat="server" ID="lblConfirmPassword" AssociatedControlID="ConfirmPassword" Text="Confirm Password:" />
							<asp:TextBox runat="server" ID="ConfirmPassword" TextMode="PassWord" TabIndex="11" />
							<asp:RequiredFieldValidator ID="rfConfirmPassword" runat="server" Text="*" ErrorMessage="You must confirm your password" ToolTip="You must confirm your password." ControlToValidate="ConfirmPassword" ValidationGroup="cuwRegister" Display="Dynamic" CssClass="nwtd-error"  />
							<asp:CompareValidator Text="*" ErrorMessage="Passwords do not Match" runat="server" ID="cvPassword" Display="Dynamic" ValidationGroup="cuwRegister"  ControlToCompare="Password" ControlToValidate="ConfirmPassword"  />	
						</div>
					</fieldset>
				</div>
			</ContentTemplate>
		</asp:CreateUserWizardStep>
		<asp:CompleteWizardStep ID="CompleteWizardStep1" runat="server">
			<ContentTemplate>
				Your account has been successfully created and you are now logged in. <a href="/default.aspx">Click here</a> to proceed to the home page.

<%--                <script type="text/javascript">
                <!--
 
                    //setTimeout('window.location="/default.aspx"',2000);
 
                //--> 
                </script> --%>

			</ContentTemplate>
		</asp:CompleteWizardStep>
	</WizardSteps>
</asp:CreateUserWizard>

