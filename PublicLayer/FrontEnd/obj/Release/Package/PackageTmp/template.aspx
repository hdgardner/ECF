<%@ Page Language="C#" ValidateRequest="false" EnableEventValidation="false" AutoEventWireup="true" Trace="false" Inherits="Mediachase.Cms.Web.template" Codebehind="template.aspx.cs" %>
<%@ Register Src="~/Structure/Base/Controls/Common/CultureHolder.ascx" TagName="CultureHolder" TagPrefix="cms" %>
<%@ Register Src="~/Structure/Base/Controls/Common/SysInfoHolder.ascx" TagName="SysInfoHolder" TagPrefix="cms" %>
<%@ Register TagPrefix="cms" TagName="SnapHolder" Src="~/Structure/Base/Controls/Common/SnapHolder.ascx" %>
<%@ Register Src="~/Structure/Base/Controls/Common/UtilScriptsLoader.ascx" TagName="UtilScriptsLoader" TagPrefix="cms" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.01 Transitional//EN" "http://www.w3.org/TR/html4/loose.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
<style type="text/css">html,body{margin:0;padding:0;width:100%;height:100%;}</style>
<meta runat="server" id="MetaKeyWord" enableviewstate="false" />
<meta runat="server" id="MetaDescription" enableviewstate="false" />
</head>
<body runat="server" id="MainBody">
    <form runat="server" id="CmsMainForm" action="template.aspx" style="height: 100%; width: 100%">
   		<asp:ScriptManager ID="ScriptManager1" runat="server" EnablePageMethods="true" EnablePartialRendering="true" EnableScriptGlobalization="true" LoadScriptsBeforeUI="true" ScriptMode="Auto"></asp:ScriptManager>
        <div runat="server" id="DesignDiv">
            <div id="TestMainWrapper" runat="server" onclick="var _uh=$find('MediachaseCmsUtil'); if(_uh!=null){if((_uh._wndPopUp!=null)&&(!_uh._wndPopUp.closed)) _uh._wndPopUp.focus();} return false;"
                class="PopUpDisactive" style="visibility: hidden; position: absolute;">
            </div>
            <asp:HiddenField runat="server" ID="hfDeletedInfo" />
            <asp:HiddenField runat="server" ID="hfEditedInfo" />                       
            <asp:Label runat="server" ID='lblPage' Visible="false" Text="%PageId%"></asp:Label>
            <asp:Label runat="server" ID='lblVersion' Visible="false" Text="%VersionId%"></asp:Label>
            <asp:Label runat="server" ID='lblStatus' Visible="false" Text="%StatusId%"></asp:Label>
            <asp:Label runat="server" ID='lblLang' Visible="false" Text="%LangId%"></asp:Label>
            <asp:HiddenField runat="server" ID="_CommandField" OnValueChanged="OnCommand" Value=""/>
            <asp:HiddenField runat="server" ID="_CommandValue" Value=""/>            
            <input id="ToolBarVisible" type="hidden" runat="server" />
            <input id="ToolBoxVisible" type="hidden" runat="server" />
            <input id="tbSettingsVisible" type="text" value="False" style="display: none" runat="server" />
            <cms:SnapHolder ID="SnapHolder1" runat="server" />
            <cms:SysInfoHolder ID="SysInfoHolder1" runat="server" />            
            <cms:UtilScriptsLoader runat="server" ID="ScriptsLoader" />
            <div id="DockingTemp" runat="server"></div>
            <asp:Button runat="server" ID="btnUpdateWindow" Visible="false" />   
        </div>
        <asp:HiddenField runat="server" ID="ClientTzOffset" />
        <script type="text/javascript">
//            function SetClientTimeZoneOffset() {
//                var clientTzOffsetObj = $get('<%= ClientTzOffset.ClientID %>');
//                if (clientTzOffsetObj != null)
//                    clientTzOffsetObj.value = new Date().getTimezoneOffset();
//            }
//            SetClientTimeZoneOffset();
        </script>
        <cms:PlaceHolderManager ID="PlaceHolderManager1" runat="server" />
        <asp:PlaceHolder EnableViewState="true" runat="server" ID="Content"></asp:PlaceHolder>
        <cms:CultureHolder ID="CultureHolder1" runat="server" />
        <asp:PlaceHolder runat="server" ID="DesignScripts">
        <script type="text/javascript">
        function UpdateWindow(){<%= ClientScript.GetPostBackEventReference(btnUpdateWindow, "")%>}
        function HideSelect(){}
        function ShowSelect(){}        
        function RunCommand(name){RunCommand(name, '');}
        function RunCommand(name, val)
        {
            //var form = Sys.WebForms.PageRequestManager.getInstance()._form;
            //alert(form.action);
            if (!theForm.onsubmit || (theForm.onsubmit() != false)) {
                theForm._CommandField.value = name;
                theForm._CommandValue.value = val;
                theForm.submit();
            }
        }
        </script>        
        </asp:PlaceHolder>
    </form>
    <asp:Literal runat="server" EnableViewState="false" ID="PageInclude"></asp:Literal>
    <script language="javascript" src='<%=CommerceHelper.GetAbsolutePath("Scripts/main.js") %>'></script>
    <script language="javascript" src='<%=CommerceHelper.GetAbsolutePath("Scripts/CallbackComplete.js")%>'></script>
</body>
</html>