<%@ Control Language="C#" AutoEventWireup="true" Inherits="Mediachase.Commerce.Manager.Core.LicensingControl" Codebehind="LicensingControl.ascx.cs" %>
<%@ Register Assembly="Mediachase.FileUploader" Namespace="Mediachase.FileUploader.Web.UI" TagPrefix="mcf" %>
<script type="text/javascript" language="javascript">
function LicenseDeleteWarning()
{
  var text = '<%= Resources.CommerceManager.LICENSE_DELETE_WARNING %>';
  if(!confirm(text)) return false;
}
</script>
<div style=" width:98%; height:98%; margin: 1% 0 0 1%;">
<div class="CommonContent">
    <table width="100%" cellpadding="0" cellspacing="0" border="0">
        <thead>
        <tr>
            <td class="CommonListHeaderLeftMost" width="300" style="height: 28px">
	            <asp:Literal ID="Literal1" runat="server" Text="<%$ Resources:CoreStrings, License_Installation %>"/>
            </td>
        </tr>
        </thead>
        <tr>
            <td class="CommonListCellLeftMost">
                <asp:Literal ID="Literal2" runat="server" Text="<%$ Resources:CoreStrings, License_Install_File %>"/>:<br />
                <mcf:mchtmlinputfile id="mcHtmlInputFile" runat="server" maxlength="-1" multifileupload="False" name="McHtmlInputFile1" size="-1" type="file" width="404px"/>
                <input id="Submit1" type="submit" value="<%$ Resources:SharedStrings, Install %>" runat="server" style="width: 100px;" onserverclick="btnUpload_ServerClick" />
            </td>
        </tr>
        <tr>
            <td class="CommonListCellLeftMost">
                <asp:Literal ID="Literal3" runat="server" Text="<%$ Resources:CoreStrings, License_Install_By_Key %>"/>:<br />
                <asp:TextBox runat="server" ID="tbLicenseKey" Width="400"/>
                <asp:Button runat="server" ID="btnDownloadLicense" onclick="btnDownloadLicense_Click" Text="<%$ Resources:SharedStrings, Install %>" Width="100" />
            </td>
        </tr>
        <tr>
            <td class="CommonListCellLeftMost">
                <asp:Label runat="server" ID="lblHostID"/>
            </td>
        </tr>
    </table>
</div>
<br />    
<asp:DataList runat="server" ID="dlLicenseList" RepeatColumns="1" Width="100%">
    <ItemTemplate>
    <div class="CommonContent">    
        <table cellspacing="0" cellpadding="0" border="0" width="100%">
		    <thead>
		    <tr>
			    <td class="CommonListHeaderLeftMost" width="300" style="height: 28px">
				    <%# DataBinder.Eval(Container.DataItem, "Product") %> &nbsp;
			    </td>
			    <td class="CommonListHeaderLeftMost" style=" text-align:right;">
			        <asp:ImageButton ImageAlign="Right" ID="btnDeleteLicense" runat="server" 
                        ImageUrl="../images/delete.png"  
                        CommandArgument='<%# Eval("Id") %>' onclick="DeleteLicense_Click" 
                        OnClientClick="return LicenseDeleteWarning()" />
			    </td>
		    </tr>
		    </thead>
		    <tbody>
            <tr>
                <td class="CommonListCellLeftMost" valign="top">
                    <b><asp:Literal ID="Literal4" runat="server" Text="<%$ Resources:CoreStrings, License_Licensed_To %>"/>:</b>
                    <div style="padding-left:14px;">
                        <asp:Literal ID="Literal9" runat="server" Text="<%$ Resources:SharedStrings, User_Name %>"/>: <%# DataBinder.Eval(Container.DataItem, "Customer") %><br/>
                        <asp:Literal ID="Literal10" runat="server" Text="<%$ Resources:SharedStrings, E_mail %>"/>: <%# DataBinder.Eval(Container.DataItem, "Email") %><br/>
                        <asp:Literal ID="Literal11" runat="server" Text="<%$ Resources:SharedStrings, Company %>"/>: <%# DataBinder.Eval(Container.DataItem, "Company") %><br/>
                    </div>
                    <br />
                    <b><asp:Literal ID="Literal5" runat="server" Text="<%$ Resources:CoreStrings, License_Activated %>"/>:</b> <%# DataBinder.Eval(Container.DataItem, "Activated")%><br/>
                    <b><asp:Literal ID="Literal6" runat="server" Text="<%$ Resources:CoreStrings, License_Expires %>"/>:</b> <%# DataBinder.Eval(Container.DataItem, "Expires")%><br/>
                </td>
                <td class="CommonListCellLeftMost" valign="top">
                    <b><asp:Literal ID="Literal7" runat="server" Text="<%$ Resources:CoreStrings, License_Edition %>"/>:</b> <%# DataBinder.Eval(Container.DataItem, "Edition") %><br/>
                    <asp:Repeater ID="Repeater1" runat="server" DataSource='<%# DataBinder.Eval(Container.DataItem, "Components")%>'>
                        <ItemTemplate>
                            <br />
                            <b><asp:Literal ID="Literal8" runat="server" Text="<%$ Resources:CoreStrings, License_Component %>"/>:</b> <%# DataBinder.Eval(Container.DataItem, "Key")%>
                            <asp:Repeater ID="Repeater1" runat="server" DataSource='<%# DataBinder.Eval(Container.DataItem, "Value")%>'>
                                <ItemTemplate>
                                    <div style="padding-left:14px;">
                                    <%# DataBinder.Eval(Container.DataItem, "Key")%>: <%# DataBinder.Eval(Container.DataItem, "Value")%>
                                    </div>
                                </ItemTemplate>
                            </asp:Repeater>
                        </ItemTemplate>
                    </asp:Repeater>
                </td>
            </tr>
            </tbody>
        </table>
    </ItemTemplate>
    </asp:DataList>
<%--There are no licenses installed or available.--%>
</div>
        
    