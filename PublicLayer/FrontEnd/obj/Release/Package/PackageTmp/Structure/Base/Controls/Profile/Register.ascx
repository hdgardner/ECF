<%@ Control Language="C#" Inherits="Mediachase.Cms.Controls.Register" Codebehind="Register.ascx.cs" %>
<div runat="server" id="divViewer"></div>
<table cellpadding="4" cellspacing="4" style="width: 100%">
	
    <tr>
        <td style="width: 100%;">
            <asp:Panel ID="Panel1" runat="server" style="width: 100%" DefaultButton="CreateUserForm$__CustomNav0$StepNextButtonButton">
            <asp:CreateUserWizard ID="CreateUserForm" LoginCreatedUser="true" 
				RequireEmail="true" runat="server" OnCreatedUser="CreatedUser"
				FinishDestinationPageUrl='<%# Page.ResolveUrl("~/Profile/AccountInfo.aspx") %>' 
				ContinueDestinationPageUrl='<%# Page.ResolveUrl("~/Profile/AccountInfo.aspx") %>' 
				CreateUserButtonText='<%#RM.GetString("ACCOUNT_NEW_TITLE")%>'
				UnknownErrorMessage='<%#RM.GetString("GENERAL_UNKNOWN_ERROR_MESSAGE") %>'
				>
                <WizardSteps>
                    <asp:CreateUserWizardStep runat="server" ID="CreateUserWizard1" Title="">
                        <ContentTemplate>
                            <table border="0" width="480">
                                <col width="140" />
                                <tr>
                                    <td colspan="2">
                                        <h1>
                                            <%=RM.GetString("ACCOUNT_REGISTER_NEW_ACCOUNT")%>
                                        </h1>
                                    </td>
                                </tr>
                                <tr>
                                    <td style="height: 122px" colspan="2">
                                        <%=RM.GetString("ACCOUNT_REGISTER_MESSAGE") %>
                                        <br />
                                        <ul>
                                            <li>
                                                <%=RM.GetString("ACCOUNT_REGISTER_LI1")%>
                                            </li><br /><br />
                                            <li>
                                                <%=RM.GetString("ACCOUNT_REGISTER_LI2")%>
                                            </li><br /><br />
                                            <li>
                                                <%=RM.GetString("ACCOUNT_REGISTER_LI3")%>
                                            </li><br /><br />
                                            <li>
                                                <%=RM.GetString("ACCOUNT_REGISTER_LI4")%>
                                            </li>
                                        </ul>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <asp:Label ID="UserNameLabel" runat="server" AssociatedControlID="UserName">
                                            <%=RM.GetString("ACCOUNT_LOGIN_USER_NAME")%>:</asp:Label></td>
                                    <td>
                                        <asp:TextBox ID="UserName" runat="server" Width="300"></asp:TextBox>
                                        <asp:RequiredFieldValidator ID="UserNameRequired" runat="server" ControlToValidate="UserName"
                                            ErrorMessage='*' ValidationGroup="CreateUserForm"></asp:RequiredFieldValidator>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <asp:Label ID="PasswordLabel" runat="server" AssociatedControlID="Password">
                                            <%=RM.GetString("ACCOUNT_LOGIN_PASSWORD")%>:</asp:Label></td>
                                    <td>
                                        <asp:TextBox ID="Password" runat="server" Width="300" TextMode="Password"></asp:TextBox>
                                        <asp:RequiredFieldValidator ID="PasswordRequired" runat="server" ControlToValidate="Password"
                                            ErrorMessage="Password is required." ToolTip="Password is required." ValidationGroup="CreateUserForm">*</asp:RequiredFieldValidator>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <asp:Label ID="ConfirmPasswordLabel" runat="server" AssociatedControlID="ConfirmPassword">
                                            <%=RM.GetString("GENERAL_CONFIRM_PASSWORD_LABEL")%>:</asp:Label></td>
                                    <td>
                                        <asp:TextBox ID="ConfirmPassword" runat="server" Width="300" TextMode="Password"></asp:TextBox>
                                        <asp:RequiredFieldValidator ID="ConfirmPasswordRequired" runat="server" ControlToValidate="ConfirmPassword" 
                                            ErrorMessage="*" ToolTip='<%#RM.GetString("GENERAL_CONFIRM_PASSWORD_REQUIRED_ERROR_MESSAGE")%>' ValidationGroup="CreateUserForm"></asp:RequiredFieldValidator>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <asp:Label ID="EmailLabel" runat="server" AssociatedControlID="Email">
                                            <%=RM.GetString("ACCOUNT_NEW_EMAIL_LABEL")%>:</asp:Label></td>
                                    <td>
                                        <asp:TextBox ID="Email" Width="300" runat="server"></asp:TextBox>
                                        <asp:RequiredFieldValidator ID="EmailRequired" runat="server" ControlToValidate="Email"
                                            ErrorMessage='*' ToolTip='<%#RM.GetString("GENERAL_EMAIL_REQUIRED_ERROR_MESSAGE") %>' Display="Dynamic" ValidationGroup="CreateUserForm">
                                            </asp:RequiredFieldValidator><asp:RegularExpressionValidator Runat="server" ID="EmailValidator" Display="Dynamic" ControlToValidate="Email" 
									        ErrorMessage='*' ToolTip='<%#RM.GetString("ERROR_INVALID_EMAIL")%>' ValidationExpression="[\w\.-]+(\+[\w-]*)?@([\w-]+\.)+[\w-]+" ValidationGroup="CreateUserForm"></asp:RegularExpressionValidator>
    
                                    </td>
                                </tr>
                                <tr>
                                    <td align="center" colspan="2">
                                        <asp:CompareValidator ID="PasswordCompare" runat="server" ControlToCompare="Password"
                                            ControlToValidate="ConfirmPassword" Display="Dynamic" ErrorMessage='<%#RM.GetString("GENERAL_CONFIRM_PASSWORD_ERROR_MESSAGE")%>'
                                            ValidationGroup="CreateUserForm"></asp:CompareValidator>
                                    </td>
                                </tr>
                            </table>
                            <asp:Label runat="server" ForeColor="red" ID="ErrorMessage"></asp:Label>
                        </ContentTemplate>
                    </asp:CreateUserWizardStep>
                    <asp:CompleteWizardStep runat="server" ID="CompleteWizardStep1" Title="">
                        <ContentTemplate>
                        <table border="0" width="600">
                            <tr>
                                <td colspan="2">
                                    <h1>
                                        <%=RM.GetString("ACCOUNT_REGISTER_SUCCESS_H1")%>
                                    </h1>                            
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:Label runat="server" ID="AccountCreated1" Text='<%#RM.GetString("ACCOUNT_REGISTER_SUCCESS_LABEL1")%>'></asp:Label>
                                    <asp:Label runat="server" ID="AccountCreatedEmailValidation1" Text='<%#RM.GetString("ACCOUNT_REGISTER_SUCCESS_EMAILVALIDATION_LABEL1")%>' Visible="false"></asp:Label>
                                    <br /><br />
                                    <asp:Label runat="server" ID="AccountCreated2" Text='<%#RM.GetString("ACCOUNT_REGISTER_SUCCESS_LABEL2")%>'></asp:Label>
                                    <asp:Label runat="server" ID="AccountCreatedEmailValidation2" Text='<%#RM.GetString("ACCOUNT_REGISTER_SUCCESS_EMAILVALIDATION_LABEL2")%>' Visible="false"></asp:Label>
                                    <br /><br />    
                                    <ul>
                                        <li><b><%=RM.GetString("ACCOUNT_REGISTER_LI5")%></b></li><br />
                                        <%=RM.GetString("ACCOUNT_REGISTER_LI5_TEXT")%><br /><br />

                                        <li><b><%=RM.GetString("ACCOUNT_REGISTER_LI6")%></b></li><br />
                                        <%=RM.GetString("ACCOUNT_REGISTER_LI6_TEXT")%><br /><br />

                                        <li><b><%=RM.GetString("ACCOUNT_REGISTER_LI7")%></b></li><br />
                                        <%=RM.GetString("ACCOUNT_REGISTER_LI7_TEXT")%><br /><br />

                                        <li><b><%=RM.GetString("ACCOUNT_REGISTER_LI8")%></b></li><br />
                                        <%=RM.GetString("ACCOUNT_REGISTER_LI8_TEXT")%>
                                    </ul>
                                </td>
                            </tr>
                            <tr>
                                <td align="right" colspan="2">
                                    <asp:Button ID="ContinueButton" runat="server" CausesValidation="False" CommandName="Continue" Text='<%#RM.GetString("ACCOUNT_REGISTRATION_CONTINUE_BUTTON_LABEL")%>' ValidationGroup="CreateUserWizard1" />
                                </td>
                            </tr>
                        </table>
                    </ContentTemplate>

                    </asp:CompleteWizardStep>
                </WizardSteps>
            </asp:CreateUserWizard>
            </asp:Panel>
        </td>
    </tr>
</table>
