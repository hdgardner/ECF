<%@ Page Language="C#" AutoEventWireup="true" Inherits="Mediachase.Cms.Controls.Editors.CuteEditor.PopUpEdit"
    ValidateRequest="false" CodeBehind="PopUpEdit.aspx.cs" %>
<%@ Register TagPrefix="CE" Namespace="CuteEditor" Assembly="CuteEditor" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>HTML Editor</title>
</head>

<script type="text/javascript">
    var PopUpFlag=false;
    var UnloadFlag=false; 
    allowClose = false;  
    
    window.onunload=ModalDivDeactivate;
    
    function ModalDivDeactivate()
    {   
        if ((window.opener==null)||(window.opener.closed)) window.close();
            
        if (!UnloadFlag)
        {
            var obj=window.opener.document.getElementById("TestMainWrapper");
            if (obj==null) return false;
            obj.style.visibility="hidden";
        }
        else
            UnloadFlag=false;
    }

    function InitEditor(id1,id2)
    {
        UnloadFlag=true;
        var objEditor = document.getElementById(id1);
        var objSource = window.opener.document.getElementById(id2);
        if (objSource != null) {        
            var objContainer = window.opener.document.getElementById(objSource.getAttribute("innerContainer"));
            
            if ((objEditor != null) && (objContainer != null)) {

                var val = objContainer.value;
                if (val == null || val.length == 0) {
                    val = objSource.innerHTML;
                }               

                objEditor.value = val;
            }
        }
        
        document.getElementById(id1).form.submit();
    }
    
    function loadEditor(id1,id2)
    {
        var objEditor=document.getElementById(id1);
        var objSource=window.opener.document.getElementById(id2);
        var objContainer = window.opener.document.getElementById(objSource.getAttribute("innerContainer"));

        if(objEditor.value=="") 
        {
            objEditor.value="Click for edit";
        }

        var valueHtml = objEditor.value;        
        objSource.innerHTML = valueHtml;
        objContainer.value = valueHtml;
    }

    function cancelEditor(id2)
    {
        var objSource=window.opener.document.getElementById(id2);
        var objContainer=window.opener.document.getElementById(objSource.getAttribute("innerContainer"));
        objContainer.value="";
    }
    
    function CloseByESC(e)
    {
        var _key = e.keyCode ? e.keyCode : e.which ? e.which : e.charCode;
        if(_key == 27)
        {
            cancelEditor('<%= Request["Object"] %>');
            window.close();
        }
    }

    
</script>

<body onkeypress="CloseByESC(event);" onload="allowClose = true;">
    <form id="form1" runat="server" method="post">
    <table class="stdTable" cellspacing="0" cellpadding="8">
        <colgroup>
            <col>
            <tbody>
                <tr>
                    <td class="header">
                        <div class="header">
                            <b>Html Editor</b></div>
                        <div class="headerdesc">
                            Allows editing Html</div>
                    </td>
                </tr>
                <tr>
                    <td class="main2" height="100%">
                        <table height="100%" cellspacing="0" cellpadding="0" width="100%">
                            <tbody>
                                <tr>
                                    <td height="100%">
                                        <div class="ItemList">
                                            <CE:Editor ID="htmlEditor" URLType="Default" FilesPath="~/Structure/Base/Controls/Editors/CuteEditor"
                                                DOCTYPE="<!DOCTYPE HTML PUBLIC '-//W3C//DTD HTML 4.01 Transitional//EN''http://www.w3.org/TR/html4/loose.dtd'>"
                                                Width="100%" Height="100%" runat="server" AutoConfigure="Simple" ThemeType="Office2007">
                                                <FrameStyle BackColor="White" BorderColor="#DDDDDD" BorderStyle="Solid" BorderWidth="1px"
                                                    CssClass="CuteEditorFrame" Height="100%" Width="100%" />
                                            </CE:Editor>
                                        </div>
                                    </td>
                                </tr>
                            </tbody>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td class="buttons" style="height: 40px">
                        <asp:Button runat="Server" ID="btnOK" Text="OK" CssClass="button" />&nbsp
                        <asp:Button runat="Server" CssClass="button" ID="btnCancel" Text="Cancel" OnClientClick="window.close();"
                            Width="80" />
                    </td>
                </tr>
            </tbody>
    </table>
    <input runat="server" type="hidden" id="ValueFCK" value="" />
    </form>
</body>
</html>
