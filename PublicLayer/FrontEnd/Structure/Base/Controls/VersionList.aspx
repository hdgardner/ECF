<%@ Page Language="C#" AutoEventWireup="true" Inherits="Controls_VersionList" Codebehind="VersionList.aspx.cs" %>
<%@ Register TagPrefix="dg" Namespace="Mediachase.Web.UI" Assembly="Mediachase.Cms.WebUtility" %>
<%@ Register TagPrefix="mc" TagName="LanguageBar" Src="~/Structure/Base/Controls/ToolBar/LanguageBar.ascx" %>
<%@ Register TagPrefix="mc" TagName="CultureHolder" Src="~/Structure/Base/Controls/Common/CultureHolder.ascx" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Mediachase ECF 5.0 - Version List</title>
    <link rel="stylesheet" href='common/GridStyle.css' />
</head>
<body>
    <form id="form1" runat="server"> 
     <mc:CultureHolder runat="server" ID="cultureHolder" />
    <div style="padding:10px;" >
    <table cellspacing="0" cellpadding="0" width="100%" border="0" >
        <tr>
            <td valign="bottom">
                <asp:Image ID="Image1" runat="server" ImageUrl="~/Images/leftCorner_w.gif"
                    Width="11px" Height="20px" /></td>
            <th background='<%=Mediachase.Commerce.Shared.CommerceHelper.GetAbsolutePath("/Images/line_w.gif")%>'>
               <nobr>Available Version List</nobr>
            </th>
            <td background='<%=Mediachase.Commerce.Shared.CommerceHelper.GetAbsolutePath("/Images/line_w.gif")%>'
                width="100%" style="background-repeat: repeat-x; background-position-y: center;">
            </td>
            <td background='<%=Mediachase.Commerce.Shared.CommerceHelper.GetAbsolutePath("/Images/line_w.gif")%>'
                style="background-repeat: repeat-x; background-position-y: center;">
                <mc:LanguageBar runat="server" ID="LanguageBar1" />
            </td>
            <td valign="bottom">
                <asp:Image runat="server" ImageUrl="~/Images/RightCorner_w.gif"
                    Width="11px" Height="20px" /></td>
        </tr>
    </table>
    <table class="blockheader-light" cellspacing="0" cellpadding="5" width="100%">
        <tr valign="top">
            <td>
                <dg:DataGridExtended ID="grdMain" runat="server" AllowCustomPaging="false" AllowPaging="false" AllowSorting="false"
                    Width="100%" AutoGenerateColumns="False" BorderWidth="0" GridLines="None" CellPadding="1"
                    LayoutFixed="false" >
                    <Columns>
                        <asp:TemplateColumn Visible="true">
                            <headerstyle cssclass="DicHeader" horizontalalign="Left"></headerstyle>
                            <itemstyle cssclass="DicItem" horizontalalign="Left"></itemstyle>
                            <headertemplate>
                                Version
                            </headertemplate>
                            <itemtemplate>
								#<%# Eval("VersionNum")%>
							</itemtemplate>
                        </asp:TemplateColumn>
                        <asp:TemplateColumn Visible="true">
                            <headerstyle cssclass="DicHeader" horizontalalign="Left"></headerstyle>
                            <itemstyle cssclass="DicItem" horizontalalign="Left"></itemstyle>
                            <headertemplate>
                                Status
                            </headertemplate>
                            <itemtemplate>
								<%# GetStatusName((int)Eval("StatusId"))%>
							</itemtemplate>
                        </asp:TemplateColumn>
                        <asp:TemplateColumn Visible="true">
                            <headerstyle cssclass="DicHeader" horizontalalign="Left"></headerstyle>
                            <itemstyle cssclass="DicItem" horizontalalign="Left"></itemstyle>
                            <headertemplate>
                                User
                            </headertemplate>
                            <itemtemplate>
								<%# Mediachase.Cms.Util.CommonHelper.GetUserName((Guid)DataBinder.Eval(Container.DataItem, "EditorUID"))%>
							</itemtemplate>
                        </asp:TemplateColumn>
                        <asp:TemplateColumn Visible="true">
                            <headerstyle cssclass="DicHeader" horizontalalign="Left"></headerstyle>
                            <itemstyle cssclass="DicItem" horizontalalign="Left"></itemstyle>
                            <headertemplate>
                                Last Change
                            </headertemplate>
                            <itemtemplate>
								<%# ((DateTime)DataBinder.Eval(Container.DataItem, "Edited")).ToString()%>
							</itemtemplate>
                        </asp:TemplateColumn>
                        <asp:TemplateColumn HeaderText="Link">
                            <headerstyle cssclass="DicHeader" width="150px" horizontalalign="Center"></headerstyle>
                            <itemstyle cssclass="DicItem" horizontalalign="Center"></itemstyle>
                            <itemtemplate>
                                <asp:Hyperlink runat="server" ImageUrl='<%# Mediachase.Commerce.Shared.CommerceHelper.GetAbsolutePath("/Images/go.gif")%>' NavigateUrl='<%#GetPageUrl((int)DataBinder.Eval(Container.DataItem,"VersionId")) %>'></asp:Hyperlink>
							</itemtemplate>
                        </asp:TemplateColumn>
                    </Columns>
                </dg:DataGridExtended>
            </td>
        </tr>
    </table>
    </div>
    </form>
</body>
</html>
