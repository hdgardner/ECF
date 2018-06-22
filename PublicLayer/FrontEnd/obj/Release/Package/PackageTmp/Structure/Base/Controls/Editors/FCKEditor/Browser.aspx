<%@ Page Language="C#" AutoEventWireup="true" Inherits="Browser" Theme="" EnableEventValidation="true" Codebehind="Browser.aspx.cs" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Image galery</title>
   <script type="text/javascript" language="JavaScript">
		function _hover(objDiv, cbId)
		{
			var objCb = document.getElementById(cbId);
			if(objCb)
			{
				if(!objCb.checked)
					objDiv.className='main_span_style_hover';
			}
			else
			{
				objDiv.className='main_span_style_hover';
			}
		}
		
		function _out(objDiv, cbId)
		{
			var objCb = document.getElementById(cbId);
			if(objCb)
			{
				if(!objCb.checked)
				{
					objDiv.className='main_span_style';
				}
			}
			else
				objDiv.className='main_span_style';
		}
		function SelectUpload()
		{
		    var tabUpload = document.getElementById('UploadTab');
		    tabUpload.className = 'TabSelected';	
		    
		    var tabCreateFolder = document.getElementById('CreateFolderTab');
		    tabCreateFolder.className = 'TabDeSelected';
		    
		    var divUpload = document.getElementById('FileUploader');
		    divUpload.className = 'FileUploader';		
		    
		    var divCreateFolder = document.getElementById('FoderCreate');
		    divCreateFolder.className = 'DivDeSelected';		
		}
		function SelectCreateFolder()
		{
		    var tabUpload = document.getElementById('UploadTab');
		    tabUpload.className = 'TabDeSelected';	
		    
		    var tabCreateFolder = document.getElementById('CreateFolderTab');
		    tabCreateFolder.className = 'TabSelected';
		    
		    var divUpload = document.getElementById('FileUploader');
		    divUpload.className = 'DivDeSelected';		
		    
		    var divCreateFolder = document.getElementById('FoderCreate');
		    divCreateFolder.className = 'FileUploader';	
		}
		
		function AreaClick(sender)
		{
		    var obj = document.getElementById(sender.id + "_a");
		    if(obj != null)
		    {
		        window.location.href = obj.href;
		    }
		    else
		    {
		       var obj2 = document.getElementById(sender.id + "_aa"); 
		       if(obj2 != null)
		       {
		            obj2.click();
		       }
		    }
		}
    </script>
    
    <style type="text/css">
		.main_span_style{width:230px;height:140px;float:left;margin:5px;}
		.main_span_style_hover{width:230px;height:140px;float:left;margin:5px;background-color:#EAEAEA;}
		.main_span_style_selected{width:230px;height:140px;float:left;margin:5px;background-color:#E5E5FF;}
		.main_style_img{width:90px; height:90px;border:1px solid #bbbbbb; text-align:center; vertical-align:middle;}
		.FileBrowser a, a:hover, a:visited 
		{
		    text-decoration: none;
		    color:#003399;
		}
		
		.Tabs a, a:hover, a:visited 
		{
		    text-decoration: none;
		    color:#003399;
		}
		
		.FileUploader
		{
		    background-color:#f7f8fd;
		}
		.FileUploader input, button
		{
		    background-color:#8cb2fd;
		    color:#0e3460;
		    border:2 px solid #466cac;
		}
		.TabSelected
		{
		    background-color: #f7f8fd;
		    font-weight: bold;
		    font-size: 10pt;
		    color: #0e3460;
		    font-family:Tahoma;
		    border-top:solid 1px #466cac;
		    border-left:solid 1px #466cac;
		    border-right:solid 1px #466cac;
		    border-bottom:solid 1px #f7f8fd;
		}
		.TabDeSelected
		{
		    background-color: #8cb2fd;
		    font-weight: normal;
		    font-size: 10pt;
		    color: #0e3460;
		    font-family:Tahoma;
		    border-top:solid 1px #466cac;
		    border-left:solid 1px #466cac;
		    border-right:solid 1px #466cac;
		    border-bottom:solid 1px #466cac;
		}
		
		.DivSelected
		{
		    display: inline;
		}
		
		.DivDeSelected
		{
		    display: none;
		}
	</style>
</head>
<body style="padding:0px;margin:0px;background-color:#f7f8fd">
    <form id="form1" runat="server">
        <div style="font-weight: bold;font-size: 14pt;color: #0e3460;background-color: #8cb2fd;padding: 3px 10px 3px 10px;font-family:Tahoma;">
            File browser: <asp:Label runat="server" ID="lbPath"></asp:Label>
        </div>
        <div style="width:100%;background-color:#f7f8fd;height:450px;overflow-y:scroll;border-top:2px solid #466cac; border-bottom:2px solid #466cac;">
            <table cellspacing="0" cellpadding="0" border="0" width="100%" bgcolor="#f7f8fd">
                <tr>
                    <td style="padding: 10 5 5 10;">
                        <asp:Repeater ID="repThumbs" runat="server" OnItemCommand="repThumbs_ItemCommand">
                            <HeaderTemplate>
                                <table width="100%" cellpadding="0" cellspacing="0" border="0" class="FileBrowser">
                                    <tr>
                                        <td>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <div id='<%#"divId_"+ DataBinder.Eval(Container.DataItem, "Id").ToString()%>'
                                    class="main_span_style" onmouseover="javascript:_hover(this, '<%#"chkItem_"+DataBinder.Eval(Container.DataItem, "Id").ToString()%>');"
                                    onmouseout="javascript:_out(this, '<%#"chkItem_"+DataBinder.Eval(Container.DataItem, "Id").ToString()%>');" onclick="AreaClick(this);" >
                                    <table border="0" cellpadding="0" cellspacing="0" style="margin: 5px;" class="text">
                                        <tr height="25px">
                                            <td width="25px">
                                                <%--<input type="checkbox" name='<%#"chkItem_"+DataBinder.Eval(Container.DataItem, "Id").ToString()%>'
                                                    id='<%#"chkItem_"+DataBinder.Eval(Container.DataItem, "Id").ToString()%>'
                                                    value='<%#DataBinder.Eval(Container.DataItem, "Id").ToString()%>'
                                                    onclick="javascript:_select(this, '<%#"divId_"+DataBinder.Eval(Container.DataItem, "Id").ToString()%>');" />--%>
                                            </td>
                                            <td colspan="2">
                                                <%#DataBinder.Eval(Container.DataItem, "Name")%>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td width="25px" valign="bottom">
                                                <%#DataBinder.Eval(Container.DataItem, "ActionVS")%>
                                                <%#DataBinder.Eval(Container.DataItem, "ActionEdit")%>
                                                <asp:ImageButton Visible='<%# (bool)DataBinder.Eval(Container.DataItem, "CanDelete")%>'
                                                    CommandArgument='<%#DataBinder.Eval(Container.DataItem, "FullPath").ToString()%>'
                                                    ID="ibDelete1" runat="server" CommandName="Delete" ImageUrl="~/Images/Delete.gif">
                                                </asp:ImageButton>
                                            </td>
                                            <td class="main_style_img">
                                                <%--<div style="overflow:hidden;width:90px; height:90px;">--%>
                                                <%# DataBinder.Eval(Container.DataItem, "BigIcon")%>
                                                <%--</div>--%>
                                            </td>
                                            <td width="105px" valign="top">
                                                <span style="margin-left: 5px;font-size:small;">
                                                    <%#((int)DataBinder.Eval(Container.DataItem, "Weight")!=0)?"<b>"+"Updated"+":</b>":""%>
                                                </span>
                                                <br>
                                                <span style="margin-left: 5px;font-size:small;">
                                                    <nobr>
                                                        <%#DataBinder.Eval(Container.DataItem, "ModifiedDate")%>
                                                    </nobr>
                                                </span>
                                                <br>
                                                <br>
                                                <span style="margin-left: 5px;font-size:small;">
                                                    <%#((int)DataBinder.Eval(Container.DataItem, "Weight")==2)?"<b>"+"Size"+":</b>":""%>
                                                </span>
                                                <br>
                                                <span style='margin-left: 5px;font-size:small;'>
                                                    <%#DataBinder.Eval(Container.DataItem, "Size")%>
                                                </span>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td colspan="3">
                                                &nbsp;</td>
                                        </tr>
                                    </table>
                                </div>
                            </ItemTemplate>
                            <FooterTemplate>
                                </td></tr></table>
                            </FooterTemplate>
                        </asp:Repeater>
                    </td>
                </tr>
            </table>
        </div>
        <div style="background-color:#8cb2fd;height:2px;overflow:hidden;"></div>
        <table width="100%" style="table-layout:fixed;font-weight: bold;font-size: 10pt;color: #0e3460;font-family:Tahoma;" cellpadding="0" cellspacing="0" class="Tabs">
            <tr>
                <td width="10px" style="background-color:#8cb2fd;border-top: solid 1px #8cb2fd;border-left: solid 1px #8cb2fd;border-right: solid 1px #8cb2fd;border-bottom: solid 1px #466cac;">
                    &nbsp;    
                </td>
                <td width="120px" align="center" class="TabSelected" id="UploadTab">
                    <a href="javascript:SelectUpload();">Upload</a> 
                </td>
                <td width="120px" align="center" class="TabDeSelected" id="CreateFolderTab">
                    <a href="javascript:SelectCreateFolder();">Create folder</a> 
                </td>
                <td width="90%" style="background-color:#8cb2fd;border-top: solid 1px #8cb2fd;border-left: solid 1px #8cb2fd;border-right: solid 1px #8cb2fd;border-bottom: solid 1px #466cac;">
                    &nbsp;
                </td>
            </tr>
        </table>
        <div align="right" id="FileUploader" class="FileUploader" style="padding: 10px 10px 10px 10px;font-family:Tahoma;">
            <asp:FileUpload ID="FileUpload1" runat="server" BackColor="#8cb2fd" BorderColor="#466cac" BorderStyle="Solid" BorderWidth="1px" ForeColor="#3B3B1F"/>
            <asp:Button ID="Button1" runat="server" Height="21px" Text="Upload..." Width="80px" BorderColor="#466cac" BorderStyle="Solid" BorderWidth="1px" ForeColor="#3B3B1F" OnClick="Button1_Click" />
            <asp:Button ID="Button2" runat="server" Height="21px" Text="Cancel" Width="80px" BorderColor="#466cac" BorderStyle="Solid" BorderWidth="1px" ForeColor="#3B3B1F" OnClientClick="window.close();" CausesValidation="false" />
        </div>
        <div align="right" id="FoderCreate" style="padding: 10px 10px 10px 10px;font-family:Tahoma;" class="DivDeSelected">
            <asp:TextBox runat="server" ID="txtFolderName" Width="150" BackColor="#8cb2fd" BorderColor="#466cac" BorderStyle="Solid" BorderWidth="1px" ForeColor="#3B3B1F"></asp:TextBox>&nbsp;<asp:Button ID="Button3" runat="server" Height="21px" Text="Create..." Width="80px" BorderColor="#466cac" BorderStyle="Solid" BorderWidth="1px" ForeColor="#3B3B1F" OnClick="Button2_Click" />
            <asp:Button ID="Button4" runat="server" Height="21px" Text="Cancel" Width="80px" BorderColor="#466cac" BorderStyle="Solid" BorderWidth="1px" ForeColor="#3B3B1F" OnClientClick="window.close();" CausesValidation="false" />
        </div>
    </form>
</body>
</html>
