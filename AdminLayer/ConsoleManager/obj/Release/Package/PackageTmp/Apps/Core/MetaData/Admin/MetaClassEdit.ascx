<%@ Control Language="c#" Inherits="Mediachase.Commerce.Manager.Core.MetaData.Admin.MetaClassEdit" Codebehind="MetaClassEdit.ascx.cs" %>
<%@ Register Src="~/Apps/Core/SaveControl.ascx" TagName="SaveControl" TagPrefix="ecf" %>
<div id="FormMultiPage">
    <table class="FormMultiPage">
        <tr>
            <td class="FormLabelCell">
                <table>
                    <tr>
                        <td class="FormLabelCell" style="vertical-align: middle; width: 120px">
                            <%=RM.GetString("ATTRIBUTECLASSEDIT_NAME")%>:</td>
                        <td class="FormFieldCell">
                            <asp:TextBox ID="Name" Width="400" runat="server" MaxLength="256"></asp:TextBox><br />
                            <asp:RequiredFieldValidator ID="NameRequired" ControlToValidate="Name" Display="None"
                                Font-Name="verdana" Font-Size="9pt" ErrorMessage="<%$ Resources:SharedStrings, Name_Required %>"
                                runat="server" />
                            <ajaxToolkit:ValidatorCalloutExtender runat="server" ID="vceNameRequired" Width="220"
                                TargetControlID="NameRequired" HighlightCssClass="FormHighlight" />
                            <asp:RegularExpressionValidator ID="NameRegularExpression" Display="None" runat="server"
                                ErrorMessage="<%$ Resources:SharedStrings, Latin_Symbols_Only %>"
                                ValidationExpression="^[a-zA-Z_\d]*" ControlToValidate="Name" />
                            <ajaxToolkit:ValidatorCalloutExtender runat="server" ID="vceNameRegularExpression"
                                Width="220" TargetControlID="NameRegularExpression" HighlightCssClass="FormHighlight" />
                            <asp:CustomValidator runat="server" ID="NameCheckCustomValidator" ControlToValidate="Name"
                                OnServerValidate="MetaClassNameCheck" Display="Dynamic" ErrorMessage="<%$ Resources:CoreStrings, MetaData_Error_CreateMetaClass_AlreadyExists %>" />    
                        </td>
                    </tr>
                    <tr>
                        <td class="FormLabelCell" style="vertical-align: middle; width: 120px">
                            <%=RM.GetString("ATTRIBUTECLASSEDIT_FRIENDLYNAME")%>:
                        </td>
                        <td class="FormFieldCell">
                            <asp:TextBox ID="FriendlyName" Width="400" runat="server" MaxLength="256"></asp:TextBox></br>
                            <asp:RequiredFieldValidator ID="RequiredFieldValidatorFriendlyName" ControlToValidate="FriendlyName"
                                Display="dynamic" Font-Name="verdana" Font-Size="9pt" ErrorMessage="<%$ Resources:SharedStrings, Friendly_Name_Required %>" runat="server" />
                            <ajaxToolkit:ValidatorCalloutExtender runat="server" ID="vceRequiredFieldValidatorFriendlyName"
                                Width="220" TargetControlID="RequiredFieldValidatorFriendlyName" HighlightCssClass="FormHighlight" />
                            <asp:CustomValidator runat="server" ID="FriendlyNameCheckCustomValidator" ControlToValidate="FriendlyName"
                                OnServerValidate="MetaClassFriendlyNameCheck" Display="Dynamic" ErrorMessage="<%$ Resources:CoreStrings, MetaData_Error_CreateMetaClass_FriendlyName_AlreadyExists %>" />        
                        </td>
                    </tr>
                    <tr>
                        <td class="FormLabelCell" style="vertical-align: top; width: 120px">
                            <%=RM.GetString("ATTRIBUTECLASSEDIT_DESCRIPTION")%>:</td>
                        <td class="FormFieldCell">
                            <asp:TextBox ID="Description" TextMode="MultiLine" Columns="80" Rows="8" Width="400"
                                runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="FormLabelCell" style="vertical-align: middle; width: 120px">
                            <%=RM.GetString("ATTRIBUTECLASSEDIT_OBJECT_TYPE")%>:</td>
                        <td class="FormFieldCell">
                            <asp:DropDownList ID="DdlObjectType" runat="server">
                            </asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2">
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
</div>
<ecf:SaveControl ID="EditSaveControl" ShowDeleteButton="false" runat="server" SavedClientScript="CSManagementClient.CloseTab();CSManagementClient.OpenTran('List', 'namespace='+CSManagementClient.QueryString('namespace')+'&fieldnamespace='+CSManagementClient.QueryString('fieldnamespace')+'&mfview='+CSManagementClient.QueryString('mfview'));"
    DeleteClientScript="CSManagementClient.CloseTab();CSManagementClient.OpenTran('List', 'namespace='+CSManagementClient.QueryString('namespace')+'&fieldnamespace='+CSManagementClient.QueryString('fieldnamespace')+'&mfview='+CSManagementClient.QueryString('mfview'));"
    CancelClientScript="CSManagementClient.CloseTab();CSManagementClient.OpenTran('List', 'namespace='+CSManagementClient.QueryString('namespace')+'&fieldnamespace='+CSManagementClient.QueryString('fieldnamespace')+'&mfview='+CSManagementClient.QueryString('mfview'));">
</ecf:SaveControl>