<%@ Control Language="C#" AutoEventWireup="true"
    Inherits="Mediachase.Commerce.Manager.Profile.Tabs.PrincipalOverviewEditTab" Codebehind="PrincipalOverviewEditTab.ascx.cs" %>
<%@ Register Src="~/Apps/Core/Controls/BooleanEditControl.ascx" TagName="BooleanEditControl" TagPrefix="ecf" %>
<div id="DataForm">
    <table width="100%">
        <tr>
            <td valign="top">
                <table class="DataForm">
                    <tr>
                        <td colspan="2" class="FormSectionCell">
                            <asp:Literal ID="Literal1" runat="server" Text="<%$ Resources:ProfileStrings, Customer_Info_Section %>"/></td>
                    </tr>
                    <tr>
                        <td class="FormLabelCell">
                            <asp:Label ID="Label1" runat="server" Text="<%$ Resources:SharedStrings, Name %>"></asp:Label>:</td>
                        <td class="FormFieldCell">
                            <asp:TextBox runat="server" Width="250" ID="Name"></asp:TextBox><br />
                            <asp:RequiredFieldValidator runat="server" ID="NameRequired" ControlToValidate="Name"
                                Display="Dynamic" ErrorMessage="<%$ Resources:SharedStrings, Name_Required %>" />
                            <ajaxToolkit:ValidatorCalloutExtender runat="Server" ID="NameRequiredE" Width="200"
                                TargetControlID="NameRequired" HighlightCssClass="FormHighlight" />
                            <asp:CustomValidator runat="server" ID="CustomerNameCustomValidator" ControlToValidate="Name"
                               OnServerValidate="CustomerNameCheck" Display="Dynamic" 
                               ErrorMessage="<%$ Resources:SharedStrings, Customer_With_Name_Exists %>" />
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2" class="FormSpacerCell">
                        </td>
                    </tr>
                    <tr>
                        <td class="FormLabelCell">
                            <asp:Label ID="Label4" runat="server" Text="<%$ Resources:SharedStrings, Organization %>"></asp:Label>:</td>
                        <td class="FormFieldCell">
                            <ComponentArt:ComboBox ID="OrganizationFilter" TextBoxEnabled="false" runat="server"
                                RunningMode="Callback" DropDownPageSize="10" Width="250">
                            </ComponentArt:ComboBox>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2" class="FormSpacerCell">
                        </td>
                    </tr>
                    <tr>
                        <td class="FormLabelCell">
                            <asp:Label runat="server" ID="CommentLabel" Text="<%$ Resources:SharedStrings, Comment %>"></asp:Label>:</td>
                        <td class="FormFieldCell">
                            <asp:TextBox TextMode="MultiLine" runat="server" ID="CommentTextBox" Rows="3" Columns="100"></asp:TextBox></td>
                    </tr>
                    <tr>
                        <td colspan="2" class="FormSpacerCell">
                        </td>
                    </tr>
                    <tr>
                        <td class="FormLabelCell">
                            <asp:Label ID="Label3" runat="server" Text="<%$ Resources:SharedStrings, Status %>"></asp:Label>:</td>
                        <td class="FormFieldCell">
                            <asp:DropDownList runat="server" ID="AccountState" DataTextField="FriendlyName" DataValueField="Name">
                                <asp:ListItem Value="" Text="<%$ Resources:SharedStrings, not_set_parenthesized %>"></asp:ListItem>
                                <asp:ListItem Value="1" Text="<%$ Resources:SharedStrings, Pending %>"></asp:ListItem>
                                <asp:ListItem Value="2" Text="<%$ Resources:SharedStrings, Active %>"></asp:ListItem>
                                <asp:ListItem Value="3" Text="<%$ Resources:SharedStrings, Suspended %>"></asp:ListItem>
                            </asp:DropDownList>
                            <asp:RequiredFieldValidator runat="server" ID="AccountStateRequiredValidator" ControlToValidate="AccountState"
                                Display="Dynamic" ErrorMessage="<%$ Resources:SharedStrings, Select_Status_Bold %>" />
                            <br />
                            <asp:Label ID="Label15" CssClass="FormFieldDescription" runat="server" 
                                Text="<%$ Resources:ProfileStrings, Status_Description %>"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2" class="FormSpacerCell">
                        </td>
                    </tr>
                    <tr>
                        <td class="FormLabelCell">
                            <asp:Label ID="Label2" runat="server" Text="<%$ Resources:SharedStrings, Customer_Group %>"></asp:Label>:</td>
                        <td class="FormFieldCell">
                            <asp:TextBox runat="server" ID="CustomerGroup" Width="200"></asp:TextBox><br />
                            <asp:Label ID="Label16" CssClass="FormFieldDescription" runat="server" 
                                Text="<%$ Resources:ProfileStrings, Group_Description %>"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                            <td colspan="2">
                                &nbsp;</td>
                        </tr>
                        <tr>
                            <td colspan="2" class="FormSectionCell">
                                <asp:Literal ID="Literal2" runat="server" Text="<%$ Resources:ProfileStrings, Security_Account_Section %>"/>
                            </td>
                        </tr>
                        <tr>
                            <td class="FormLabelCell">
                                <asp:Label runat="server" ID="UserNameLabel" Text="<%$ Resources:SharedStrings, User_Name %>"></asp:Label>:</td>
                            <td class="FormFieldCell">
                                <asp:TextBox runat="server" ID="UserNameTextBox" Width="200"></asp:TextBox>
                                <asp:RequiredFieldValidator runat="server" ID="UserNameRequired" ControlToValidate="UserNameTextBox"
                                    Display="None" ErrorMessage="<%$ Resources:SharedStrings, User_Name_Required %>" />
                                <ajaxToolkit:ValidatorCalloutExtender runat="Server" ID="ValidatorCalloutExtender1" Width="200"
                                    TargetControlID="UserNameRequired" HighlightCssClass="FormHighlight" />
                                <asp:CustomValidator runat="server" ID="UserNameCustomValidator" ControlToValidate="UserNameTextBox"
                                   OnServerValidate="UserNameCheck" Display="Dynamic" 
                                   ErrorMessage="<%$ Resources:SharedStrings, User_With_Name_Exists %>" />
                            </td>
                        </tr>
                        <tr>
                            <td colspan="2" class="FormSpacerCell">
                            </td>
                        </tr>
                        <tr runat="server" id="PasswordTr">
                            <td class="FormLabelCell">
                                <asp:Label runat="server" ID="Label9" Text="<%$ Resources:SharedStrings, Password %>"></asp:Label>:</td>
                            <td class="FormFieldCell">
                                <asp:TextBox runat="server" ID="PasswordTextBox" TextMode="Password" Width="200"></asp:TextBox></td>
                        </tr>
                        <tr>
                            <td colspan="2" class="FormSpacerCell">
                            </td>
                        </tr>
                        <tr>
                            <td class="FormLabelCell">
                                <asp:Label ID="Label14" runat="server" Text="<%$ Resources:SharedStrings, Description %>"></asp:Label>:</td>
                            <td class="FormFieldCell">
                                <asp:TextBox runat="server" Width="250" ID="Description"></asp:TextBox><br />
                            </td>
                        </tr>
                        <tr>
                            <td colspan="2" class="FormSpacerCell">
                            </td>
                        </tr>
                        <tr>
                            <td class="FormLabelCell">
                                <asp:Label runat="server" ID="Label5" Text="<%$ Resources:SharedStrings, Email %>"></asp:Label>:</td>
                            <td class="FormFieldCell">
                                <asp:TextBox runat="server" ID="EmailText" Width="200"></asp:TextBox></td>
                        </tr>
                        <tr>
                            <td colspan="2" class="FormSpacerCell">
                            </td>
                        </tr>
                        <tr>
                            <td class="FormLabelCell">
                                <asp:Label runat="server" ID="Label6" Text="<%$ Resources:SharedStrings, Approved %>"></asp:Label>:</td>
                            <td class="FormFieldCell">
                                <ecf:BooleanEditControl ID="IsApproved" runat="server"></ecf:BooleanEditControl>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="2" class="FormSpacerCell">
                            </td>
                        </tr>
                        <tr>
                            <td class="FormLabelCell">
                                <asp:Label runat="server" ID="Label7" Text="<%$ Resources:SharedStrings, Locked_Out %>"></asp:Label>:</td>
                            <td class="FormFieldCell">
                                <ecf:BooleanEditControl ID="IsLockedOut" runat="server"></ecf:BooleanEditControl>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="2" class="FormSpacerCell">
                            </td>
                        </tr>
                        <tr>
                            <td class="FormLabelCell">
                                <asp:Label runat="server" ID="Label10" Text="<%$ Resources:SharedStrings, Roles %>"></asp:Label>:</td>
                            <td class="FormFieldCell">
                                <asp:CheckBoxList runat="server" ID="RolesList" RepeatColumns="3">
                                </asp:CheckBoxList>
                            </td>
                        </tr>
                        <tr>
                            <tr>
                                <td colspan="2" class="FormSpacerCell">
                                </td>
                            </tr>
                            <tr>
                                <td class="FormLabelCell">
                                    <asp:Label runat="server" ID="Label8" Text="<%$ Resources:SharedStrings, Last_Activity %>"></asp:Label>:</td>
                                <td class="FormLabelCell">
                                    <asp:Label runat="server" ID="LastActivityDate"></asp:Label></td>
                            </tr>
                            <tr>
                                <td colspan="2" class="FormSpacerCell">
                                </td>
                            </tr>
                            <tr>
                                <td class="FormLabelCell">
                                    <asp:Label runat="server" ID="Label11" Text="<%$ Resources:SharedStrings, Last_Lockout %>"></asp:Label>:</td>
                                <td class="FormLabelCell">
                                    <asp:Label runat="server" ID="LastLockoutDate"></asp:Label></td>
                            </tr>
                            <tr>
                                <td colspan="2" class="FormSpacerCell">
                                </td>
                            </tr>
                            <tr>
                                <td class="FormLabelCell">
                                    <asp:Label runat="server" ID="Label12" Text="<%$ Resources:SharedStrings, Last_Login %>"></asp:Label>:</td>
                                <td class="FormLabelCell">
                                    <asp:Label runat="server" ID="LastLoginDate"></asp:Label></td>
                            </tr>
                            <tr>
                                <td colspan="2" class="FormSpacerCell">
                                </td>
                            </tr>
                            <tr>
                                <td class="FormLabelCell">
                                    <asp:Label runat="server" ID="Label13" Text="<%$ Resources:SharedStrings, Last_Password_Changed %>"></asp:Label>:</td>
                                <td class="FormLabelCell">
                                    <asp:Label runat="server" ID="LastPasswordChangedDate"></asp:Label></td>
                            </tr>
                </table>
            </td>
            <td valign="top" runat="server" id="ActionTd">
                <table class="DataForm">
                    <tr>
                        <td colspan="2" class="FormSectionCell">
                            <asp:Literal ID="Literal3" runat="server" Text="<%$ Resources:ProfileStrings, Perform_Account_Actions %>"/>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2">
                            <asp:ChangePassword runat="server" ID="PasswordCtrl" CancelButtonText="" 
                                CancelButtonType="Link" ContinueButtonText="" ContinueButtonType="Link">
                            </asp:ChangePassword>
                            
                            <asp:PasswordRecovery runat="server" ID="RecoveryCtrl">
                                <UserNameTemplate>
                                    <table border="0" cellpadding="1" cellspacing="0" style="border-collapse: collapse">
                                        <tr>
                                            <td>
                                                <table border="0" cellpadding="0">
                                                    <tr>
                                                        <td align="right" style="height: 24px">
                                                            <asp:Label ID="UserNameLabel" runat="server" AssociatedControlID="UserName" 
                                                                Visible="false" Text="<%$ Resources:ProfileStrings, User_Name_Colon %>"></asp:Label>
                                                        </td>
                                                        <td style="height: 24px">
                                                            <asp:TextBox ID="UserName" runat="server" Visible="false"></asp:TextBox>
                                                            <asp:RequiredFieldValidator ID="UserNameRequired" runat="server" 
                                                                Visible="false" ControlToValidate="UserName"
                                                                ErrorMessage="<%$ Resources:SharedStrings, User_Name_Is_Required %>" 
                                                                ToolTip="<%$ Resources:SharedStrings, User_Name_Is_Required %>" 
                                                                ValidationGroup="ctl00$RecoveryCtrl">*</asp:RequiredFieldValidator>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td align="center" colspan="2" style="color: red">
                                                            <asp:Literal ID="FailureText" runat="server" EnableViewState="False"></asp:Literal>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td align="right" colspan="2">
                                                            <asp:LinkButton ID="SubmitLinkButton" runat="server" CommandName="Submit" 
                                                                ValidationGroup="ctl00$PasswordRecovery" 
                                                                Text="<%$ Resources:ProfileStrings, Recover_Password %>"></asp:LinkButton>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </td>
                                        </tr>
                                    </table>
                                </UserNameTemplate>
                            </asp:PasswordRecovery>
                            <br />
                            &nbsp;&nbsp;&nbsp;<asp:HyperLink runat="server" ID="LoginOnBehalf" Target="_blank" Text="<%$ Resources:SharedStrings, Login_As_Customer %>"></asp:HyperLink>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
</div>
