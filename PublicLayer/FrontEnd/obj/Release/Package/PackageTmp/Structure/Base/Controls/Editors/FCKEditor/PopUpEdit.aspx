<%@ Page Language="C#" AutoEventWireup="true" Inherits="Mediachase.Cms.Controls.Editors.FCKEditor.PopUpEdit" ValidateRequest="false" Theme="PopupEdit" Codebehind="PopUpEdit.aspx.cs" %>

<%@ Register TagPrefix="FCKeditorV2" Namespace="FredCK.FCKeditorV2" Assembly="FredCK.FCKeditorV2" %>
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

    function InitFCK(id1,id2)
    {
        UnloadFlag=true;
        var objEditor=document.getElementById(id1);
        var objSource=window.opener.document.getElementById(id2);
    
        if ((objEditor!=null)&&(objSource!=null))
        {
            objEditor.value = objSource.innerHTML;
        }
        document.getElementById(id1).form.submit();
    }
    
    function loadFCK(id1,id2)
    {
        var objEditor=document.getElementById(id1);
        var objSource=window.opener.document.getElementById(id2);
        var objContainer=window.opener.document.getElementById(objSource.getAttribute("innerContainer"));
     
        if(objEditor.value=="") 
        {
            objEditor.value="Click for edit";
        }
     
        objContainer.value=objEditor.value;
        objSource.innerHTML= objEditor.value;
    }

    function cancelFCK(id2)
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
            cancelFCK('<%= Request["Object"] %>');
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
                        <td class="main" height="100%">
                            <table height="100%" cellspacing="0" cellpadding="0" width="100%">
                                <tbody>
                                    <tr>
                                        <td height="100%">
                                            <FCKeditorV2:FCKeditor ID="htmlEditor" runat="server" Height="100%">
                                            </FCKeditorV2:FCKeditor>
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
        <input runat="server" type="hidden" id="ValueFCK" value="test" />
    </form>
</body>
</html>
